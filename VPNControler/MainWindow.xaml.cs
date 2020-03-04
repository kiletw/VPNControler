using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VPNControler.Models;

namespace VPNControler
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<RouteMask> ListRouteMasks { get; set; }
        private VPNInfo vPNInfo;
        private VPNHelper vPNHelper;


        public MainWindow()
        {
            InitializeComponent();
            ListRouteMasks = VPNControler.Functions.CsvControl.LoadRouteMask();
            vPNInfo = VPNControler.Functions.CsvControl.LoadInfo();
            dg_route_mask.ItemsSource = ListRouteMasks;
            dg_route_mask.DataContext = this;
            tb_vpn_name.SetBinding(TextBox.TextProperty, new Binding() { Source = vPNInfo, Path = new PropertyPath("VPNName") });
            tb_vpn_ip.SetBinding(TextBox.TextProperty, new Binding() { Source = vPNInfo, Path = new PropertyPath("VPNIP") });
            tb_vpn_account.SetBinding(TextBox.TextProperty, new Binding() { Source = vPNInfo, Path = new PropertyPath("VPNAccount") });
            pb_vpn_pass.SetBinding(TextBox.TextProperty, new Binding() { Source = vPNInfo, Path = new PropertyPath("VPNPass") });



        }

            private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            var ip = "219.100.37.100";
            VPNHelper vPNHelper = new VPNHelper(ip, "Japen", "vpn", "vpn");
            vPNHelper.TryDisConnectVPN();
        }

        private void btn_mask_save_Click(object sender, RoutedEventArgs e)
        {

            VPNControler.Functions.CsvControl.SaveRouteMask(ListRouteMasks);

            
        }

        private void btn_vpn_connect_Click(object sender, RoutedEventArgs e)
        {
            string error = "";

            if(tb_vpn_name.Text.Trim() == "")
            {
                error += $"請填寫你的VPN連線名稱！{Environment.NewLine}";
               
            }

            if (tb_vpn_ip.Text.Trim() == "")
            {
                error += $"請填寫你的VPN IP！{Environment.NewLine}";
                
            }

            if(error != "")
            {
                MessageBox.Show(error);
                return;
            }




            vPNHelper = new VPNHelper(tb_vpn_ip.Text, tb_vpn_name.Text, tb_vpn_account.Text, pb_vpn_pass.Password);
            vPNHelper.CreateOrUpdateVPN();
            vPNHelper.TryConnectVPN();

            bool getVpnIP = false;
            int tryCount = 0;
            while(getVpnIP == false && tryCount < 10)
            {
                getVpnIP = VPNControler.Functions.NetworkControl.TryGetVpnIP(vPNHelper.VPNName,vPNHelper.IPToPing, ListRouteMasks);

                if(getVpnIP == false)
                {
                    tryCount++;
                    Thread.Sleep(500);
                }
            }

            if (getVpnIP)
            {
                MessageBox.Show("連線成功！");
                VPNControler.Functions.CsvControl.SaveInfo(vPNInfo);

            }
            else
            {
                MessageBox.Show("連線失敗！");
            }

            
        }

        private void btn_vpn_disconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vPNHelper.TryDisConnectVPN();
                MessageBox.Show("離線成功！");
            }
            catch
            {
                MessageBox.Show("離線失敗！");
            }
            
          
        }
    }
}
