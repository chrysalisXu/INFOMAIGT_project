# unity version: 2021.3.14
# play
https://chrysalisgames.store/

If you want to construct new map, just edit ExampleMap.txt in Assets/Map.

# Implementation
In general you just need to look at managers. they control every logic.
- AIManager : control AI behavior, AI implementation is in Assets/AI folder
- DataManager: collect user data
- GameplayManager : implement game machanics.
- LevelManager : manage level data
- MapManager : generate map and render the map
- UIManager : Control UI logic

It has following features:
- Rendering & physics : implemented a simplified 2D physics engine.
- AI : configurable AI behavior, easy AI will try to behave humanly while hard ai focus on beating players.
- Content generation: random map.
- Network: Nginx + uwsgi + python backend server on aws to collect data
