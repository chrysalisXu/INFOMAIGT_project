using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using INFOMAIGT.Gameplay;


namespace INFOMAIGT.UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance = null;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<UIManager>();
                if (_instance == null) throw new Exception("UI not found");
                return _instance;
            }
        }

        [NonSerialized]
        public Dictionary<string, UIElement> ComponentsList = new Dictionary<string, UIElement>();
    }
}
