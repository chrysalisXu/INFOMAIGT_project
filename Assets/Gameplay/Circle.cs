// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace INFOMAIGT.Gameplay
{
    public class Circle
    {
        public Vector3 location;
        public float radius = 2f;
        static int CIRCLE_EDGES = 20;
        public Mesh circleMesh;

        public void CreateCircle() // not filled
        {
            circleMesh = new Mesh();
            circleMesh.vertices = new Vector3[CIRCLE_EDGES];
            for (int i=0; i< CIRCLE_EDGES; i++) circleMesh.vertices[i] = new Vector3(0,0,0);
            var standardCircleTriangles = new int [CIRCLE_EDGES * 3 + 3];
            for (int i=0; i< CIRCLE_EDGES; i++){
                standardCircleTriangles[i*3] = i;
                standardCircleTriangles[i*3+1] = (i+1) % CIRCLE_EDGES;
                standardCircleTriangles[i*3+2] = (i+2) % CIRCLE_EDGES;
            }
            circleMesh.triangles = standardCircleTriangles;
        }

        public void CreateFilledCircle()
        {
            circleMesh = new Mesh();
            circleMesh.vertices = new Vector3[CIRCLE_EDGES];
            for (int i=0; i< CIRCLE_EDGES; i++) circleMesh.vertices[i] = new Vector3(0,0,0);
            var standardFilledCircleTriangles = new int [CIRCLE_EDGES * 3 - 6];
            for (int i=0; i< CIRCLE_EDGES - 2; i++){
                standardFilledCircleTriangles[i*3] = 0;
                standardFilledCircleTriangles[i*3+1] = i+1;
                standardFilledCircleTriangles[i*3+2] = i+2;
            }
            circleMesh.triangles = standardFilledCircleTriangles;
        }

        public virtual void UpdateMesh()
        {
            var vertices = circleMesh.vertices;
            for (int i=0; i< CIRCLE_EDGES; i++){
                float angle = 2 * MathF.PI / CIRCLE_EDGES * i;
                vertices[i].x = MathF.Cos(angle) * radius + location.x;
                vertices[i].y = MathF.Sin(angle) * radius + location.y;
            }
            circleMesh.vertices = vertices;
        }
    }

}