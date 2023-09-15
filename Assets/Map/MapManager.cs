// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using INFOMAIGT.Gameplay;

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
        public TextAsset MapData;
        public Material WallMaterial;
        [NonSerialized]
        public int width, height;

        public int GetWallID(int x, int y)
        {
            return y * height + x;
        }

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

        void RenderMap()
        {
            RenderParams rp = new RenderParams(WallMaterial);
            foreach(Wall wall in wallMap.Values)
            {
                Graphics.RenderMesh(rp, wall.mesh, 0, Matrix4x4.Translate(Vector3.zero));
            }
        }

        void Start()
        {
            // initiate a map
            ReadMap(MapData.text);
        }

        void Update() 
        {
            RenderMap();
        }
    }

}
