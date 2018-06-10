using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Main
{
  class MedicinesOrders : ActiveRecord
  {
    public int Id { get; set; }
    public int IdOrder { get; set; }
    public int IdMedicine { get; set; }
    public int MedicineAmount { get; set; }
    public string PrescriptionNumber { get; set; }

    public MedicinesOrders()
    {
    }
    public MedicinesOrders(int idOrder, int idMedicine, int medicineAmount, string prescriptionNumber, int id = 0)
    {
      Id = id;
      IdOrder = idOrder;
      IdMedicine = idMedicine;
      MedicineAmount = medicineAmount;
      PrescriptionNumber = prescriptionNumber;
    }

    public override int Add()
    {
      const string sqlQuery = "INSERT INTO MedicinesOrders ([IdOrder],[IdMedicine],[MedicineAmount],[PrescriptionNumber]) " +
                              "VALUES (@IdOrder,@IdMedicine,@MedicineAmount,@PrescriptionNumber);" +
                              "select SCOPE_IDENTITY();";
      try
      {
        using (var connection = new SqlConnection(ActiveRecord.ConnectionString))
        using (var command = new SqlCommand(sqlQuery, connection))
        {
          var pIdOrder = new SqlParameter("@IdOrder", SqlDbType.Int) { Value = IdOrder };
          var pIdMedicine = new SqlParameter("@IdMedicine", SqlDbType.Int) { Value = IdMedicine };
          var pMedicineAmount = new SqlParameter("@MedicineAmount", SqlDbType.Int) { Value = MedicineAmount };
          var pPrescriptionNumber = new SqlParameter("@PrescriptionNumber", SqlDbType.VarChar);

          if (PrescriptionNumber is null)
          {
            pPrescriptionNumber.Value = DBNull.Value;
          }
          else
          {
            pPrescriptionNumber.Value = PrescriptionNumber;
          }

          command.Parameters.Add(pIdOrder);
          command.Parameters.Add(pIdMedicine);
          command.Parameters.Add(pMedicineAmount);
          command.Parameters.Add(pPrescriptionNumber);

          connection.Open();
          var newMedicinesOrdersId = int.Parse(command.ExecuteScalar().ToString());
          return newMedicinesOrdersId;

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