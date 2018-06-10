using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace Main
{
  class Order : ActiveRecord
  {
    //properties
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Cost { get; set; }

    //constructors
    public Order()
    {
    }
    public Order(DateTime date, decimal cost, int id = 0)
    {
      Id = id;
      Date = date;
      Cost = cost;
    }

    public string ShowOrder(int listId)
    {
      StringBuilder table = new StringBuilder();

      table.Append("|" + $"{listId}".PadLeft(3) + "".PadLeft(1));
      table.Append("|" + $"{Date:d}".PadLeft(15) + "".PadLeft(1));
      table.Append("|" + $"{Cost:C}".PadLeft(15) + "".PadLeft(1)+"|");
      table.Append("\n");
      table.Append("".PadLeft(40, '-'));

      return table.ToString();
    }

    public static List<Order> ShowOrders()
    {
      const string sqlQuery = "SELECT * FROM Orders;";

      try
      {
        using (var connection = new SqlConnection(ActiveRecord.ConnectionString))
        using (var command = new SqlCommand(sqlQuery, connection))
        {
          connection.Open();

          using (var result = command.ExecuteReader())
          {
            var listOfOrders = new List<Order>();

            while (result.Read())
            {
              listOfOrders.Add(
                new Order()
                {
                  Id = (int)result["Id"],
                  Date = (DateTime)result["Date"],
                  Cost = (decimal)result["Cost"]
                }
              );
            }
            return listOfOrders;
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return null;
      }
    }

    public override int Add()
    {
      const string sqlQuery = "insert into Orders([Date],[Cost]) " +
                              "values(@Date, @Cost);" +
                              "select SCOPE_IDENTITY();";
      try
      {
        using (var connection = new SqlConnection(ActiveRecord.ConnectionString))
        using (var command = new SqlCommand(sqlQuery, connection))
        {
          var pDate = new SqlParameter("@Date", SqlDbType.VarChar) { Value = Date };
          var pCost = new SqlParameter("@Cost", SqlDbType.Decimal) { Value = Cost };

          command.Parameters.Add(pDate);
          command.Parameters.Add(pCost);


          connection.Open();
          var newOrderId = int.Parse(command.ExecuteScalar().ToString()) ;

          Console.WriteLine($"New order added.");
          Thread.Sleep(1000);
          return newOrderId;

        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return 0;
      }
    }

  }
}
