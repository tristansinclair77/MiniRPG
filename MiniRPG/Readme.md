# MiniRPG - Change Log

## Latest Update: Inn Stay UI in BuildingInteriorView

### Changes Made ✨

#### BuildingInteriorView.xaml - Inn Stay UI Added
- **Added**: Inn-specific UI elements for staying the night
  - **Visibility**: UI section only visible when `Building.Type == "Inn"` using `IsInn` binding
  - **Stay the Night Button**: 
    - Content: "Stay the Night"
    - Bound to `StayAtInnCommand` from BuildingInteriorViewModel
    - Positioned below the occupant list within the Occupants Expander
    - Styled with green theme (#446644) to differentiate from other buttons
    - Button highlights on hover (#557755)
    - Minimum width: 180px
    - Padding: 12,8 for comfortable click area
  - **Cost Note TextBlock**:
    - Text: "Cost: 20 Gold"
    - FontSize: 12
    - FontStyle: Italic
    - Foreground: #CCCCCC (light gray)
    - Centered below the button
    - Margin: 0,6,0,0 (6px spacing from button)
  - **Container**: StackPanel with 15px top margin to separate from Talk button
  - **Converter**: Uses `BoolToVis` converter (BooleanToVisibilityConverter) to show/hide based on `IsInn` property

- **Added**: TODO Comments for Future Inn Features
  - `<!-- TODO: Replace with innkeeper NPC dialogue and fade-out sleep animation -->`
    - Future enhancement: Talk to innkeeper first before staying
    - Innkeeper offers room options (standard, luxury)
    - Fade-out animation when going to sleep
    - Fade-in animation when waking up
    - Visual transition effect (screen darkens)
  - `<!-- TODO: Add sound effect for morning wake-up -->`
    - Morning wake-up sound effect (rooster crow, birds chirping)
    - Bed creak sound when lying down
    - Door close sound when entering room
    - Ambient inn sounds (fire crackling, distant chatter)

#### Requirements Fulfilled

All requirements from Instructions.txt have been implemented:

**BuildingInteriorView.xaml:**
- ✅ If `Building.Type == "Inn"`:
  - ✅ Added Button labeled "Stay the Night"
  - ✅ Button bound to `StayAtInnCommand`
  - ✅ Placed below occupant list (within Occupants Expander StackPanel)
- ✅ Added note TextBlock: "Cost: 20 Gold"
  - ✅ Positioned below the "Stay the Night" button
  - ✅ Styled as italic, light gray text
- ✅ Added TODO comments:
  - ✅ `<!-- TODO: Replace with innkeeper NPC dialogue and fade-out sleep animation -->`
  - ✅ `<!-- TODO: Add sound effect for morning wake-up -->`

#### Integration Flow

**Entering an Inn:**
1. **Player at Region**: Clicks on inn building (e.g., "Cozy Inn")
2. **Enter Building**: BuildingInteriorView loads with BuildingInteriorViewModel
3. **Check Building Type**: `IsInn` property evaluates to true
4. **UI Displays**: 
   - Occupants list shows innkeeper NPC
   - "Talk" button for speaking to innkeeper
   - **NEW**: "Stay the Night" button appears below Talk button
   - **NEW**: "Cost: 20 Gold" note displayed below Stay button
5. **Visibility**: Inn-specific UI is visible due to `BoolToVis` converter

**Non-Inn Buildings:**
1. **Player Enters Shop/House**: BuildingInteriorView loads
2. **Check Building Type**: `IsInn` property evaluates to false
3. **UI Displays**:
   - Occupants list shows NPCs
   - "Talk" button visible
   - Inn-specific UI is **hidden** (collapsed)
4. **No Inn Options**: Stay button and cost note not visible

**Staying at Inn:**
1. **Player Clicks "Stay the Night"**: StayAtInnCommand executes
2. **Check Gold**: BuildingInteriorViewModel verifies Player.Gold >= 20
3. **Success Path** (if enough gold):
   - `Player.SpendGold(20)` reduces gold by 20
   - `Player.HP = Player.MaxHP` restores HP to full
   - `TimeService.AdvanceHours(8)` advances time by 8 hours
   - Debug log: "You rest at the inn and feel refreshed."
   - **Future**: Fade-out animation, sleep cutscene, wake-up sound
4. **Failure Path** (if not enough gold):
   - Debug log: "You can't afford a room."
   - **Future**: Display message box or in-game notification
   - No gold spent, HP unchanged, time unchanged

#### Code Examples

**Inn-Specific UI Section in BuildingInteriorView.xaml:**```xaml
<!-- Inn Stay Section (only visible for inns) -->
<StackPanel Visibility="{Binding IsInn, Converter={StaticResource BoolToVis}}" 
            Margin="0,15,0,0">
    <!-- TODO: Replace with innkeeper NPC dialogue and fade-out sleep animation -->
    <!-- TODO: Add sound effect for morning wake-up -->
    
    <!-- Stay the Night Button -->
    <Button Content="Stay the Night" 
            Command="{Binding StayAtInnCommand}" 
            Padding="12,8"
            Foreground="#F9E97A" 
            Background="#446644" 
            FontWeight="Bold"
            FontSize="14"
            HorizontalAlignment="Center"
            MinWidth="180">
        <Button.Style>
            <Style TargetType="Button">
                <Setter Property="Background" Value="#446644" />
                <Style.Triggers>
                    <Trigger Property="IsMouseOver" Value="True">
                        <Setter Property="Background" Value="#557755" />
                    </Trigger>
                </Style.Triggers>
            </Style>
        </Button.Style>
    </Button>
    
    <!-- Cost Note -->
    <TextBlock Text="Cost: 20 Gold" 
               FontSize="12" 
               FontStyle="Italic" 
               Foreground="#CCCCCC" 
               HorizontalAlignment="Center" 
               Margin="0,6,0,0" />
</StackPanel>
**BuildingInteriorViewModel.cs IsInn Property:**```csharp
public bool IsInn => CurrentBuilding?.Type == "Inn";
**BuildingInteriorViewModel.cs StayAtInnCommand:**```csharp
// Add StayAtInnCommand if this is an inn
if (CurrentBuilding.Type == "Inn")
{
    StayAtInnCommand = new RelayCommand(_ => StayAtInn());
}

private void StayAtInn()
{
    const int cost = 20;
    if (Player.Gold >= cost)
    {
        Player.SpendGold(cost);
        Player.HP = Player.MaxHP;
        TimeService.AdvanceHours(8);
        Debug.WriteLine("You rest at the inn and feel refreshed.");
    }
    else
    {
        Debug.WriteLine("You can't afford a room.");
    }
}
#### Visual Layout

**Inn Building Interior:**┌─────────────────────────────────────────┐
│       The Cozy Inn                      │
│  A warm and welcoming place to rest     │
├─────────────────────────────────────────┤
│ ▼ Occupants                             │
│ ┌─────────────────────────────────────┐ │
│ │ • Innkeeper (Merchant)              │ │
│ │                                     │ │
│ └─────────────────────────────────────┘ │
│           [    Talk    ]                │
│                                         │
│      [  Stay the Night  ]               │
│         Cost: 20 Gold                   │
└─────────────────────────────────────────┘
        [  Leave Building  ]
**Shop Building Interior (non-inn):**┌─────────────────────────────────────────┐
│       General Shop                      │
│  A well-stocked shop with supplies      │
├─────────────────────────────────────────┤
│ ▼ Occupants                             │
│ ┌─────────────────────────────────────┐ │
│ │ • Shopkeeper (Merchant)             │ │
│ │                                     │ │
│ └─────────────────────────────────────┘ │
│           [    Talk    ]                │
│                                         │
│   (No inn-specific UI shown)            │
└─────────────────────────────────────────┘
        [  Leave Building  ]
#### Gameplay Examples

**Example 1: Successful Inn Stay**
- **Player Status**: 100 gold, 15/30 HP, Day 1 Morning (8:00 AM)
- **Location**: Inside "Cozy Inn" building
- **UI Shows**: 
  - Occupants: Innkeeper
  - Buttons: "Talk", "Stay the Night"
  - Note: "Cost: 20 Gold"
- **Action**: Player clicks "Stay the Night"
- **Result**: 
  - Gold: 100 → 80
  - HP: 15/30 → 30/30
  - Time: Day 1, Morning (8:00 AM) → Day 1, Afternoon (4:00 PM)
  - Message: "You rest at the inn and feel refreshed."
  - UI updates to show new stats and time

**Example 2: Insufficient Gold**
- **Player Status**: 10 gold, 20/30 HP, Day 1 Evening (8:00 PM)
- **Location**: Inside "Cozy Inn" building
- **UI Shows**: "Stay the Night" button available
- **Action**: Player clicks "Stay the Night"
- **Result**:
  - Gold: 10 (unchanged)
  - HP: 20/30 (unchanged)
  - Time: Day 1, Evening (unchanged)
  - Message: "You can't afford a room."
  - Player remains in inn, can leave or try again later

**Example 3: Non-Inn Building**
- **Location**: Inside "General Shop" building
- **Building Type**: Shop (not Inn)
- **UI Shows**:
  - Occupants: Shopkeeper
  - Buttons: "Talk" only
  - **No** "Stay the Night" button
  - **No** "Cost: 20 Gold" note
- **Reason**: `IsInn` is false, inn-specific UI is collapsed

#### Potential Future Enhancements

Based on the new TODO comments:

**Innkeeper NPC Dialogue Integration:**
- **Talk to Innkeeper First**:
  - Player talks to innkeeper NPC
  - Innkeeper greets: "Welcome to the inn! Need a place to rest?"
  - Dialogue options appear:
    - "I'd like a room for the night." → Opens room selection
    - "Tell me about your inn." → Lore/information
    - "Maybe later." → Closes dialogue
- **Room Selection Dialogue**:
  - Standard Room: 20 gold, 8 hours, HP restore
  - Luxury Room: 50 gold, 8 hours, HP+MP restore, remove status effects
  - Decline: Return to main dialogue
- **Replacement Strategy**:
  - Remove direct "Stay the Night" button
  - Innkeeper dialogue triggers room selection
  - More immersive and realistic
  - Allows for innkeeper personality and lore

**Fade-Out Sleep Animation:**
- **Going to Sleep Sequence**:
  1. Player confirms room selection
  2. Screen fades to black (1-2 seconds)
  3. Text appears: "You sleep peacefully through the night..."
  4. Optional: Show sleeping player sprite or moon/stars visual
  5. Text continues: "Morning has arrived. You feel refreshed!"
  6. Screen fades back in (1-2 seconds)
  7. Player back in inn lobby, time advanced, HP restored
- **Implementation**:
  - Use Storyboard with DoubleAnimation on Grid Opacity
  - Transition from 1.0 to 0.0 (fade out)
  - Show sleep message overlay
  - Transition from 0.0 to 1.0 (fade in)
- **Visual Elements**:
  - Black overlay during sleep
  - Text overlay with sleep messages
  - Optional dream sequence images
  - Smooth animation timing

**Sound Effect for Morning Wake-Up:**
- **Morning Sound Effects**:
  - **Rooster Crow**: Classic wake-up sound
