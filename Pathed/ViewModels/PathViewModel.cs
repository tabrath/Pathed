using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathed.ViewModels
{
    public class PathViewModel : ViewModelBase
    {
        private string value;
        public string Value
        {
            get { return this.value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;

                    RaisePropertyChanged();
                }
            }
        }

        public bool Exists
        {
            get { return Directory.Exists(this.value); }
        }

        private bool isSet;
        public bool IsSet
        {
            get { return this.isSet; }
            set
            {
                if (this.isSet != value)
                {
                    this.isSet = value;

                    RaisePropertyChanged();
                }
            }
        }

        private bool isDuplicate;
        public bool IsDuplicate
        {
            get { return this.isDuplicate; }
            set
            {
                if (this.isDuplicate != value)
                {
                    this.isDuplicate = value;

                    RaisePropertyChanged();
                }
            }
        }

        public PathViewModel(string value, bool isSet = true)
        {
            this.value = value;
            this.isSet = isSet;
            this.isDuplicate = false;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
