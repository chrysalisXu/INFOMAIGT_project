// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace INFOMAIGT.AI
{
    [Serializable]
    public struct AISetting {
        public int playerID;
        public float closingDistance;   // how far ai will stop closing when it can shoot you, min 10
        public float shootingDistance;  // how far ai will start to shoot when it can shoot you, min 10
        public float closingWeight;         // how eager ai is when trying to move closer, min 0
        public string name;
        public bool advanceDodgeOn;         // whether ai care bullets' speed
        public float randomShootRadiance;   // ai shoot angle rand
        public float additionalCoolDown;    // additional cooldown mulitplier
        public Material material;

        public AISetting(int id){
            playerID = id;
            closingDistance = 10;
            shootingDistance = 10;
            closingWeight = 1;
            name = "AI";
            advanceDodgeOn = true;
            randomShootRadiance = 0;
            additionalCoolDown = 0;
            material = AIManager.Instance.defaultMatAI;
        }
    }

    // this class decide AI setting
    public class AIManager : MonoBehaviour
    {

        // singleton
        private static AIManager _instance = null;
        public static AIManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<AIManager>();
                if (_instance == null) throw new Exception("AI manager not exists!");
                return _instance;
            }
        }

        [SerializeField]
        AISetting [] settingsAI;

        [SerializeField]
        public Material defaultMatAI;

        public BaseAI CreateAI(int playerID)
        {
            foreach (var setting in settingsAI)
                if (setting.playerID == playerID) return new BaseAI(setting);
            return new BaseAI(playerID);
        }


    }
}