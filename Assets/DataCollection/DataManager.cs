// Author: Chrysalis shiyuchongf@gmail.com
// store and handle levels.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
            levelName           = name;

            winnerID            = 0;
            winnerHP            = 0;

            framesCount         = 0;
            levelTime           = 0;
            pauseframes         = 0;

            pcTravelDistance    = 0;
            inputTimes          = 0; // TODO

            bulletsFiredPC      = 0; // TODO
            pcRating            = 0;
        }
    }

    public class DataManager : MonoBehaviour
    {
        const string REPORT_URL = "http://3.76.106.191:1197/api/ai_data_collection/report";
        const string COOKIE_URL = "http://3.76.106.191:1197/api/ai_data_collection/cookie";

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
        public int winTimes = 0;
        public int loseTimes = 0;

        void Awake()
        {
            if (DataManager.Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        public void CheckCookie()
        {
            WWWForm form = new WWWForm();
            var request = UnityWebRequest.Post(DataManager.COOKIE_URL, form);
            request.SendWebRequest();
        }

        public void LevelStart()
        {
            report = new LevelData(LevelManager.currentLevel.name);
        }

        public void SendReport()
        {
            if (report.winnerID == 1) winTimes ++;
            else loseTimes ++;
            WWWForm form = new WWWForm();
            form.AddField("levelName", report.levelName);
            form.AddField("winnerID", report.winnerID);
            form.AddField("winnerHP", report.winnerHP);
            form.AddField("framesCount", report.framesCount);
            form.AddField("pauseframes", report.pauseframes);
            form.AddField("levelTime", report.levelTime.ToString());
            form.AddField("pcTravelDistance", report.pcTravelDistance.ToString());
            form.AddField("inputTimes", report.inputTimes);
            form.AddField("bulletsFiredPC", report.bulletsFiredPC);
            form.AddField("pcRating", report.pcRating);

            var request = UnityWebRequest.Post(DataManager.REPORT_URL, form);
            request.SendWebRequest();
        }
    }
}
