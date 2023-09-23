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
        public float closingDistance;  // how far ai will stop closing when it can shoot you, min 10
        public float closingWeight;     // how eager ai is when trying to move closer, min 0
        public float suicideThreshold;  
        // how early a ai will stop to dodge. the bigger the better.
        // 1 means ai will stop with a bullet 10 meters away, 100 means 1 meters away.

        public AISetting(int id){
            playerID = id;
            closingDistance = 10;
            closingWeight = 1;
            suicideThreshold = 1000000; // surrender in harsh close combat
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

        public BaseAI CreateAI(int playerID)
        {
            foreach (var setting in settingsAI)
                if (setting.playerID == playerID) return new BaseAI(setting);
            return new BaseAI(playerID);
        }


    }
}