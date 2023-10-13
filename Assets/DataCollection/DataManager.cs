// Author: Chrysalis shiyuchongf@gmail.com
// store and handle levels.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using INFOMAIGT.Map;

namespace INFOMAIGT.Data
{
    [Serializable]
    public struct LevelData {
        public string levelName;

        public int winnerID;
        public int winnerHP;

        public int framesCount; // without pause
        public float levelTime; // without pause
        public int pauseframes;

        public float pcTravelDistance;
        public int inputTimes;

        public int bulletsFiredPC;

        public int pcRating;

        public LevelData(string name)
        {
            levelName = name;

            winnerID = 0;
            winnerHP = 0;

            framesCount = 0;
            levelTime = 0;
            pauseframes = 0;

            pcTravelDistance = 0;
            inputTimes = 0; // TODO

            bulletsFiredPC = 0; // TODO
            pcRating = 0;
        }
    }

    public class DataManager : MonoBehaviour
    {
        // singleton
        private static DataManager _instance = null;
        public static DataManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<DataManager>();
                return _instance;
            }
        }

        public LevelData report;

        void Awake()
        {
            if (DataManager.Instance == this)
                DontDestroyOnLoad(gameObject);
            else
                Destroy(gameObject);
        }

        public void LevelStart()
        {
            report = new LevelData(LevelManager.currentLevel.name);
        }

        public void SendReport()
        {
            
        }
    }
}
