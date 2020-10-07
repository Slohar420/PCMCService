using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
//Add MySql Library
using System.Data;
using System.Configuration;
using System.Net;
using System.Runtime.Serialization.Json;
using LipiRMS;

namespace Classes
{
    public class DBConnect : IDisposable
    {
        
        public string database_type;      
        public ReqConnectDB reqConnectDB;
        public string connectionString;
        public string URL;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            reqConnectDB = new ReqConnectDB();
            reqConnectDB.ServerIP = ConfigurationManager.AppSettings["DBServerIP"].ToString();//   "WIN-T04S48MMAPI"; //IP where Database is present
                                                                                              //  database = ConfigurationManager.AppSettings["Database"].ToString(); //"kmsdatabase";
                                                                                              //uid = "lipi";
            reqConnectDB.UserName = ConfigurationManager.AppSettings["UserId"].ToString();
            //password = "L!p!d@t@";
            reqConnectDB.Password = ConfigurationManager.AppSettings["Password"].ToString();
            reqConnectDB.DBName = ConfigurationManager.AppSettings["DBName"].ToString();
            // string connectionString;
            // connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            database_type = ConfigurationManager.AppSettings["DataBaseType"].ToString();
            if (ConfigurationManager.AppSettings["DataBaseType"].ToString() == "mssql")
                reqConnectDB.DBType = Database.MSSQL;
            else
                reqConnectDB.DBType = Database.MySQL;
            reqConnectDB.ColumnEncryption = false;
            URL = ConfigurationManager.AppSettings["URL"].ToString();
           
        }



        public string DatabaseName
        {
            get { return database_type; }
        }

        //open connection to database
        //private bool OpenConnection(out string strError)
        //{
        //    strError = "";
        //    try
        //    {
        //        if (database_type == "mssql")
        //        {
        //            if (connection.State == ConnectionState.Closed)
        //            {
        //                connection.Open();
        //                //ThreadSafe.WriteFile("S", "Database connection :", "Connection Open", "MainFolder");
        //            }
        //            return true;
        //        }
        //        else 
        //        {
        //            if (myconnection.State == ConnectionState.Closed)
        //            {
        //                myconnection.Open();
        //                //ThreadSafe.WriteFile("S", "Database connection :", "Connection Open", "MainFolder");
        //            }
        //            return true;
        //        }
        //    }
        //    catch (SqlException ex)
        //    {               
        //        //When handling errors, you can your application's response based on the error number.
        //        //The two most common error numbers when connecting are as follows:
        //        //0: Cannot connect to server.
        //        //1045: Invalid user name and/or password.
        //        //switch (ex.Number)
        //        //{
        //        //    case 0:
        //        //        MessageBox.Show("Cannot connect to server.  Contact administrator");
        //        //        //ThreadSafe.WriteFile("Cannot connect to server");

        //        //        break;

        //        //    case 1045:
        //        //        MessageBox.Show("Invalid username/password, please try again");
        //        //        //ThreadSafe.WriteFile("Invalid username/password for database connectivity");
        //        //        break;

        //        //    //added ankush for the server not present
        //        //    case 1042:
        //        //        MessageBox.Show("Server Not Present for connection");
        //        //        //ThreadSafe.WriteFile("Server Not Presented for connection");
        //        //        break;
        //        //    default:
        //        //        ThreadSafe.WriteFile("S", "Excp -> Database connection :", "Connection Open", "MainFolder");
        //        //        break;

        //        //}
        //        strError = ex.Message;
        //        return false;
        //    }
        //}


        //public bool DeleteParameterized(string query, List<string> data, out string strError)
        //{
        //    strError = "";
        //    try
        //    {
        //        //open connection
        //        if (this.OpenConnection(out strError))
        //        {
        //            if (database_type == "mssql")
        //            {
        //                //create command and assign the query and connection from the constructor
        //                int status;

        //                SqlCommand cmd = new SqlCommand(query, connection);

        //                for (int i = 0, j = 0; i < data.Count; i += 2)
        //                {
        //                    cmd.Parameters.AddWithValue(data[j], data[i + 1]);
        //                    j = j + 2;
        //                }

        //                SqlDataAdapter myadp = new SqlDataAdapter(query, connection);
        //                myadp.DeleteCommand = cmd;
        //                status = cmd.ExecuteNonQuery();

        //                //close connection
        //                if (this.CloseConnection(out strError) && status != 0)
        //                    return true;
        //            }
        //            else
        //            {
        //                int status;
        //                MySqlCommand cmd = new MySqlCommand(query, myconnection);
        //                for (int i = 0, j = 0; i < data.Count; i += 2)
        //                {
        //                    cmd.Parameters.AddWithValue(data[j], data[i + 1]);
        //                    j = j + 2;
        //                }                      
        //                 status = cmd.ExecuteNonQuery();
        //                if (this.CloseConnection(out strError))
        //                    return true;

