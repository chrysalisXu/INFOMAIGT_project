// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace INFOMAIGT.Gameplay
{
    public class Bullet : Circle
    {
        public Vector3 velocity;
        int lifespan; // TODO: life span of bullet.
        public bool alive = true;
        public Bullet(Vector3 position, Vector3 initVelocity, bool needRendering)
        {
            radius = 0.8f;
            location = position;
            velocity = initVelocity;
            if (needRendering){
                CreateFilledCircle();
                UpdateMesh();
            }
        }

        public void LogicalUpdate(){
            location += velocity;
        }
    }

}