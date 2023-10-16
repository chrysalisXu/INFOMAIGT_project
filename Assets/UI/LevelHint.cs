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
            hint.text = $"level {DataManager.Instance.winTimes+1}";
            hint2.text = $"lost {DataManager.Instance.loseTimes}";
        }
    }
}