        //            }
        //        }
        //        return false;
        //    }
        //    catch (SqlException ex)
        //    {
        //        strError = ex.Message;
        //        return false;
        //    }
        //}

        ////Close connection
        //private bool CloseConnection(out string strError)
        //{
        //    strError = "";
        //    try
        //    {
        //        if (database_type == "mssql")
        //        {
        //            if (connection.State == ConnectionState.Open)
        //            {
        //                connection.Close();
        //                //ThreadSafe.WriteFile("S", "Database connection :", "Connection Close", "MainFolder");
        //            }
        //            return true;
        //        }
        //        else
        //        {
        //            if (myconnection.State == ConnectionState.Open)
        //            {
        //                myconnection.Close();
        //                //ThreadSafe.WriteFile("S", "Database connection :", "Connection Close", "MainFolder");
        //            }
        //            return true;
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        //MessageBox.Show(ex.Message);
        //        //ThreadSafe.WriteFile("Exception DBConnect : CloseConnection()", "", "", "MainFolder");

        //        strError = ex.Message;
        //        return false;
        //    }
        //}
        //public bool SelectParameterized(string query, List<string> data, out DataSet ds, out string strError)
        //{
        //    ds = null;
        //    strError = "";
        //    try
        //    {
        //        if (this.OpenConnection(out strError))
        //        {
        //            if (database_type == "mssql")
        //            {
        //                SqlCommand cmd = new SqlCommand(query, connection);
        //                int count = query.Split('@').Length - 1;
        //                for (int i = 0, j = 0; i < data.Count; i += 2)
        //                {
        //                    cmd.Parameters.AddWithValue(data[j], data[i + 1]);
        //                    j = j + 2;
        //                }

        //                adp = new SqlDataAdapter(query, connection);
        //                adp.SelectCommand = cmd;
        //                objDSset = new DataSet();
        //                adp.Fill(objDSset);

        //                ds = objDSset;

        //                if (this.CloseConnection(out strError))
        //                    return true;
        //            }
        //            else
        //            {

        //                myadp = new MySqlDataAdapter(query, myconnection);
        //                //   adp.SelectCommand.CommanobjDSimeout = 180;
        //                objDSset = new DataSet();
        //                myadp.Fill(objDSset);
        //                ds = objDSset;
        //                if (this.CloseConnection(out strError))
        //                    return true;
        //            }

        //        }
        //        return false;
        //    }
        //    catch (Exception excp)
        //    {
        //        strError = excp.Message;
        //        return false;
        //    }
        //}
        //Insert statement
        public bool Insert(string query, out string strError)
        {
            //string query = "INSERT INTO tableinfo (name, age) VALUES('John Smith', '33')";
            strError = "";
            try
            {
                ReqInsert reqInsert = new ReqInsert();
                reqInsert.Connection = reqConnectDB;
                reqInsert.Query = query;
                reqInsert.Parameters = null;
                WebClient objWC = new WebClient();
                objWC.Headers[HttpRequestHeader.ContentType] = "text/json";
                DataContractJsonSerializer objJsonSerSend = new DataContractJsonSerializer(typeof(ReqInsert));
                MemoryStream memStrToSend = new MemoryStream();
                objJsonSerSend.WriteObject(memStrToSend, reqInsert);
                string data = Encoding.Default.GetString(memStrToSend.ToArray());
                string result = objWC.UploadString(URL + "/Insert", "POST", data);
                MemoryStream memstrToReceive = new MemoryStream(Encoding.UTF8.GetBytes(result));
                DataContractJsonSerializer objJsonSerRecv = new DataContractJsonSerializer(typeof(ResInsert));
                ResInsert resInsert = (ResInsert)objJsonSerRecv.ReadObject(memstrToReceive);
                strError = resInsert.Error;
                return resInsert.Result;

            }
            catch (Exception ex)
            {
                //ThreadSafe.WriteFile("Exception DBCOnnect : Insert()");
                //MessageBox.Show("insert dbconnect " + ex.Message);
                strError = ex.Message;
                return false;
            }
        }

        //Update statement
        public bool Update(string query, out string strError)
        {
            //string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";
            try
            {
                ReqUpdate reqUpdate = new ReqUpdate();
                reqUpdate.Connection = reqConnectDB;
                reqUpdate.Query = query;
                reqUpdate.Parameters = null;
                WebClient objWC = new WebClient();
                objWC.Headers[HttpRequestHeader.ContentType] = "text/json";
                DataContractJsonSerializer objJsonSerSend = new DataContractJsonSerializer(typeof(ReqUpdate));
                MemoryStream memStrToSend = new MemoryStream();
                objJsonSerSend.WriteObject(memStrToSend, reqUpdate);
                string data = Encoding.Default.GetString(memStrToSend.ToArray());
                string result = objWC.UploadString(URL + "/Update", "POST", data);
                MemoryStream memstrToReceive = new MemoryStream(Encoding.UTF8.GetBytes(result));
                DataContractJsonSerializer objJsonSerRecv = new DataContractJsonSerializer(typeof(ResUpdate));
                ResUpdate resUpdate = (ResUpdate)objJsonSerRecv.ReadObject(memstrToReceive);
                strError = resUpdate.Error;
                return resUpdate.Result;
            }
            catch (Exception excp)
            {
                strError = excp.Message;
                return false;
            }
        }

