// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace INFOMAIGT.UI
{
    public class HelpPanel : UIElement
    {
        public void ClickUnderstand()
        {
            UIManager.Instance.ComponentsList["StartMenu"].gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
