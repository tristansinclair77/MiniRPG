# MiniRPG - Change Log

## Latest Update: Weather Display UI

### MapView.xaml Enhancements
- **Added weather display to region header**:
  - New TextBlock showing current weather near the region/time header
  - Weather text is color-coded based on weather type:
    - Blue for Rain and Storm
    - White for Snow
    - Gray for Fog
    - Yellow for Clear
  - Uses data binding to `CurrentWeather` property in MapViewModel
- **Added TODO comments**:
  - `<!-- TODO: Add weather icons or animated particle layers -->`
  - `<!-- TODO: Add ambient rain and snow overlays -->`
- **Added WeatherColorConverter to resources**:
  - Registered new converter for use in weather display

### MapViewModel.cs Enhancements
- **Added `CurrentWeather` property**:
  - Binds to `EnvironmentService.Weather` for UI display
  - Provides real-time weather data to the view
- **Updated `OnWeatherChanged` handler**:
  - Now calls `OnPropertyChanged(nameof(CurrentWeather))` to notify UI
  - Ensures weather display updates immediately when weather changes
  - Continues to log weather changes to GlobalLog

### WeatherColorConverter.cs (New File)
- **Created new value converter**:
  - Converts `WeatherType` enum to appropriate color brushes
  - Implements `IValueConverter` interface
  - Color mapping:
    - `WeatherType.Rain` and `WeatherType.Storm` → Blue
    - `WeatherType.Snow` → White
    - `WeatherType.Fog` → Gray
    - `WeatherType.Clear` → Yellow
  - Default fallback color is White

### Technical Details
- Weather display integrates seamlessly with existing weather system
- UI updates automatically when weather changes (every 6 hours or on event)
- Color-coded weather provides visual feedback at a glance
- Foundation laid for future weather icons and animated particle effects
- Event-driven architecture ensures real-time UI updates

---

## Previous Update: TimeService Weather Integration

### TimeService.cs Enhancements
- **Added `CurrentRegionName` static property**:
  - Tracks the current region name for weather calculations
  - Defaults to "Default"
  - Set externally by MapViewModel when region changes
  - Enables region-aware weather changes every 6 hours
- **Subscribed to `OnHourChanged` event**:
  - Static constructor subscribes to the service's own OnHourChanged event
  - Enables automatic weather change handling
- **Added `HandleWeatherChanges` event handler**:
  - Checks if the current hour is divisible by 6 (0, 6, 12, 18)
  - Calls `EnvironmentService.RandomizeWeather()` with CurrentRegionName or "Default"
  - Automatically triggers weather changes every 6 hours as time advances
- **Added TODO comment**:
  - // TODO: Add gradual weather transitions and forecast system

### MapViewModel.cs Enhancements
- **Sets `TimeService.CurrentRegionName`**:
  - Updated region-specific constructor to set the current region name in TimeService
  - Ensures weather changes are region-appropriate
- **Subscribed to `EnvironmentService.OnWeatherChanged` event**:
  - Both constructors now subscribe to weather change events
  - Added `OnWeatherChanged` handler method
- **Added `OnWeatherChanged` handler**:
  - Logs message to GlobalLog: "The weather changes to {newWeather}."
  - Provides player feedback when weather changes occur
  - Integrates with existing event-driven architecture

### Technical Details
- Weather changes now fully automated through TimeService's event subscription
- Every 6 hours (at hours 0, 6, 12, 18), weather randomizes based on current region
- Region-aware weather patterns ensure appropriate weather for the player's location
- Event-driven architecture allows multiple systems to react to weather changes
- Weather log messages appear in the game's GlobalLog for player awareness
- System integrates seamlessly with existing time advancement from battles, resting, fast travel, and inn stays
- Foundation laid for gradual weather transitions and forecast systems in future updates

---

## Previous Update: Dynamic Weather System

### EnvironmentService.cs Enhancements
- **Added `WeatherType` enum**:
  - Five weather types: Clear, Rain, Storm, Snow, Fog
  - Used for tracking current weather conditions
- **Added `Weather` static property**:
  - Defaults to `WeatherType.Clear`
  - Public getter with private setter for controlled weather changes
- **Added `OnWeatherChanged` event**:
  - Event triggered when weather changes
  - Passes the new WeatherType as an event argument
  - Enables UI and gameplay systems to react to weather changes
