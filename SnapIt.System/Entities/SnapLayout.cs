using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SnapIt.Entities
{
    public class SnapLayout : INotifyPropertyChanged
    {
        private string name;

        public Guid Guid { get; set; }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}