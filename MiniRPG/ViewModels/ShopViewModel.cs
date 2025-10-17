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
            set 
            { 
                _selectedItem = value;
                OnPropertyChanged();
                // Clear inventory selection when shop item is selected
                if (value != null)
                {
                    SelectedInventoryItem = null;
                }
            }
        }

        private Item? _selectedInventoryItem;
        public Item? SelectedInventoryItem
        {
            get => _selectedInventoryItem;
            set
            {
                _selectedInventoryItem = value;
                OnPropertyChanged();
                // Clear shop selection when inventory item is selected
                if (value != null)
                {
                    SelectedItem = null;
                }
            }
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

            // Check inventory capacity
            if (Player.Inventory.Count >= Player.MaxInventoryCapacity)
            {
                string fullMsg = $"Cannot buy {SelectedItem.Item.Name}: Inventory Full!";
                _globalLog.Add(fullMsg);
                return;
            }

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
            // Sell from player's selected inventory item
            if (SelectedInventoryItem != null)
            {
                // Calculate sell price (half of item value or use shop's sell price if available)
                int sellPrice = SelectedInventoryItem.Value / 2;
                
                // Check if this item exists in shop inventory to get proper sell price
                var shopItem = ShopInventory.FirstOrDefault(si => si.Item.Name == SelectedInventoryItem.Name);
                if (shopItem != null)
                {
                    sellPrice = shopItem.SellPrice;
                }
                
                Player.RemoveItem(SelectedInventoryItem);
                Player.AddGold(sellPrice);
                string message = $"Sold {SelectedInventoryItem.Name} for {sellPrice} gold!";
                _globalLog.Add(message);
                
                // Clear selection
                SelectedInventoryItem = null;
            }
            else if (SelectedItem != null)
            {
                // Legacy: Sell from shop inventory (check if player has it)
                var similarItem = Player.Inventory.FirstOrDefault(item => item.Name == SelectedItem.Item.Name);
                
                if (similarItem != null)
                {
                    Player.RemoveItem(similarItem);
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
            else
            {
                string message = "No item selected to sell.";
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