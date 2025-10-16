using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// Base class for ViewModels implementing INotifyPropertyChanged (MVVM pattern).
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
