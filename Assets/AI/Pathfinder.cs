// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using INFOMAIGT.Map;

namespace INFOMAIGT.AI
{
    public class Pathfinder
    {
        // A* algorithm with distance to pc as opt
        // escape means no way to there or in same tile;
        public static KeyCode MapDirectionToPos(Vector3 start, Vector3 end)
        {
            Vector2Int startMapPos = MapManager.GetMapXY(start);
            Vector2Int endMapPos = MapManager.GetMapXY(end);
            return Pathfinder.MapDirectionToPos(startMapPos, endMapPos);
        }
        
        public static KeyCode MapDirectionToPos(Vector2Int startMapPos, Vector2Int endMapPos)
        {
            if (startMapPos == endMapPos) return KeyCode.Escape;

            // cur pos, (from direction, distance to travel, visited before)
            Dictionary<Vector2Int, (Vector2Int, float)> VisitedDict = new Dictionary<Vector2Int, (Vector2Int, float)>();
            Dictionary<Vector2Int, (Vector2Int, float)> NextDict = new Dictionary<Vector2Int, (Vector2Int, float)>();
            NextDict.Add(startMapPos, (startMapPos, 0));

            while(true)
            {
                // find nearest pos in nextdict
                (Vector2Int, float) target = (Vector2Int.zero, 1000000); // some init value very faraway
                foreach (var item in NextDict)
                {
                    float weight = item.Value.Item2 + (endMapPos - item.Key).magnitude;
                    if (weight < target.Item2)
                        target = (item.Key, weight);
                }

                if (target.Item2 == 1000000) return KeyCode.Escape;

                // expand nextdict 
                void expandTile(Vector2Int targetPos, Vector2Int fromPos, float cost){
                    if (MapManager.Instance.IsWall(targetPos)) return;
                    if (VisitedDict.ContainsKey(targetPos))
                    {
                        if (VisitedDict[targetPos].Item2 > cost)
                            VisitedDict.Remove(targetPos);
                        else return;
                    }
                    if (NextDict.ContainsKey(targetPos))
                    {
                        if (NextDict[targetPos].Item2 > cost)
                            NextDict.Remove(targetPos);
                        else return;
                    }
                    NextDict.Add(targetPos, (fromPos, cost));
                }

                Vector2Int left = new Vector2Int(target.Item1.x - 1 , target.Item1.y);
                expandTile(left, target.Item1, NextDict[target.Item1].Item2 + 1);
                Vector2Int right = new Vector2Int(target.Item1.x + 1 , target.Item1.y);
                expandTile(right, target.Item1, NextDict[target.Item1].Item2 + 1);
                Vector2Int up = new Vector2Int(target.Item1.x , target.Item1.y + 1);
                expandTile(up, target.Item1, NextDict[target.Item1].Item2 + 1);
                Vector2Int down = new Vector2Int(target.Item1.x , target.Item1.y - 1);
                expandTile(down, target.Item1, NextDict[target.Item1].Item2 + 1);

                // insert self to visited
                VisitedDict.Add(target.Item1, NextDict[target.Item1]);
                NextDict.Remove(target.Item1);
                

                if (NextDict.ContainsKey(endMapPos))
                    break;
            }

            Vector2Int pathPos = NextDict[endMapPos].Item1;
            while(true)
            {
                if (VisitedDict[pathPos].Item1 == startMapPos){
                    if (pathPos.x > startMapPos.x) return KeyCode.RightArrow;
                    else if (pathPos.x < startMapPos.x) return KeyCode.LeftArrow;
                    else if (pathPos.y > startMapPos.y) return KeyCode.UpArrow;
                    else return KeyCode.DownArrow;
                }
                pathPos = VisitedDict[pathPos].Item1;
            }
            return KeyCode.Escape;
        }
    }
}