- **Added `RandomizeWeather(string regionName)` method**:
  - Uses Random to pick weather based on region type
  - **Plains/Town weather probabilities**:
    - 60% Clear, 25% Rain, 10% Fog, 5% Storm
  - **Mountain weather probabilities**:
    - 40% Clear, 40% Snow, 20% Fog
  - Region type determined by parsing region name (plains, town, mountain, woods, etc.)
  - Sets Weather property and triggers OnWeatherChanged event when weather changes
- **Added automatic weather changes**:
  - Weather randomizes every 6 in-game hours
  - Integrated into `UpdateLighting()` method
  - Tracks hours since last weather change
- **Added helper method `DetermineRegionType(string regionName)`**:
  - Parses region names to categorize them (Mountain, Town, Plains)
  - Enables region-appropriate weather patterns
  - Woods/Forest regions use Plains weather probabilities
- **Added TODO comments**:
  - // TODO: Add localized weather (per-region tracking)
  - // TODO: Add weather-based enemy effects

### Technical Details
- Weather system is event-driven and integrates with existing time advancement
- Static Random instance ensures consistent probability distribution
- Weather changes automatically every 6 hours as time advances (battles, resting, fast travel, inn stays)
- Foundation laid for future weather-based gameplay mechanics (enemy buffs/debuffs, visual effects)
- Region-aware weather patterns make the world feel more dynamic and realistic
- Event architecture allows UI to display weather icons and effects to show weather visuals

---

## Previous Update: Time-Based Building Interior NPC Visibility

### BuildingInteriorViewModel.cs Enhancements
- **Added `VisibleOccupants` property**:
  - Filters NPCs based on time and building type
  - For home buildings (`IsHome = true`): NPCs appear when NOT available outside (i.e., at home during off-hours)
  - For non-home buildings (shops, inns): NPCs appear during their work hours
  - NPCs now automatically appear/disappear from buildings based on time of day
- **Subscribed to `TimeService.OnHourChanged` event**:
  - Automatically refreshes visible occupants when hour changes
  - Updates UI in real-time when time advances (from resting, traveling, or staying at inn)
- **Enhanced `StayAtInn()` method**:
  - Now calls `EnvironmentService.UpdateLighting()` after time advancement
  - Refreshes visible occupants after staying at inn
  - Ensures NPCs appear/disappear correctly after player rests
- **Memory management**:
  - Unsubscribes from time events in `ExitBuilding()` to prevent memory leaks

### BuildingInteriorView.xaml Enhancements
- **Updated occupant binding**:
  - Changed from `Occupants` to `VisibleOccupants` for time-aware NPC filtering
  - NPCs now only appear in buildings during appropriate hours
- **Added TODO**: Add lighting for building windows at night

### Technical Details
- NPCs at home buildings appear when outside their work schedule (e.g., Mira appears at home at night)
- NPCs at shop buildings appear during their work hours (e.g., Shopkeeper appears at shop during business hours)
- System integrates with existing time advancement from battles, resting, fast travel, and inn stays
- Event-driven architecture ensures real-time updates when time changes
- Completes the time-based NPC scheduling system across all game areas (map and building interiors)

### Testing Checklist Completion
✅ 1. Observe NPC availability during morning → some appear.
✅ 2. Rest or travel to advance hours → NPCs disappear or change.
✅ 3. Confirm environment color and ambient sounds change accordingly.
✅ 4. Enter buildings → verify NPCs at home at night.
✅ 5. Save and reload → time and environment persist.
✅ 6. Add TODO placeholders:
   - <!-- TODO: Add festivals and events on specific days -->
   - <!-- TODO: Add lighting for building windows at night -->
   - <!-- TODO: Add time-based shop opening hours -->

---

## Previous Update: Time Persistence in Save System

### SaveLoadService.cs Enhancements
- **Extended save data with time persistence**:
  - `SaveData` class already includes `Day` and `Hour` fields for time tracking
  - `SavePlayer()` serializes current `TimeService.Day` and `TimeService.Hour` to save file
  - Debug logging confirms time is saved with each save operation
- **Enhanced LoadPlayer() with time restoration**:
  - Restores `TimeService.Day` and `TimeService.Hour` from save data
  - **Triggers `EnvironmentService.UpdateLighting()`** after restoring time values
  - Ensures lighting state (Daylight/Twilight/Night) matches the loaded game time
  - Works for both new SaveData format and legacy Player-only format
  - Also triggers lighting update when loading from backup saves
