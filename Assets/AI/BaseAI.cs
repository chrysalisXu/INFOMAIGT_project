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
        private List<KeyCode> wanderingInstruction = new List<KeyCode>();
        private KeyCode[] ValidWanderingMove = new KeyCode[4] {KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow};

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
            Player myself = GameplayManager.Instance.playerDict[setting.playerID];
            Player enemy = GameplayManager.Instance.playerDict[1];
            if (!setting.targetPlayerOnly)
            {
                float shortestDistance = 10000;
                bool closeCombat = false;
                foreach ((int id, Player player) in GameplayManager.Instance.playerDict){
                    if (id== setting.playerID)
                        continue;
                    if (SimpleShoot.CanHit(myself.location, player.location, setting.closingDistance))
                    {
                        if (!closeCombat){
                            enemy = player;
                            shortestDistance = (myself.location - player.location).sqrMagnitude;
                        }
                        else if ((myself.location - player.location).sqrMagnitude < shortestDistance)
                        {
                            shortestDistance = (myself.location - player.location).sqrMagnitude;
                            enemy = player;
                        }
                        closeCombat = true;
                    }
                    else if(closeCombat) continue;
                    else if ((myself.location - player.location).sqrMagnitude < shortestDistance)
                    {
                        shortestDistance = (myself.location - player.location).sqrMagnitude;
                        enemy = player;
                    }

                }
            }

            // get closer => multiply weight if it can make ai get closer
            KeyCode closer = Pathfinder.MapDirectionToPos(myself.location, enemy.location);

            // detect whether can direct hit player and keep closing
            if (SimpleShoot.CanHit(myself.location, enemy.location, setting.closingDistance))
                closer = KeyCode.Escape;

            // dodge
            if (setting.dodgeOff)
            {
                if (UnityEngine.Random.value < 0.05f)
                {
                    wanderingInstruction.Clear();
                    foreach(KeyCode key in ValidWanderingMove)
                        if(UnityEngine.Random.value < 0.25f) wanderingInstruction.Add(key);
                }
                foreach(KeyCode key in wanderingInstruction)
                    myself.location = MapManager.Instance.GetFixedPlayerPosition(
                        myself.location, key, myself.maxVelocity, myself.radius
                    );
            }
            else
                myself.location = SimpleDodge.Dodge(myself, closer, -setting.closingWeight, setting.advanceDodgeOn);
            myself.FixRotation();
            // shoot
            myself.RotateToward(enemy.location);
            
            if (setting.randomShootRadiance!=0) // make ai shoot less precise
            {
                aiDebuffRadiance = (aiDebuffRadiance + UnityEngine.Random.value * 0.01f) % (2 * setting.randomShootRadiance);
                float debuffRad = aiDebuffRadiance > setting.randomShootRadiance
                                ? 2 * setting.randomShootRadiance - aiDebuffRadiance : aiDebuffRadiance;
                myself.orientation += debuffRad - setting.randomShootRadiance/2;
                myself.FixRotation();
            }
            if(SimpleShoot.CanHit(myself.location, enemy.location, setting.shootingDistance))
                myself.Shoot(GameplayManager.Instance);
        }

    }
}