using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPNControler.Models
{
    public class VPNInfo : INotifyPropertyChanged
    {
        public string _VPNName;

        public string _VPNIP;

        public string _VPNAccount;

        public string _VPNPass;

        public string VPNName
        {
            get
            {
                return _VPNName;
            }
            set
            {
                if (_VPNName != value)
                {
                    _VPNName = value;
                    OnPropertyChanged("VPNName");
                }
            }
        }

        public string VPNIP
        {
            get
            {
                return _VPNIP;
            }
            set
            {
                if (_VPNIP != value)
                {
                    _VPNIP = value;
                    OnPropertyChanged("VPNIP");
                }
            }
        }

        public string VPNAccount
        {
            get
            {
                return _VPNAccount;
            }
            set
            {
                if (_VPNAccount != value)
                {
                    _VPNAccount = value;
                    OnPropertyChanged("VPNAccount");
                }
            }
        }

        public string VPNPass
        {
            get
            {
                return _VPNPass;
            }
            set
            {
                if (_VPNPass != value)
                {
                    _VPNPass = value;
                    OnPropertyChanged("VPNPass");
                }
            }
        }





        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
