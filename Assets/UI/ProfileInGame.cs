// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using INFOMAIGT.Gameplay;

namespace INFOMAIGT.UI
{
    public class ProfileInGame : UIElement
    {
        [NonSerialized]
        public Image[] UIhearts = new Image[9];
        [NonSerialized]
        static int interval = 20;
        
        [SerializeField]
        RectTransform hpBarRect;
        [SerializeField]
        GameObject hpPointTemplate;
        [SerializeField]
        public Text name;
        [SerializeField]
        public Image profile;

        [NonSerialized]
        Player player;
        [NonSerialized]
        int activeHeart = 0;
        
        public override void UIStart()
        {
            for (int i=0; i<9; i++)
            {
                UIhearts[i] = Instantiate(hpPointTemplate, hpBarRect).GetComponent<Image>();
                UIhearts[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    interval * i, UIhearts[i].GetComponent<RectTransform>().anchoredPosition.y
                );
                UIhearts[i].gameObject.SetActive(false);
            }
        }

        public override void UIUpdate()
        {
            RefreshPlayerHealth();
        }

        public void SetPlayer(Player myself)
        {
            player = myself;
            profile.material = player.ai.setting.material;
            name.text = player.ai.setting.name;
        }

        public void RefreshPlayerHealth(){
            for(int i = activeHeart; i < player.health; i++)
                UIhearts[i].gameObject.SetActive(true);
            for(int i = player.health; i < activeHeart; i++)
                UIhearts[i].gameObject.SetActive(false);
            activeHeart = player.health;
        }
    }
}
