using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SearchApp.ViewModels
{
    public class BaseViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public void ChangeProperty<T>(ref T oldValue, T newValue, Action<T> validate = null, [CallerMemberName]string prop = "")
        {
            if (!oldValue?.Equals(newValue) ?? true)
            {
                validate?.Invoke(newValue);
                oldValue = newValue;
                OnPropertyChanged(prop);
            }
        }
    }
}
