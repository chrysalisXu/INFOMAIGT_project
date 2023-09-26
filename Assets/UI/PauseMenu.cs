// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace INFOMAIGT.UI
{
    public class PauseMenu : UIElement
    {
        public void ClickRestart()
        {
            SceneManager.LoadScene("Main");
        }

        public void ClickQuit()
        {
            SceneManager.LoadScene("Start");
        }
    }
}
