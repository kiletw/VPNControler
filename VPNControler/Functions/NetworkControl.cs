using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using VPNControler.Models;

namespace VPNControler.Functions
{
    public class NetworkControl
    {

        public static bool TryGetVpnIP(string vpnName,string ip, ObservableCollection<RouteMask> ListRouteMasks)
        {
            bool isGet = false;

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            NetworkInterface adapter = adapters.Where(x => x.Name == vpnName).FirstOrDefault();

            if (adapter != null)
            {

                IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection uniCast = adapterProperties.UnicastAddresses;
                if (uniCast.Count > 0)
                {
                    foreach (UnicastIPAddressInformation uni in uniCast)
                    {
                       Match match =  Regex.Match(uni.Address.ToString(), @"(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])");

                        if(match.Value != "")
                        {
                            SetRouteLists(match.Value, ListRouteMasks);
                            isGet = true;
                        }
                    }
                }
            }
            return isGet;
        }


        private static void SetRouteLists(string ip , ObservableCollection<RouteMask> ListRouteMasks)
        {
            Process proIP = new Process();
            proIP.StartInfo.FileName = "cmd.exe ";
            proIP.StartInfo.UseShellExecute = false;
            proIP.StartInfo.RedirectStandardInput = true;
            proIP.StartInfo.RedirectStandardOutput = true;
            proIP.StartInfo.RedirectStandardError = true;
            proIP.StartInfo.CreateNoWindow = true;
            proIP.Start();
            for(int i = 0; i < ListRouteMasks.Count; i++)
            {
                proIP.StandardInput.WriteLine($"route add {ListRouteMasks[i].IP} mask {ListRouteMasks[i].Mask} {ip} ");
            }
            proIP.StandardInput.WriteLine("exit");
        }

    }
}
