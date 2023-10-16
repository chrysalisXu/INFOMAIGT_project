// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using INFOMAIGT.Data;
using INFOMAIGT.Map;

namespace INFOMAIGT.UI
{
    public class StartMenu : UIElement
    {
        private GameObject soundEffect;
        private AudioSource pressEffect;

        private void Start()
        {
            soundEffect = GameObject.Find("SoundEffect");
            if (soundEffect != null)
            {
                pressEffect = soundEffect.GetComponent<AudioSource>();
            }
        }

        public void ClickStart()
        {
            // pressEffect.Play();
            // temporary data collection
            if (DataManager.Instance.winTimes == 0)
                LevelManager.Instance.SelectLevel(0);
            SceneManager.LoadScene("Main");
            return;

            UIManager.Instance.ComponentsList["LevelSelect"].gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void ClickCredits()
        {
            pressEffect.Play();
            UIManager.Instance.ComponentsList["Credits"].gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void ClickSetting()
        {
            pressEffect.Play();
            return; // TODO
            UIManager.Instance.ComponentsList["Setting"].gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void ClickHelp()
        {
            pressEffect.Play();
            UIManager.Instance.ComponentsList["Help"].gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void ClickQuit()
        {
            Application.Quit();
        }
    }
}
