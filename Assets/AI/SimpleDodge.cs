// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using INFOMAIGT.Map;
using INFOMAIGT.Gameplay;

namespace INFOMAIGT.AI
{
    public class SimpleDodge
    {
        public static Vector3 Dodge(Player myself, KeyCode closingDirection, float closingWeight, float suicideThreshold)
        {
            // posible options.
            Dictionary<Vector3, float> targetDict = new Dictionary<Vector3, float>();
            Vector3 up = MapManager.Instance.GetFixedPlayerPosition(
                myself.location, KeyCode.UpArrow, myself.maxVelocity, myself.radius
            );
            Vector3 upLeft = MapManager.Instance.GetFixedPlayerPosition(
                up, KeyCode.LeftArrow, myself.maxVelocity, myself.radius
            );
            Vector3 upRight = MapManager.Instance.GetFixedPlayerPosition(
                up, KeyCode.RightArrow, myself.maxVelocity, myself.radius
            );
            Vector3 left = MapManager.Instance.GetFixedPlayerPosition(
                myself.location, KeyCode.LeftArrow, myself.maxVelocity, myself.radius
            );
            Vector3 right = MapManager.Instance.GetFixedPlayerPosition(
                myself.location, KeyCode.RightArrow, myself.maxVelocity, myself.radius
            );
            Vector3 down = MapManager.Instance.GetFixedPlayerPosition(
                myself.location, KeyCode.DownArrow, myself.maxVelocity, myself.radius
            );
            Vector3 downLeft = MapManager.Instance.GetFixedPlayerPosition(
                down, KeyCode.LeftArrow, myself.maxVelocity, myself.radius
            );
            Vector3 downRight = MapManager.Instance.GetFixedPlayerPosition(
                down, KeyCode.RightArrow, myself.maxVelocity, myself.radius
            );
            // corner weight * 0.9 to make ai prefer move in simple direction
            targetDict.TryAdd(up, closingDirection == KeyCode.UpArrow ? closingWeight: 0);
            targetDict.TryAdd(upLeft, closingDirection == KeyCode.UpArrow ? closingWeight * 0.9f: 
                closingDirection == KeyCode.LeftArrow ? closingWeight * 0.9f : 1
            );
            targetDict.TryAdd(upRight, closingDirection == KeyCode.UpArrow ? closingWeight * 0.9f: 
                closingDirection == KeyCode.RightArrow ? closingWeight * 0.9f : 0
            );
            targetDict.TryAdd(myself.location, -0.01f);
            targetDict.TryAdd(left, closingDirection == KeyCode.LeftArrow ? closingWeight: 0);
            targetDict.TryAdd(right, closingDirection == KeyCode.RightArrow ? closingWeight: 0);
            targetDict.TryAdd(down, closingDirection == KeyCode.DownArrow ? closingWeight: 0);
            targetDict.TryAdd(downLeft, closingDirection == KeyCode.DownArrow ? closingWeight * 0.9f: 
                closingDirection == KeyCode.LeftArrow ? closingWeight * 0.9f : 0
            );
            targetDict.TryAdd(downRight, closingDirection == KeyCode.DownArrow ? closingWeight * 0.9f: 
                closingDirection == KeyCode.RightArrow ? closingWeight * 0.9f : 0
            );

            (Vector3 moveResult, float directionWeight) target = (myself.location, suicideThreshold);
            foreach(var (pos, weight) in targetDict)
            {
                float rawWeight = weight;
                foreach(Bullet bullet in GameplayManager.Instance.bulletList)
                    // the weight of a bullet 10 meters away = 1 
                    rawWeight += 100f/((bullet.location - pos).sqrMagnitude);
                // Debug.Log($"pos:{pos}, raw:{rawWeight}, weight:{weight}" );
                
                if (rawWeight < target.directionWeight)
                    target = (pos, rawWeight);
            }
            // Debug.Log($"AI result: {target.moveResult}");
            return target.moveResult;
        }
    }
}