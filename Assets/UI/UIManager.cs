using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using INFOMAIGT.Gameplay;


namespace INFOMAIGT.UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance = null;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<UIManager>();
                if (_instance == null) throw new Exception("UI not found");
                return _instance;
            }
        }

        [NonSerialized]
        public Mesh uiMeshPlayer = null;
        [NonSerialized]
        public Mesh uiMeshAI = null;

        [NonSerialized]
        public List<Vector3> location = new List<Vector3>(); 

        [SerializeField]
        public Material uiMaterialPlayer;
        public Material uiMaterialAI;

        float barWidthPlayer = 30f;
        float barWidthAI = 30f;
        float barHeight = 5f;

        void CreatUI()
        {
            uiMeshPlayer = CreateMesh(barWidthPlayer, uiMeshPlayer);
            uiMeshAI = CreateMesh(barWidthAI, uiMeshAI);

            // better location define?
            if (GameplayManager.Instance.playerDict[1].alive)
                location.Add(GameplayManager.Instance.playerDict[1].location + new Vector3(-85, 150, 0));
            if (GameplayManager.Instance.playerDict[2].alive)
                location.Add(GameplayManager.Instance.playerDict[2].location + new Vector3(65, 30, 0));
            
            RenderParams playerUI = new RenderParams(uiMaterialPlayer);
            Graphics.RenderMesh(playerUI, uiMeshPlayer, 0, Matrix4x4.Translate(location[0]));
            RenderParams aiUI = new RenderParams(uiMaterialAI);
            Graphics.RenderMesh(aiUI, uiMeshAI, 0, Matrix4x4.Translate(location[1]));
        }

        Mesh CreateMesh(float barWidth, Mesh uiMesh) 
        {
            if (barWidth >= 0)
            {
                if (uiMesh == null || Mathf.Abs(uiMesh.vertices[1].x - uiMesh.vertices[0].x) != barWidth)
                {
                    if (uiMesh != null)
                    {
                        Destroy(uiMesh);
                    }

                    uiMesh = new Mesh();
                    uiMesh.vertices = new Vector3[] {
                        new Vector3(0, 0, 0),
                        new Vector3(barWidth, 0, 0),
                        new Vector3(barWidth, barHeight, 0),
                        new Vector3(0, barHeight, 0),
                    };
                    uiMesh.triangles = new int[] {
                        0, 1, 2, 0, 2, 3
                    };
                }
            }
            return uiMesh;
        }
        void UpdateUI()
        {
            barWidthPlayer = GameplayManager.Instance.playerDict[1].health*6;
            barWidthAI = GameplayManager.Instance.playerDict[2].health*6;
            Debug.Log(barWidthAI);
            CreatUI();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateUI();
        }
    }
}
