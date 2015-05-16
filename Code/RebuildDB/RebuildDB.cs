using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace RebuildDB
{
    public class RebuildDb
    {
        public static void dropalltables(string connectionstring)
        {
            #region sql DropAllTables
            string sql = @"--Delete All Keys
                        DECLARE @Sql NVARCHAR(500) DECLARE @Cursor CURSOR
                        SET @Cursor = CURSOR FAST_FORWARD FOR
                        SELECT DISTINCT sql = 'ALTER TABLE [' + tc2.TABLE_NAME + '] DROP [' + rc1.CONSTRAINT_NAME + ']'
                        FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc1
                        LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc2 ON tc2.CONSTRAINT_NAME =rc1.CONSTRAINT_NAME
                        OPEN @Cursor FETCH NEXT FROM @Cursor INTO @Sql
                        WHILE (@@FETCH_STATUS = 0)
                        BEGIN
                            Exec SP_EXECUTESQL @Sql
                            FETCH NEXT FROM @Cursor INTO @Sql
                        END
                        CLOSE @Cursor DEALLOCATE @Cursor
                        EXEC sp_MSForEachTable 'DROP TABLE ?'";
            #endregion
            
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand comm = new SqlCommand(sql, conn))
                    {
                        comm.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static void DropLocaSetupDb(string connectionString)
        {
            string sql =    @"EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'DistributrLocalSetup'
                            USE [master]
                            ALTER DATABASE [DistributrLocalSetup] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
                            USE [master]
                            /****** Object:  Database [[DistributrLocalSetup]]    Script Date: 02/08/2013 15:24:49 ******/
                            DROP DATABASE [DistributrLocalSetup]";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand comm = new SqlCommand(sql, conn))
                    {
                        comm.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                   
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public static void RecreateTables(string connectionString, string scriptlocation)
        {
            FileInfo file = new FileInfo(scriptlocation);
            string script = file.OpenText().ReadToEnd();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //string[] splitter = new string[] {"\r\nGO\r\n"};
                string[] splitter = new string[] { "\nGO\n" };
                string[] commandTexts = script.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                if (commandTexts.Count()<5)
                {
                    splitter = new string[] { "\r\nGO\r\n" };
                    commandTexts = script.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                }
                foreach (string sql in commandTexts)
                {
                    Console.WriteLine(sql);
                    try
                    {
                        conn.Open();
                        using (SqlCommand comm = new SqlCommand(sql, conn))
                        {
                            comm.ExecuteNonQuery();
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

        }

        
    }
}
