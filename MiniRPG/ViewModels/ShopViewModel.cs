using System.Collections.ObjectModel;
using System.Linq;
using MiniRPG.Models;

namespace MiniRPG.ViewModels
{
    /// <summary>
    /// ViewModel for the shop interface, handles buying and selling items.
    /// </summary>
    public class ShopViewModel : BaseViewModel
    {
        private readonly ObservableCollection<string> _globalLog;

        public ObservableCollection<ShopItem> ShopInventory { get; set; }
        
        private ShopItem? _selectedItem;
        public ShopItem? SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        public Player Player { get; set; }

        public RelayCommand BuyCommand { get; set; }
        public RelayCommand SellCommand { get; set; }
        public RelayCommand ExitShopCommand { get; set; }

        // Event for when the player wants to exit the shop
        public event System.Action? OnExitShop;

        public ShopViewModel(Player player, ObservableCollection<string> globalLog)
        {
            Player = player;
            _globalLog = globalLog;
            
            // Initialize ShopInventory with sample items
            ShopInventory = new ObservableCollection<ShopItem>
            {
                new ShopItem(new Item("Potion", "Restores 10 HP", "Consumable", 25), 30, 15),
                new ShopItem(new Item("Wooden Sword", "A basic starter weapon.", "Weapon", 0) 
                { 
                    IsEquippable = true, 
                    SlotType = "Weapon", 
                    AttackBonus = 2 
                }, 50, 25),
                new ShopItem(new Item("Leather Armor", "Simple protection made of hide.", "Armor", 0) 
                { 
                    IsEquippable = true, 
                    SlotType = "Armor", 
                    DefenseBonus = 1 
                }, 40, 20)
            };

            // Initialize commands
            BuyCommand = new RelayCommand(ExecuteBuy);
            SellCommand = new RelayCommand(ExecuteSell);
            ExitShopCommand = new RelayCommand(_ => OnExitShop?.Invoke());
        }

        private void ExecuteBuy(object? parameter)
        {
            if (SelectedItem == null) return;

            if (Player.SpendGold(SelectedItem.BuyPrice))
            {
                Player.AddItem(SelectedItem.Item);
                string message = $"Purchased {SelectedItem.Item.Name}!";
                _globalLog.Add(message);
            }
            else
            {
                string message = "Not enough gold.";
                _globalLog.Add(message);
            }
        }

        private void ExecuteSell(object? parameter)
        {
            if (SelectedItem == null) return;

            // Check if Player.Inventory contains similar item
            var similarItem = Player.Inventory.FirstOrDefault(item => item.Name == SelectedItem.Item.Name);
            
            if (similarItem != null)
            {
                Player.Inventory.Remove(similarItem);
                Player.AddGold(SelectedItem.SellPrice);
                string message = $"Sold {SelectedItem.Item.Name}!";
                _globalLog.Add(message);
            }
            else
            {
                string message = "You don't have that item.";
                _globalLog.Add(message);
            }
        }

        private void ExecuteExitShop(object? parameter)
        {
            // This will be handled by the parent ViewModel (MainViewModel)
            // For now, just log the action
            _globalLog.Add("Exiting shop...");
        }

        // TODO: Add shop restock and random inventory later
    }
}