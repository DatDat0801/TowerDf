using System;
using System.Collections.Generic;
using UnityEngine;
using Zitga.UIFramework;

namespace EW2
{
    /// <summary>
    /// Contain screen info to be opened
    /// </summary>
    public class RequestScreenOpen
    {
        public string ScreenId { get; set; }
        public WindowProperties Properties { get; set; }

        public void ExecuteRequest()
        {
            this.ScreenId = string.Empty;
        }
    }
    public class GamePlayControllerBase : Singleton<GamePlayControllerBase>
    {
        public static PlayMode playMode = PlayMode.None;

        public static GameMode gameMode = GameMode.CampaignMode;

        public static int CampaignId { get; set; }

        public static readonly List<int> heroList = new List<int>();

        public static RequestScreenOpen RequestScreenOpenOnMenu { get; set; }
        public int WorldId { get; protected set; }

        public int MapId { get; protected set; }

        public int ModeId { get; protected set; }

        public bool IsPause { get; set; }

        private float speed;

        public float Speed
        {
            get => speed;

            set
            {
                if (speed != value)
                {
                    speed = value;

                    Time.timeScale = value;
                }
            }
        }

        public int TotalUseSpellInWave { get; set; }
        public int TotalUseEnv { get; set; }
        public GamePlayState State { get; set; }
        [SerializeField] protected CameraController cameraController;

        [SerializeField] protected StartWaveButtonController startWaveButtonController;

        [SerializeField] protected SpawnControllerBase spawnController;

        public SpawnControllerBase SpawnController => spawnController;
        public readonly List<HeroBase> heroes = new List<HeroBase>();

        public virtual void SpawnGamePlayUi()
        {
        }

        public virtual void CallEarlyWave()
        {
        }

        public virtual void AddBuilding(Building building)
        {
        }

        public virtual void RemoveBuilding(Building building)
        {
        }

        public virtual void AddHero(HeroBase hero)
        {
        }

        public virtual List<HeroBase> GetHeroes()
        {
            return this.heroes;
        }

        public virtual HeroBase GetHeroes(int heroId)
        {
            for (int i = 0; i < heroes.Count; i++)
            {
                if (heroes[i].Id == heroId)
                {
                    return heroes[i];
                }
            }

            return null;
        }

        public virtual void CountUseSpell(int spellId)
        {
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            IsPause = true;
        }

        public CameraController GetCameraController()
        {
            if (cameraController == null)
            {
                var camera = ResourceUtils.GetUnitOther("main_camera", transform);

                if (camera)
                {
                    cameraController = camera.GetComponent<CameraController>();
                }
            }

            return cameraController;
        }

        public void ResumeGame()
        {
            Time.timeScale = Speed;
            Instance.IsPause = false;
        }

        public virtual void ShowEndGame(bool isWin, int star)
        {
        }
    }
}