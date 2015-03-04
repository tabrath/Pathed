using System.IO;
using GalaSoft.MvvmLight;

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

        public PathViewModel(string value, bool isSet)
        {
            this.value = value;
            this.isSet = isSet;
            this.isDuplicate = false;
        }

        public PathViewModel(string value)
            : this(value, true)
        {
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
