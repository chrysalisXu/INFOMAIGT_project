// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using INFOMAIGT.Data;

namespace INFOMAIGT.UI
{
    public class RatePanel : UIElement
    {
        [SerializeField]
        public Image[] tickBoxes;
        [SerializeField]
        public Image background;
        [SerializeField]
        public Sprite defaultBoxImg;
        [SerializeField]
        public Sprite tickedBoxImg;

        public override void UIUpdate()
        {
            if (background.color.a == 0) return;
            if (background.color.a <= 0.1)
                background.color = new Color(1,0,0,0);
            else
                background.color = new Color(1, 0, 0, background.color.a * 0.95f);
        }

        public void Highlight()
        {
            background.color = new Color(1,0,0,1);
        }

        public void TickRate(int value)
        {
            foreach(Image image in tickBoxes)
            {
                image.sprite = defaultBoxImg;
            }
            tickBoxes[value-1].sprite = tickedBoxImg;
            DataManager.Instance.report.pcRating = value;
        }
    }
}
