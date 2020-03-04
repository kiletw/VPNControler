using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPNControler.Models;

namespace VPNControler.Functions
{
    public class CsvControl
    {
        private static string RouteMaskPath = $"{Environment.CurrentDirectory}\\RouteMask.config";
        private static string VPNPath = $"{Environment.CurrentDirectory}\\Info.config";


        public static ObservableCollection<RouteMask> LoadRouteMask()
        {
            ObservableCollection<RouteMask> RouteMasks = new ObservableCollection<RouteMask>();

            if (File.Exists(RouteMaskPath))
            {
                try
                {
                    using (StreamReader streamReader = new StreamReader(RouteMaskPath))
                    {
                        string line = "";
                        while ( (line = streamReader.ReadLine()) != null)
                        {
                            string[] param = line.Split(',');

                            RouteMask routeMask = new RouteMask()
                            {
                                IP = param[0],
                                Mask = param[1],
                                Desc = param[2]
                            };

                            RouteMasks.Add(routeMask);
                        }
                    }
                }
                catch
                {
                    return RouteMasks;
                }
            }
            return RouteMasks;
        }


        public static void SaveRouteMask(ObservableCollection<RouteMask> routeMasks)
        {
            StringBuilder stringBuilder = new StringBuilder(); 

            for(int i = 0; i < routeMasks.Count; i++)
            {
                stringBuilder.Append($"{routeMasks[i].IP},{routeMasks[i].Mask},{routeMasks[i].Desc}{Environment.NewLine}");
            }
            File.WriteAllText(RouteMaskPath, stringBuilder.ToString(), Encoding.UTF8);

        }


        public static VPNInfo LoadInfo()
        {
            VPNInfo LoadInfo = new VPNInfo() {
                VPNAccount = "vpn",
                VPNName = "VPN Control",
                VPNIP = "219.100.37.100",
                VPNPass = "vpn"
            
            };

            

            if (File.Exists(VPNPath))
            {
                try
                {
                    using (StreamReader streamReader = new StreamReader(RouteMaskPath))
                    {
                        string line = "";
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            string[] param = line.Split(',');

                            LoadInfo = new VPNInfo()
                            {
                                VPNAccount = param[0],
                                VPNName = param[1],
                                VPNIP = param[2],
                                VPNPass = param[3]
                            };
                        }
                    }
                }
                catch
                {
                    return LoadInfo;
                }
            }
            return LoadInfo;
        }


        public static void SaveInfo(VPNInfo vPNInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"{vPNInfo.VPNAccount},{vPNInfo.VPNName},{vPNInfo.VPNIP},{vPNInfo.VPNPass}{Environment.NewLine}");
            File.WriteAllText(VPNPath, stringBuilder.ToString(), Encoding.UTF8);

        }





    }
}
