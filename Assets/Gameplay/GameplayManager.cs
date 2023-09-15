// Author: Chrysalis shiyuchongf@gmail.com

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using INFOMAIGT.Map;

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
            playerDict.Add(playerID, new Player(position, this==Instance));
        }

        public void CreateBullet(int playerID)
        {
            Player player = playerDict[playerID];
            bulletList.Add(new Bullet(
            player.location + new Vector3(
                MathF.Sin(player.orientation) * player.radius * 2,
                MathF.Cos(player.orientation) * player.radius * 2,
                0
            ),
            new Vector3(
                MathF.Sin(player.orientation) * player.MaxBulletSpeed,
                MathF.Cos(player.orientation) * player.MaxBulletSpeed,
                0
            ),
            this==Instance));
        }

        public void LogicalUpdate()
        {
            // destroy bullets dead last frame
            DestroyDeadBullets();
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
            if (this != Instance) return; // TODO: switch to AI in AI prediction mode
            // move
            if (Input.GetKey("w")){
                playerDict[1].location += new Vector3 (0, playerDict[1].maxVelocity, 0);
                FixPlayerPosition(playerDict[1], KeyCode.UpArrow);
            }
            if (Input.GetKey("a")){
                playerDict[1].location += new Vector3 (-playerDict[1].maxVelocity, 0, 0);
                FixPlayerPosition(playerDict[1], KeyCode.LeftArrow);
            }
            if (Input.GetKey("s")){
                playerDict[1].location += new Vector3 (0, -playerDict[1].maxVelocity, 0);
                FixPlayerPosition(playerDict[1], KeyCode.DownArrow);
            }
            if (Input.GetKey("d")){
                playerDict[1].location += new Vector3 (playerDict[1].maxVelocity, 0, 0);
                FixPlayerPosition(playerDict[1], KeyCode.RightArrow);
            }

            // menu
            if (Input.GetKey("escape")) // TODO: MENU / Restart, now just quit the game
                Application.Quit();

            // rotation
            Vector3 mouseDirection = pcCamera.ScreenToWorldPoint(Input.mousePosition) - playerDict[1].location;
            float rad = 0;
            if (mouseDirection.y==0)
            {
                rad = MathF.PI/2;
                if (mouseDirection.x<0) rad = -rad;
            }
            else
            {
                rad = MathF.Atan(mouseDirection.x/mouseDirection.y);
                if (mouseDirection.y<0)
                    rad += MathF.PI;
            }
            playerDict[1].orientation = rad; // TODO: add max rotation speed;

            // shoot!
            if (Input.GetMouseButtonDown(0))
                CreateBullet(1);
        }

        public void MoveAI()
        {
            // TODO: AI
        }

        public void UpdateBullets()
        {
            foreach(Bullet b in bulletList)
            {
                b.LogicalUpdate();
                foreach(var item in playerDict){
                    if(!item.Value.alive) continue;
                    Vector3 distance = b.location - item.Value.location;
                    if (distance.sqrMagnitude < b.radius + item.Value.radius){
                        DestroyPlayer(item.Key);
                        b.alive = false;
                        break;
                    }
                }
            }
        }

        // stop player from passing walls like ghost
        public void FixPlayerPosition(Player p, KeyCode direction)
        {
            if (direction == KeyCode.UpArrow)
            {
                int wallID = MapManager.Instance.GetWallID(
                    (int)MathF.Floor(p.location.x / Wall.size),
                    (int)MathF.Floor((p.location.y + p.radius) / Wall.size)
                );
                if (!MapManager.Instance.wallMap.ContainsKey(wallID)) return;
                p.location.y = MathF.Floor((p.location.y + p.radius) / Wall.size) * Wall.size - p.radius;
            }
            else if (direction == KeyCode.DownArrow)
            {
                int wallID = MapManager.Instance.GetWallID(
                    (int)MathF.Floor(p.location.x / Wall.size),
                    (int)MathF.Floor((p.location.y - p.radius) / Wall.size)
                );
                if (!MapManager.Instance.wallMap.ContainsKey(wallID)) return;
                p.location.y = MathF.Ceiling((p.location.y - p.radius) / Wall.size) * Wall.size + p.radius;
            }
            else if (direction == KeyCode.LeftArrow)
            {
                int wallID = MapManager.Instance.GetWallID(
                    (int)MathF.Floor((p.location.x - p.radius) / Wall.size),
                    (int)MathF.Floor(p.location.y / Wall.size)
                );
                if (!MapManager.Instance.wallMap.ContainsKey(wallID)) return;
                p.location.x = MathF.Ceiling((p.location.x - p.radius) / Wall.size) * Wall.size + p.radius;
            }
            else if (direction == KeyCode.RightArrow)
            {
                int wallID = MapManager.Instance.GetWallID(
                    (int)MathF.Floor((p.location.x + p.radius) / Wall.size),
                    (int)MathF.Floor(p.location.y / Wall.size)
                );
                if (!MapManager.Instance.wallMap.ContainsKey(wallID)) return;
                p.location.x = MathF.Floor((p.location.x + p.radius) / Wall.size) * Wall.size - p.radius;
            }
        }


        public void HandleCollision()
        {
            // bullet X wall = bounce, using simple method to accelerate
            foreach(Bullet b in bulletList)
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
            for(int i=0; i<bulletList.Count; i++)
            {
                if (!bulletList[i].alive) continue;
                for(int j=i+1; j<bulletList.Count; j++)
                {
                    if (!bulletList[j].alive) continue;
                    Vector3 distance = bulletList[i].location - bulletList[j].location;
                    if (distance.sqrMagnitude < bulletList[i].radius + bulletList[j].radius){
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
            Debug.Log($"Player {playerID} Lost!");
            playerDict[playerID].alive = false;
        }

        public void DestroyDeadBullets()
        {
            List<Bullet> removeList = new List<Bullet>();
            foreach(Bullet b in bulletList)
                if (!b.alive) removeList.Add(b);
            foreach(Bullet b in removeList)
                bulletList.Remove(b);
        }

        void DisplayBullet()
        {
            RenderParams rp = new RenderParams(matBullet);
            foreach(Bullet b in bulletList)
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
            foreach(var item in playerDict)
            {
                if(!item.Value.alive) continue;
                if(item.Key == 1) rp = pc_rp;
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
            LogicalUpdate();
            DisplayBullet();
            DisplayPlayers();
        }
    }
}