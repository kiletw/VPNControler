using DotRas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VPNControler
{
    public class VPNHelper
    {
        // 系統路徑 C:\windows\system32\
        private static string WinDir = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"";
        // rasdial.exe
        private static string RasDialFileName = "\\rasdial.exe";
        // VPN路徑 C:\windows\system32\rasdial.exe
        private static string VPNPROCESS = WinDir + RasDialFileName;
        // VPN地址
        public string IPToPing { get; set; }
        // VPN名稱
        public string VPNName { get; set; }
        // VPN使用者名稱
        public string UserName { get; set; }
        // VPN密碼
        public string PassWord { get; set; }

        public VPNHelper()
        {
        }
        /// <summary>
        /// 帶參建構函式
        /// </summary>
        /// <param name="_vpnIP"></param>
        /// <param name="_vpnName"></param>
        /// <param name="_userName"></param>
        /// <param name="_passWord"></param>
        public VPNHelper(string _vpnIP, string _vpnName, string _userName, string _passWord)
        {
            this.IPToPing = _vpnIP;
            this.VPNName = _vpnName;
            this.UserName = _userName;
            this.PassWord = _passWord;
        }
        /// <summary>
        /// 嘗試連線VPN(預設VPN)
        /// </summary>
        /// <returns></returns>
        public void TryConnectVPN()
        {
            this.TryConnectVPN(this.VPNName, this.UserName, this.PassWord);
        }
        /// <summary>
        /// 嘗試斷開連線(預設VPN)
        /// </summary>
        /// <returns></returns>
        public void TryDisConnectVPN()
        {
            this.TryDisConnectVPN(this.VPNName);
        }
        /// <summary>
        /// 建立或更新一個預設的VPN連線
        /// </summary>
        public void CreateOrUpdateVPN()
        {
            this.CreateOrUpdateVPN(this.VPNName, this.IPToPing);
        }
        /// <summary>
        /// 嘗試刪除連線(預設VPN)
        /// </summary>
        /// <returns></returns>
        public void TryDeleteVPN()
        {
            this.TryDeleteVPN(this.VPNName);
        }
        /// <summary>
        /// 嘗試連線VPN(指定VPN名稱，使用者名稱，密碼)
        /// </summary>
        /// <returns></returns>
        public void TryConnectVPN(string connVpnName, string connUserName, string connPassWord)
        {
            try
            {
                string args = string.Format("{0} {1} {2}", connVpnName, connUserName, connPassWord);
                ProcessStartInfo myProcess = new ProcessStartInfo(VPNPROCESS, args);
                myProcess.CreateNoWindow = true;
                myProcess.UseShellExecute = false;
                Process.Start(myProcess);
            }
            catch (Exception Ex)
            {
                Debug.Assert(false, Ex.ToString());
            }
        }
        /// <summary>
        /// 嘗試斷開VPN(指定VPN名稱)
        /// </summary>
        /// <returns></returns>
        public void TryDisConnectVPN(string disConnVpnName)
        {
            try
            {
                string args = string.Format(@"{0} /d", disConnVpnName);
                ProcessStartInfo myProcess = new ProcessStartInfo(VPNPROCESS, args);
                myProcess.CreateNoWindow = true;
                myProcess.UseShellExecute = false;
                Process.Start(myProcess);

            }
            catch (Exception Ex)
            {
                Debug.Assert(false, Ex.ToString());
            }
        }
        /// <summary>
        /// 建立或更新一個VPN連線(指定VPN名稱，及IP)
        /// </summary>
        public void CreateOrUpdateVPN(string updateVPNname, string updateVPNip)
        {
            RasDialer dialer = new RasDialer();
            RasPhoneBook allUsersPhoneBook = new RasPhoneBook();
            string path = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            allUsersPhoneBook.Open(path);
            // 如果已經該名稱的VPN已經存在，則更新這個VPN伺服器地址
            if (allUsersPhoneBook.Entries.Contains(updateVPNname))
            {
                allUsersPhoneBook.Entries[updateVPNname].PhoneNumber = updateVPNip;
                // 不管當前VPN是否連線，伺服器地址的更新總能成功，如果正在連線，則需要VPN重啟後才能起作用
                allUsersPhoneBook.Entries[updateVPNname].Update();
                
            }
            // 建立一個新VPN
            else
            {
                RasEntry entry = RasEntry.CreateVpnEntry(updateVPNname, updateVPNip, RasVpnStrategy.L2tpOnly, RasDevice.Create(updateVPNname, RasDeviceType.Vpn),false);
                entry.Options.RequireEncryptedPassword = false;
                entry.Options.UsePreSharedKey = true;
               
                entry.Options.RequirePap = false;
                entry.Options.RequireChap = true;
                entry.Options.RequireMSChap = false;
                entry.Options.RequireMSChap2 = true;
                allUsersPhoneBook.Entries.Add(entry);
                allUsersPhoneBook.Entries[updateVPNname].UpdateCredentials(RasPreSharedKey.Client,"vpn");
                dialer.EntryName = updateVPNname;
                dialer.PhoneBookPath = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            }
        }
        /// <summary>
        /// 刪除指定名稱的VPN
        /// 如果VPN正在執行，一樣會在電話本里刪除，但是不會斷開連線，所以，最好是先斷開連線，再進行刪除操作
        /// </summary>
        /// <param name="delVpnName"></param>
        public void TryDeleteVPN(string delVpnName)
        {
            RasDialer dialer = new RasDialer();
            RasPhoneBook allUsersPhoneBook = new RasPhoneBook();
            string path = RasPhoneBook.GetPhoneBookPath(RasPhoneBookType.AllUsers);
            allUsersPhoneBook.Open(path);
            if (allUsersPhoneBook.Entries.Contains(delVpnName))
            {
                allUsersPhoneBook.Entries.Remove(delVpnName);
            }
        }
        /// <summary>
        /// 獲取當前正在連線中的VPN名稱
        /// </summary>
        public List<string> GetCurrentConnectingVPNNames()
        {
            List<string> ConnectingVPNList = new List<string>();
            Process proIP = new Process();
            proIP.StartInfo.FileName = "cmd.exe ";
            proIP.StartInfo.UseShellExecute = false;
            proIP.StartInfo.RedirectStandardInput = true;
            proIP.StartInfo.RedirectStandardOutput = true;
            proIP.StartInfo.RedirectStandardError = true;
            proIP.StartInfo.CreateNoWindow = true;//不顯示cmd視窗
            proIP.Start();
            proIP.StandardInput.WriteLine(RasDialFileName);
            proIP.StandardInput.WriteLine("exit");
            // 命令列執行結果
            string strResult = proIP.StandardOutput.ReadToEnd();
            proIP.Close();
            // 用正則表示式匹配命令列結果，只限於簡體中文系統哦^_^
            Regex regger = new Regex("(?<=已連線\r\n)(.*\n)*(?=命令已完成)", RegexOptions.Multiline);
            // 如果匹配，則說有正在連線的VPN
            if (regger.IsMatch(strResult))
            {
                string[] list = regger.Match(strResult).Value.ToString().Split('\n');
                for (int index = 0; index < list.Length; index++)
                {
                    if (list[index] != string.Empty)
                        ConnectingVPNList.Add(list[index].Replace("\r", ""));
                }
            }
            // 沒有正在連線的VPN，則直接返回一個空List<string>
            return ConnectingVPNList;
        }




    }
}
