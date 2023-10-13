// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using INFOMAIGT.Map;

namespace INFOMAIGT.UI
{
    public class LevelSelectPanel : UIElement
    {
        [NonSerialized]
        public LevelDetail[] detailPanels = new LevelDetail[6];
        public override void UIStart()
        {
            for(int i=0; i<6; i++)
            {
                detailPanels[i] = (LevelDetail) UIManager.Instance.ComponentsList[$"Level{i+1}"];
                if (i < LevelManager.levels.Length)
                    detailPanels[i].SetContent(LevelManager.levels[i], i);
                else
                    detailPanels[i].gameObject.SetActive(false);
            }
        }
    }
}
