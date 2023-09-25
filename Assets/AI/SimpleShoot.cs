// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using INFOMAIGT.Map;
using INFOMAIGT.Gameplay;

namespace INFOMAIGT.AI
{
    public class SimpleShoot
    {
        public static bool CanHit(Vector3 from, Vector3 to, float distance)
        {
            if ((to - from).magnitude > distance)
                return false;
            if (MapManager.Instance.RaycastAgainstWall(from, to))
                return false;
            return true;
        }
    }
}