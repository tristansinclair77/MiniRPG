using System;
using System.Windows.Input;

namespace MiniRPG.ViewModels
{
    public class TitleViewModel : BaseViewModel
    {
        public ICommand NewGameCommand { get; }
        public ICommand ContinueCommand { get; }
        public event Action<string>? TitleSelectionMade;

        public TitleViewModel()
        {
            NewGameCommand = new RelayCommand(_ => TitleSelectionMade?.Invoke("New"));
            ContinueCommand = new RelayCommand(_ => TitleSelectionMade?.Invoke("Continue"));
        }
        // TODO: // Later - add fade transition and intro dialogue
    }
}
