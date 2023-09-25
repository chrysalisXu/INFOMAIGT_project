// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using INFOMAIGT.Map;
using INFOMAIGT.AI;

namespace INFOMAIGT.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {

        // singleton, only Instance is valid and will be updated, others are for ai.
        private static GameplayManager _instance = null;
        public static GameplayManager Instance
        {
            get
            {
                if (_instance == null) _instance = FindObjectOfType<GameplayManager>();
                if (_instance == null) throw new Exception("Gameplay manager not exists!");
                return _instance;
            }
        }

        [SerializeField]
        public Material matPC;

        [SerializeField]
        public Material matAI;

        [SerializeField]
        public Material matBullet;

        [SerializeField]
        public Camera pcCamera;

        // player 1 is PC, others are ai
        public Dictionary<int, Player> playerDict = new Dictionary<int, Player>();
        public List<Bullet> bulletList = new List<Bullet>();

        public GameplayManager Clone()
        {
            // TODO:
            // Deep copy a gameplay manager for AI to predict what will happen next.
            return new GameplayManager();
        }

        public void CreatePlayer(Vector3 position, int playerID)
        {
            playerDict.Add(playerID, new Player(
                position,
                this==Instance,
                AIManager.Instance.CreateAI(playerID))
            );
        }

        

        public void LogicalUpdate()
        {
            // destroy bullets dead last frame
            DestroyDeadBullets();
            // update status
            foreach(Player player in playerDict.Values)
                player.UpdateCooldown();
            // pc move
            MovePC();
            // ai move
            MoveAI();
            // update bullets, bullet X player collision detect
            UpdateBullets();
            // handle collision
            HandleCollision();
        }

        public void MovePC()
        {
            Player pc = playerDict[1];
            if (this != Instance) return; // TODO: switch to AI in AI prediction mode
            if (!pc.alive) return;

            // move
            if (Input.GetKey("w")){
                pc.location = MapManager.Instance.GetFixedPlayerPosition(
                    pc.location, KeyCode.UpArrow, pc.maxVelocity, pc.radius
                );
            }
            if (Input.GetKey("a")){
                pc.location = MapManager.Instance.GetFixedPlayerPosition(
                    pc.location, KeyCode.LeftArrow, pc.maxVelocity, pc.radius
                );
            }
            if (Input.GetKey("s")){
                pc.location = MapManager.Instance.GetFixedPlayerPosition(
                    pc.location, KeyCode.DownArrow, pc.maxVelocity, pc.radius
                );
            }
            if (Input.GetKey("d")){
                pc.location = MapManager.Instance.GetFixedPlayerPosition(
                    pc.location, KeyCode.RightArrow, pc.maxVelocity, pc.radius
                );
            }

            // rotation
            Vector3 mouseDirection =  - pc.location;
            pc.rotateToward(pcCamera.ScreenToWorldPoint(Input.mousePosition));

            // shoot!
            if (Input.GetMouseButtonDown(0))
                pc.Shoot(this);
        }

        public void MoveAI()
        {
            foreach ((int id, Player player) in playerDict){
                if (id!=1) player.ai.Move();
            }
        }

        public void UpdateBullets()
        {
            foreach (Bullet b in bulletList)
            {
                b.LogicalUpdate();
                foreach (var item in playerDict)
                {
                    if (!item.Value.alive) continue;
                    Vector3 distance = b.location - item.Value.location;
                    if (distance.magnitude < b.radius + item.Value.radius)
                    {
                        DestroyPlayer(item.Key);
                        b.alive = false;
                        break;
                    }
                }
            }
        }

        


        public void HandleCollision()
        {
            // bullet X wall = bounce, using simple method to accelerate
            foreach (Bullet b in bulletList)
            {
                int wallID = MapManager.Instance.GetWallID(
                    (int)MathF.Floor(b.location.x / Wall.size),
                    (int)MathF.Floor(b.location.y / Wall.size)
                );
                if (MapManager.Instance.wallMap.ContainsKey(wallID))
                {
                    Vector3 lastLocation = b.location - b.velocity;
                    if ((int)MathF.Floor(b.location.x / Wall.size) == (int)MathF.Floor(lastLocation.x / Wall.size))
                    {
                        // from y axis
                        b.location.y = (int)MathF.Floor(b.location.y / Wall.size) * Wall.size;
                        if (b.velocity.y < 0)
                            b.location.y += Wall.size;
                        b.velocity.y *= -1;
                    }
                    else
                    {
                        // from x axis
                        b.location.x = (int)MathF.Floor(b.location.x / Wall.size) * Wall.size;
                        if (b.velocity.x < 0)
                            b.location.x += Wall.size;
                        b.velocity.x *= -1;
                    }
                }
            }

            // bullet X bullet = kill each other
            for (int i = 0; i < bulletList.Count; i++)
            {
                if (!bulletList[i].alive) continue;
                for (int j = i + 1; j < bulletList.Count; j++)
                {
                    if (!bulletList[j].alive) continue;
                    Vector3 distance = bulletList[i].location - bulletList[j].location;
                    if (distance.magnitude < bulletList[i].radius + bulletList[j].radius)
                    {
                        bulletList[i].alive = false;
                        bulletList[j].alive = false;
                        break;
                    }
                }
            }

        }

        public void DestroyPlayer(int playerID)
        {
            // TODO: UI display.
            if (playerDict[playerID].health <= 0)
            {
                Debug.Log($"Player {playerID} Lost!");
                playerDict[playerID].alive = false;
            }
            else
            {
                playerDict[playerID].health -= 1f;
                if (playerDict[playerID].health == 0)
                    playerDict[playerID].alive = false;
            }
        }

        public void DestroyDeadBullets()
        {
            List<Bullet> removeList = new List<Bullet>();
            foreach (Bullet b in bulletList)
                if (!b.alive) removeList.Add(b);
            foreach (Bullet b in removeList)
                bulletList.Remove(b);
        }

        void DisplayBullet()
        {
            RenderParams rp = new RenderParams(matBullet);
            foreach (Bullet b in bulletList)
            {
                b.UpdateMesh();
                Graphics.RenderMesh(rp, b.circleMesh, 0, Matrix4x4.Translate(Vector3.zero));
            }
        }

        void DisplayPlayers()
        {
            RenderParams pc_rp = new RenderParams(matPC);
            RenderParams ai_rp = new RenderParams(matAI);
            RenderParams rp;
            foreach (var item in playerDict)
            {
                if (!item.Value.alive) continue;
                if (item.Key == 1) rp = pc_rp;
                else rp = ai_rp;
                item.Value.UpdateMesh();
                Graphics.RenderMesh(rp, item.Value.circleMesh, 0, Matrix4x4.Translate(Vector3.zero));
                Graphics.RenderMesh(rp, item.Value.artilleryMesh, 0, Matrix4x4.Translate(Vector3.zero));
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (this != Instance) return;
            // menu
            if (Input.GetKey("escape")) // TODO: MENU / Restart, now just quit the game
                Application.Quit();
            LogicalUpdate();
            DisplayBullet();
            DisplayPlayers();
        }
    }
}