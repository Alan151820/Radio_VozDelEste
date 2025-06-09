using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace RadioVozDelEste.DataAccess
{
    public class PConnection
    {

        public string ConnectionString = "Server=DESKTOP-16NGALL\\SQLEXPRESS10;Database=Radio_VozDelEste;User Id=Alan;Password=1234"; //Cambiar el connection string"
        protected SqlConnection GetConnection()
        {
            SqlConnection context = new SqlConnection(ConnectionString);
            return context;
        }
    }
}