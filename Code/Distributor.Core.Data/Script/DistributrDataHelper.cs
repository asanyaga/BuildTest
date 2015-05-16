using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using StructureMap;

namespace Distributr.Core.Data.Script
{
  public  class DistributrDataHelper
    {
      public static void Migrate()
      {
          var context = ObjectFactory.GetInstance<CokeDataContext>();
          Migrate(context);
          //using (SqlConnection conn = new SqlConnection(connectionString))
          //{
          //    ////string[] splitter = new string[] {"\r\nGO\r\n"};
          //    //string[] splitter = new string[] { "\nGO\n" };
          //    //string[] commandTexts = script.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
          //    //if (commandTexts.Count() < 5)
          //    //{
          //    //    splitter = new string[] { "\r\nGO\r\n" };
          //    //    commandTexts = script.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
          //    //}
          //    //foreach (string sql in commandTexts)
          //    //{
          //    //    Console.WriteLine(sql);
          //    //    try
          //    //    {
          //    //        conn.Open();
          //    //        using (SqlCommand comm = new SqlCommand(sql, conn))
          //    //        {
          //    //            comm.ExecuteNonQuery();
          //    //        }

          //    //    }
          //    //    catch (Exception ex)
          //    //    {
          //    //        Console.WriteLine(ex.Message);
          //    //    }
          //    //    finally
          //    //    {
          //    //        conn.Close();
          //    //    }
          //    //}
          //}
      }

       public static void Migrate(CokeDataContext context)
      {
          var entityConnection = (System.Data.EntityClient.EntityConnection) context.Connection;
          DbConnection conn = entityConnection.StoreConnection;
          ConnectionState initialState = conn.State;
          bool tblExist = CheckIfExist(conn, initialState);
          var data =
              Assembly.GetExecutingAssembly()
                  .GetManifestResourceNames()
                  .Where(s => s.StartsWith("Distributr.Core.Data.Script."))
                  .OrderBy(s => s)
                  .ToList();
          var executed = context.C_dsMigration.Select(s => s.SfileName).Distinct().ToList();
          data = data.Where(s => !executed.Contains(s)).ToList();
          string[] splitter = new string[] {"GO"};
          foreach (var sqlfile in data)
          {
              var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(sqlfile);
              using (StreamReader sr = new StreamReader(resource))
              {
                  var filedata = sr.ReadToEnd();
                  if (!string.IsNullOrEmpty(filedata))
                  {
                      var split = filedata
                          .Replace("\r", " ")
                          .Replace("\t", " ")
                          .Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                      foreach (var s in split)
                      {
                          try
                          {
                              if (initialState != ConnectionState.Open)
                                  conn.Open(); // open connection if not already open
                              using (DbCommand cmd = conn.CreateCommand())
                              {
                                  cmd.CommandText = s;
                                  cmd.ExecuteNonQuery();
                              }
                              UpdateMigrationTracker(sqlfile, conn, initialState);
                          }
                          catch (Exception exception)
                          {
                          }
                          finally
                          {
                              if (initialState != ConnectionState.Open)
                                  conn.Close(); // only close connection if not initially open
                          }
                      }
                  }
              }
          }
      }

      private static void UpdateMigrationTracker(string sqlfile, DbConnection conn, ConnectionState initialState)
      {
          string sql = string.Format(@"INSERT INTO [_dsMigration]
           ([Id]
           ,[SfileName]
           ,[DateMigrated]
           ,[ProductVersion])
             VALUES
           ('{0}'
           ,'{1}'
           ,'{2}'
           ,'{3}')", Guid.NewGuid(), sqlfile, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "10.0.0.1");
          try
          {
              if (conn.State != ConnectionState.Open)
                  conn.Open(); // open connection if not already open
              using (DbCommand cmd = conn.CreateCommand())
              {
                  cmd.CommandText = sql;

                  cmd.ExecuteNonQuery();

              }
          }
          catch (Exception exception)
          {

          }
          finally
          {
              if (initialState != ConnectionState.Open)
                  conn.Close(); // only close connection if not initially open
          }


      }

      private static bool CheckIfExist(DbConnection conn, ConnectionState initialState)
      {
          try
          {
              if (initialState != ConnectionState.Open)
                  conn.Open(); // open connection if not already open
              using (DbCommand cmd = conn.CreateCommand())
              {
                  cmd.CommandText =
                      @"IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE   TABLE_NAME = '_dsMigration'))
BEGIN
CREATE TABLE _dsMigration (
  Id     uniqueidentifier PRIMARY KEY NOT NULL,
  SfileName  varchar(250) NOT NULL,
  DateMigrated  date NOT NULL,
  [ProductVersion] [nvarchar](32) NOT NULL
  
); END";
                  cmd.ExecuteNonQuery();
                  return true;
              }
          }
          catch (Exception exception)
          {

          }
          finally
          {
              if (conn.State != ConnectionState.Open)
                  conn.Close(); // only close connection if not initially open
          }
          return true;
      }
    }
}
