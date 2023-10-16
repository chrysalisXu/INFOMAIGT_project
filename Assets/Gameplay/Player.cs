// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using INFOMAIGT.Map;
using INFOMAIGT.AI;
using INFOMAIGT.Data;

namespace INFOMAIGT.Gameplay
{
    public class Player : Circle
    {
        public float orientation = 0;
        public float maxVelocity = 0.4f;
        public float maxRotationSpeed = 0.01f;

        public float MaxBulletSpeed = 0.8f;

        public Mesh artilleryMesh = null;
        public float artilleryWidth = 0.4f;

        public BaseAI ai;

        public bool alive = true;

        public int maxCooldown, currentCooldown;

        public int health = 5;

        public Player(Vector3 position, bool needRendering, BaseAI initAI)
        {
            location = position;
            if (needRendering)
            {
                CreateFilledCircle();
                UpdateMesh();
            }
            ai = initAI;
            maxCooldown = (int)MathF.Ceiling(50 * (1 + ai.setting.additionalCoolDown));
            currentCooldown = 0;
        }

        public void FixRotation(float maxDegree = 0.01f)
        {
            if (!MapManager.Instance.RaycastAgainstWall(location, GetBulletStartLocation()))
                return;
            orientation += maxDegree;
            if (!MapManager.Instance.RaycastAgainstWall(location, GetBulletStartLocation()))
                return;
            orientation -= 2 * maxDegree;
            if (!MapManager.Instance.RaycastAgainstWall(location, GetBulletStartLocation()))
                return;
            orientation += maxDegree;
            FixRotation(maxDegree + 0.01f);
        }


        public void RotateToward(Vector3 target)
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
            float oldrad = orientation;
            orientation = rad; // TODO: add max rotation speed;
            if (MapManager.Instance.RaycastAgainstWall(location, GetBulletStartLocation()))
                orientation = oldrad;
        }

        public void UpdateCooldown()
        {
            if (currentCooldown > 0)
                currentCooldown -= 1;
        }

        public Vector3 GetBulletStartLocation()
        {
            return location + new Vector3(
                MathF.Sin(orientation) * radius * 2,
                MathF.Cos(orientation) * radius * 2,
                0
            );
        }

        public void Shoot(GameplayManager gameplay)
        {
            if (currentCooldown != 0) return;
            gameplay.bulletList.Add(new Bullet(
                GetBulletStartLocation(),
                new Vector3(
                    MathF.Sin(orientation) * MaxBulletSpeed,
                    MathF.Cos(orientation) * MaxBulletSpeed,
                    0
                ),
                gameplay == GameplayManager.Instance)
            );
            currentCooldown = maxCooldown;

            if (ai.setting.playerID == 1)
            { 
                DataManager.Instance.report.bulletsFiredPC += 1;
                
                // Play shooting sound
                GameObject soundEffectObject = GameObject.Find("SoundEffect");
                if (soundEffectObject != null)
                {
                    AudioSource shootEffect = soundEffectObject.GetComponent<AudioSource>();
                    shootEffect.Play();
                }
            }
        }

        public override void UpdateMesh()
        {
            base.UpdateMesh();
            if (artilleryMesh == null)
            {
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
            else
            {
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