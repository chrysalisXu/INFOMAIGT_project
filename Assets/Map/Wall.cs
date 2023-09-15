// Author: Chrysalis shiyuchongf@gmail.com

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace INFOMAIGT.Map
{
    public class Wall
    {
        public static float size = 10;
        static int[] standardWallTriangles = new int[] {
            0, 1, 2, 1, 2, 3
        };

        public Vector3 location;
        public int hp; // destructible walls? but I think it will be a too costly to store different maps in AI prediction...
        public Mesh mesh = new Mesh();

        public Wall(int x, int y)
        {
            location = new Vector3(x*size, y*size, 0);
            mesh.vertices = new Vector3[] {
                new Vector3(location.x, location.y, 0),
                new Vector3(location.x+size, location.y, 0),
                new Vector3(location.x, location.y+size, 0),
                new Vector3(location.x+size, location.y+size, 0)
            };
            mesh.triangles = standardWallTriangles;
        }
    }
}
