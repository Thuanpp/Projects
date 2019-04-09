using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace PMUpgrade
{
    public class IUIniFile
    {
        [DllImport("KERNEL32.DLL", CharSet = CharSet.Auto)]
        public extern static int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, System.Text.StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("KERNEL32.DLL", CharSet = CharSet.Auto)]
        public extern static int GetPrivateProfileInt(string lpAppName, string lpKeyName, int nDefault, string lpFileName);

        [DllImport("KERNEL32.DLL")]
        public extern static int WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

        public static string IniFileStrGetting(string ini_AppName, string ini_KeyName, string ini_DefValue, string ini_IniName)
        {
            System.Text.StringBuilder strBuffer = new System.Text.StringBuilder();
            int ret;
            strBuffer.Capacity = 1024;
            ret = GetPrivateProfileString(ini_AppName, ini_KeyName, ini_DefValue, strBuffer, strBuffer.Capacity, ini_IniName);
            return strBuffer.ToString();
        }

        public static int IniFileIntGetting(string ini_AppName, string ini_KeyName, int ini_DefValue, string ini_IniName)
        {
            return GetPrivateProfileInt(ini_AppName, ini_KeyName, ini_DefValue, ini_IniName);
        }

        public static void IniFileStrSetting(string ini_AppName, string ini_KeyName, string ini_Target, string ini_IniName)
        {
            WritePrivateProfileString(ini_AppName, ini_KeyName, ini_Target, ini_IniName);
        }

        public static string fnc_ReturnMainPath()
        {
            string strReturnMainPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            strReturnMainPath = fnc_DirSep_Add(strReturnMainPath);

            return strReturnMainPath;
        }

        public static string fnc_DirSep_Add(string strPathName)
        {
            string strPathReturn;
            strPathName = strPathName.TrimEnd();
            if (strPathName.Substring(strPathName.Length - 1, 1) != @"\")
            {
                strPathReturn = strPathName.TrimEnd() + @"\";
            }
            else
            {
                strPathReturn = strPathName;
            }
            return strPathReturn;
        }

        public static bool IsExistConnect()
        {
            string sSerrver = IUIniFile.IniFileStrGetting("ODBC", "Serrver", "", IUIniFile.fnc_ReturnMainPath() + "Setting.ini");
            string sDB = IUIniFile.IniFileStrGetting("ODBC", "DB", "", IUIniFile.fnc_ReturnMainPath() + "Setting.ini");
            string sUser = IUIniFile.IniFileStrGetting("ODBC", "User", "", IUIniFile.fnc_ReturnMainPath() + "Setting.ini");
            string sPw = IUIniFile.IniFileStrGetting("ODBC", "Password", "", IUIniFile.fnc_ReturnMainPath() + "Setting.ini");

            if (string.IsNullOrEmpty(sSerrver) || string.IsNullOrEmpty(sDB) || string.IsNullOrEmpty(sUser) || string.IsNullOrEmpty(sPw))
            {
                return false;
            }
            return true;
        }

    }
}
   



