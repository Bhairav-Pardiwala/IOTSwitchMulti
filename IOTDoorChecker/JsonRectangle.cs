using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTMultiSwitch
{
   public  class JsonRectangle:INotifyPropertyChanged
    {
        public double x { get; set; } 
        public double y { get; set; }
        public double x1 { get; set; }
        public double y1 { get; set; }

        private bool sw;
        public bool? on
        {
            get
            {
                return sw;
            }
            set
            {
                if(value==true)
                {
                    sw = true;
                }
                else
                {
                    sw = false;
                }
                OnPropertyChanged("on");
               
            }
        }
        public String name { get; set; }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
