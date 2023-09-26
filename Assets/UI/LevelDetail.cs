// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using INFOMAIGT.Map;

namespace INFOMAIGT.UI
{
    public class LevelDetail : UIElement
    {
        public Text name;
        public Text desc;
        public Image image;
        [NonSerialized]
        public int levelID;

        public void SetContent(LevelSetting setting, int id)
        {
            name.text = setting.name;
            desc.text = setting.desc;
            image.sprite = setting.image;
            levelID = id;
        }

        public void ClickLevel()
        {
            LevelManager.Instance.SelectLevel(levelID);
            SceneManager.LoadScene("Main");
        }
    }
}
