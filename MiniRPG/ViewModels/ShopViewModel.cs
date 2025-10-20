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
        private readonly Region? _currentRegion;

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

        private int _selectedInventoryIndex = -1;
        public int SelectedInventoryIndex
        {
            get => _selectedInventoryIndex;
            set
            {
                _selectedInventoryIndex = value;
                OnPropertyChanged();
            }
        }

        public Player Player { get; set; }

        public RelayCommand BuyCommand { get; set; }
        public RelayCommand SellCommand { get; set; }
        public RelayCommand ExitShopCommand { get; set; }

        // Event for when the player wants to exit the shop
        public event System.Action? OnExitShop;

        public ShopViewModel(Player player, ObservableCollection<string> globalLog, Region? currentRegion = null)
        {
            Player = player;
            _globalLog = globalLog;
            _currentRegion = currentRegion;
            
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

            // Apply faction-based price adjustments
            ApplyFactionPriceModifiers();

            // Initialize commands
            BuyCommand = new RelayCommand(ExecuteBuy, CanBuy);
            SellCommand = new RelayCommand(ExecuteSell);
            ExitShopCommand = new RelayCommand(_ => OnExitShop?.Invoke());
        }

        /// <summary>
        /// Applies faction reputation-based price modifiers to all shop items.
        /// </summary>
        private void ApplyFactionPriceModifiers()
        {
            if (_currentRegion == null || string.IsNullOrEmpty(_currentRegion.FactionName))
                return;

            var faction = Player.GetFaction(_currentRegion.FactionName);
            float modifier = faction.Reputation >= 50 ? 0.9f :
                             faction.Reputation <= -50 ? 1.2f : 1.0f;

            // Adjust prices for all items in shop
            foreach (var shopItem in ShopInventory)
            {
                shopItem.BuyPrice = (int)(shopItem.BuyPrice * modifier);
                shopItem.SellPrice = (int)(shopItem.SellPrice * modifier);
            }

            // Log price adjustments for player awareness
            if (modifier < 1.0f)
            {
                _globalLog.Add($"The shopkeeper offers you a discount due to your good reputation!");
            }
            else if (modifier > 1.0f)
            {
                _globalLog.Add($"The shopkeeper charges extra due to your poor reputation.");
            }

            // TODO: Add special item discounts for Allied factions
        }

        /// <summary>
        /// Determines if the player can buy from this shop based on faction reputation.
        /// </summary>
        private bool CanBuy(object? parameter)
        {
            if (_currentRegion == null || string.IsNullOrEmpty(_currentRegion.FactionName))
                return true;

            var faction = Player.GetFaction(_currentRegion.FactionName);
            
            if (faction.Reputation < -50)
            {
                return false;
            }

            return true;
        }

        private void ExecuteBuy(object? parameter)
        {
            // Check if trading is allowed
            if (_currentRegion != null && !string.IsNullOrEmpty(_currentRegion.FactionName))
            {
                var faction = Player.GetFaction(_currentRegion.FactionName);
                if (faction.Reputation < -50)
                {
                    _globalLog.Add("They refuse to do business with you.");
                    return;
                }
            }

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
            string message = string.Empty;
            // Defensive: Check Player and Inventory
            if (Player == null)
            {
                _globalLog.Add("Error: Player is null.");
                return;
            }
            if (Player.Inventory == null)
            {
                _globalLog.Add("Error: Player.Inventory is null.");
                return;
            }
            // Defensive: Remove by slot
            if (SelectedInventoryItem != null && SelectedInventoryIndex >= 0 && SelectedInventoryIndex < Player.Inventory.Count)
            {
                var itemInSlot = Player.Inventory[SelectedInventoryIndex];
                string soldName = itemInSlot?.Name ?? "(unknown)";
                if (itemInSlot == null)
                {
                    message = "Error: Inventory slot already empty.";
                }
                else if (!object.ReferenceEquals(itemInSlot, SelectedInventoryItem))
                {
                    message = "Error: Selected item does not match inventory slot.";
                }
                else
                {
                    int sellPrice = 0;
                    if (SelectedInventoryItem != null)
                    {
                        sellPrice = (SelectedInventoryItem.Value > 0 ? SelectedInventoryItem.Value : 0) / 2;
                        var shopItem = ShopInventory?.FirstOrDefault(si => si?.Item != null && si.Item.Name == SelectedInventoryItem.Name);
                        if (shopItem != null)
                        {
                            sellPrice = shopItem.SellPrice;
                        }
                    }
                    Player.Inventory[SelectedInventoryIndex] = null;
                    Player.AddGold(sellPrice);
                    message = $"Sold {soldName} for {sellPrice} gold!";
                    // Notify inventory count changed
                    Player.NotifyInventoryChanged();
                    Player.CompactInventory();
                }
                _globalLog.Add(message);
                SelectedInventoryItem = null;
                SelectedInventoryIndex = -1;
            }
            else if (SelectedItem != null)
            {
                int similarItemIndex = -1;
                string soldName = SelectedItem.Item?.Name ?? "(unknown)";
                if (Player.Inventory != null)
                {
                    similarItemIndex = Player.Inventory.ToList().FindIndex(item => item != null && SelectedItem.Item != null && item.Name == SelectedItem.Item.Name);
                }
                if (similarItemIndex >= 0 && Player.Inventory[similarItemIndex] != null)
                {
                    Player.Inventory[similarItemIndex] = null;
                    Player.AddGold(SelectedItem.SellPrice);
                    message = $"Sold {soldName}!";
                    // Notify inventory count changed
                    Player.NotifyInventoryChanged();
                    Player.CompactInventory();
                }
                else
                {
                    message = "You don't have that item.";
                }
                _globalLog.Add(message);
            }
            else
            {
                message = "No item selected to sell.";
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