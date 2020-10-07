using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace LipiRMS
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        //Client Function Refrence
        //[OperationContract]
        //[WebInvoke(Method = "POST",
        //ResponseFormat = WebMessageFormat.Json,
        //RequestFormat = WebMessageFormat.Json,
        //BodyStyle = WebMessageBodyStyle.Bare,
        //UriTemplate = "HealthData")]
        //EncResponse HealthData(EncRequest objEncRequest);

        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "HealthData")]
        ResLipiHealth HealthData(string strHealthData);

        //Client Function Refrence
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "CheckLipiMonitor")]
        EncResponse CheckLipiMonitor(EncRequest objEncRequest);

        //Client Function Refrence
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "CheckLipiRDService")]
        EncResponse CheckLipiRDService(EncRequest objEncRequest);

        //Client Function Refrence
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetCommand")]
        EncResponse GetCommand(EncRequest objEncRequest);

        //Client Function Refrence
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "CheckConnectivity")]
        string CheckConnectivity();

        //Client Function Refrence
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetClientPatch")]
        ResPatchUpdate GetClientPatch(string data);

        //Client Function Refrence
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetClientMonitor")]
        EncResponse GetClientMonitor(EncRequest objEncRequest);  //LipiPatchAck

        //Client Function Refrence
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetRDService")]
        EncResponse GetRDService(EncRequest objEncRequest);

        //Client Function Refrence
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "LipiDownloadData")]
        EncResponse LipiDownloadData(EncRequest objEncRequest);

        //Client Function Refrence
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "LipiUploadData")]
        bool LipiUploadData(LogDetails logDetails);

        //Client Function Refrence
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "LipiPatchAck")]
        string LipiPatchAck(EncRequest objEncRequest);

        //Client Function Refrence
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "LipiCommandAck")]
        EncResponse LipiCommandAck(EncRequest objEncRequest);


        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetTxnDetails")]
        EncResponse GetTxnDetails(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetUserType")]
        EncResponse GetUserType(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetUserDetails")]
        EncResponse GetUserDetails(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "Adduser")]
        EncResponse Adduser(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "UpdateUser")]
        EncResponse UpdateUser(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "UpdatePassword")]
        EncResponse UpdatePassword(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetActivatedKioskReport")]
        EncResponse GetActivatedKioskReport(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetKioskReport")]
        EncResponse GetKioskReport(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetKioskReportWhiteList")]
        EncResponse GetKioskReportWhiteList(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetScreenData")]
        EncResponse GetScreenData(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "KioskDetails")]
        string KioskDetails(string strKioskDetails);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetKioskHealth")]
        EncResponse GetKioskHealth(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "DeleteUser")]
        EncResponse DeleteUser(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetKioskMasterList")]
        EncResponse GetKioskMasterList(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetPieChart")]
        EncResponse GetPieChart(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "Login")]
        EncResponse Login(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "GetLocation")]
        EncResponse GetLocation(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "LipiLogData")]
        string LipiLogData(string strLogData);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "CommandIniUpdate")]
        EncResponse CommandIniUpdate(EncRequest objEncRequest);

        ///web portal//
        [OperationContract]
        [WebInvoke(Method = "POST",
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Bare,
        UriTemplate = "PatchSave")]
        EncResponse PatchSave(EncRequest objEncRequest);
    }

    public enum OSType
    {
        Linux,
        Windows
    }

    public enum KioskType
    {
        Cheque,
        Passbook
    }


    [DataContract]
    public class ResLipiHealth
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 1)]
        public string PatchName { get; set; }

        [DataMember(Order = 2)]
        public string PatchData { get; set; }

        [DataMember(Order = 3)]
        public string Error { get; set; }
    }

    [DataContract]
    public class ResPatchUpdate
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 1)]
        public string PatchName { get; set; }

        [DataMember(Order = 2)]
        public string PatchData { get; set; }

        [DataMember(Order = 3)]
        public string Error { get; set; }
    }

    [DataContract]
    public class ResDownloadData
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 2)]
        public string DownalodData { get; set; }

        [DataMember(Order = 3)]
        public string Error { get; set; }
    }

    [DataContract]
    public class ResLipiRDServiceUpdate
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 1)]
        public string PatchName { get; set; }

        [DataMember(Order = 2)]
        public string PatchData { get; set; }

        [DataMember(Order = 3)]
        public string Error { get; set; }
    }

    public class EncRequest
    {
        [DataMember(Order = 0)]
        public string RequestData { get; set; }
    }

    [DataContract]
    public class EncResponse
    {
        [DataMember(Order = 0)]
        public string ResponseData { get; set; }
    }

    [DataContract]
    public class ResMonitorPatchUpdate
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 1)]
        public string PatchName { get; set; }

        [DataMember(Order = 2)]
        public string PatchData { get; set; }

        [DataMember(Order = 3)]
        public string Error { get; set; }
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class ReqKioskDetails
    {
        [DataMember(Order = 0)]
        public OSType OS { get; set; }
    }

    [DataContract]
    public class ReqPatch
    {
        [DataMember(Order = 0)]
        public OSType OS { get; set; }

        [DataMember(Order = 1)]
        public Version Version { get; set; }

        [DataMember(Order = 2)]
        public string ClientIP { get; set; }

        [DataMember(Order = 3)]
        public KioskType KioskType { get; set; }

        [DataMember(Order = 4)]
        public string FileNameToDownload { get; set; }
    }

    [DataContract]
    public class ResPatch
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 1)]
        public string OS { get; set; }

        [DataMember(Order = 2)]
        public string PatchName { get; set; }

        [DataMember(Order = 3)]
        public string PatchData { get; set; }

        [DataMember(Order = 4)]
        public string Error { get; set; }
    }

    [DataContract]
    public class LogDetails
    {
        [DataMember(Order = 0)]
        public string Logdata { get; set; }

        [DataMember(Order = 1)]
        public string Filename { get; set; }

        [DataMember(Order = 2)]
        public string ClientIP { get; set; }

    }

    [DataContract]
    public class ReqCustomData
    {
        [DataMember(Order = 0)]
        public OSType OS { get; set; }

        [DataMember(Order = 1)]
        public string ClientIP { get; set; }

        [DataMember(Order = 2)]
        public KioskType KioskType { get; set; }

        [DataMember(Order = 3)]
        public string FileNameToDownload { get; set; }
    }

    [DataContract]
    public class ResCustomData
    {
        [DataMember(Order = 0)]
        public bool Result { get; set; }

        [DataMember(Order = 1)]
        public string OS { get; set; }

        [DataMember(Order = 2)]
        public string FileName { get; set; }

        [DataMember(Order = 3)]
        public string DataBytes { get; set; }

        [DataMember(Order = 4)]
        public string Error { get; set; }
    }
    [DataContract]
    public class UserLogin
    {
        [DataMember(Order = 0)]
        public string UserName { get; set; }
        [DataMember(Order = 2)]
        public string Password { get; set; }
    }
}
