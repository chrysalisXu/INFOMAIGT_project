// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace INFOMAIGT.UI
{
    public class StartMenu : UIElement
    {
        public void ClickStart()
        {
            UIManager.Instance.ComponentsList["LevelSelect"].gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void ClickCredits()
        {
            UIManager.Instance.ComponentsList["Credits"].gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void ClickSetting()
        {
            return; // TODO
            UIManager.Instance.ComponentsList["Setting"].gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void ClickHelp()
        {
            UIManager.Instance.ComponentsList["Help"].gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void ClickQuit()
        {
            Application.Quit();
        }
    }
}