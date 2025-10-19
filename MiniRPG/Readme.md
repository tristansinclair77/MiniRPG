# MiniRPG - Change Log

## Latest Update: Time-Based NPC Scheduling System

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
