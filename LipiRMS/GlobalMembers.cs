using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LipiRMS
{
    class GlobalMembers
    {
    }

    public enum District
    {
        Ajmer = 1,
        Alwar = 2,
        Banswara = 3,
        Barmer = 4,
        Bharatpur = 5,
        Bhilwara = 6,
        Bikaner = 7,
        Bundi = 8,
        Chittaurgarh = 9,
        Churu = 10,
        Dholpur = 11,
        Dungarpur = 12,
        Sri_Ganganagar = 13,
        Jaipur_South = 14,
        Jaisalmer = 15,
        Jalore = 16,
        Jhalawar = 17,
        Jhunjhunu = 18,
        Jodhpur = 19,
        Kota = 20,
        Nagaur = 21,
        Pali = 22,
        Sikar = 23,
        Sirohi = 24,
        Sawai_Madhopur = 25,
        Tonk = 26,
        Udaipur = 27,
        Baran = 28,
        Dausa = 29,
        Rajsamand = 30,
        Hanumangarh = 31,
        Kotputli = 32,
        Ramganj_Mandi = 33,
        Karauli = 34,
        Pratapgarh = 35,
        Beawar = 36,
        Didwana = 37,
        Rawat_bhata = 38,
        Balotra_Barmer = 39,
        Bhiwadi_Alwar = 40,
        Chomu_Jaipur = 41,
        Kishangarh_Ajmer = 42,
        Faloudi_Jodhpur = 43,
        Sujangarh_Churu = 44,
        Jaipur_North = 45,
        Abu_Road_Sirohi = 46,
        Dudu_Jaipur = 47,
        Kekari_Ajmer = 48,
        Nohar_Hanumangadh = 49,
        Nokha_Bikaner = 50,
        Shahpura_Bhilwara = 51,
        Shahpura_Jaipur = 52
    }

    public enum Devices
    {
        Camera = 1,
        Card_Reader,
        Cash_Acceptor,
        Finger_Scanner,
        Receipt_Printer,
        Statement_Printer,
        Touch_Screen,
        Digital_Signage
    }

    public enum CameraStatus
    {
        Ok = 0,
        Disconnected
    }

    public enum CardReaderStatus
    {
        Ok = 0,
        Disconnected
    }

    public enum CashAcceptorStatus
    {
        Ok = 0,
        Disconnected
    }

    public enum FingerScannerStatus
    {
        Ok = 0,
        Disconnected
    }

    public enum ReceiptPrinterStatus
    {
        Ok = 0,
        Disconnected,
        HeadOpen,
        PaperLow,
        PaperOut
    }

    public enum StatementPrinterStatus
    {
        Ok = 0,
        Disconnected
    }

    public enum TouchScreenStatus
    {
        Ok = 0,
        Disconnected
    }
}
