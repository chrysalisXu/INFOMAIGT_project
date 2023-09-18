// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace INFOMAIGT.Gameplay
{
    public class Player : Circle
    {
        public float orientation = 0;
        public float maxVelocity = 0.2f;

        public float MaxBulletSpeed = 0.5f;

        public Mesh artilleryMesh = null;
        public float artilleryWidth = 0.3f;

        public bool alive = true;

        public Player(Vector3 position, bool needRendering)
        {
            location = position;
            if (needRendering){
                CreateFilledCircle();
                UpdateMesh();
            }
        }

        public void UpdateMesh()
        {
            base.UpdateMesh();
            if (artilleryMesh == null){
                artilleryMesh = new Mesh();
                artilleryMesh.vertices = new Vector3[] {
                    new Vector3(location.x - artilleryWidth, location.y, 0),
                    new Vector3(location.x + artilleryWidth, location.y, 0),
                    new Vector3(location.x + artilleryWidth, location.y + radius*2, 0),
                    new Vector3(location.x - artilleryWidth, location.y + radius*2, 0),
                };
                artilleryMesh.triangles = new int[] {
                    0, 1, 2, 1, 2, 3
                };
            }
            else{
                var vertices = artilleryMesh.vertices;
                vertices[0].x = location.x - artilleryWidth * MathF.Cos(orientation);
                vertices[0].y = location.y + artilleryWidth * MathF.Sin(orientation);
                vertices[1].x = location.x + artilleryWidth * MathF.Cos(orientation);
                vertices[1].y = location.y - artilleryWidth * MathF.Sin(orientation);
                vertices[2].x = location.x + artilleryWidth * MathF.Cos(orientation) + radius * 2 * MathF.Sin(orientation);
                vertices[2].y = location.y - artilleryWidth * MathF.Sin(orientation) + radius * 2 * MathF.Cos(orientation);
                vertices[3].x = location.x - artilleryWidth * MathF.Cos(orientation) + radius * 2 * MathF.Sin(orientation);
                vertices[3].y = location.y + artilleryWidth * MathF.Sin(orientation) + radius * 2 * MathF.Cos(orientation);
                artilleryMesh.vertices = vertices;
            }
        }
    }
}