        //Delete statement
        public bool Delete(string query, out string strError)
        {
            //string query = "DELETE FROM tableinfo WHERE name='John Smith'";

            strError = "";
            try
            {
                ReqDelete reqDelete = new ReqDelete();
                reqDelete.Connection = reqConnectDB;
                reqDelete.Query = query;
                reqDelete.Parameters = null;
                WebClient objWC = new WebClient();
                objWC.Headers[HttpRequestHeader.ContentType] = "text/json";
                DataContractJsonSerializer objJsonSerSend = new DataContractJsonSerializer(typeof(ReqDelete));
                MemoryStream memStrToSend = new MemoryStream();
                objJsonSerSend.WriteObject(memStrToSend, reqDelete);
                string data = Encoding.Default.GetString(memStrToSend.ToArray());
                string result = objWC.UploadString(URL + "/Delete", "POST", data);
                MemoryStream memstrToReceive = new MemoryStream(Encoding.UTF8.GetBytes(result));
                DataContractJsonSerializer objJsonSerRecv = new DataContractJsonSerializer(typeof(ResDelete));
                ResDelete resDelete = (ResDelete)objJsonSerRecv.ReadObject(memstrToReceive);
                strError = resDelete.Error;
                return resDelete.Result;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                //ThreadSafe.WriteFile("Exception in Delete query " + ex.ToString(), "", "", "MainFolder");
                return false;
            }
        }

        //Select statement
        public bool Select(string query, out DataSet ds, out string strError)
        {
            //string query = "SELECT * FROM tableinfo";
            ds = null;
            strError = "";

            try
            {
               
                ReqSelect reqSelect = new ReqSelect();
                reqSelect.Connection = reqConnectDB;
                reqSelect.Query = query;
                reqSelect.Parameters = null;
                WebClient objWC = new WebClient();
                objWC.Headers[HttpRequestHeader.ContentType] = "text/json";
                DataContractJsonSerializer objJsonSerSend = new DataContractJsonSerializer(typeof(ReqSelect));
                MemoryStream memStrToSend = new MemoryStream();
                objJsonSerSend.WriteObject(memStrToSend, reqSelect);
                string data = Encoding.Default.GetString(memStrToSend.ToArray());
                string result = objWC.UploadString(URL + "/Select", "POST", data);
               
                MemoryStream memstrToReceive = new MemoryStream(Encoding.UTF8.GetBytes(result));
                DataContractJsonSerializer objJsonSerRecv = new DataContractJsonSerializer(typeof(ResSelect));
                ResSelect resSelect = (ResSelect)objJsonSerRecv.ReadObject(memstrToReceive);
                if (resSelect.Result)
                {
                    ds = resSelect.DS;
                    return true;
                }
                else
                {
                    ds = null;
                    strError = resSelect.Error;
                    return false;
                }
            }
            catch (Exception excp)
            {
                strError = excp.Message;
                return false;
            }
        }


     


        #region IDisposable Members

        public void Dispose()
        {
            //connection = null;
            //myconnection = null;
            //server = "";
            //database = "";
            //uid = "";
            //password = "";
            //adp = null;
            //objDSset = null;
            GC.Collect();

            //throw new NotImplementedException();
        }

        #endregion

    }
    public class ReqConnectDB
    {
        public string ServerIP;
        public string UserName;
        public string Password;
        public Database DBType;
        public string DBName;
        public bool ColumnEncryption;
    }
    public enum Database
    {
        MSSQL,
        MySQL
    }
    public class ResConnectDB
    {
        public bool Result;
        public string Error;
    }
    public class ReqSelect
    {
        public ReqConnectDB Connection;
        public string Query;
        public List<string> Parameters;
    }
    public class ResSelect
    {
        public bool Result;
        public string Error;
        public DataSet DS;
    }
    public class ReqInsert
    {
        public ReqConnectDB Connection;
        public string Query;
        public List<object> Parameters;
    }
    public class ResInsert
    {
        public bool Result;
        public string Error;
        public int RecordsInserted;
    }
    public class ReqDelete
    {
        public ReqConnectDB Connection;
        public string Query;
        public List<string> Parameters;
    }
    public class ResDelete
    {
        public bool Result;
        public string Error;
        public string RecoedsDeleted;
    }
    public class ReqUpdate
    {
        public ReqConnectDB Connection;
        public string Query;
        public List<string> Parameters;
    }
    public class ResUpdate
    {
        public bool Result;
        public string Error;
        public int RecordsUpdated;
    }
}