- **Added TODO**: Add autosave on midnight or event triggers later

### Technical Details
- Time data is persisted in the `SaveData` wrapper class alongside Player and UnlockedRegions
- When loading, the time is restored first, then `UpdateLighting()` is called to sync the environment
- The lighting update ensures the visual state matches the loaded time (e.g., Night lighting at midnight)
- Backup save restoration also includes time recovery and lighting sync
- Foundation laid for future autosave triggers on time events (midnight, dawn, etc.)

---

## Previous Update: NPC Home Assignment System

### Building.cs Enhancements
- **Added `IsHome` property**: 
  - Boolean property to identify buildings that serve as NPC homes
  - Defaults to `false`
  - Can be set to `true` for buildings assigned as homes for specific NPCs

### WorldMapService.cs Enhancements
- **Assigned buildings as homes for specific NPCs**:
  - Mira's Home is marked as `IsHome = true` and assigned as Mira's home
  - General Shop is marked as `IsHome = true` and assigned as Shopkeeper's home
- **Updated NPC initialization**:
  - `mira.CurrentLocation` set to "Mira's Home"
  - `shopkeeper.CurrentLocation` set to "General Shop"
  - NPCs now have their home locations properly initialized
- **Added TODO**: Add NPC bedtime dialogues and idle animations

### Technical Details
- Buildings can now be designated as NPC homes using the `IsHome` property
- NPCs have their `CurrentLocation` property set to their home building name during initialization
- This enables future features like NPC pathfinding, schedules, and location-based interactions
- Foundation laid for bedtime routines and idle animations

---

## Previous Update: Time-Based NPC Scheduling System

### TimeService Enhancements
- **Added `OnHourChanged` event**: 
  - Event triggered whenever `AdvanceHours()` modifies the Hour property
  - Passes the new hour value as an event argument
  - Enables real-time reactions to time changes throughout the game
- **Event firing logic**: 
  - Compares old hour vs new hour before firing the event
  - Ensures event only fires when hour actually changes
  - Handles day rollover properly (when hour goes from 23 to 0)

### MapViewModel Enhancements
- **Subscribed to `TimeService.OnHourChanged` event**:
  - Both constructors now subscribe to the time change event
  - Automatically refreshes NearbyNPCs when hour changes
  - Updates time display in UI
- **New `OnHourChanged(object? sender, int newHour)` handler**:
  - Calls `RefreshTimeDisplay()` to update UI
  - Calls `RefreshNearbyNPCs()` to update NPC availability using `IsAvailableNow()`
  - Calls `LogTimedEvents(int hour)` to log time-specific messages
- **New `LogTimedEvents(int hour)` method**:
  - Logs contextual messages at specific times:
    - 6:00 AM: "The sun rises. Shops are opening for the day."
    - 12:00 PM: "It's noon. The sun is at its peak."
    - 8:00 PM: "Shops are closing for the night."
    - 12:00 AM: "Midnight strikes. The world grows quiet."
  - Added TODO: Add fade-to-night transitions for NPC sprites later

### Technical Details
- Event-driven architecture ensures NPCs automatically appear/disappear based on their schedules
- Existing `RefreshNearbyNPCs()` method already logs NPC arrivals and departures with context-aware messages
- Time-based events are now fully automated - no manual refresh needed
- System integrates seamlessly with existing time advancement from battles, resting, and fast travel

---

## Previous Update: Dynamic Ambient Audio System

### AudioService Enhancements
- **Added `PlayAmbientForLighting(string lighting)` method**: 
  - Plays ambient sounds based on lighting conditions
  - "Daylight" → birds.wav
  - "Twilight" → crickets.wav
  - "Night" → nightwind.wav
- **Integrated with EnvironmentService**: 
  - AudioService now subscribes to `EnvironmentService.OnLightingChanged` event
  - Automatically switches ambient loops when lighting changes (day/night cycle)
- **New private `PlayAmbientLoop(string fileName)` method**:
  - Manages ambient sound player lifecycle
  - Properly stops and disposes previous ambient tracks before playing new ones
- **Added TODO**: Add layered environmental tracks (rain, crowd chatter, wind)

### Technical Details
- Static constructor added to AudioService to initialize event subscription
- New static field `_ambientPlayer` for managing ambient sound playback
- Event-driven architecture ensures ambient sounds stay synchronized with the game's time/lighting system
