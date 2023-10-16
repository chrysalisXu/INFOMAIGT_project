// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using INFOMAIGT.Data;

namespace INFOMAIGT.UI
{
    public class CookiePermission : UIElement
    {
        public void ClickAgree()
        {
            DataManager.Instance.CheckCookie();
            UIManager.Instance.ComponentsList["StartMenu"].gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
