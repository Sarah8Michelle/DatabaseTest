using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTest
{
    public class DatabaseTestService
    {
        private string connectionString = "Data Source = Database server; Initial Catalog = Name of the database; User ID = Your user; Password = Your password; Trusted_Connection = false (true if using Windows Authentication)";

        /// <summary>
        /// This method copies the data in Table1 and paste it in Table2 of the DatabaseTest database. It also updates the IsChecked column after the copy is done.
        /// </summary>
        public void UpdateDatabase()
        {
            try
            {
                SqlConnection sqlConnection = new SqlConnection(connectionString);

                //Openning SQL Connection.
                sqlConnection.Open();

                using(SqlDataAdapter sqlDataAdapter = new SqlDataAdapter())
                {
                    /*
                     * Summary of the code inside the 'using' statement:
                     * First we create a DataSet to represent the in-memory cache of the data and a SqlCommand to select the columns from the first table.
                     * The data from the first table will be saved in a temporary table called 'TablesInfo' and we will iterate through it's rows to fill the second table.
                     * When the second table is filled, we will update the value of 'IsChecked' in the first table to complete the cycle.
                     */
                    DataSet dataSet = new DataSet();
                    SqlCommand sqlCommand = new SqlCommand("SELECT [FullName], [IdNumber], [Email] FROM [dbo].[Table1] WHERE IsChecked = 0", sqlConnection);
                    sqlDataAdapter.SelectCommand = sqlCommand;

                    sqlDataAdapter.Fill(dataSet, "TablesInfo");
                    DataTable dataTable = dataSet.Tables["TablesInfo"];

                    foreach(DataRow row in dataTable.Rows)
                    {
                        SqlCommand insertCommand = new SqlCommand("INSERT INTO [dbo].[Table2]([FullName1], [IdNumber1], [Email1]) VALUES ('" + row[0].ToString() + "'," + row[1] + ", '" + row[2].ToString() +"')", sqlConnection);

                        if(insertCommand.ExecuteNonQuery() > 0)
                        {
                            SqlCommand updateCommand = new SqlCommand("UPDATE [dbo].[Table1] SET IsChecked = 1 WHERE IsChecked = 0", sqlConnection);
                            int i = updateCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            EventLog.WriteEntry("DatabaseTestLog", "No rows were affected.", EventLogEntryType.Warning);
                        }
                    }

                    //Releases all resources used by SqlDataAdapter.
                    sqlDataAdapter.SelectCommand.Dispose();

                    //Closing SQL Connection.
                    sqlConnection.Close();                    
                }
            }
            
            catch(SqlException exception)
            {
                //log event.
                EventLog.WriteEntry("DatabaseTestLog", $"SQL Error: {exception.Message}, Found at: {exception.StackTrace}", EventLogEntryType.Error);
            }

            catch (Exception exception)
            {
                //log event.
                EventLog.WriteEntry("DatabaseTestLog", $"Error: {exception.Message}, Found at: {exception.StackTrace}", EventLogEntryType.Error);
            }
        }
    }
}
