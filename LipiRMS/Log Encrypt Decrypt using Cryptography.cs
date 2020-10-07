using System;
using System.Security.Cryptography;
using System.Text;

public class SSTCryptographer
{
    private static string _key;

    public SSTCryptographer()
    {
    }

    public static string Key
    {
        set
        {
            _key = value;
        }
    }

    public static string Encrypt(string strToEncrypt)
    {
        try
        {
            return Encrypt(strToEncrypt, "lipi");
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
    }

    public static string Encrypt(byte[] byToEncrypt)
    {
        try
        {
            return Encrypt(byToEncrypt, "lipi");
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
    }

    public static string Decrypt(string strEncrypted)
    {
        try
        {
            return Decrypt(strEncrypted, "lipi");
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
    }

    public static string Decrypt(byte[] byEncrypted)
    {
        try
        {
            return Decrypt(byEncrypted, "lipi");
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
    }

    public static string Encrypt(string strToEncrypt, string strKey)
    {
        try
        {
            TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

            byte[] byteBuff;
            objHashMD5 = null;
            objDESCrypto.Key = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strKey));
            objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

            byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
            return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
        finally
        { GC.Collect(); }
    }

    public static string Encrypt(byte[] byToEncrypt, string strKey)
    {
        try
        {
            TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

            objHashMD5 = null;
            objDESCrypto.Key = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strKey));
            objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

            //byteBuff = ASCIIEncoding.ASCII.GetBytes(strToEncrypt);
            return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byToEncrypt, 0, byToEncrypt.Length));
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
        finally
        { GC.Collect(); }
    }

    public static string Decrypt(string strEncrypted, string strKey)
    {
        try
        {
            TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

            byte[] byteBuff;
            objHashMD5 = null;
            objDESCrypto.Key = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strKey));
            objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

            byteBuff = Convert.FromBase64String(strEncrypted);
            string strDecrypted = ASCIIEncoding.ASCII.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            objDESCrypto = null;

            return strDecrypted;
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
        finally
        { GC.Collect(); }
    }

    public static string Decrypt(byte[] byEncrypted, string strKey)
    {
        try
        {
            TripleDESCryptoServiceProvider objDESCrypto = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();

            objHashMD5 = null;
            objDESCrypto.Key = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strKey));
            objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

            //byteBuff = Convert.FromBase64String(strEncrypted);
            string strDecrypted = ASCIIEncoding.ASCII.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byEncrypted, 0, byEncrypted.Length));
            objDESCrypto = null;

            return strDecrypted;
        }
        catch (Exception ex)
        {
            return "Wrong Input. " + ex.Message;
        }
        finally
        { GC.Collect(); }
    }
}