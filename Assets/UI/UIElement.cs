// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace INFOMAIGT.UI
{
    public class UIElement : MonoBehaviour
    {
        [SerializeField]
        string elementID = ""; // any new component should change it!
        [SerializeField]
        bool defaultInactive = false;
        [NonSerialized]
        public RectTransform rect;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
            UIManager.Instance.ComponentsList.Add(elementID, this);
            if (defaultInactive) gameObject.SetActive(false);
        }

        void Start()
        {
            UIStart();
        }
        public virtual void UIStart(){}

        void Update()
        {
            UIUpdate();
        }
        public virtual void UIUpdate(){}
    }
}