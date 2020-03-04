using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPNControler.Models
{
    public class RouteMask : INotifyPropertyChanged
    {
        private string _IP;

        private string _Mask;

        private string _Desc;

        public string IP
        {
            get
            {
                return _IP;
            }
            set
            {
                if (_IP != value)
                {
                    _IP = value;
                    OnPropertyChanged("IP");
                }
            }
        }

        public string Mask
        {
            get
            {
                return _Mask;
            }
            set
            {
                if (_Mask != value)
                {
                    _Mask = value;
                    OnPropertyChanged("Mask");
                }
            }
        }

        public string Desc
        {
            get
            {
                return _Desc;
            }
            set
            {
                if (_Desc != value)
                {
                    _Desc = value;
                    OnPropertyChanged("Desc");
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
