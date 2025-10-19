# MiniRPG - Change Log

## Latest Update: Dynamic Ambient Audio System

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
