// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using INFOMAIGT.Data;
using INFOMAIGT.Map;

namespace INFOMAIGT.UI
{
    public class FinishMenu : UIElement
    {
        [SerializeField]
        public Text ResultText;

        [SerializeField]
        public Button NextLevelButton;
        [SerializeField]
        public Button RestartButton;
        [SerializeField]
        public Image background;
        bool transition = false;

        public override void UIUpdate()
        {
            if (background.color.a == 1) return;
            if (background.color.a >= 0.95f)
                background.color = new Color(1,1,1,1);
            else
                background.color = new Color(1, 1, 1, background.color.a * 1.05f);
        }

        public void Transition()
        {
            background.color = new Color(1,1,1,0.4f);
            transition = true;
        }

        public void LateUpdate()
        {
            if (DataManager.Instance.report.winnerID==1)
            {
                NextLevelButton.gameObject.SetActive(true);
                RestartButton.gameObject.SetActive(false);
                ResultText.text = "You Win";
            }
            else
            {
                NextLevelButton.gameObject.SetActive(false);
                RestartButton.gameObject.SetActive(true);
                ResultText.text = "You Lose";
            }
            if(!transition) Transition();
        }

        public void ClickRestart()
        {
            if(CheckRated()) {
                DataManager.Instance.SendReport();
                SceneManager.LoadScene("Main");
            }
        }

        public void ClickNext()
        {
            if(CheckRated()) 
            {
                int levelID = Array.IndexOf(LevelManager.levels, LevelManager.currentLevel) + 1;
                if (levelID >= LevelManager.levels.Length)
                    levelID = LevelManager.levels.Length - 1;
                LevelManager.Instance.SelectLevel(levelID);
                DataManager.Instance.SendReport();
                SceneManager.LoadScene("Main");
            }
        }

        public void ClickQuit()
        {
            if(CheckRated())
            {
                DataManager.Instance.SendReport();
                SceneManager.LoadScene("Start");
            }
        }

        public bool CheckRated()
        {
            if(DataManager.Instance.report.pcRating == 0)
            {
                ((RatePanel)UIManager.Instance.ComponentsList["RatePanel"]).Highlight();
                return false;
            }
            return true;
        }
    }
}
