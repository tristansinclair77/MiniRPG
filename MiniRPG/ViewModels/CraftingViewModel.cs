using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MiniRPG.Models;
using MiniRPG.Services;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// ViewModel for the Crafting & Enhancement interface.
    /// </summary>
    public class CraftingViewModel : BaseViewModel
    {
        public Player Player { get; }

        public ObservableCollection<CraftingRecipe> AllRecipes { get; }

        private CraftingRecipe? _selectedRecipe;
        public CraftingRecipe? SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                _selectedRecipe = value;
                OnPropertyChanged();
            }
        }

        public ICommand CraftCommand { get; set; }
        public ICommand ExitCraftingCommand { get; set; }

        // Event for when the player wants to exit the crafting view
        public event System.Action? OnExitCrafting;

        public CraftingViewModel(Player player)
        {
            Player = player;
            AllRecipes = new ObservableCollection<CraftingRecipe>(CraftingService.GetAllRecipes());

            CraftCommand = new RelayCommand(_ => CraftItem());
            ExitCraftingCommand = new RelayCommand(_ => OnExitCrafting?.Invoke());

            // TODO: Add bulk crafting and auto-select available recipes
        }

        private void CraftItem()
        {
            if (SelectedRecipe == null)
            {
                return;
            }

            if (SelectedRecipe.CanCraft(Player))
            {
                SelectedRecipe.Craft(Player);
                SaveLoadService.SavePlayer(Player);
                System.Diagnostics.Debug.WriteLine($"Successfully crafted {SelectedRecipe.ResultItem.Name}!");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Missing materials or gold.");
            }

            // Update UI to reflect changes
            OnPropertyChanged(nameof(SelectedRecipe));
            OnPropertyChanged(nameof(Player));
        }
    }
}
