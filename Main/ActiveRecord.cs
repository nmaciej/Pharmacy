using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Main
{
  abstract class ActiveRecord
  {
    public static readonly string ConnectionString = "Integrated Security=SSPI;" +
                                                      "Initial Catalog=Pharmacy;" +
                                                      "Data Source=.\\SQLEXPRESS01;";

    public abstract int Add();
  }
}
