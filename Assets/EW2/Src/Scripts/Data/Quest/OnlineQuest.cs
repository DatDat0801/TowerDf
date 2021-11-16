using TigerForge;
using UnityEngine;

namespace EW2
{
    public class OnlineQuest : QuestItem
    {
        public override void Init()
        {
            base.Init();
            if (targetType == TargetType.Login)
            {
                EventManager.StartListening(GamePlayEvent.OnGameStart, OnStartGame);
            }
        }

        private void OnStartGame()
        {
            if (IsCanIncrease())
            {
                Count++;
                //Debug.LogError($"Online: {Count}");
            }
        }


        public override void CleanEventQuest()
        {
            base.CleanEventQuest();
            if (targetType == TargetType.Login)
                EventManager.StopListening(GamePlayEvent.OnGameStart, OnStartGame);
        }
    }
}