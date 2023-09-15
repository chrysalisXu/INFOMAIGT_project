# unity version: 2021.3.14
# play
WASD to move.

mouse control shooting direction, left click to shoot.

If you want to construct new map, just edit ExampleMap.txt in Assets/Map, I think it is simple enough for anyone to understand.

# Implementation
In general you just need to look at two manager. they control every logic.

I did not use unity's physic engine or gameobject system to update bullets and so on. Managers takes care of everything. By doing so we can run our game mechanics by deep copy a GameplayManager and let AI predict what will happen after many frames without any calculation on rendering or costly unity gameobject system. Otherwise you will need 10+ GTX4090 to collect enough data for such a simple game.

# Further work
## AI
we will discuss it later.

## Map generation
I like that idea

## UI
Add scoreboard and some effect so players can know who won.

## Restart Mechanic

## Gameplay
1. bouncing bullet - done 
2. destructible walls - I think it is a bad idea for AI training?
3. various bullet types(size, lifespan, speed, bouncing times)
4. different player abilities. (mines, bombs, accelaration)
5. pickable tools/upgrades
6. limit on rotation speed of players.
7. BattleFog
8. Balancing the game(max speed, map size, etc).

## sound effect, BGM, beautify the game art style
It is so boring to play it now... so please add them if you have some free time?
