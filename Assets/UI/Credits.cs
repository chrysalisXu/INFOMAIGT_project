// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace INFOMAIGT.UI
{
    public class Credits : UIElement
    {
        public void ClickThanks()
        {
            UIManager.Instance.ComponentsList["StartMenu"].gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
