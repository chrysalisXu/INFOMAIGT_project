// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using INFOMAIGT.Map;
using INFOMAIGT.Data;

namespace INFOMAIGT.UI
{
    public class LevelHint : UIElement
    {
        public Text hint;
        public Text hint2;

        public override void UIStart()
        {
            hint.text = $"Win {DataManager.Instance.winTimes}";
            hint2.text = $"Lose {DataManager.Instance.loseTimes}";
        }
    }
}
