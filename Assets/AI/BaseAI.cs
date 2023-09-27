// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using INFOMAIGT.Gameplay;
using INFOMAIGT.Map;

namespace INFOMAIGT.AI
{
    public class BaseAI
    {
        // logic:
        // if can shoot pc -> shoot and dodge 
        // else -> go closer (to end the game quicker)
        public AISetting setting;

        public BaseAI(int playerID)
        {
            setting = new AISetting(playerID);
        }

        public BaseAI(AISetting initSetting)
        {
            setting = initSetting;
        }

        public void Move()
        {
            Player enemy = GameplayManager.Instance.playerDict[1];
            Player myself = GameplayManager.Instance.playerDict[setting.playerID];

            // get closer => multiply weight if it can make ai get closer
            KeyCode closer = Pathfinder.MapDirectionToPos(myself.location, enemy.location);

            // detect whether can direct hit player and keep closing
            if (SimpleShoot.CanHit(myself.location, enemy.location, setting.closingDistance))
                closer = KeyCode.Escape;

            // dodge
            myself.location = SimpleDodge.Dodge(myself, closer, -setting.closingWeight);
            // shoot
            myself.rotateToward(enemy.location);
            if(SimpleShoot.CanHit(myself.location, enemy.location, setting.shootingDistance))
                myself.Shoot(GameplayManager.Instance);
        }

    }
}