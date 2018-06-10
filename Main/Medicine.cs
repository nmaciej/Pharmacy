using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Main
{
  class Medicine : ActiveRecord
  {
    //properties
    public int Id { get; set; }
    public string Manufacturer { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Amount { get; set; }
    public bool Prescription { get; set; }

    //constructors
    public Medicine()
    {
    }

    public Medicine(string manufacturer, string name, decimal price, int amount, bool prescription, int id = 0)
    {
      Id = id;
      Manufacturer = manufacturer;
      Name = name;
      Price = price;
      Amount = amount;
      Prescription = prescription;
    }

    //DONE - printing info about one medicine
    public string ShowMedicine(int listId)
    {
      StringBuilder table = new StringBuilder();

      table.Append("|" + $"{listId}".PadLeft(3) + "".PadLeft(1));
      table.Append("|" + $"{Name}".PadLeft(15) + "".PadLeft(1));
      table.Append("|" + $"{Manufacturer}".PadLeft(15) + "".PadLeft(1));
      table.Append("|" + $"{Price:F}".PadLeft(7) + "".PadLeft(1));
      table.Append("|" + $"{Amount}".PadLeft(7) + "".PadLeft(1));
      table.Append("|" + $"{Prescription}".PadLeft(13) + "".PadLeft(1) + "|");
      table.Append("\n");
      table.Append("".PadLeft(73, '-'));

      return table.ToString();

    }

    public static List<Medicine> ShowMedicines()
    {
      const string sqlQuery = "SELECT * FROM Medicines;";

      try
      {
        using (var connection = new SqlConnection(ActiveRecord.ConnectionString))
        using (var command = new SqlCommand(sqlQuery, connection))
        {
          connection.Open();

          using (var result = command.ExecuteReader())
          {
            var listOfMedicines = new List<Medicine>();

            while (result.Read())
            {
              listOfMedicines.Add(
                new Medicine()
                {
                  Id = (int)result["Id"],
                  Manufacturer = result["Manufacturer"].ToString(),
                  Name = result["Name"].ToString(),
                  Price = (decimal)result["Price"],
                  Amount = (int)result["Amount"],
                  Prescription = (bool)result["Prescription"]
                }
              );
            }

            return listOfMedicines;
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return null;
      }
    }

    public void RemoveMedicine()
    {
      const string sqlQuery = "DELETE FROM Medicines where Id=@Id";

      try
      {
        using (var connection = new SqlConnection(ActiveRecord.ConnectionString))
        using (var command = new SqlCommand(sqlQuery, connection))
        {
          var pId = new SqlParameter("@Id", SqlDbType.Int) { Value = Id };
          command.Parameters.Add(pId);

          connection.Open();
          var result = command.ExecuteNonQuery();

          if (result == 1)
          {
            Medicine.ShowMedicines();
            Console.WriteLine($"Record {Name} deleted successfully.");
            Thread.Sleep(1000);
          }
          else
          {
            Console.WriteLine("No records deleted");
            Thread.Sleep(1000);
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }

    public override int Add()
    {
      const string sqlQuery = "insert into Medicines([Manufacturer],[Name],[Price],[Amount],[Prescription]) " +
                              "values(@Manufacturer, @Name, @Price, @Amount, @Prescription);" +
                              "select SCOPE_IDENTITY();";
      try
      {
        using (var connection = new SqlConnection(ActiveRecord.ConnectionString))
        using (var command = new SqlCommand(sqlQuery, connection))
        {
          var pManufacturer = new SqlParameter("@Manufacturer", SqlDbType.VarChar) { Value = Manufacturer };
          var pName = new SqlParameter("@Name", SqlDbType.VarChar) { Value = Name };
          var pPrice = new SqlParameter("@Price", SqlDbType.Money) { Value = Price };
          var pAmount = new SqlParameter("@Amount", SqlDbType.Int) { Value = Amount };
          var pPrescription = new SqlParameter("@Prescription", SqlDbType.Bit) { Value = Prescription };

          command.Parameters.Add(pManufacturer);
          command.Parameters.Add(pName);
          command.Parameters.Add(pPrice);
          command.Parameters.Add(pAmount);
          command.Parameters.Add(pPrescription);

          connection.Open();
          var newMedicineId = int.Parse(command.ExecuteScalar().ToString());

          return newMedicineId;
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("Medicine can not be removed from database since there are archived orders for whis item.");
        return 0;
      }
    }
    public void EditMedicine()
    {
      const string sqlQuery =
        "UPDATE Medicines SET [Manufacturer] = @Manufacturer, [Name] = @Name, " +
        "[Price] = @Price, [Amount] = @Amount, [Prescription] = @Prescription WHERE [Id] = @Id;";

      try
      {
        using (var connection = new SqlConnection(ActiveRecord.ConnectionString))
        using (var command = new SqlCommand(sqlQuery, connection))
        {
          var pId = new SqlParameter("@Id", SqlDbType.Int) { Value = Id };
          var pManufacturer = new SqlParameter("@Manufacturer", SqlDbType.VarChar) { Value = Manufacturer };
          var pName = new SqlParameter("@Name", SqlDbType.VarChar) { Value = Name };
          var pPrice = new SqlParameter("@Price", SqlDbType.Money) { Value = Price };
          var pAmount = new SqlParameter("@Amount", SqlDbType.Int) { Value = Amount };
          var pPrescription = new SqlParameter("@Prescription", SqlDbType.Bit) { Value = Prescription };

          command.Parameters.Add(pId);
          command.Parameters.Add(pManufacturer);
          command.Parameters.Add(pName);
          command.Parameters.Add(pPrice);
          command.Parameters.Add(pAmount);
          command.Parameters.Add(pPrescription);

          connection.Open();
          command.ExecuteScalar();

          Console.WriteLine($"Medicine successfully edited.");
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }
  }
}