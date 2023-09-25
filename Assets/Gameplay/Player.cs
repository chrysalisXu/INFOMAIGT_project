// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using INFOMAIGT.AI;

namespace INFOMAIGT.Gameplay
{
    public class Player : Circle
    {
        public float orientation = 0;
        public float maxVelocity = 0.2f;
        public float maxRotationSpeed = 0.1f;

        public float MaxBulletSpeed = 0.5f;

        public Mesh artilleryMesh = null;
        public float artilleryWidth = 0.3f;

        public BaseAI ai;

        public bool alive = true;

        public int maxCooldown, currentCooldown;

        public Player(Vector3 position, bool needRendering, BaseAI initAI)
        {
            location = position;
            if (needRendering){
                CreateFilledCircle();
                UpdateMesh();
            }
            ai = initAI;
            maxCooldown = 50;
            currentCooldown = 0;
        }

        public void rotateToward(Vector3 target)
        {
            Vector3 direction = target - location;
            float rad = 0;
            if (direction.y==0)
            {
                rad = MathF.PI/2;
                if (direction.x<0) rad = -rad;
            }
            else
            {
                rad = MathF.Atan(direction.x/direction.y);
                if (direction.y<0)
                    rad += MathF.PI;
            }
            orientation = rad; // TODO: add max rotation speed;
        }

        public void UpdateCooldown()
        {
            if (currentCooldown > 0)
                currentCooldown -= 1;
        }

        public void Shoot(GameplayManager gameplay)
        {
            if(currentCooldown!=0) return;
            gameplay.bulletList.Add(new Bullet(
                location + new Vector3(
                    MathF.Sin(orientation) * radius * 2,
                    MathF.Cos(orientation) * radius * 2,
                    0
                ),
                new Vector3(
                    MathF.Sin(orientation) * MaxBulletSpeed,
                    MathF.Cos(orientation) * MaxBulletSpeed,
                    0
                ),
                gameplay==GameplayManager.Instance)
            );
            currentCooldown = maxCooldown;
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