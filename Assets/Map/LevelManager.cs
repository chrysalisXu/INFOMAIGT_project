// Author: Chrysalis shiyuchongf@gmail.com
// store and handle levels.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace INFOMAIGT.Map
{
    [Serializable]
    public struct LevelSetting {
        public string name;
        public string desc;
        public Sprite image;
        public TextAsset mapData;
    }

    public class LevelManager : MonoBehaviour
    {
        // singleton
        private static LevelManager _instance = null;
        public static LevelManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<LevelManager>();
                return _instance;
            }
        }

        [SerializeField]
        public LevelSetting [] levels;
        [NonSerialized]
        public static LevelSetting currentLevel;

        void Awake()
        {
            DontDestroyOnLoad(transform.gameObject);
        }

        public void SelectLevel(int selectID)
        {
            LevelManager.currentLevel = levels[selectID];
        }
    }

}
