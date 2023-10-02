// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using INFOMAIGT.Gameplay;
using INFOMAIGT.AI;

namespace INFOMAIGT.Map
{
    public class MapManager : MonoBehaviour
    {

        // singleton, for other system to call
        private static MapManager _instance = null;
        public static MapManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<MapManager>();
                if (_instance == null) throw new Exception("Map not exists!");
                return _instance;
            }
        }

        [NonSerialized]
        public bool initiated = false;
        [NonSerialized]
        // id = y * height + x
        public Dictionary<int, Wall> wallMap = new Dictionary<int, Wall>();

        [SerializeField]
        public TextAsset DefaultMapData;
        public Material WallMaterial;
        [NonSerialized]
        public int width, height;

        public static Vector2Int GetMapXY(Vector3 precisePos)
        {
            return new Vector2Int(
                (int)MathF.Floor(precisePos.x / Wall.size),
                (int)MathF.Floor(precisePos.y / Wall.size)
            );
        }
        public Vector2Int GetMapXY(int wallID)
        {
            return new Vector2Int(
                wallID % height,
                (int)MathF.Floor(wallID / height)
            );
        }

        public int GetWallID(int x, int y){return y * height + x;}
        public int GetWallID(float preciseX, float preciseY){
            return GetWallID(
                (int)MathF.Floor(preciseX / Wall.size),
                (int)MathF.Floor(preciseY / Wall.size)
            );
        }
        public int GetWallID(Vector2Int mapXY){return GetWallID(mapXY.x, mapXY.y);}
        public int GetWallID(Vector3 precisePos)
        {
            Vector2Int mapXY = MapManager.GetMapXY(precisePos);
            return GetWallID(mapXY.x, mapXY.y);
        }

        public bool IsWall(Vector2Int mapXY){return wallMap.ContainsKey(GetWallID(mapXY));}
        public bool IsWall(Vector3 precisePos){return wallMap.ContainsKey(GetWallID(precisePos));}
        public bool IsWall(float preciseX, float preciseY){return wallMap.ContainsKey(GetWallID(preciseX, preciseY));}

        void ReadMap(string mapdata)
        {
            if (initiated) throw new Exception("Map already created!");
            initiated = true;
            string[] lines = mapdata.Split('\n');
            width = Int32.Parse(lines[0].Split(' ')[0]);
            height = Int32.Parse(lines[0].Split(' ')[1]); 
            for (int y=0; y<height; y++){
                for (int x=0; x<width; x++)
                {
                    if (lines[y+1][x] == 'x')
                    {
                        wallMap.Add(GetWallID(x, y), new Wall(x, y));
                    }
                    else if (lines[y+1][x] != 'o')
                        GameplayManager.Instance.CreatePlayer(
                            new Vector3((x+0.5f) * Wall.size, (y+0.5f) * Wall.size, 0),
                            Int32.Parse(Char.ToString(lines[y+1][x]))
                        );
                }
            }
        }

        public bool GenerateMap(LevelSetting setting)
        {
            // init
            width = setting.width;
            height = setting.height;

            int [,] wallsTemp = new int [height, width];
            int [,] wallsTempNew = new int [height, width];
            // generate random noise
            for (int y=0; y<height; y++)
                for (int x=0; x<width; x++)
                {
                    if ((y==0 || x==0 || y==height-1 || x==width-1) || 
                        UnityEngine.Random.value < setting.percentWalls)
                        wallsTemp[y,x] = 1;
                    else
                        wallsTemp[y,x] = 0;
                }

            // a single process
            int countNeighborWalls(int x, int y)
            {
                int count = wallsTemp[y-1,x-1] + wallsTemp[y-1,x] + wallsTemp[y-1,x+1]
                + wallsTemp[y,x-1] + wallsTemp[y,x+1] + wallsTemp[y+1,x-1]
                + wallsTemp[y+1,x] + wallsTemp[y+1,x+1];
                if (y==1 || x==1 || y==height-2 || x==width-2)
                    count -= 2;
                return count;
            }
            void wallReduction()
            {
                for (int y=0; y<height; y++)
                    for (int x=0; x<width; x++)
                    {
                        if (y==0 || x==0 || y==height-1 || x==width-1) wallsTempNew[y,x] = 1;
                        else if (countNeighborWalls(x,y) >= 4)
                            wallsTempNew[y,x] = 1;
                        else
                            wallsTempNew[y,x] = 0;
                    }
                wallsTemp = wallsTempNew;
            }
            // multi times
            wallReduction();
            wallReduction();
            wallReduction();

            // initiate map
            List <int> voidMap = new List<int>();
            for (int y=0; y<height; y++)
                for (int x=0; x<width; x++)
                    if (wallsTemp[y,x] == 1)
                        wallMap.Add(GetWallID(x, y), new Wall(x, y));
                    else
                        voidMap.Add(GetWallID(x, y));
            
            // place users
            var sysRand = new System.Random();
            List<int> playerPlaces = new List<int>();
            foreach(int playerID in setting.playerIDs)
            {
                int voidID = sysRand.Next(voidMap.Count);
                if (playerPlaces.Count > 0)
                {
                    for (int i=0; i<6; i++)
                    {
                        if (i==5)// terrible map, usually separated places
                        {
                            wallMap.Clear();
                            return false;
                        }
                        if (Pathfinder.MapDirectionToPos(GetMapXY(voidMap[voidID]), GetMapXY(playerPlaces[0]))!=KeyCode.Escape)
                            break;
                        voidID = sysRand.Next(voidMap.Count);
                    }
                }
                playerPlaces.Add(voidMap[voidID]);
                voidMap.Remove(voidID);
            }
            for (int i=0; i<playerPlaces.Count; i++)
            {
                int x = playerPlaces[i] % height;
                int y = (playerPlaces[i]-x) / height;
                GameplayManager.Instance.CreatePlayer(
                    new Vector3((x+0.5f) * Wall.size, (y+0.5f) * Wall.size, 0),
                    setting.playerIDs[i]
                );
            }
            return true;
        }

        // stop player from passing walls like ghost
        public Vector3 GetFixedPlayerPosition(Vector3 pos, KeyCode direction, float maxVelocity, float playerRadius)
        {
            Vector3 result = new Vector3(pos.x, pos.y, pos.z);
            if (direction == KeyCode.UpArrow) result.y += maxVelocity;
            else if (direction == KeyCode.DownArrow) result.y -= maxVelocity;
            else if (direction == KeyCode.LeftArrow) result.x -= maxVelocity;
            else if (direction == KeyCode.RightArrow) result.x += maxVelocity;

            // soft detect, cover most cases
            // can opt, each direction only need to care 2 points
            if (!IsWall(result.x + playerRadius, result.y + playerRadius)
                && !IsWall(result.x - playerRadius, result.y + playerRadius)
                && !IsWall(result.x + playerRadius, result.y - playerRadius)
                && !IsWall(result.x - playerRadius, result.y - playerRadius)
            ) return result;
            // deal with simple situation (border collision)
            if ((direction == KeyCode.UpArrow) && IsWall(result.x, result.y + playerRadius))
                result.y = MathF.Floor((result.y + playerRadius) / Wall.size) * Wall.size - playerRadius;
            else if ((direction == KeyCode.DownArrow) && IsWall(result.x, result.y - playerRadius))
                result.y = MathF.Ceiling((result.y - playerRadius) / Wall.size) * Wall.size + playerRadius;
            else if ((direction == KeyCode.LeftArrow) && IsWall(result.x - playerRadius, result.y))
                result.x = MathF.Ceiling((result.x - playerRadius) / Wall.size) * Wall.size + playerRadius;
            else if ((direction == KeyCode.RightArrow) && IsWall(result.x + playerRadius, result.y))
                result.x = MathF.Floor((result.x + playerRadius) / Wall.size) * Wall.size - playerRadius;
            else // deal with corner collision
            {
                // detect which wall corner
                Vector3 wallCorner;
                if(IsWall(result.x + playerRadius, result.y + playerRadius))
                    wallCorner = new Vector3(
                        (int)MathF.Floor((result.x + playerRadius) / Wall.size) * Wall.size,
                        (int)MathF.Floor((result.y + playerRadius) / Wall.size) * Wall.size,
                        0
                    );
                else if(IsWall(result.x + playerRadius, result.y - playerRadius))
                    wallCorner = new Vector3(
                        (int)MathF.Floor((result.x + playerRadius) / Wall.size) * Wall.size,
                        (int)MathF.Ceiling((result.y - playerRadius) / Wall.size) * Wall.size,
                        0
                    );
                else if(IsWall(result.x - playerRadius, result.y + playerRadius))
                    wallCorner = new Vector3(
                        (int)MathF.Ceiling((result.x - playerRadius) / Wall.size) * Wall.size,
                        (int)MathF.Floor((result.y + playerRadius) / Wall.size) * Wall.size,
                        0
                    );
                else
                    wallCorner = new Vector3(
                        (int)MathF.Ceiling((result.x - playerRadius) / Wall.size) * Wall.size,
                        (int)MathF.Ceiling((result.y - playerRadius) / Wall.size) * Wall.size,
                        0
                    );
                // detect whether actually collide
                Vector3 vectorToWall = wallCorner - result;
                if (vectorToWall.magnitude >= playerRadius)
                    return result;
                // help cross the corner
                result -= (playerRadius - vectorToWall.magnitude) * vectorToWall.normalized;
            }
            return result;
        }

        void RenderMap()
        {
            RenderParams rp = new RenderParams(WallMaterial);
            foreach(Wall wall in wallMap.Values)
            {
                Graphics.RenderMesh(rp, wall.mesh, 0, Matrix4x4.Translate(Vector3.zero));
            }
        }

        // true if hit wall;
        public bool RaycastAgainstWall(Vector3 from, Vector3 to)
        {
            Vector2Int minMapXY = GetMapXY(from);
            Vector2Int maxMapXY = GetMapXY(to);
            if (minMapXY.x > maxMapXY.x){var temp = minMapXY.x; minMapXY.x = maxMapXY.x; maxMapXY.x = temp;}
            if (minMapXY.y > maxMapXY.y){var temp = minMapXY.y; minMapXY.y = maxMapXY.y; maxMapXY.y = temp;}
            
            foreach((int wallID, Wall wall) in wallMap)
            {
                if (wall.mapXY.x < minMapXY.x) continue;
                if (wall.mapXY.y < minMapXY.y) continue;
                if (wall.mapXY.x > maxMapXY.x) continue;
                if (wall.mapXY.y > maxMapXY.y) continue;
                if (wall.RaycastIsCollide(from, to)) return true;
            }
            return false;
        }

        void Start()
        {
            // initiate a map
            if (LevelManager.Instance == null)                
                ReadMap(DefaultMapData.text);
            else
            {
                if (LevelManager.currentLevel.random==false)
                    ReadMap(LevelManager.currentLevel.mapData.text);
                else
                    while(!GenerateMap(LevelManager.currentLevel)){};
            }
        }

        void Update() 
        {
            RenderMap();
        }
    }

}
