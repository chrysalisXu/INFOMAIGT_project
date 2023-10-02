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
        public float aiDebuffRadiance;

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
            myself.location = SimpleDodge.Dodge(myself, closer, -setting.closingWeight, setting.advanceDodgeOn);
            myself.FixRotation();
            // shoot
            myself.RotateToward(enemy.location);
            
            if (setting.randomShootRadiance!=0) // make ai shoot less precise
            {
                aiDebuffRadiance = (aiDebuffRadiance + UnityEngine.Random.value * 0.01f) % (2 * setting.randomShootRadiance);
                float debuffRad = aiDebuffRadiance > setting.randomShootRadiance
                                ? 2 * setting.randomShootRadiance - aiDebuffRadiance : aiDebuffRadiance;
                myself.orientation += aiDebuffRadiance - setting.randomShootRadiance/2;
                myself.FixRotation();
            }
            if(SimpleShoot.CanHit(myself.location, enemy.location, setting.shootingDistance))
                myself.Shoot(GameplayManager.Instance);
        }

    }
}