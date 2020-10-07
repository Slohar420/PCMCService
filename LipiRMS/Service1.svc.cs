using Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Messaging;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
using System.Text;
using Ionic.Zip;

namespace LipiRMS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        //MSMQ Object Define
        private MessageQueue TCPMsg = null;
        private byte[] arrTCPData;

        public Service1()
        {
            Log.strLogPath = ConfigurationManager.AppSettings["LogDirectory"].ToString();

            string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
            if (!Directory.Exists(strPatchPath))
                Directory.CreateDirectory(strPatchPath);

            if (!Directory.Exists(strPatchPath + "\\RMS Client Updated Machines"))
                Directory.CreateDirectory(strPatchPath + "\\RMS Client Updated Machines");

            if (!Directory.Exists(strPatchPath + "\\RMS Monitor Updated Machines"))
                Directory.CreateDirectory(strPatchPath + "\\RMS Monitor Updated Machines");

            if (!Directory.Exists(strPatchPath + "\\Lipi RD Service Updated Machines"))
                Directory.CreateDirectory(strPatchPath + "\\Lipi RD Service Updated Machines");

            if (!Directory.Exists(strPatchPath + "\\RMS Client Patch"))
                Directory.CreateDirectory(strPatchPath + "\\RMS Client Patch");

            if (!Directory.Exists(strPatchPath + "\\RMS Monitor Patch"))
                Directory.CreateDirectory(strPatchPath + "\\RMS Monitor Patch");

            if (!Directory.Exists(strPatchPath + "\\RD Service Patch"))
                Directory.CreateDirectory(strPatchPath + "\\RD Service Patch");

            string strReceivedData = ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString();
            if (!Directory.Exists(strReceivedData))
                Directory.CreateDirectory(strReceivedData);

            //Lokesh Add[27Nov2018]MessageQueue instance creation
            if (!MessageQueue.Exists(".\\private$\\tcpmsg"))
            {
                // Create if not
                try
                { MessageQueue.Create(".\\private$\\tcpmsg"); }
                catch { }
            }
           
        }

       

        public EncResponse Login(EncRequest objEncRequest)
        {
            Log.Write("In Login" , "");
            EncResponse objEncResponse = new EncResponse();
            DBConnect objDB = new DBConnect();
            UserLogin usrlogin = new UserLogin();
            DataSet objDS = new DataSet();
            String query = "";
            string strError = "";
            Reply response = new Reply();
            try
            {
                string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                usrlogin = JsonConvert.DeserializeObject<UserLogin>(strDecrtyptData);
                //  string[] credentials = usrlogin.UserName.Split('#');
                string DecryptPassword = usrlogin.Password;

              

                if (objDB.database_type == "mssql")
                {
                    query = "select * from [pcmc].[dbo].[login] where username='" + usrlogin.UserName + "' and password='" + DecryptPassword + "'";
                    objDB.Select(query, out objDS, out strError);
                    if (objDS.Tables[0].Rows.Count > 0)
                    {
                        Log.Write("Query fired", "");

                        response.DS = objDS;
                        response.res = true;
                        response.strError = "";
                        objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                        return objEncResponse;
                    }
                    else
                    {
                        response.DS = null;
                        response.res = false;
                        response.strError = strError;
                        objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                        return objEncResponse;
                    }

                }
                else
                {

                    query = "select * from pcmc.login where username='" + usrlogin.UserName + "' and password='" + DecryptPassword + "'";
                    Log.Write("Query - "+query, "");
                    objDB.Select(query, out objDS, out strError);
                    if (objDS.Tables[0].Rows.Count > 0)
                    {
                        Log.Write("Query fired", "");
                        response.DS = objDS;
                        response.res = true;
                        response.strError = "";
                        objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                        return objEncResponse;
                    }
                    else
                    {
                        Log.Write("Query Could Not fire", "");
                        response.DS = null;
                        response.res = false;
                        response.strError = strError;
                        objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                        return objEncResponse;
                    }
                }
            }

            catch (Exception ex)
            {
                ex.Message.ToString();
                response.DS = null;
                response.res = false;
                response.strError = strError;
                objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                return objEncResponse;
            }
        }

        public EncResponse GetUserType(EncRequest objEncRequest)
        {
            Reply response = new Reply();
            DBConnect objDB = new DBConnect();
            EncResponse objEncResponse = new EncResponse();
           
            string query = "", strError = "";
            DataSet objDS = new DataSet();
            try
            {
                string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                string s = JsonConvert.DeserializeObject<string>(strDecrtyptData);

                if (objDB.database_type == "mssql")
                {
                    query = "select * from [pcmc].[dbo].[login] where username = '" + s.ToString()+"'";
                    objDB.Select(query, out objDS, out strError);
                    if (objDS.Tables[0].Rows.Count > 0)
                    {
                        response.DS = objDS;
                        response.res = true;
                        response.strError = "";
                        objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                        return objEncResponse;
                    }
                    else
                    {
                        response.DS = null;
                        response.res = false;
                        response.strError = strError;
                        objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                        return objEncResponse;
                    }

                }
                else
                {

                    query = "select * from pcmc.login where username = '" + s.ToString() + "'";
                    objDB.Select(query, out objDS, out strError);
                    if (objDS.Tables[0].Rows.Count > 0)
                    {
                        response.DS = objDS;
                        response.res = true;
                        response.strError = "";
                        objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                        return objEncResponse;
                    }
                    else
                    {
                        response.DS = null;
                        response.res = false;
                        response.strError = strError;
                        objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                        return objEncResponse;
                    }
                }
            }

            catch (Exception ex)
            {
                ex.Message.ToString();
                response.DS = null;
                response.res = false;
                response.strError = strError;
                objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                return objEncResponse;
            }

        }

        //Get User List for Usermanagment
        public EncResponse GetUserDetails(EncRequest objEncRequest)
        {
            Reply reply = new Reply();
            EncResponse objEncResponse = new EncResponse();
            DBConnect objDb = new DBConnect();
            DataSet objDS = new DataSet();

            string query = "", strError = "";
            try
            {
                if (objDb.database_type == "mssql")
                {
                    query = "select * from [pcmc].[dbo].[login] where credential_type ='administrator' || credential_type ='Administrator' ";
                }
                else
                {
                    query = "select username , credential_type from pcmc.login where pcmc.login.credential_type !=  'Administrator| User| Location' && credential_type != 'administrator' && credential_type !='Administrator| User'";
                }
                if (objDb.Select(query, out objDS, out strError))
                {
                    reply.res = true;
                    reply.DS = objDS;
                    reply.strError = "";  
                }
                else 
                {
                    reply.res = false;
                    reply.DS = null;
                    reply.strError = strError;
                                  
                }
            }
            catch (Exception ex)
            {
                reply.res = false;
                reply.DS = null;
                reply.strError = ex.Message;        
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(reply));
            return objEncResponse;
        } 
        public EncResponse GetKioskHealth(EncRequest objEncRequest)
        {
            DBConnect objDB = new DBConnect();
            EncResponse objEncResponse = new EncResponse();
            string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
            string col = JsonConvert.DeserializeObject<string>(strDecrtyptData);
            int count = Convert.ToInt32(ConfigurationManager.AppSettings["DeviceCount"].ToString());
            string[] DeviceName = new string[count];

            for (int i = 0; i < count; i++)
            {
                DeviceName[i] = ConfigurationManager.AppSettings["Device" + (i + 1)].ToString();
            }
            DataSet objDS = new DataSet();

            Reply response = new Reply();
            response.DeviceCount = count;

            string query = "", strError = "";

            try
            {
                if (objDB.database_type == "mssql")
                {
                    query = "select [f_kiosk_id] as [Kiosk ID],[pcmc].[dbo].[kiosk_master].[kiosk_ip] as [Kiosk IP],[client_status_with_rms] as [Client Status] ,";
                    for (int i = 0; i < count; i++)
                    {
                        query += "[device" + (i + 1) + "_status] as [" + DeviceName[i] + "],";
                    }
                    query += "[date_time] as [DateTime] from [pcmc].[dbo].[health_recent] inner join[pcmc].[dbo].[kiosk_master] on[pcmc].[dbo].[health_recent].[f_kiosk_id] = [pcmc].[dbo].[kiosk_master].[kiosk_id] ";

                    if (col != "KioskList")
                    {
                        query += " where [pcmc].[dbo].[kiosk_master].[kiosk_ip] ='" + col + "'";
                    }
                }
                else
                {
                    query = "select f_kiosk_id as 'Kiosk ID',pcmc.kiosk_master.kiosk_ip as 'Kiosk IP', client_status_with_rms as 'Client Status',";

                    for (int i = 0; i < count; i++)
                    {
                        query += "device" + (i + 1) + "_status as '" + DeviceName[i] + "',";
                    }

                    query += "date_time as 'DateTime'  from pcmc.health_recent inner join  pcmc.kiosk_master on pcmc.health_recent.f_kiosk_id = pcmc.kiosk_master.kiosk_id ";

                    if (col != "KioskList")
                    {
                        query += " where pcmc.kiosk_master.kiosk_ip='" + col + "'";
                    }
                }


                if (objDB.Select(query, out objDS, out strError) && objDS.Tables[0].Rows.Count > 0 && objDS != null)
                {
                    response.res = true;
                    response.DS = objDS;
                    objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                    return objEncResponse;
                }
                else
                {
                    response.res = false;
                    if (strError == "")
                        strError = "No kiosk registered";
                    response.strError = strError;
                    objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                    return objEncResponse;
                }
            }
            catch (Exception ex)
            {
                response.res = false;
                response.strError = ex.Message;
                 objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                    return objEncResponse;
            }
        }

        //ADD USERE

        public EncResponse Adduser(EncRequest objEncRequest)
        {
            UserDetails objUserReq = new UserDetails();
            DBConnect objDB = new DBConnect();
            DataSet objDS = new DataSet();
            Reply reply = new Reply();
            EncResponse objEncResponse = new EncResponse();
            string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
            objUserReq = JsonConvert.DeserializeObject<UserDetails>(strDecrtyptData);
            
            bool res;
            string strError = "", query = "";
            try
            {
                if (objDB.database_type == "mssql")
                {
                    query = "select username from pcmc.login where username='" + objUserReq.Username + "' ";

                    res = objDB.Select(query, out objDS, out strError);

                    if (res == true && objDS.Tables[0].Rows.Count == 0)
                    {
                        string EncryptPassword = AesGcm256.Encrypt(objUserReq.Password);
                        query = "insert into pcmc.login (username, password, question, answer , credential_type,location) values ('" + objUserReq.Username + "' , '" + EncryptPassword + "', '" + objUserReq.Question + "' ,'" + objUserReq.Answer + "', '" + objUserReq.Role + "','" + objUserReq.Location + "')";

                        if (objDB.Insert(query, out strError))
                        {
                            reply.res = true;
                        }
                        else
                        {
                            reply.res = false;
                            reply.strError = strError;
                        }
                    }
                    else
                    {
                        reply.res = false;
                        reply.strError = strError;
                       
                    }
                }
                else
                {
                  
                    query = "select username from pcmc.login where username = '" + objUserReq.Username + "'";
                    res = objDB.Select(query, out objDS, out strError);

                    if (res == true && objDS.Tables[0].Rows.Count == 0)
                    {
                        string EncryptPassword = AesGcm256.Encrypt(objUserReq.Password);
                        query = "insert into pcmc.login (username, password , question , answer , credential_type,location) values ('" + objUserReq.Username + "', '" + EncryptPassword + "','" + objUserReq.Question + "','" + objUserReq.Answer + "','" + objUserReq.Role + "','"+objUserReq.Location+"')";
                        if (objDB.Insert(query, out strError))

                        {
                            reply.res = true;
                        }
                        else
                        {
                            reply.res = false;
                            reply.strError = strError;
                        }
                    }
                    else
                    {
                        reply.res = false;
                        reply.strError = strError;
                    }
                }
                objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(reply));
                return objEncResponse;
            }
            catch(Exception ex)
            {
                ex.Message.ToString();
                return objEncResponse;
            }
        }


        public EncResponse UpdateUser(EncRequest objEncRequest)
        {
            UserDetails objUserReq = new UserDetails();
            DBConnect objDB = new DBConnect();
            DataSet objDS = new DataSet();
            Reply reply = new Reply();
            EncResponse objEncResponse = new EncResponse();
            string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
            objUserReq = JsonConvert.DeserializeObject<UserDetails>(strDecrtyptData);

            bool res;
            string strError = "", query = "";
            try
            {
                if (objDB.database_type == "mssql")
                {
                    query = "update pcmc.login set username = '" + objUserReq.Username + "' , password = '" + objUserReq.Password + "' , question = '" + objUserReq.Question + "' , answer = '" + objUserReq.Answer + "' , credential_type = '" + objUserReq.Role + "' , location = '" + objUserReq.Location + "' where username = '"+objUserReq.Username+"'";
                }
                else
                {
                    query = "update pcmc.login set username = '" + objUserReq.Username + "' , password = '" + objUserReq.Password + "' , question = '" + objUserReq.Question + "' , answer = '" + objUserReq.Answer + "' , credential_type = '" + objUserReq.Role + "' , location = '" + objUserReq.Location + "' where username = '" + objUserReq.Username + "'";
                }
                if (objDB.Update(query, out strError))
                {
                    reply.res = true;
                    reply.DS = objDS;
                    reply.strError = "";
                }
                else
                {
                    reply.res = false;
                    reply.DS = null;
                    reply.strError = strError;
                }
                objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(reply));
                return objEncResponse;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(reply));
                return objEncResponse;
            }
        }
        public EncResponse GetTxnDetails(EncRequest objEncRequest)
        {

             DBConnect objDB = new DBConnect();
            DataSet objDS = new DataSet();
            Reply response = new Reply();
            EncResponse objEncResponse = new EncResponse();
            string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
            string strData = JsonConvert.DeserializeObject<string>(strDecrtyptData);
            string[] s = strData.Split('#');
            string query = "", strError = "";
            query = " SELECT kiosk_ip as 'Machine IP',kiosk_id as 'Machine ID', bill_payemnt_txn as 'Bill Payment',birth_certificate_txn 'Birth Certificate',death_certificate_txn as 'Death Certificate',location 'Location',txn_date_time 'DateTime' FROM seva_sindhu_18.txn_detail_Dec,seva_sindhu.kiosk_master where seva_sindhu_18.txn_detail_Dec.f_kiosk_id = seva_sindhu.kiosk_master.kiosk_id ";
            if (s[0].Trim() == "Location")
            {
                query += " and location = '" + s[1] + "'";
            }
            else if (s[0].Trim() == "kioskid")
            {
                query += " and kiosk_id = '"+s[1]+"'";
            }
           
            if (objDB.Select(query, out objDS, out strError) && objDS.Tables[0].Rows.Count > 0 && objDS != null)
            {
                response.res = true;
                response.DS = objDS;
                objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                return objEncResponse;
            }
            else
            {
                response.res = false;
                response.strError = strError;
                objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                return objEncResponse;
            }
        }
        public EncResponse GetKioskMasterList(EncRequest objEncRequest)
        {
            DBConnect objDB = new DBConnect();

            DataSet objDS = new DataSet();
            Reply reply = new Reply();
            EncResponse objEncResponse = new EncResponse();
            string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
           string col = JsonConvert.DeserializeObject<string>(strDecrtyptData);
            String query = "", strError = "";

            try
            {
                if (objDB.database_type == "mssql")
                {
                    query = "select [kiosk_id] as [Kiosk ID],[kiosk_ip] as [Kiosk IP], [machine_serial_no] as [Machine Serial Number], [client_version] as [Version],[last_patch_updated] as [Last Patch Updated], [drives] as [Drives]  from [pcmc].[dbo].[kiosk_master]";
                }
                else
                {
                    query = "select kiosk_id as 'Kiosk ID',kiosk_ip as 'Kiosk IP', machine_serial_no as 'Machine Serial Number', client_version as 'Version', last_patch_updated as 'Last Patch Updated', drives as 'Drives'  from pcmc.kiosk_master";
                }

                if (objDB.Select(query, out objDS, out strError) && objDS.Tables[0].Rows.Count > 0 && objDS != null)
                {
                    reply.res = true;
                    reply.DS = objDS;
                    objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(reply));
                    return objEncResponse;
                }
                else
                {
                    reply.res = false;
                    if (strError == "")
                        strError = "No kiosk registered";
                    reply.strError = strError;
                    objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(reply));
                    return objEncResponse;
                }

            }
            catch (Exception ex)
            {
                reply.res = false;
                reply.strError = ex.Message;
                objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(reply));
                return objEncResponse;
            }
        }

        public EncResponse GetPieChart(EncRequest objEncRequest)
        {
            DBConnect objDB = new DBConnect();
            EncResponse objEncResponse = new EncResponse();
            string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
            string strQuery  = JsonConvert.DeserializeObject<string>(strDecrtyptData);
           
            DataSet objDS = new DataSet();
            Reply response = new Reply();

            string strError = "";

            try
            {
                if (objDB.Select(strQuery, out objDS, out strError) && objDS.Tables[0].Rows.Count > 0 && objDS != null)
                {
                    response.res = true;
                    response.DS = objDS;
                    objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                    return objEncResponse;
                }
                else
                {
                    response.res = false;
                    if (strError == "")
                        strError = "No kiosk registered";
                    response.strError = strError;
                    objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                    return objEncResponse;
                }

            }
            catch (Exception ex)
            {
                response.res = false;
                response.strError = ex.Message;
                objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
                return objEncResponse;
            }
        }

        public EncResponse CommandIniUpdate(EncRequest objEncRequest)
        {
            EncResponse objEncResponse = new EncResponse();
            try
            {
                CommandIniUpdate objINI = new CommandIniUpdate();
             
                string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                objINI = JsonConvert.DeserializeObject<CommandIniUpdate>(strDecrtyptData);

                string strCommandDir = ConfigurationManager.AppSettings["CommandDirectory"].ToString();
                if (!Directory.Exists(strCommandDir))
                    Directory.CreateDirectory(strCommandDir);

                string strCommFileName = strCommandDir + "\\Command.ini";
                INIFile objCommandIni = new INIFile(strCommFileName);

                string[] command = objINI.Command.Split('#');

                for (int iCount = 0; iCount < objINI.KioskIP.Length; iCount++)
                {
                    Log.Write("In Command Received  :- " + objINI.Command, objINI.KioskIP[iCount]);

                    objCommandIni.Write(objINI.KioskIP[iCount], "CommandCount", objINI.CommandCount);

                    for (int iCommandCount = 1; iCommandCount <= Convert.ToInt32(objINI.CommandCount); iCommandCount++)
                    {
                        objCommandIni.Write(objINI.KioskIP[iCount], "Parameter" + iCommandCount.ToString(), command[iCommandCount - 1]);
                    }

                    Log.Write("Command Received :- " + objINI.KioskIP[iCount], objINI.KioskIP[iCount]);

                    string strPort = ConfigurationManager.AppSettings["Port"].ToString();
                    if (strPort == "")
                        strPort = "11001";                    

                    string IsTCPCommunication = ConfigurationManager.AppSettings["IsTCPCommunication"].ToString();

                    if (IsTCPCommunication == "false")
                    {
                        Log.Write("UDP Communication is true - " + strPort, objINI.KioskIP[iCount]);

                        Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                        IPAddress send_to_address = IPAddress.Parse(objINI.KioskIP[iCount]);


                        Log.Write("Command Send to ip - " + send_to_address, objINI.KioskIP[iCount]);
                        IPEndPoint sending_end_point = new IPEndPoint(send_to_address, Convert.ToInt32(strPort));
                        byte[] send_buffer = Encoding.ASCII.GetBytes(objINI.Command);  //Send Command
                        try
                        {
                            sending_socket.SendTo(send_buffer, sending_end_point);
                            Log.Write("Command Send - " + objINI.Command, objINI.KioskIP[iCount]);
                        }
                        catch (Exception exp)
                        {
                            Log.Write("Exception Occured in UDP :- " + exp.Message + "," + exp.StackTrace, objINI.KioskIP[iCount]);
                        }
                    }
                    else
                    {
                        try
                        {
                            Log.Write("TCP Communication is true - " + strPort, objINI.KioskIP[iCount]);

                            if (TCPMsg == null)
                                TCPMsg = new MessageQueue(".\\private$\\tcpmsg");

                            TCPMsg.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);

                            TCPMsg.MessageReadPropertyFilter.SetAll();
                            TCPMsg.DefaultPropertiesToSend.Recoverable = true;

                            Message rqMessage = new Message();
                            rqMessage.Label = objINI.KioskIP[iCount];
                            rqMessage.Recoverable = true;

                            arrTCPData = Encoding.ASCII.GetBytes(objINI.Command);
                            rqMessage.BodyStream.Write(arrTCPData, 0, arrTCPData.Length);
                            TCPMsg.Send(rqMessage);
                            TCPMsg.Dispose();
                            TCPMsg = null;
                            rqMessage.Dispose();
                            rqMessage = null;
                            Array.Clear(arrTCPData, 0, arrTCPData.Length);
                            GC.Collect();
                        }
                        catch (Exception exp)
                        {
                            Log.Write("Exception Occured in TCP :- " + exp.Message + "," + exp.StackTrace, objINI.KioskIP[iCount]);
                        }
                    }

                }
                objEncResponse.ResponseData = "true";
             }
            catch (Exception ex)
             {
                objEncResponse.ResponseData = ex.Message;
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(objEncResponse.ResponseData));
             return objEncResponse;
        }

        public EncResponse GetLocation(EncRequest objEncRequest)
        {
            DBConnect objDB = new DBConnect();
            DataSet objDS = new DataSet();
            Reply reply = new Reply();
            EncResponse objEncResponse = new EncResponse();
            string query = "", strError = "",fildname = ""; 
           
            try
            {
                string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                string strMessage = JsonConvert.DeserializeObject<string>(DecryptData);

                if (strMessage == "Location")
                {
                    fildname = "location";
                }
                else
                {
                    fildname = "kiosk_id";
                }

                if (objDB.database_type == "mssql")
                {
                    query = "select " + fildname + " from [pcmc].[dbo].[kiosk_master]";
                }
                else
                {
                    query = "select " + fildname + " from pcmc.kiosk_master";
                }
                if (objDB.Select(query, out objDS, out strError))
                {
                    reply.res = true;
                    reply.DS = objDS;
                    reply.strError = "";
                }
                else
                {
                    reply.res = false;
                    reply.DS = null;
                    reply.strError = strError;
                }
            }
            catch(Exception ex)
            {
                reply.res = false;
                reply.DS = null;
                reply.strError = ex.Message;
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(reply));
            return objEncResponse;
        } 
        public EncResponse PatchSave(EncRequest objEncRequest)
        {
            PatchUpdateINI objPatch = new PatchUpdateINI();
            EncResponse objEncResponse = new EncResponse();
            string returnstring = "";
            string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
            objPatch = JsonConvert.DeserializeObject<PatchUpdateINI>(strDecrtyptData);
            try
             { 
                byte[] bPatchData = Convert.FromBase64String(objPatch.patch);
                
                string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
                if (!Directory.Exists(strPatchPath))
                    Directory.CreateDirectory(strPatchPath);

                if (objPatch.PatchName.ToLower().Contains("client"))
                {
                    if (!Directory.Exists(strPatchPath + "\\RMS Client Patch"))
                        Directory.CreateDirectory(strPatchPath + "\\RMS Client Patch");

                    File.WriteAllBytes(strPatchPath + "\\RMS Client Patch\\" + objPatch.PatchName, bPatchData);

                    for (int iCount = 0; iCount < objPatch.KioskIP.Length; iCount++)
                    {
                        string strPatchUpdatedDirPath = strPatchPath + "\\RMS Client Updated Machines";
                        string strFileName = strPatchUpdatedDirPath + "\\MSN- " + objPatch.MachineSrNo[iCount] + " (IP- " + objPatch.KioskIP[iCount] + ").ini";

                        INIFile objClientPatchIni = new INIFile(strFileName);
                        objClientPatchIni.Write("Patch", "Name", objPatch.PatchName);
                        objClientPatchIni.Write("Patch", "Updated", "false");
                        returnstring = "client";
                    }
                }
                else if (objPatch.PatchName.ToLower().Contains("monitor"))
                {
                    if (!Directory.Exists(strPatchPath + "\\RMS Monitor Patch"))
                        Directory.CreateDirectory(strPatchPath + "\\RMS Monitor Patch");

                    File.WriteAllBytes(strPatchPath + "\\RMS Monitor Patch\\" + objPatch.PatchName, bPatchData);

                    for (int iCount = 0; iCount < objPatch.KioskIP.Length; iCount++)
                    {
                        string strPatchUpdatedDirPath = strPatchPath + "\\RMS Monitor Updated Machines";
                        string strFileName = strPatchUpdatedDirPath + "\\MSN- " + objPatch.MachineSrNo[iCount] + " (IP- " + objPatch.KioskIP[iCount] + ").ini";

                        INIFile objClientPatchIni = new INIFile(strFileName);
                        objClientPatchIni.Write("Patch", "Name", objPatch.PatchName);
                        objClientPatchIni.Write("Patch", "Updated", "false");
                        returnstring = "monitor";
                    }
                }
                else if (objPatch.PatchName.ToLower().Contains("lipirdservice"))
                {
                    if (!Directory.Exists(strPatchPath + "\\RD Service Patch"))
                        Directory.CreateDirectory(strPatchPath + "\\RD Service Patch");

                    File.WriteAllBytes(strPatchPath + "\\RD Service Patch\\" + objPatch.PatchName, bPatchData);

                    for (int iCount = 0; iCount < objPatch.KioskIP.Length; iCount++)
                    {
                        string strPatchUpdatedDirPath = strPatchPath + "\\Lipi RD Service Updated Machines";
                        string strFileName = strPatchUpdatedDirPath + "\\MSN- " + objPatch.MachineSrNo[iCount] + " (IP- " + objPatch.KioskIP[iCount] + ").ini";

                        INIFile objClientPatchIni = new INIFile(strFileName);
                        objClientPatchIni.Write("Patch", "Name", objPatch.PatchName);
                        objClientPatchIni.Write("Patch", "Updated", "false");
                        returnstring = "lipirdservice";
                    }
                }
                else
                {
                    returnstring = "Invalid Patch";
                }

                if (returnstring != "Invalid Patch" && objPatch.Instant)
                {
                    for (int iCount = 0; iCount < objPatch.KioskIP.Length; iCount++)
                    {
                        Log.Write("Instant Update Request Comes From Sever So Broadcasting Messsage To IP:-" + objPatch.KioskIP[iCount], objPatch.KioskIP[iCount]);

                        string strPort = ConfigurationManager.AppSettings["Port"].ToString();
                        if (strPort == "")
                            strPort = "11000";


                        string IsTCPCommunication = ConfigurationManager.AppSettings["IsTCPCommunication"].ToString();

                        if (IsTCPCommunication == "false")
                        {
                            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                            IPAddress send_to_address = IPAddress.Parse(objPatch.KioskIP[iCount]);

                            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, Convert.ToInt32(strPort));

                            byte[] send_buffer = null;

                            if (objPatch.PatchName.ToLower().Contains("client"))
                                send_buffer = Encoding.ASCII.GetBytes("clientupdate#" + objPatch.PatchName);
                            else if (objPatch.PatchName.ToLower().Contains("monitor"))
                                send_buffer = Encoding.ASCII.GetBytes("monitorupdate#" + objPatch.PatchName);
                            else
                                send_buffer = Encoding.ASCII.GetBytes("rdserviceupdate#" + objPatch.PatchName);

                            try
                            {
                                sending_socket.SendTo(send_buffer, sending_end_point);
                            }
                            catch (Exception exp)
                            {
                                Log.Write("Exception Occured:- " + exp.Message + "," + exp.StackTrace, objPatch.KioskIP[iCount]);
                            }
                        }
                        else
                        {
                            try
                            {
                                if (TCPMsg == null)
                                    TCPMsg = new MessageQueue(".\\private$\\tcpmsg");

                                TCPMsg.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);

                                TCPMsg.MessageReadPropertyFilter.SetAll();
                                TCPMsg.DefaultPropertiesToSend.Recoverable = true;

                                Message rqMessage = new Message();
                                rqMessage.Label = objPatch.KioskIP[iCount];
                                rqMessage.Recoverable = true;

                                if (objPatch.PatchName.ToLower().Contains("client"))
                                    arrTCPData = Encoding.ASCII.GetBytes("clientupdate#" + objPatch.PatchName);
                                else if (objPatch.PatchName.ToLower().Contains("monitor"))
                                    arrTCPData = Encoding.ASCII.GetBytes("monitorupdate#" + objPatch.PatchName);
                                else
                                    arrTCPData = Encoding.ASCII.GetBytes("rdserviceupdate#" + objPatch.PatchName);
                              
                                rqMessage.BodyStream.Write(arrTCPData, 0, arrTCPData.Length);
                                TCPMsg.Send(rqMessage);
                                TCPMsg.Dispose();
                                TCPMsg = null;
                                rqMessage.Dispose();
                                rqMessage = null;
                                Array.Clear(arrTCPData, 0, arrTCPData.Length);
                                GC.Collect();
                            }
                            catch (Exception exp)
                            {
                                Log.Write("Exception Occured in TCP :- " + exp.Message + "," + exp.StackTrace, objPatch.KioskIP[iCount]);
                            }
                        }
                    }
                }
                objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(returnstring));
               return objEncResponse;
            }
            catch(Exception exp)
            {
                Log.Write("Exception Occured in PatchSave :- " + exp.Message + "," + exp.StackTrace,"");                
            }

            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(returnstring));
            return objEncResponse;
        }


        public EncResponse GetActivatedKioskReport(EncRequest objEncRequest)
        {
            Reply reply = new Reply();
            EncResponse objEncResponse = new EncResponse();
            string strMessage = "";
            DBConnect objDB = new DBConnect();
            DataSet objDS = new DataSet();
            string strerror = "";
            try
            {
                string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                strMessage = JsonConvert.DeserializeObject<string>(strDecrtyptData);
                string[] vs = strMessage.Split('#');
                if (objDB.database_type == "mssql")
                {
                    if (vs.Length == 2)
                    {
                        string query = "";
                        if (vs[0] == "0")
                        {
                            query = "select kiosk_ip as [Machine IP],kiosk_id as [Machine ID],  activation_status as [Activation Status], activation_mode as [Activation Mode], company_name as [Company Name], bank_name as [Bank Name], DATE_FORMAT(Activation_date, '%D, %b %Y %H:%i:%s') as [Activation Date] from [seva_sindhu].[dbo].[kiosk_master]";
                            if (vs[1] == "0")
                            { }
                            else if (vs[1] == "1")
                            {
                                query += " where activation_status='Activate'";
                            }
                            else if (vs[1] == "2")
                            {
                                query += " where activation_status='Pending'";
                            }

                            else
                            {
                                reply.DS = null;
                                reply.res = false;
                                reply.strError = "Improper Request";
                            }
                            query += " ORDER BY activation_status = 'Pending' DESC ";
                        }
                        else if (vs[0] == "1")
                        {
                            query = "select kiosk_ip as [Machine IP],kiosk_id as [Machine ID],  activation_status as [Activation Status], activation_mode as [Activation Mode], company_name as [Company Name], bank_name as [Bank Name], DATE_FORMAT(Activation_date, '%D, %b %Y %H:%i:%s') as [Activation Date] from [seva_sindhu].[dbo].[kiosk_master] where activation_mode='Online'";
                            if (vs[1] == "0")
                            { }
                            else if (vs[1] == "1")
                            {
                                query += " and activation_status='Activate'";
                            }
                            else if (vs[1] == "2")
                            {
                                query += " and activation_status='Pending'";
                            }
                            else
                            {
                                reply.DS = null;
                                reply.res = false;
                                reply.strError = "Improper Request";
                            }
                        }
                        else if (vs[0] == "2")
                        {
                            query = "select kiosk_ip as [Machine IP],kiosk_id as [Machine ID],  activation_status as [Activation Status], activation_mode as [Activation Mode], company_name as [Company Name], bank_name as [Bank Name], DATE_FORMAT(Activation_date, '%D, %b %Y %H:%i:%s') as [Activation Date] from [seva_sindhu].[dbo].[kiosk_master]  where activation_mode='Offline'";
                            if (vs[1] == "0")
                            { }
                            else if (vs[1] == "1")
                            {
                                query += " and activation_status='Activate'";
                            }
                            else if (vs[1] == "2")
                            {
                                query += " and activation_status='Pending'";
                            }
                            else
                            {
                                reply.DS = null;
                                reply.res = false;
                                reply.strError = "Improper Request";
                            }
                        }
                        else
                        {
                            reply.DS = null;
                            reply.res = false;
                            reply.strError = "Improper Request";
                        }
                        if (objDB.Select(query, out objDS, out strerror) && objDS != null && objDS.Tables[0].Rows.Count > 0)
                        {
                            reply.DS = objDS;
                            reply.res = true;
                            reply.strError = "";
                        }
                        else
                        {
                            reply.DS = null;
                            reply.res = false;
                            reply.strError = strerror;
                        }
                    }
                    else
                    {
                        reply.DS = null;
                        reply.res = false;
                        reply.strError = "Improper Request";
                    }
                }
                else 
                {
                    if (vs.Length == 2)
                    {
                        string query = "";
                        if (vs[0] == "0")
                        {
                            query = "select kiosk_ip as 'Machine IP',kiosk_id as 'Machine ID',  activation_status as 'Activation Status', activation_mode as 'Activation Mode', company_name as 'Company Name',  DATE_FORMAT(Activation_date, '%D, %b %Y %H:%i:%s') as 'Activation Date' from seva_sindhu.kiosk_master";
                            if (vs[1] == "0")
                            { }
                            else if (vs[1] == "1")
                            {
                                query += " where activation_status='Activate'";
                            }
                            else if (vs[1] == "2")
                            {
                                query += " where activation_status='Pending'";
                            }

                            else
                            {
                                reply.DS = null;
                                reply.res = false;
                                reply.strError = "Improper Request";
                            }
                            query += " ORDER BY activation_status = 'Pending' DESC ";
                        }
                        else if (vs[0] == "1")
                        {
                            query = "select kiosk_ip as 'Machine IP',kiosk_id as 'Machine ID',  activation_status as 'Activation Status', activation_mode as 'Activation Mode', company_name as 'Company Name',  DATE_FORMAT(Activation_date, '%D, %b %Y %H:%i:%s') as 'Activation Date' from seva_sindhu.kiosk_master where activation_mode='Online'";
                            if (vs[1] == "0")
                            { }
                            else if (vs[1] == "1")
                            {
                                query += " and activation_status='Activate'";
                            }
                            else if (vs[1] == "2")
                            {
                                query += " and activation_status='Pending'";
                            }
                            else
                            {
                                reply.DS = null;
                                reply.res = false;
                                reply.strError = "Improper Request";
                            }
                        }
                        else if (vs[0] == "2")
                        {
                            query = "select kiosk_ip as 'Machine IP',kiosk_id as 'Machine ID',  activation_status as 'Activation Status', activation_mode as 'Activation Mode', company_name as 'Company Name',  DATE_FORMAT(Activation_date, '%D, %b %Y %H:%i:%s') as 'Activation Date' from seva_sindhu.kiosk_master where activation_mode='Offline'";
                            if (vs[1] == "0")
                            { }
                            else if (vs[1] == "1")
                            {
                                query += " and activation_status='Activate'";
                            }
                            else if (vs[1] == "2")
                            {
                                query += " and activation_status='Pending'";
                            }
                            else
                            {
                                reply.DS = null;
                                reply.res = false;
                                reply.strError = "Improper Request";
                            }
                        }
                        else
                        {
                            reply.DS = null;
                            reply.res = false;
                            reply.strError = "Improper Request";
                        }
                        if (objDB.Select(query, out objDS, out strerror) && objDS != null && objDS.Tables[0].Rows.Count > 0)
                        {
                            reply.DS = objDS;
                            reply.res = true;
                            reply.strError = "";
                        }
                        else
                        {
                            reply.DS = null;
                            reply.res = false;
                            reply.strError = strerror;
                        }
                    }
                    else
                    {
                        reply.DS = null;
                        reply.res = false;
                        reply.strError = "Improper Request";
                    }
                }
               
            }
            catch (Exception ex)
            {
                reply.DS = null;
                reply.res = false;
                reply.strError = ex.Message;
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(reply));
            return objEncResponse;
        }

        public EncResponse UpdatePassword(EncRequest objEncRequest)
        {
            DBConnect objDB = new DBConnect();
            UserDetailsInfo objUserPass = new UserDetailsInfo();
            EncResponse objEncResponse = new EncResponse();
            DataSet objDS = new DataSet();
            Reply response = new Reply();
            String query_select = "", strError;
            bool Res;

            String query = "";

            try
            {
                string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                objUserPass = JsonConvert.DeserializeObject<UserDetailsInfo>(strDecrtyptData);
                query = "select username from pcmc.login where pcmc.login.username = '" + objUserPass.UserName + "' ";

                Res = objDB.Select(query, out objDS, out strError);

                if (Res == true && objDS.Tables[0].Rows.Count >= 1)
                {
                    string EncryptPassword = AesGcm256.Encrypt(objUserPass.Pwd);
                    query = "update pcmc.login set password='" + EncryptPassword + "' " + " where pcmc.login.username='" + objUserPass.UserName + "' ";


                    bool resp = objDB.Update(query, out strError);

                    if (resp == true)
                    {
                        response.res = true;

                    }
                    else
                    {
                        response.res = false;
                        response.strError = strError;

                    }
                }
                else
                {
                    response.res = false;
                    if (strError == "")
                        strError = "User with such name doesn't exist";
                    response.strError = strError;
                }


            }
            catch (Exception ex)
            {
                response.res = false;
                response.strError = ex.Message;
              
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(response));
            return objEncResponse;
        }


        public EncResponse DeleteUser(EncRequest objEncRequest)
        {
            string returnString = "", strError;
            string Username="";
            string query = "";
            EncResponse objEncResponse = new EncResponse();
            try
            {
                string strDecrtyptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                Username = JsonConvert.DeserializeObject<string>(strDecrtyptData);
                DBConnect objDB = new DBConnect();
                DataSet objDS = new DataSet();
                if (objDB.database_type == "mssql")
                {
                    query = "delete from pcmc.login where username ='" + Username + "'";
                }
                else
                {
                    query = "delete from pcmc.login where username ='" + Username + "'";
                }
              
                if (objDB.Delete(query, out strError))
                {
                    returnString = "success";
                }
                else
                {
                    returnString = "fail";
                }
            }
            catch
            { }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(returnString));
            return objEncResponse;
        }

        public EncResponse GetKioskReport(EncRequest objEncRequest)
        {
            EncResponse objEncResponse = new EncResponse();
            Reply reply = new Reply();
            DBConnect objDB = new DBConnect();
            DataSet objDS = new DataSet();
            string query = "", strError = "";
           
            try
            {
                string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                DecryptData = JsonConvert.DeserializeObject<string>(DecryptData);
                string[] strMessage = DecryptData.Split('#');
                try
                {
                    if (strMessage[0].Trim() == "MachineID")
                    {
                        if (objDB.database_type == "mssql")
                        {
                            query = "select [pcmc].[kiosk_master].[kiosk_id]  from [pcmc].[kiosk_master] inner join [pcmc].[health_recent] on [pcmc].[health_recent].[f_kiosk_id]=[pcmc].[kiosk_master].[kiosk_id]";
                        }
                        else
                        {
                            query = "select pcmc.kiosk_master.kiosk_id  from pcmc.kiosk_master inner join pcmc.health_recent on pcmc.health_recent.f_kiosk_id=pcmc.kiosk_master.kiosk_id";
                        }
                    }
                    else if (strMessage[0].Trim() == "MachineIP")
                    {
                        if (objDB.database_type == "mssql")
                        {
                            query = "select *  from [pcmc].[kiosk_master] inner join [pcmc].[health_recent] on [pcmc].[health_recent].[f_kiosk_id]=[pcmc].[kiosk_master].[kiosk_id]";
                        }
                        else
                        {
                            query = "select * from pcmc.kiosk_master inner join pcmc.health_recent on pcmc.health_recent.f_kiosk_id=pcmc.kiosk_master.kiosk_id";
                        }
                    }
                    else
                    {
                        if (strMessage[0].Trim() == "1")
                        {
                            query = "select kiosk_ip as 'Machine IP', kiosk_id as 'Machine ID',HR.device2_status as 'HDD_Encryption',DATE_FORMAT(HR.date_time, '%D, %b %Y %H:%i:%s') as Date from pcmc.kiosk_master as KM inner join pcmc.health_recent as HR on KM.kiosk_id = HR.f_kiosk_id";
                            if (strMessage[1].Trim() == "ID")
                            {
                                query += " where KM.kiosk_id='" + strMessage[2] + "'";
                            }
                            else if (strMessage[1].Trim() == "IP")
                            {
                                query += " where KM.kiosk_ip='" + strMessage[2] + "'";
                            }
                        }
                        else if (strMessage[0].Trim() == "2")
                        {
                            DateTime StartDateTime = DateTime.ParseExact(strMessage[1] + " 00:00:00", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            DateTime EndDateTime = DateTime.ParseExact(strMessage[2] + " 23:59:59", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            int j = (Convert.ToInt32(strMessage[2].Substring(6, 4)) - Convert.ToInt32(strMessage[1].Substring(6, 4))) * 12 + (Convert.ToInt32(strMessage[2].Substring(3, 2)) - Convert.ToInt32(strMessage[1].Substring(3, 2)));
                          
                              query = "SELECT kiosk_ip as 'Machine IP', kiosk_id as 'Machine ID',YR19.Time_Base_Access,DATE_FORMAT(YR19.Datetime, '%D, %b %Y %H:%i:%s') as Date FROM pcmc.kiosk_master as KM inner join pcmc_" + StartDateTime.ToString("yy") + ".pcmc_health_detail_" + StartDateTime.ToString("MMM") + " as YR19 on KM.kiosk_id = YR19.f_kioskid where";
                            string idwise = "";
                            if (strMessage[3] == "ID")
                            {
                                idwise = " KM.kiosk_id='" + strMessage[4] + "' and";
                            }
                            else if (strMessage[3] == "IP")
                            {
                                idwise = " KM.kiosk_ip='" + strMessage[4] + "' and";
                            }
                            query += idwise + " YR19.Datetime >= '" + StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                            for (int i = 1; i <= j; i++)
                            {
                                query += "union SELECT kiosk_ip, kiosk_id,YR19.Time_Base_Access,DATE_FORMAT(YR19.Datetime, '%D, %b %Y %H:%i:%s') as Date FROM pcmc.kiosk_master as KM inner join pcmc_" + StartDateTime.AddMonths(i).ToString("yy") + ".pcmc_health_detail_" + StartDateTime.AddMonths(i).ToString("MMM") + " as YR19 on KM.kiosk_id = YR19.f_kioskid ";
                                if (idwise != "")
                                {
                                    query += " where " + idwise.Substring(0, idwise.Length - 3);
                                }
                                if (j == i && idwise != "")
                                {
                                    query += " and ";
                                }
                                else if (j == i && idwise == "")
                                {
                                    query += " where ";
                                }
                            }
                            if (j == 0)
                                query += " and YR19.Datetime <='" + EndDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                            else
                                query += "  YR19.Datetime <='" + EndDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        }
                        else if (strMessage[0] == "3")
                        {

                            DateTime StartDateTime = DateTime.ParseExact(strMessage[1] + " 00:00:00", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            DateTime EndDateTime = DateTime.ParseExact(strMessage[2] + " 23:59:59", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            int j = (Convert.ToInt32(strMessage[2].Substring(6, 4)) - Convert.ToInt32(strMessage[1].Substring(6, 4))) * 12 + (Convert.ToInt32(strMessage[2].Substring(3, 2)) - Convert.ToInt32(strMessage[1].Substring(3, 2)));
                            query = " SELECT kiosk_ip as 'Machine IP', kiosk_id as 'Machine ID', YR19.task, YR19.result, DATE_FORMAT(YR19.Datetime, '%D, %b %Y %H:%i:%s') as 'Date' FROM pcmc.kiosk_master as KM inner join pcmc_" + StartDateTime.ToString("yy") + ".whitelist_health_detail_" + StartDateTime.ToString("MMM") + " as YR19 on KM.kiosk_id = YR19.f_kioskid where";
                            string idwise = "";
                            if (strMessage[3] == "ID")
                            {
                                idwise = " KM.kiosk_id='" + strMessage[4] + "' and";
                            }
                            else if (strMessage[3] == "IP")
                            {
                                idwise = " KM.kiosk_ip='" + strMessage[4] + "' and";
                            }
                            query += idwise + " YR19.Datetime >= '" + StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                            for (int i = 1; i <= j; i++)
                            {
                                query += "union SELECT kiosk_ip, kiosk_id,YR19.task, YR19.result, DATE_FORMAT(YR19.Datetime, '%D, %b %Y %H:%i:%s') as Date FROM pcmc.kiosk_master as KM inner join pcmc_" + StartDateTime.AddMonths(i).ToString("yy") + ".whitelist_health_detail_" + StartDateTime.AddMonths(i).ToString("MMM") + " as YR19 on KM.kiosk_id = YR19.f_kioskid ";
                                if (idwise != "")
                                {
                                    query += " where " + idwise.Substring(0, idwise.Length - 3);
                                }
                                if (j == i && idwise != "")
                                {
                                    query += " and ";
                                }
                                else if (j == i && idwise == "")
                                {
                                    query += " where ";
                                }
                            }
                            if (j == 0)
                                query += " and YR19.Datetime<='" + EndDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                            else
                                query += "  YR19.Datetime<='" + EndDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        }

                        else if (strMessage[0] == "4")
                        {

                            DateTime StartDateTime = DateTime.ParseExact(strMessage[1] + " 00:00:00", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            DateTime EndDateTime = DateTime.ParseExact(strMessage[2] + " 23:59:59", "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                            int j = (Convert.ToInt32(strMessage[2].Substring(6, 4)) - Convert.ToInt32(strMessage[1].Substring(6, 4))) * 12 + (Convert.ToInt32(strMessage[2].Substring(3, 2)) - Convert.ToInt32(strMessage[1].Substring(3, 2)));
                            query = "SELECT kiosk_ip as 'Machine IP', kiosk_id as 'Machine ID', YR19.Process, YR19.Action,YR19.Status, DATE_FORMAT(YR19.Datetime, '%D, %b %Y %H:%i:%s') as Date FROM pcmc.kiosk_master as KM inner join pcmc_" + StartDateTime.ToString("yy") + ".process_health_detail_" + StartDateTime.ToString("MMM") + " as YR19 on KM.kiosk_id = YR19.f_kioskid where";
                            string idwise = "";
                            if (strMessage[3] == "ID")
                            {
                                idwise = " KM.kiosk_id='" + strMessage[4] + "' and";
                            }
                            else if (strMessage[3] == "IP")
                            {
                                idwise = " KM.kiosk_ip='" + strMessage[4] + "' and";
                            }
                            query += idwise + " YR19.Datetime >= '" + StartDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                            for (int i = 1; i <= j; i++)
                            {
                                query += " union SELECT kiosk_ip, kiosk_id,YR19.Process, YR19.Action,YR19.Status, DATE_FORMAT(YR19.Datetime, '%D, %b %Y %H:%i:%s') as Date FROM pcmc.kiosk_master as KM inner join pcmc_" + StartDateTime.AddMonths(i).ToString("yy") + ".process_health_detail_" + StartDateTime.AddMonths(i).ToString("MMM") + " as YR19 on KM.kiosk_id = YR19.f_kioskid ";
                                if (idwise != "")
                                {
                                    query += " where " + idwise.Substring(0, idwise.Length - 3);
                                }
                                if (j == i && idwise != "")
                                {
                                    query += " and ";
                                }
                                else if (j == i && idwise == "")
                                {
                                    query += " where ";
                                }
                            }
                            if (j == 0)
                                query += " and YR19.Datetime<='" + EndDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                            else
                                query += "  YR19.Datetime<='" + EndDateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                        }
                    }

                    if (objDB.Select(query, out objDS, out strError) && objDS != null && objDS.Tables[0].Rows.Count > 0)
                    {
                        reply.res = true;
                        reply.DS = objDS;
                        reply.strError = "";
                    }
                    else
                    {
                        reply.res = false;
                        reply.DS = null;
                        reply.strError = strError;
                    }
                }

                catch (Exception ex)
                {
                    reply.res = false;
                    reply.DS = null;
                    reply.strError = ex.Message;
                }
            }
            catch(Exception ex)
            {
                reply.res = false;
                reply.DS = null;
                reply.strError = ex.Message;
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(reply));
            return objEncResponse;
        }

        public EncResponse GetScreenData(EncRequest objEncRequest)
        {
            DBConnect objDB = new DBConnect();
            DataSet objDS = new DataSet();
            string ret = "fail";
           
            EncResponse objEncResponse = new EncResponse();
            try
            {
                string strAckDet = "";
                strAckDet = AesGcm256.Decrypt(objEncRequest.RequestData);
                strAckDet = JsonConvert.DeserializeObject<string>(strAckDet);
                string[] vs = strAckDet.Split('#');
                if (vs.Length == 2)
                {
                    string ip = "";
                    if (vs[0] == "1")
                        ip = vs[1];
                    else if (vs[0] == "2")
                    {
                        string query = "Select kiosk_ip from pcmc.kiosk_master where kiosk_id='" + vs[1] + "'";
                        string str = "";
                        if (objDB.Select(query, out objDS, out str) && objDS != null && objDS.Tables[0].Rows.Count == 1)
                        {
                            ip = objDS.Tables[0].Rows[0]["kiosk_ip"].ToString();
                        }
                        else
                        {
                            ret = "fail";
                            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(ret));
                            return objEncResponse;
                        }
                    }
                    else
                    {
                        ret = "fail";
                        objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(ret));
                        return objEncResponse;
                    }
                    if (!Directory.Exists(ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString() + "\\ExportedScreenCapture\\" + ip))
                    {
                        ret = "fail";
                        objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(ret));
                        return objEncResponse;
                    }
                    string strFileUploadName = ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString() + "\\ExportedScreenCapture\\" + ip;
                    string szFileZipPath = ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString() + "\\ExportedScreenCapture\\" + ip + ".zip";
                    using (ZipFile zips = new ZipFile(szFileZipPath))
                    {
                        string[] vs1 = Directory.GetFiles(strFileUploadName);
                        zips.Password = "L!p!d@t@";
                        zips.AddFiles(vs1, ip);
                        zips.Save();
                        byte[] byFileName = new byte[0];
                        byte[] buffer = File.ReadAllBytes(szFileZipPath);
                        File.Delete(szFileZipPath);
                        ret = Convert.ToBase64String(buffer);
                    }
                }
                else
                {
                    ret = "fail";
                    objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(ret));
                    return objEncResponse;
                }
            }
            catch (Exception ex)
            { ret = "fail"; }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(ret));
            return objEncResponse;
        }
        public string KioskDetails(string strKioskDetails)
        {
            string strMessage = "";
            string strDocketNo = "";
            string strQuery, strError, strClientVer, strClientIP = "";

            DateTime objClientTime = DateTime.Now;
            DBConnect objDB = new DBConnect();
            DataSet objDS = null;
            DataSet objdataset = null;

            try
            {  
                byte[] byStr = Convert.FromBase64String(strKioskDetails);
                string[] strMsgFields = Encoding.ASCII.GetString(byStr).Split('#');              
                
                strClientIP = strMsgFields[1];

                Log.Write("KioskDetails received- " + Encoding.ASCII.GetString(byStr),strClientIP );

                //Get docket no
                strDocketNo = Crc32.Compute(Encoding.Default.GetBytes(strMsgFields[1])).ToString("X").PadLeft(8, '0');

                //To check mac address is not null
                if (strMsgFields.Length > 7 && strMsgFields[3] != null)
                {
                    strQuery = "";

                    if (objDB.database_type == "mssql")
                       strQuery = "select * from [pcmc].[dbo].[kiosk_master] where mac_address = '" + strMsgFields[3] + "'";
                    else
                       strQuery = "select * from pcmc.kiosk_master where mac_address = '" + strMsgFields[3] + "'";
                    
                    if (objDS == null)
                        objDS = new DataSet();

                    if (objDB.Select(strQuery, out objdataset, out strError) && objdataset != null && objdataset.Tables[0].Rows.Count == 0)
                    {
                        Log.Write("New Machine Detected So Insertion New Record In The Database", strClientIP);

                        string strKioskID, strMacAddress, strChecksum, strMachineNo,  strDrives, strActvdOn, strLastPatchUpdated;
                        strKioskID = strMacAddress = strChecksum = strMachineNo = strClientVer = strActvdOn = strDrives = strLastPatchUpdated = "";
                        
                        strKioskID = strMsgFields[2];           //Kiosk ID
                        strMacAddress = strMsgFields[3];        //Mac Address
                        strMachineNo = strMsgFields[4];         //Machine Serial No
                        strClientVer = strMsgFields[5];         //Verion of Client,RD Service and RMS Monitor
                        strDrives = strMsgFields[6];            //Drives 
                        strLastPatchUpdated = strMsgFields[7];  //Last patch Updated
                        strActvdOn = DateTime.Now.ToString("dd-MMM-yy HH:mm:ss");      //Activation Date

                        try
                        {
                            if (strMacAddress == "" || strMacAddress == "00000000000000E0" || strMacAddress == "FFFFFFFFFFFFFFFF")
                               Log.Write("MacAddress is blank/Invalid in KioskDetails. MAC Address:- " + strMacAddress, strClientIP);
                            else
                            {
                                strQuery = "";
                                if (objDB.database_type == "mssql")
                                {
                                    strQuery = "insert into [pcmc].[dbo].[kiosk_master]  (kiosk_ip, kiosk_id, mac_address, machine_serial_no, client_version, drives, last_patch_updated) " +
                                              "select '" + strClientIP + "', '" + strKioskID + "', '" + strMacAddress + "' , '" + strMachineNo + "' , '" + strClientVer + "' , '" + strDrives + "' , '" + strLastPatchUpdated + "'";
                                }
                                else
                                {
                                    strQuery = "insert into pcmc.kiosk_master  (kiosk_ip, kiosk_id, mac_address, machine_serial_no, client_version, drives, last_patch_updated) " +
                                              "select '" + strClientIP + "', '" + strKioskID + "', '" + strMacAddress + "' , '" + strMachineNo + "' , '" + strClientVer + "' , '" + strDrives + "' , '" + strLastPatchUpdated + "'";
                                }
                                
                                if (objDB.Insert(strQuery, out strError))
                                {
                                    Log.Write("KioskDetails saved:- IP " + strClientIP, strClientIP);
                                    strMessage = "Success:" + strDocketNo;
                                }
                                else
                                {
                                    Log.Write("KioskDetails not saved. Query :- " + strQuery + ",Error:- " + strError, strClientIP);
                                    strMessage = "Fail";
                                }
                            }
                        }
                        catch (Exception excp)
                        {
                            Log.Write("Exception in DB : " + excp.Message.ToString() + "," + excp.StackTrace, strClientIP);
                        }
                    }
                    else if (objdataset != null && objdataset.Tables[0].Rows.Count == 1 && objdataset.Tables[0].Rows[0]["mac_address"].ToString() == strMsgFields[3]) // Log if kiosk details already exist
                    {
                        Log.Write(" Machine Already Registered So Updating The Record In The Database", strClientIP);

                        string strKioskID, strMacAddress, strMachineNo, strDrives, strActvdOn, strLastPatchUpdated;
                        strKioskID = strMacAddress = strMachineNo = strClientVer = strActvdOn = strDrives = strLastPatchUpdated = "";

                        strKioskID = strMsgFields[2];           //Kiosk ID
                        strMacAddress = strMsgFields[3];        //Mac Address
                        strMachineNo = strMsgFields[4];         //Machine Serial No
                        strClientVer = strMsgFields[5];         //Verion of Client,RD Service and RMS Monitor
                        strDrives = strMsgFields[6];            //Drives 
                        strLastPatchUpdated = strMsgFields[7];  //Last patch Updated

                        strQuery = "";

                        if (objDB.database_type == "mssql")
                        {
                            strQuery = "update [pcmc].[dbo].[kiosk_master] set " +
                                  "kiosk_id = '" + strKioskID + "', kiosk_ip = '" + strClientIP + "', last_patch_updated = '" + strLastPatchUpdated + "', machine_serial_no = '" + strMachineNo + "', drives = '" + strDrives + "', client_version = '" + strClientVer + "' " +
                                  "where mac_address = '" + strMacAddress + "'";
                        }
                        else
                        {
                            strQuery = "update pcmc.kiosk_master set " +
                                  "kiosk_id = '" + strKioskID + "', kiosk_ip = '" + strClientIP + "', last_patch_updated = '" + strLastPatchUpdated + "', machine_serial_no = '" + strMachineNo + "', drives = '" + strDrives + "', client_version = '" + strClientVer + "' " +
                                  "where mac_address = '" + strMacAddress + "'";
                        }

                        // Execute the query and log the result
                        if (objDB.Update(strQuery, out strError))
                        {
                            Log.Write("KioskDetails Updated", strClientIP);
                            strMessage = "Success:" + strDocketNo;
                        }
                        else
                        {
                            Log.Write("KioskDetails not Updated. Query :- " + strQuery + ",Error:-" + strError, strClientIP);
                            strMessage = "Fail";
                        }
                    }
                    else
                    {
                        Log.Write("Kiosk Details Received:- " + Encoding.ASCII.GetString(byStr), strClientIP);
                        Log.Write("Mac Address Is Already Registered", strClientIP);
                        strMessage = "Fail";
                    }
                }
            }
            catch (Exception excp)
            {
                Log.Write("Excp Occured in KioskDetail Function Message:- " + excp.Message.ToString() + ", StackTrace :- " + excp.StackTrace, strClientIP);
                strMessage ="exception";
        
            }
            return strMessage;
        }


        public EncResponse GetKioskReportWhiteList(EncRequest objEncRequest)
        {
            EncResponse objEncResponse = new EncResponse();
            Reply reply = new Reply();
            DBConnect objDB = new DBConnect();
            DataSet objDs = new DataSet();
            string query = "", strError = "";

            try
            {
                string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                DecryptData = JsonConvert.DeserializeObject<string>(DecryptData);
                string[] strMessage = DecryptData.Split('#');

                if (strMessage[0].Trim() == "MachineID")
                {
                    if (objDB.database_type == "mssql")
                    {
                        query = "select [pcmc].[kiosk_master].[kiosk_id] from [pcmc].[kiosk_master] inner join [pcmc].[whitelist_recent] on [pcmc].[whitelist_recent].[f_kioskid]=[pcmc].[kiosk_master].[kiosk_id]";
                    }
                    else
                    {
                        query = "select pcmc.kiosk_master.kiosk_id from pcmc.kiosk_master inner join pcmc.whitelist_recent on pcmc.whitelist_recent.f_kioskid=pcmc.kiosk_master.kiosk_id";
                    }
                    if (objDB.Select(query, out objDs, out strError) && objDs != null && objDs.Tables[0].Rows.Count > 0)
                    {
                        reply.res = true;
                        reply.DS = objDs;
                        reply.strError = "";
                    }
                    else if (objDs != null)
                    {
                        reply.res = false;
                        reply.DS = null;
                        reply.strError = "No Machine Registered";
                    }
                    else
                    {
                        reply.res = false;
                        reply.DS = null;
                        reply.strError = strError;
                    }
                }
                else if (strMessage[0].Trim() == "MachineIP")
                {
                    if (objDB.database_type == "mssql")
                    {
                        query = "select * from [pcmc].[kiosk_master] inner join [pcmc].[whitelist_recent] on [pcmc].[whitelist_recent].[f_kioskid]=[pcmc].[kiosk_master].[kiosk_id]";
                    }
                    else
                    {
                        query = "select * from pcmc.kiosk_master inner join pcmc.whitelist_recent on pcmc.whitelist_recent.f_kioskid=pcmc.kiosk_master.kiosk_id";
                    }
                    
                    if (objDB.Select(query, out objDs, out strError) && objDs != null && objDs.Tables[0].Rows.Count > 0)
                    {
                        reply.DS = objDs;
                        reply.res = true;
                        reply.strError = "";
                    }
                    else if (objDs != null)
                    {
                        reply.DS = null;
                        reply.res = false;
                        reply.strError = "No Machine Registered";
                    }
                    else
                    {
                        reply.DS = null;
                        reply.res = false;
                        reply.strError = strError;
                    }
                }
                else
                {
                    if (strMessage.Length == 2)
                    {
                        int i = Convert.ToInt32(strMessage[0]);
                        if (i == 1)
                        {
                            string path = ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString() + "\\ExportedWhitelist\\" + strMessage[1];
                            string file = Extract(path, "ExportList");
                            StreamReader read = new StreamReader(path + "\\" + file);
                            string strData = read.ReadToEnd();
                            string[] list = strData.Split('\n');
                            DataSet customerOrders = new DataSet("Whitelist");

                            DataTable ordersTable = customerOrders.Tables.Add("WhitelistTable");

                            DataColumn pkOrderID =
                                ordersTable.Columns.Add("Sr. Number", typeof(Int32));
                            ordersTable.Columns.Add("Process Name with Path", typeof(string));
                            ordersTable.Columns.Add("Status", typeof(string));
                            for (int j = 0; j < list.Length - 1; j++)
                            {
                                string[] vs1 = list[j].Split('#');
                                DataRow dataRow = customerOrders.Tables["WhitelistTable"].NewRow();
                                dataRow["Sr. Number"] = Convert.ToInt32(vs1[0]);
                                dataRow["Process Name with Path"] = vs1[1];
                                int k = 2;
                                for (k = 2; k < vs1.Length - 2; k++)
                                    dataRow["Process Name with Path"] += "#" + vs1[k];
                                dataRow["Status"] = vs1[k];
                                customerOrders.Tables["WhitelistTable"].Rows.Add(dataRow);
                            }
                            reply.DS = customerOrders;
                            reply.strError = "";
                            reply.res = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                reply.res = false;
                reply.DS = null;
                reply.strError = ex.Message;
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(reply));
            return objEncResponse;
        }

        public string Extract(string strPatchPath, string strPatchName)
        {
            string fileName = "";
            if (strPatchPath != null)
            {
                //Log.WriteFile("Started extraction at:" + strPatchPath + "\\" + strPatchName);
                using (ZipFile zip = ZipFile.Read(strPatchPath + "\\" + strPatchName + ".zip"))
                {
                    foreach (ZipEntry zipEntry in zip)
                    {
                        try
                        {
                            zipEntry.ExtractWithPassword(strPatchPath, ExtractExistingFileAction.OverwriteSilently, "L!p!d@t@");  // overwrite == true  
                            fileName = zipEntry.FileName;                                                                          //objLog.WriteFile("Extract File Name: " + zipEntry.ToString());
                        }
                        catch (Exception ex)  //added catch region to extract file when .tmp and .PendingOverwritefile exists
                        {
                            //Log.WriteFile("Exception in extracting file:" + ex.Message);
                            foreach (var postFix in new[] { ".tmp", ".PendingOverwrite" })
                            {
                                var errorPath = Path.Combine(strPatchPath + "\\", zipEntry.FileName) + postFix;
                                if (File.Exists(errorPath))
                                {
                                    try
                                    {
                                        File.Delete(errorPath);

                                    }
                                    catch (Exception Excp)
                                    {

                                        throw Excp;
                                    }
                                }
                            }

                            zipEntry.Extract(strPatchPath, ExtractExistingFileAction.OverwriteSilently);

                        }
                    }


                }
            }
            return fileName;
        }
     
        
        public string LipiLogData(string strLogData)
        {
            string returnString = "";
            try
            {
                string IP = strLogData.Substring(0, strLogData.IndexOf('#'));
                strLogData = strLogData.Substring(strLogData.IndexOf('#') + 1);
                byte[] logdata = Convert.FromBase64String(strLogData);
                string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
                string strCommFileName = strPatchPath + "\\Command.ini";
                INIFile objCommandIni = new INIFile(strCommFileName);
                if (!Directory.Exists(ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString() + "\\" + "(IP- " + IP + ")"))
                {
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString() + "\\" + "(IP- " + IP + ")");
                }
                File.WriteAllBytes(ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString() + "\\" + "(IP- " + IP + ")\\" + objCommandIni.Read(IP, "Command1", "") + ".zip", logdata);
                Log.Write("Log Zip File Saved Successfully At Path"+ ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString() + "\\" + "(IP- " + IP + ")\\", IP);


                Log.Write("Resetting The Command INI After Updation For IP:-" + IP, IP);
                objCommandIni.Write(IP, "CommandCount", "0");
                objCommandIni.Write(IP, "Command1", "");
                objCommandIni.Write(IP, "Command2", "");
                objCommandIni.Write(IP, "Command3", "");
                objCommandIni.Write(IP, "Command4", "");                   // Resetting the INI After Saving Log Zip File 
                returnString = "Success";

            }
            catch
            {
                returnString = "fail";
            }
            return returnString;
        }


        //Client Funcation
        //public EncResponse HealthData(EncRequest objEncRequest)
        //{
        //    EncResponse objEncResponse = new EncResponse();
        //    ResLipiHealth objResponse = new ResLipiHealth();
        //    string strHealthDateTime, strError, strClientIP, strMachineSrNo;
        //    strHealthDateTime = strError = strClientIP = strMachineSrNo = "";

        //    DBConnect objDB = new DBConnect();
        //    DataSet objDS = null;
        //    DateTime objClientTime = DateTime.Now;

        //    string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
        //    DecryptData = JsonConvert.DeserializeObject<string>(DecryptData);
        //    byte[] byStr = Convert.FromBase64String(DecryptData);
        //    Log.Write("Message Recieved For HealthData:- " + Encoding.ASCII.GetString(byStr), strClientIP);

        //    string[] strMsgFields = Encoding.ASCII.GetString(byStr).Split(new string[] { "$" }, StringSplitOptions.RemoveEmptyEntries);

        //    strClientIP = strMsgFields[1];
        //    strMachineSrNo = strMsgFields[2];
        //    strHealthDateTime = strMsgFields[3];

        //    Log.Write("After Splitting Message Fields HealthData Recieved Is:- " + strMsgFields[4], strClientIP);
        //    //To get device count
        //    string[] arrHealthData = strMsgFields[4].Split('#');
        //    int iDeviceCount = Convert.ToInt32(arrHealthData[0]);
        //    string query = "";

        //    if (strMsgFields != null && strMsgFields[0] == "02" && arrHealthData != null)
        //    {
        //        try
        //        {
        //            query = "";

        //            if (objDB.database_type == "mssql")
        //            {
        //                query = "select * from [pcmc].[dbo].[health_recent] where " +
        //                       "f_kiosk_id = (select kiosk_id from [pcmc].[dbo].[kiosk_master] where kiosk_ip = '" + strClientIP + "')";
        //            }
        //            else
        //            {
        //                query = "select * from pcmc.health_recent where " +
        //                      "f_kiosk_id = (select kiosk_id from pcmc.kiosk_master where kiosk_ip = '" + strClientIP + "')";
        //            }

        //            if (objDS == null)
        //                objDS = new DataSet();

        //            objDS.Clear();

        //            if (objDB.Select(query, out objDS, out strError) && objDS != null && objDS.Tables[0].Rows.Count == 0)
        //            {
        //                query = "";

        //                if (objDB.database_type == "mssql")
        //                {
        //                    query = "insert into [pcmc].[dbo].[health_recent](f_kiosk_id, client_status_with_rms, ";

        //                    string strTemp = "";
        //                    for (int iCount = 1; iCount <= iDeviceCount; iCount++)
        //                    {
        //                        query += "device" + iCount + "_status, ";
        //                        strTemp += arrHealthData[iCount] + "', '";
        //                    }

        //                    query += "date_time)" + "select kiosk_id, 'Connected', '" + strTemp + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) +
        //                             "-" + strHealthDateTime.Substring(6) + "' from [pcmc].[dbo].[kiosk_master] where kiosk_ip = '" + strClientIP + "'";

        //                }
        //                else
        //                {
        //                    query = "insert into pcmc.health_recent(f_kiosk_id, client_status_with_rms, ";

        //                    string strTemp = "";
        //                    for (int iCount = 1; iCount <= iDeviceCount; iCount++)
        //                    {
        //                        query += "device" + iCount + "_status, ";
        //                        strTemp += arrHealthData[iCount] + "', '";
        //                    }

        //                    query += "date_time)" + "select kiosk_id, 'Connected', '" + strTemp + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) +
        //                             "-" + strHealthDateTime.Substring(6) + "' from pcmc.kiosk_master where kiosk_ip = '" + strClientIP + "'";
        //                }

        //                if (objDB.Insert(query, out strError))
        //                {
        //                    Log.Write("Passbook Health saved in Recent health", strClientIP);
        //                    objResponse.Error = "";
        //                    objResponse.Result = true;
        //                }
        //                else
        //                {
        //                    Log.Write("Passbook Health not saved in Recent health Error:-" + strError, strClientIP);
        //                    objResponse.Error = "Passbook health Not Saved";
        //                    objResponse.Result = false;
        //                }
        //            }
        //            else if (objDS != null && objDS.Tables[0].Rows.Count == 1)
        //            {
        //                query = "";

        //                if (objDB.database_type == "mssql")
        //                {
        //                    query = "update [pcmc].[dbo].[health_recent] set " +
        //                             "client_status_with_rms = 'Connected',";

        //                    for (int iCount = 1; iCount <= iDeviceCount; iCount++)
        //                    {
        //                        query += "device" + iCount + "_status ='" + arrHealthData[iCount] + "', ";
        //                    }

        //                    query = query + " date_time = '" + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) + "-" + strHealthDateTime.Substring(6) + "'" +
        //                             " where f_kiosk_id = (select kiosk_id from [pcmc].[dbo].[kiosk_master] where kiosk_ip = '" + strClientIP + "')";

        //                }
        //                else
        //                {
        //                    query = "update pcmc.health_recent set " +
        //                            "client_status_with_rms = 'Connected',";

        //                    for (int iCount = 1; iCount <= iDeviceCount; iCount++)
        //                    {
        //                        query += "device" + iCount + "_status ='" + arrHealthData[iCount] + "', ";
        //                    }

        //                    query = query + " date_time = '" + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) + "-" + strHealthDateTime.Substring(6) + "'" +
        //                             " where f_kiosk_id = (select kiosk_id from pcmc.kiosk_master where kiosk_ip = '" + strClientIP + "')";
        //                }


        //                if (objDB.Update(query, out strError))
        //                {
        //                    Log.Write("Passbook Health updated in Recent health", strClientIP);
        //                    objResponse.Error = "";
        //                    objResponse.Result = true;
        //                }
        //                else
        //                {
        //                    Log.Write("Passbook Health not updated in Recent health Error:-" + strError, strClientIP);
        //                    objResponse.Error = "Passbook Health Not Updated";
        //                    objResponse.Result = false;
        //                }
        //            }

        //            // For The Deatiled health
        //            try
        //            {
        //                DateTime objTxnDtTime = DateTime.Today;

        //                query = "";

        //                if (objDB.database_type == "mssql")
        //                {
        //                    // Query to insert the current kiosk details fetched from file
        //                    query = "insert into [pcmc_20].[dbo].[health_detail_" + objTxnDtTime.ToString("MMM]") + " (f_kiosk_id, client_status_with_rms, ";

        //                    string strTemp = "";
        //                    for (int iCount = 1; iCount <= iDeviceCount; iCount++)
        //                    {
        //                        query += "device" + iCount + "_status, ";
        //                        strTemp += arrHealthData[iCount] + "', '";
        //                    }

        //                    query += "date_time)" + "select kiosk_id, 'Connected', '" + strTemp + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) +
        //                             "-" + strHealthDateTime.Substring(6) + "'from [pcmc].[dbo].[kiosk_master] where kiosk_ip = '" + strClientIP + "'";
        //                }
        //                else
        //                {
        //                    query = "insert into  pcmc_20.health_detail_" + objTxnDtTime.ToString("MMM") + " (f_kiosk_id, client_status_with_rms, ";

        //                    string strTemp = "";
        //                    for (int iCount = 1; iCount <= iDeviceCount; iCount++)
        //                    {
        //                        query += "device" + iCount + "_status, ";
        //                        strTemp += arrHealthData[iCount] + "', '";
        //                    }

        //                    query += "date_time)" + "select kiosk_id, 'Connected', '" + strTemp + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) +
        //                             "-" + strHealthDateTime.Substring(6) + "' from pcmc.kiosk_master where kiosk_ip = '" + strClientIP + "'";

        //                }
        //                // Execute the query and log the result
        //                if (objDB.Insert(query, out strError))
        //                {
        //                    Log.Write(" Health saved in Detailed table", strClientIP);
        //                    objResponse.Error = "";
        //                    objResponse.Result = true;

        //                }

        //                else
        //                {
        //                    Log.Write("Health not saved in Detailed table :-", strClientIP);
        //                    objResponse.Error = "Health not saved in Detailed table";
        //                    objResponse.Result = false;
        //                }

        //            }
        //            catch (Exception excp)
        //            {
        //                Log.Write("DB Excpn - " + excp.Message.ToString(), strClientIP);
        //                //strMessage = "DB Excpn ";
        //                //return; V27.1.1.4
        //            }
        //            string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
        //            if (!Directory.Exists(strPatchPath))
        //                Directory.CreateDirectory(strPatchPath);

        //            if (!Directory.Exists(strPatchPath + "\\RMS Client Updated Machines"))
        //                Directory.CreateDirectory(strPatchPath + "\\RMS Client Updated Machines");

        //            if (!Directory.Exists(strPatchPath + "\\RMS Monitor Updated Machines"))
        //                Directory.CreateDirectory(strPatchPath + "\\RMS Monitor Updated Machines");

        //            if (!Directory.Exists(strPatchPath + "\\Lipi RD Service Updated Machines"))
        //                Directory.CreateDirectory(strPatchPath + "\\Lipi RD Service Updated Machines");

        //            if (!Directory.Exists(strPatchPath + "\\RMS Client Patch"))
        //                Directory.CreateDirectory(strPatchPath + "\\RMS Client Patch");

        //            if (!Directory.Exists(strPatchPath + "\\RMS Monitor Patch"))
        //                Directory.CreateDirectory(strPatchPath + "\\RMS Monitor Patch");

        //            if (!Directory.Exists(strPatchPath + "\\RD Service Patch"))
        //                Directory.CreateDirectory(strPatchPath + "\\RD Service Patch");

        //            string strPatchName = "";
        //            string IsPatchUpdated = "";
        //            string strPatchUpdatedDirPath = strPatchPath + "\\RMS Client Updated Machines";
        //            string strFileName = strPatchUpdatedDirPath + "\\MSN- " + strMachineSrNo + " (IP- " + strClientIP + ").ini";

        //            if (File.Exists(strFileName))
        //            {
        //                INIFile objClientPatchIni = new INIFile(strFileName);

        //                strPatchName = objClientPatchIni.Read("Patch", "Name", "");
        //                IsPatchUpdated = objClientPatchIni.Read("Patch", "Updated", "").ToLower();

        //                Log.Write("Patch name read From INI " + strPatchName, strMsgFields[1]);
        //                Log.Write("Patch Updated value From INI:- " + objClientPatchIni.Read("Patch", "Updated", ""), strMsgFields[1]);

        //                if (strPatchName != "" && IsPatchUpdated == "false")
        //                {
        //                    string[] strFiles = Directory.GetFiles(strPatchPath + "\\RMS Client Patch", "Remote Client*.zip", SearchOption.TopDirectoryOnly);
        //                    for (int i = 0; i < strFiles.Length; ++i)
        //                    {
        //                        if (strFiles.Length > 0 && (strPatchName == strFiles[i].Substring(strFiles[i].LastIndexOf("\\") + 1)))
        //                        {
        //                            objResponse.PatchName = strPatchName;
        //                            objResponse.PatchData = Convert.ToBase64String(File.ReadAllBytes(strFiles[i]));
        //                            Log.Write("Patch sent- " + objResponse.PatchName, strMsgFields[1]);
        //                            break;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception excp)
        //        {
        //            Log.Write("DB Excpn - " + excp.Message.ToString(), strClientIP);
        //            objResponse.Error = "Exception Occured";
        //            objResponse.Result = false;
        //        }
        //    }
        //    else
        //    {
        //        Log.Write("Message Field Comes Empty No Data Found/Fields Come Are Not Proper", strClientIP);
        //        objResponse.Error = "Fields Not Proper";
        //        objResponse.Result = false;
        //    }
        //    objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(objResponse));
        //    return objEncResponse;
        //}


        public ResLipiHealth HealthData(string strHealthData)
        {
            ResLipiHealth objResponse = new ResLipiHealth();
            string strHealthDateTime, strError, strClientIP, strMachineSrNo;
            strHealthDateTime = strError = strClientIP = strMachineSrNo = "";


            DBConnect objDB = new DBConnect();
            DataSet objDS = null;
            DateTime objClientTime = DateTime.Now;


            byte[] byStr = Convert.FromBase64String(strHealthData);

            Log.Write("Message Recieved For HealthData:- " + Encoding.ASCII.GetString(byStr), strClientIP);

            string[] strMsgFields = Encoding.ASCII.GetString(byStr).Split(new string[] { "$" }, StringSplitOptions.RemoveEmptyEntries);

            strClientIP = strMsgFields[1];
            strMachineSrNo = strMsgFields[2];
            strHealthDateTime = strMsgFields[3];

            Log.Write("After Splitting Message Fields HealthData Recieved Is:- " + strMsgFields[4], strClientIP);

            //To get device count
            string[] arrHealthData = strMsgFields[4].Split('#');
            int iDeviceCount = Convert.ToInt32(arrHealthData[0]);
            string query = "";

            if (strMsgFields != null && strMsgFields[0] == "02" && arrHealthData != null)
            {
                try
                {
                    query = "";

                    if (objDB.database_type == "mssql")
                    {
                        query = "select * from [pcmc].[dbo].[health_recent] where " +
                               "f_kiosk_id = (select kiosk_id from [pcmc].[dbo].[kiosk_master] where kiosk_ip = '" + strClientIP + "')";
                    }
                    else
                    {
                        query = "select * from pcmc.health_recent where " +
                              "f_kiosk_id = (select kiosk_id from pcmc.kiosk_master where kiosk_ip = '" + strClientIP + "')";
                    }

                    if (objDS == null)
                        objDS = new DataSet();

                    objDS.Clear();

                    if (objDB.Select(query, out objDS, out strError) && objDS != null && objDS.Tables[0].Rows.Count == 0)
                    {
                        query = "";
                       
                        if (objDB.database_type == "mssql")
                        {
                            query = "insert into [pcmc].[dbo].[health_recent](f_kiosk_id, client_status_with_rms, ";

                            string strTemp = "";
                            for (int iCount = 1; iCount <= iDeviceCount; iCount++)
                            {
                                query += "device" + iCount + "_status, ";
                                strTemp += arrHealthData[iCount] + "', '";
                            }

                            query += "date_time)" + "select kiosk_id, 'Connected', '" + strTemp + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) +
                                     "-" + strHealthDateTime.Substring(6) + "' from [pcmc].[dbo].[kiosk_master] where kiosk_ip = '" + strClientIP + "'";

                        }
                        else
                        {
                            query = "insert into pcmc.health_recent(f_kiosk_id, client_status_with_rms, ";

                            string strTemp = "";
                            for (int iCount = 1; iCount <= iDeviceCount; iCount++)
                            {
                                query += "device" + iCount + "_status, ";
                                strTemp += arrHealthData[iCount] + "', '";
                            }

                            query += "date_time)" + "select kiosk_id, 'Connected', '" + strTemp + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) +
                                     "-" + strHealthDateTime.Substring(6) + "' from pcmc.kiosk_master where kiosk_ip = '" + strClientIP + "'";
                        }

                        if (objDB.Insert(query, out strError))
                        {
                            Log.Write("Passbook Health saved in Recent health", strClientIP);
                            objResponse.Error = "";
                            objResponse.Result = true;
                        }
                        else
                        {
                            Log.Write("Passbook Health not saved in Recent health Error:-" + strError, strClientIP);
                            objResponse.Error = "Passbook health Not Saved";
                            objResponse.Result = false;
                        }
                    }
                    else if (objDS != null && objDS.Tables[0].Rows.Count == 1)
                    {
                        query = "";

                        if (objDB.database_type == "mssql")
                        {
                            query = "update [pcmc].[dbo].[health_recent] set " +
                                     "client_status_with_rms = 'Connected',";

                            for (int iCount = 1; iCount <= iDeviceCount; iCount++)
                            {
                                query += "device" + iCount + "_status ='" + arrHealthData[iCount] + "', ";
                            }

                            query = query + " date_time = '" + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) + "-" + strHealthDateTime.Substring(6) + "'" +
                                     " where f_kiosk_id = (select kiosk_id from [pcmc].[dbo].[kiosk_master] where kiosk_ip = '" + strClientIP + "')";

                        }
                        else
                        {
                            query = "update pcmc.health_recent set " +
                                    "client_status_with_rms = 'Connected',";

                            for (int iCount = 1; iCount <= iDeviceCount; iCount++)
                            {
                                query += "device" + iCount + "_status ='" + arrHealthData[iCount] + "', ";
                            }

                            query = query + " date_time = '" + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) + "-" + strHealthDateTime.Substring(6) + "'" +
                                     " where f_kiosk_id = (select kiosk_id from pcmc.kiosk_master where kiosk_ip = '" + strClientIP + "')";
                        }


                        if (objDB.Update(query, out strError))
                        {
                            Log.Write("Passbook Health updated in Recent health", strClientIP);
                            objResponse.Error = "";
                            objResponse.Result = true;
                        }
                        else
                        {
                            Log.Write("Passbook Health not updated in Recent health Error:-" + strError, strClientIP);
                            objResponse.Error = "Passbook Health Not Updated";
                            objResponse.Result = false;
                        }
                    }

                    // For The Deatiled health
                    #region  Detailed health txn data
                    //try
                    //{
                    //    DateTime objTxnDtTime = DateTime.Today;

                    //    query = "";

                    //    if (objDB.database_type == "mssql")
                    //    {
                    //        // Query to insert the current kiosk details fetched from file
                    //        query = "insert into [pcmc_20].[dbo].[health_detail_" + objTxnDtTime.ToString("MMM]") + " (f_kiosk_id, client_status_with_rms, ";

                    //        string strTemp = "";
                    //        for (int iCount = 1; iCount <= iDeviceCount; iCount++)
                    //        {
                    //            query += "device" + iCount + "_status, ";
                    //            strTemp += arrHealthData[iCount] + "', '";
                    //        }

                    //        query += "date_time)" + "select kiosk_id, 'Connected', '" + strTemp + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) +
                    //                 "-" + strHealthDateTime.Substring(6) + "'from [pcmc].[dbo].[kiosk_master] where kiosk_ip = '" + strClientIP + "'";
                    //    }
                    //    else
                    //    {
                    //        query = "insert into  pcmc_20.health_detail_" + objTxnDtTime.ToString("MMM") + " (f_kiosk_id, client_status_with_rms, ";

                    //        string strTemp = "";
                    //        for (int iCount = 1; iCount <= iDeviceCount; iCount++)
                    //        {
                    //            query += "device" + iCount + "_status, ";
                    //            strTemp += arrHealthData[iCount] + "', '";
                    //        }

                    //        query += "date_time)" + "select kiosk_id, 'Connected', '" + strTemp + strHealthDateTime.Substring(0, 4) + "-" + strHealthDateTime.Substring(4, 2) +
                    //                 "-" + strHealthDateTime.Substring(6) + "' from pcmc.kiosk_master where kiosk_ip = '" + strClientIP + "'";

                    //    }
                    //    // Execute the query and log the result
                    //    if (objDB.Insert(query, out strError))
                    //    {
                    //        Log.Write(" Health saved in Detailed table", strClientIP);
                    //        objResponse.Error = "";
                    //        objResponse.Result = true;

                    //    }

                    //    else
                    //    {
                    //        Log.Write("Health not saved in Detailed table :-", strClientIP);
                    //        objResponse.Error = "Health not saved in Detailed table";
                    //        objResponse.Result = false;
                    //    }

                    //}
                    //catch (Exception excp)
                    //{
                    //    Log.Write("DB Excpn - " + excp.Message.ToString(), strClientIP);
                    //    //strMessage = "DB Excpn ";
                    //    //return; V27.1.1.4
                    //}

                    #endregion

                    string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
                    if (!Directory.Exists(strPatchPath))
                        Directory.CreateDirectory(strPatchPath);

                    if (!Directory.Exists(strPatchPath + "\\RMS Client Updated Machines"))
                        Directory.CreateDirectory(strPatchPath + "\\RMS Client Updated Machines");

                    if (!Directory.Exists(strPatchPath + "\\RMS Monitor Updated Machines"))
                        Directory.CreateDirectory(strPatchPath + "\\RMS Monitor Updated Machines");

                    if (!Directory.Exists(strPatchPath + "\\Lipi RD Service Updated Machines"))
                        Directory.CreateDirectory(strPatchPath + "\\Lipi RD Service Updated Machines");

                    if (!Directory.Exists(strPatchPath + "\\RMS Client Patch"))
                        Directory.CreateDirectory(strPatchPath + "\\RMS Client Patch");

                    if (!Directory.Exists(strPatchPath + "\\RMS Monitor Patch"))
                        Directory.CreateDirectory(strPatchPath + "\\RMS Monitor Patch");

                    if (!Directory.Exists(strPatchPath + "\\RD Service Patch"))
                        Directory.CreateDirectory(strPatchPath + "\\RD Service Patch");

                    string strPatchName = "";
                    string IsPatchUpdated = "";
                    string strPatchUpdatedDirPath = strPatchPath + "\\RMS Client Updated Machines";
                    string strFileName = strPatchUpdatedDirPath + "\\MSN- " + strMachineSrNo + " (IP- " + strClientIP + ").ini";

                    if (File.Exists(strFileName))
                    {
                        INIFile objClientPatchIni = new INIFile(strFileName);

                        strPatchName = objClientPatchIni.Read("Patch", "Name", "");
                        IsPatchUpdated = objClientPatchIni.Read("Patch", "Updated", "").ToLower();

                        Log.Write("Patch name read From INI " + strPatchName, strMsgFields[1]);
                        Log.Write("Patch Updated value From INI:- " + objClientPatchIni.Read("Patch", "Updated", ""), strMsgFields[1]);

                        if (strPatchName != "" && IsPatchUpdated == "false")
                        {
                            string[] strFiles = Directory.GetFiles(strPatchPath + "\\RMS Client Patch", "Remote Client*.zip", SearchOption.TopDirectoryOnly);
                            for (int i = 0; i < strFiles.Length; ++i)
                            {
                                if (strFiles.Length > 0 && (strPatchName == strFiles[i].Substring(strFiles[i].LastIndexOf("\\") + 1)))
                                {
                                    objResponse.PatchName = strPatchName;
                                    objResponse.PatchData = Convert.ToBase64String(File.ReadAllBytes(strFiles[i]));
                                    Log.Write("Patch sent- " + objResponse.PatchName, strMsgFields[1]);
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception excp)
                {
                    Log.Write("DB Excpn - " + excp.Message.ToString(), strClientIP);
                    objResponse.Error = "Exception Occured";
                    objResponse.Result = false;
                }

            }
            else
            {
                Log.Write("Message Field Comes Empty No Data Found/Fields Come Are Not Proper", strClientIP);
                objResponse.Error = "Fields Not Proper";
                objResponse.Result = false;
            }

            return objResponse;
        }


        public EncResponse CheckLipiMonitor(EncRequest objEncRequest)
        {
            EncResponse objEncResponse = new EncResponse();
            ResMonitorPatchUpdate objResMonitorPatchUpdate = new ResMonitorPatchUpdate();
            string[] strMsgFields = new string[0];
            try
            {
                string DecryptData = objEncRequest.RequestData;
                DecryptData = JsonConvert.DeserializeObject<string>(DecryptData);
               // byte[] byStr = Convert.FromBase64String(DecryptData);
                strMsgFields = DecryptData.Split('#');

                if (strMsgFields.Length > 0)
                {
                    string strMachineSrNo = strMsgFields[0];
                    string strKioskIP = strMsgFields[1];

                    string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
                    if (!Directory.Exists(strPatchPath))
                        Directory.CreateDirectory(strPatchPath);

                    bool bRMSPatchUpdated = false;
                    string strPatchName = "";
                    string strPatchUpdatedDirPath = strPatchPath + "\\RMS Monitor Updated Machines";
                    string strFileName = strPatchUpdatedDirPath + "\\MSN- " + strMachineSrNo + " (IP- " + strKioskIP + ").ini";

                    if (Directory.Exists(strPatchUpdatedDirPath))
                    {
                        if (File.Exists(strFileName))
                        {
                            INIFile objClientPatchIni = new INIFile(strFileName);
                            strPatchName = objClientPatchIni.Read("Patch", "Name", "");
                            if (objClientPatchIni.Read("Patch", "Updated", "").ToLower() == "true")
                            {
                                bRMSPatchUpdated = true;
                            }
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(strPatchUpdatedDirPath);
                    }

                    string[] strFiles = Directory.GetFiles(strPatchPath + "\\RMS Monitor Patch", "RMSClientMonitor*.zip", SearchOption.TopDirectoryOnly);

                    for (int iCount = 0; iCount < strFiles.Length; iCount++)
                    {
                        if (strFiles.Length > 0 &&
                        (strPatchName == strFiles[iCount].Substring(strFiles[iCount].LastIndexOf("\\") + 1) && !bRMSPatchUpdated))
                        {
                            objResMonitorPatchUpdate.Result = true;
                            objResMonitorPatchUpdate.PatchName = strPatchName;
                            objResMonitorPatchUpdate.PatchData = Convert.ToBase64String(File.ReadAllBytes(strFiles[iCount]));
                            Log.Write("Monitor sent- " + objResMonitorPatchUpdate.PatchName, strMsgFields[1]);
                            break;
                        }
                    }
                }
            }
            catch (Exception excp)
            {
                objResMonitorPatchUpdate.Result = false;
                objResMonitorPatchUpdate.Error = excp.Message;
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(objResMonitorPatchUpdate));
            return objEncResponse;
        }

        public EncResponse CheckLipiRDService(EncRequest objEncRequest)
        {
            EncResponse objEncResponse = new EncResponse();
            ResLipiRDServiceUpdate objResLipiRDServiceUpdate = new ResLipiRDServiceUpdate();
            string[] strMsgFields = new string[0];
            try
            {
                string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                DecryptData = JsonConvert.DeserializeObject<string>(DecryptData);
              //  byte[] byStr = Convert.FromBase64String(DecryptData);
                strMsgFields = DecryptData.Split('#');

                if (strMsgFields.Length > 0)
                {
                    string strMachineSrNo = strMsgFields[0];
                    string strKioskIP = strMsgFields[1];

                    string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
                    if (!Directory.Exists(strPatchPath))
                        Directory.CreateDirectory(strPatchPath);

                    bool bRMSPatchUpdated = false;
                    string strPatchName = "";
                    string strPatchUpdatedDirPath = strPatchPath + "\\Lipi RD Service Updated Machines";
                    string strFileName = strPatchUpdatedDirPath + "\\MSN- " + strMachineSrNo + " (IP- " + strKioskIP + ").ini";

                    if (Directory.Exists(strPatchUpdatedDirPath))
                    {
                        if (File.Exists(strFileName))
                        {
                            INIFile objClientPatchIni = new INIFile(strFileName);
                            strPatchName = objClientPatchIni.Read("Patch", "Name", "");
                            if (objClientPatchIni.Read("Patch", "Updated", "").ToLower() == "true")
                                bRMSPatchUpdated = true;
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(strPatchUpdatedDirPath);
                    }

                    string[] strFiles = Directory.GetFiles(strPatchPath + "\\RD Service Patch", "LipiRDService*.zip", SearchOption.TopDirectoryOnly);
                    for (int iCount = 0; iCount < strFiles.Length; iCount++)
                    {
                        if (strFiles.Length > 0 &&
                        (strPatchName == strFiles[iCount].Substring(strFiles[iCount].LastIndexOf("\\") + 1) && !bRMSPatchUpdated))
                        {
                            objResLipiRDServiceUpdate.Result = true;
                            objResLipiRDServiceUpdate.PatchName = strPatchName;
                            objResLipiRDServiceUpdate.PatchData = Convert.ToBase64String(File.ReadAllBytes(strFiles[iCount]));
                            Log.Write("Lipi RD Service sent- " + objResLipiRDServiceUpdate.PatchName, strMsgFields[1]);
                            break;
                        }
                    }
                }
            }
            catch (Exception excp)
            {
                objResLipiRDServiceUpdate.Result = false;
                objResLipiRDServiceUpdate.Error = excp.Message;
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(objResLipiRDServiceUpdate));
            return objEncResponse;
        }

        public EncResponse GetCommand(EncRequest objEncRequest)
        {
            EncResponse objEncResponse = new EncResponse();
            string strReturn = "";
            string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
            string  strClientIP = JsonConvert.DeserializeObject<string>(DecryptData);
            string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
            string strCommFileName = strPatchPath + "\\Command.ini";

            INIFile objCommandIni = new INIFile(strCommFileName);
            int iCommandCount = Convert.ToInt32(objCommandIni.Read(strClientIP, "CommandCount", "0"));
            if (iCommandCount >= 1)
            {
                strReturn += iCommandCount;
                for (int iCount = 1; iCount <= iCommandCount; iCount++)
                    strReturn += "#" + objCommandIni.Read(strClientIP, "Parameter" + iCount, "");
            }
            else if (iCommandCount == 0)
            {
                strReturn = "True#NoCommand#" + DateTime.Now.ToString("yyyyMMdd");
            }

            /*strReturn = "Presementdate:12/15/2018"*/;
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(strReturn));
            return objEncResponse;
           
        }


        public string CheckConnectivity()
        {
            return "Success";
        }

        public ResPatchUpdate GetClientPatch(string strClientPatch)
        {
            EncResponse objEncResponse = new EncResponse();
            ResPatchUpdate objResPatchUpdate = new ResPatchUpdate();
            try
            {
                //string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                //string strClientPatch = JsonConvert.DeserializeObject<string>(data);
                string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
                if (!Directory.Exists(strPatchPath))
                    Directory.CreateDirectory(strPatchPath);

                string[] strFiles = Directory.GetFiles(strPatchPath + "\\RMS Client Patch\\", "Remote Client*.zip", SearchOption.TopDirectoryOnly);      //V35.1.0.7

                for (int iCount = 0; iCount < strFiles.Length; iCount++)
                {
                    if (strClientPatch.ToLower() == strFiles[iCount].ToLower().Substring(strFiles[0].ToLower().LastIndexOf("\\") + 1))
                    {
                        objResPatchUpdate.Result = true;
                        objResPatchUpdate.PatchName = strFiles[iCount].Substring(strFiles[iCount].LastIndexOf("\\") + 1);
                        objResPatchUpdate.PatchData = Convert.ToBase64String(File.ReadAllBytes(strFiles[iCount]));
                        break;
                    }
                }

            }
            catch (Exception excp)
            {
                objResPatchUpdate.Result = false;
                objResPatchUpdate.Error = excp.Message;
            }
           // objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(objResPatchUpdate));
            return objResPatchUpdate;
           
        }

        public EncResponse GetClientMonitor(EncRequest objEncRequest)
        {
            EncResponse objEncResponse = new EncResponse();
            ResPatchUpdate objResPatchUpdate = new ResPatchUpdate();
            try
            {
                string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                string strMonitorPatch = JsonConvert.DeserializeObject<string>(DecryptData);
                string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
                if (!Directory.Exists(strPatchPath))
                    Directory.CreateDirectory(strPatchPath);

                string[] strFiles = Directory.GetFiles(strPatchPath + "\\RMS Monitor Patch\\", "RMSClientMonitor*.zip", SearchOption.TopDirectoryOnly);      //V35.1.0.7

                for (int iCount = 0; iCount < strFiles.Length; iCount++)
                {
                    if (strMonitorPatch == strFiles[iCount].Substring(strFiles[0].LastIndexOf("\\") + 1))
                    {
                        objResPatchUpdate.Result = true;
                        objResPatchUpdate.PatchName = strFiles[iCount].Substring(strFiles[iCount].LastIndexOf("\\") + 1);
                        objResPatchUpdate.PatchData = Convert.ToBase64String(File.ReadAllBytes(strFiles[iCount]));
                        break;
                    }
                }

            }
            catch (Exception excp)
            {
                objResPatchUpdate.Result = false;
                objResPatchUpdate.Error = excp.Message;
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(objResPatchUpdate));
            return objEncResponse;
           
        }

        public string LipiPatchAck(EncRequest request)
        {
            string returnString = "";
            try
            {

                string jsonData = AesGcm256.Decrypt(request.RequestData);
                string strAckData = JsonConvert.DeserializeObject<string>(jsonData);

                string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();

                if (strAckData.Split('#')[2] == "Client")
                {
                    string strPatchUpdatedDirPath = strPatchPath + "\\RMS Client Updated Machines";
                    string strFileName = strPatchUpdatedDirPath + "\\MSN- " + strAckData.Split('#')[1] + " (IP- " + strAckData.Split('#')[0] + ").ini";

                    INIFile objClientPatchIni = new INIFile(strFileName);

                    objClientPatchIni.Write("Patch", "Updated", "false");
                    objClientPatchIni.Write("Patch", "Name", "");            //Resetting the INI File
                }
                else if (strAckData.Split('#')[2] == "Monitor")
                {
                    string strPatchUpdatedDirPath = strPatchPath + "\\RMS Monitor Updated Machines";
                    string strFileName = strPatchUpdatedDirPath + "\\MSN- " + strAckData.Split('#')[1] + " (IP- " + strAckData.Split('#')[0] + ").ini";

                    INIFile objClientPatchIni = new INIFile(strFileName);
                    objClientPatchIni.Write("Patch", "Updated", "false");
                    objClientPatchIni.Write("Patch", "Name", "");             //Resetting the INI File
                }
                else if (strAckData.Split('#')[2] == "LipiRDService")
                {
                    string strPatchUpdatedDirPath = strPatchPath + "\\Lipi RD Service Updated Machines";
                    string strFileName = strPatchUpdatedDirPath + "\\MSN- " + strAckData.Split('#')[1] + " (IP- " + strAckData.Split('#')[0] + ").ini";

                    INIFile objClientPatchIni = new INIFile(strFileName);
                    objClientPatchIni.Write("Patch", "Updated", "false");
                    objClientPatchIni.Write("Patch", "Name", "");             //Resetting the INI File
                }
                else if (strAckData.Split('#')[2] == "Kproc")
                {
                    string strPatchUpdatedDirPath = strPatchPath + "\\RMS Kproc Updated Machines";
                    string strFileName = strPatchUpdatedDirPath + "\\MSN- " + strAckData.Split('#')[1] + " (IP- " + strAckData.Split('#')[0] + ").ini";

                    INIFile objClientPatchIni = new INIFile(strFileName);

                    objClientPatchIni.Write("Patch", "Updated", "false");
                    objClientPatchIni.Write("Patch", "Name", "");            //Resetting the INI File
                }

                returnString = "success";
            }
            catch
            {
                returnString = "error";
            }
            return returnString;
        }


        public EncResponse GetRDService(EncRequest objEncRequest)
        {
            EncResponse objEncResponse = new EncResponse();
            ResPatchUpdate objResRDServiceUpdate = new ResPatchUpdate();

            try
            {
                string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                string strPatchName = JsonConvert.DeserializeObject<string>(DecryptData);
                string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
                string[] strFiles = Directory.GetFiles(strPatchPath + "\\RD Service Patch", "LipiRDService*.zip", SearchOption.TopDirectoryOnly);      //V35.1.0.7
                for (int i = 0; i < strFiles.Length; ++i)
                {
                    if (strPatchName == strFiles[0].Substring(strFiles[0].LastIndexOf("\\") + 1))
                    {
                        objResRDServiceUpdate.Result = true;
                        objResRDServiceUpdate.PatchName = strFiles[0].Substring(strFiles[0].LastIndexOf("\\") + 1);
                        objResRDServiceUpdate.PatchData = Convert.ToBase64String(File.ReadAllBytes(strFiles[0]));
                        break;
                    }
                }
            }
            catch (Exception excp)
            {
                objResRDServiceUpdate.Result = false;
                objResRDServiceUpdate.Error = excp.Message;
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(objResRDServiceUpdate));
            return objEncResponse;
           
        }

        public EncResponse LipiDownloadData(EncRequest objEncRequest)
        {
            EncResponse objEncResponse = new EncResponse();
            ResDownloadData objResDownloadData = new ResDownloadData();
    
             string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
            string strFileName = JsonConvert.DeserializeObject<string>(DecryptData);
            string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
            if (!Directory.Exists(strPatchPath))
                Directory.CreateDirectory(strPatchPath);

            if (File.Exists(strPatchPath + "\\" + strFileName))
            {
                objResDownloadData.Result = true;
                objResDownloadData.DownalodData = Convert.ToBase64String(File.ReadAllBytes(strPatchPath + "\\" + strFileName));
            }
            else
            {
                objResDownloadData.Result = false;
                objResDownloadData.Error = "File not found";
            }

            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(objResDownloadData));
            return objEncResponse;
            
        }

        public bool LipiUploadData(LogDetails objLogDetails)
        {
            bool isUploaded = false;
            EncResponse objEncResponse = new EncResponse();
            try
            {
              //  string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                
                Log.Write("LipiUploadData called; IP: " + objLogDetails.ClientIP + "; UploadFileName: " + objLogDetails.Filename, "");

                //check if directory exists
                if (!Directory.Exists(ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString() + "\\" + "(IP- " + objLogDetails.ClientIP + ")"))
                    Directory.CreateDirectory(ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString() + "\\" + "(IP- " + objLogDetails.ClientIP + ")");

                //making and saving zip file
                byte[] bLogData = Convert.FromBase64String(objLogDetails.Logdata);

                //Creating zip file
                File.WriteAllBytes(ConfigurationManager.AppSettings["DataReceivedFromClients"].ToString() + "\\" + "(IP- " + objLogDetails.ClientIP + ")\\" + objLogDetails.Filename, bLogData);
                isUploaded = true;

                string strCommandDir = ConfigurationManager.AppSettings["CommandDirectory"].ToString();
                string strCommFileName = strCommandDir + "\\Command.ini";
                INIFile objCommandIni = new INIFile(strCommFileName);
                objCommandIni.Write(objLogDetails.ClientIP, "CommandCount", "0");
                objCommandIni.Write(objLogDetails.ClientIP, "Parameter1", "");
                objCommandIni.Write(objLogDetails.ClientIP, "Parameter2", "");
                objCommandIni.Write(objLogDetails.ClientIP, "Parameter3", "");
                objCommandIni.Write(objLogDetails.ClientIP, "Parameter4", "");


                Log.Write("Resetting The Command INI After Updation For IP:-" + objLogDetails.ClientIP, objLogDetails.ClientIP);
                objEncResponse.ResponseData = "true";
            }
            catch (Exception excp)
            {
                Log.Write("LipiUploadData : IP - " + objLogDetails.ClientIP + "; " + excp.Message + Environment.NewLine + excp.StackTrace, "");
                objEncResponse.ResponseData = "false";
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(objEncResponse.ResponseData));
            return isUploaded;
        }

        //public EncResponse LipiPatchAck(EncRequest objEncRequest)
        //{
        //    EncResponse objEncResponse = new EncResponse();
        //    string returnString = "";
        //    try
        //    {
        //        //  string DecryptData =objEncRequest.RequestData;
        //        string strAckData = objEncRequest.RequestData;


        //        string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();

        //        if (strAckData.Split('#')[2] == "Client")
        //        {
        //            string strPatchUpdatedDirPath = strPatchPath + "\\RMS Client Updated Machines";
        //            string strFileName = strPatchUpdatedDirPath + "\\MSN- " + strAckData.Split('#')[1] + " (IP- " + strAckData.Split('#')[0] + ").ini";

        //            INIFile objClientPatchIni = new INIFile(strFileName);

        //            objClientPatchIni.Write("Patch", "Updated", "false");
        //            objClientPatchIni.Write("Patch", "Name", "");            //Resetting the INI File
        //        }
        //        else if (strAckData.Split('#')[2] == "Monitor")
        //        {
        //            string strPatchUpdatedDirPath = strPatchPath + "\\RMS Monitor Updated Machines";
        //            string strFileName = strPatchUpdatedDirPath + "\\MSN- " + strAckData.Split('#')[1] + " (IP- " + strAckData.Split('#')[0] + ").ini";

        //            INIFile objClientPatchIni = new INIFile(strFileName);
        //            objClientPatchIni.Write("Patch", "Updated", "false");
        //            objClientPatchIni.Write("Patch", "Name", "");             //Resetting the INI File
        //        }
        //        else if (strAckData.Split('#')[2] == "LipiRDService")
        //        {
        //            string strPatchUpdatedDirPath = strPatchPath + "\\Lipi RD Service Updated Machines";
        //            string strFileName = strPatchUpdatedDirPath + "\\MSN- " + strAckData.Split('#')[1] + " (IP- " + strAckData.Split('#')[0] + ").ini";

        //            INIFile objClientPatchIni = new INIFile(strFileName);
        //            objClientPatchIni.Write("Patch", "Updated", "false");
        //            objClientPatchIni.Write("Patch", "Name", "");             //Resetting the INI File
        //        }
        //        returnString = "success";
        //    }
        //    catch
        //    {
        //        returnString = "error";
        //    }
        //    objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(returnString));
        //    return objEncResponse;
            
        //}

        public EncResponse LipiCommandAck(EncRequest objEncRequest)
        {
            EncResponse objEncResponse = new EncResponse();
            
            string returnString = "";
            try
            {
                string DecryptData = AesGcm256.Decrypt(objEncRequest.RequestData);
                string strClientIP = JsonConvert.DeserializeObject<string>(DecryptData);
                string strPatchPath = ConfigurationManager.AppSettings["DataSentToClients"].ToString();
                string strCommFileName = strPatchPath + "\\Command.ini";
                INIFile objCommandIni = new INIFile(strCommFileName);
                objCommandIni.Write(strClientIP, "CommandCount", "0");
                objCommandIni.Write(strClientIP, "Parameter1", "");
                objCommandIni.Write(strClientIP, "Parameter2", "");
                objCommandIni.Write(strClientIP, "Parameter3", "");
                objCommandIni.Write(strClientIP, "Parameter4", "");

                returnString = "true";
            }
            catch (Exception ex)
            {
                returnString = "AckFail - " + ex.Message;
            }
            objEncResponse.ResponseData = AesGcm256.Encrypt(JsonConvert.SerializeObject(returnString));
            return objEncResponse;
           
        }

    }

    class INIFile
    {
        private string filePath;
        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        private static extern UInt32 GetPrivateProfileSection
            (
                [In] [MarshalAs(UnmanagedType.LPStr)] string strSectionName,
                // Note that because the key/value pars are returned as null-terminated
                // strings with the last string followed by 2 null-characters, we cannot
                // use StringBuilder.
                [In] IntPtr pReturnedString,
                [In] UInt32 nSize,
                [In] [MarshalAs(UnmanagedType.LPStr)] string strFileName
            );

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public INIFile(string filePath)
        {
            this.filePath = filePath;
        }

        public void Write(string section, string key, string value)
        {
            long a = WritePrivateProfileString(section, key, value, this.filePath);
            System.Threading.Thread.Sleep(60);
        }

        public string Read(string section, string key, string def)
        {
            string strReturnVal = "";
            try
            {
                StringBuilder SB = new StringBuilder(255);
                int i = GetPrivateProfileString(section, key, def, SB, 255, this.filePath);
                strReturnVal = SB.ToString();
            }
            catch (Exception)
            {
                strReturnVal = "";
            }
            return strReturnVal;
        }

        public bool IniReadDateValue(string Section, string Key, out DateTime objDT, out string strExcp)
        {
            try
            {
                StringBuilder temp = new StringBuilder(25);
                int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.filePath);

                objDT = new DateTime(Convert.ToInt32(temp.ToString().Substring(0, 4)), Convert.ToInt32(temp.ToString().Substring(5, 2)), Convert.ToInt32(temp.ToString().Substring(8, 2)), Convert.ToInt32(temp.ToString().Substring(11, 2)), Convert.ToInt32(temp.ToString().Substring(14, 2)), Convert.ToInt32(temp.ToString().Substring(17, 2)));
                strExcp = "";   //Added [Shubhit 03May13]
                return true;
            }
            catch (Exception excp)
            {
                objDT = DateTime.Now;
                strExcp = excp.Message.ToString();  //Added [Shubhit 03May13]
                return false;
            }
        }

        public double IniReadDoubleValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.filePath);
            double dRes;
            Double.TryParse(temp.ToString(), out dRes);
            return dRes;
        }

        public string[] GetAllKeysInIniFileSection(string strSectionName)
        {
            string[] strArray = null;
            try
            {

                // Allocate in unmanaged memory a buffer of suitable size.
                // I have specified here the max size of 32767 as documentated 
                // in MSDN.
                IntPtr pBuffer = Marshal.AllocHGlobal(32767);
                // Start with an array of 1 string only. 
                // Will embellish as we go along.

                strArray = new string[0];
                UInt32 uiNumCharCopied = 0;

                uiNumCharCopied = GetPrivateProfileSection(strSectionName, pBuffer, 32767, this.filePath);

                // iStartAddress will point to the first character of the buffer,
                int iStartAddress = pBuffer.ToInt32();
                // iEndAddress will point to the last null char in the buffer.
                int iEndAddress = iStartAddress + (int)uiNumCharCopied;

                // Navigate through pBuffer.
                while (iStartAddress < iEndAddress)
                {
                    // Determine the current size of the array.
                    int iArrayCurrentSize = strArray.Length;
                    // Increment the size of the string array by 1.
                    Array.Resize<string>(ref strArray, iArrayCurrentSize + 1);
                    // Get the current string which starts at "iStartAddress".
                    string strCurrent = Marshal.PtrToStringAnsi(new IntPtr(iStartAddress));
                    // Insert "strCurrent" into the string array.
                    strArray[iArrayCurrentSize] = strCurrent;
                    // Make "iStartAddress" point to the next string.
                    iStartAddress += (strCurrent.Length + 1);
                }

                Marshal.FreeHGlobal(pBuffer);
                pBuffer = IntPtr.Zero;
                for (int i = 0; i < strArray.Length; i++)
                {
                    strArray[i] = strArray[i].Substring(strArray[i].LastIndexOf('=') + 1).ToLower();
                }
            }
            catch (Exception) { }//EventLog.WriteEntry("LipiWhiteListing", "Exception Message: " + ex.Message, //EventLogEntryType.Error); }

            return strArray;
        }
        public string FilePath
        {
            get { return this.filePath; }
            set { this.filePath = value; }
        }
    }
    public enum Database
    {
        MSSQL,
        MySQL
    }
    public class ReqConnectDB
    {
        [DataMember(Order = 0)]
        public string ServerIP { get; set; }

        [DataMember(Order = 1)]
        public string UserName { get; set; }

        [DataMember(Order = 2)]
        public string Password { get; set; }

        [DataMember(Order = 3)]
        public Database DBType { get; set; }

        [DataMember(Order = 4)]
        public string DBName { get; set; }

        [DataMember(Order = 5)]
        public bool ColumnEncryption { get; set; }
    }

    [DataContract]
    public class ResConnectDB
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 1)]
        public string Error { get; set; }
    }

    [DataContract]
    public class ReqSelect
    {
        [DataMember(Order = 0)]
        public ReqConnectDB Connection { get; set; }

        [DataMember(Order = 1)]
        public string Query { get; set; }

        [DataMember(Order = 2)]
        public List<string> Parameters { get; set; }
    }

    [DataContract]
    public class ResSelect
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 1)]
        public string Error { get; set; }

        [DataMember(Order = 2)]
        public DataSet DS { get; set; }
    }

    [DataContract]
    public class ReqInsert
    {
        [DataMember(Order = 0)]
        public ReqConnectDB Connection { get; set; }

        [DataMember(Order = 1)]
        public string Query { get; set; }

        [DataMember(Order = 2)]
        public List<object> Parameters { get; set; }
    }

    [DataContract]
    public class ResInsert
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 1)]
        public string Error { get; set; }

        [DataMember(Order = 2)]
        public int RecordsInserted { get; set; }
    }

    [DataContract]
    public class ReqDelete
    {
        [DataMember(Order = 0)]
        public ReqConnectDB Connection { get; set; }

        [DataMember(Order = 1)]
        public string Query { get; set; }

        [DataMember(Order = 2)]
        public List<string> Parameters { get; set; }
    }

    [DataContract]
    public class ResDelete
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 1)]
        public string Error { get; set; }

        [DataMember(Order = 2)]
        public int RecordsDeleted { get; set; }
    }

    [DataContract]
    public class ReqUpdate
    {
        [DataMember(Order = 0)]
        public ReqConnectDB Connection { get; set; }

        [DataMember(Order = 1)]
        public string Query { get; set; }

        [DataMember(Order = 2)]
        public List<string> Parameters { get; set; }
    }

    [DataContract]
    public class ResUpdate
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 1)]
        public string Error { get; set; }

        [DataMember(Order = 2)]
        public int RecordsUpdated { get; set; }
    }

    public class Reply
    {
        public DataSet DS { get; set; }
        public bool res { get; set; }
        public int DeviceCount { get; set; }
        public string strError { get; set; }
    }

    public class PatchUpdateINI
    {
        public string[] KioskIP;

        public string[] MachineSrNo;

        public string patch;
        public string PatchName { get; set; }
        public string StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        public bool Instant { get; set; }

    }

    public class CommandIniUpdate
    {
        public string[] KioskIP;

        public string[] MachineSrNo;

        public string CommandCount;
        public string Command { get; set; }
        public bool Instant { get; set; }

    }

    public class UserDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Question { get; set; }

        public string Answer { get; set; }
        public string Location { get; set; }
        public string Role { get; set; }
    }
    public struct UserDetailsInfo
    {
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public string Role { get; set; }
    }
}