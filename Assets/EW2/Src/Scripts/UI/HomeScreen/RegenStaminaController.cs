using System.Collections;
using TigerForge;
using UnityEngine;
using Zitga.TrackingFirebase;

namespace EW2
{
    public class RegenStaminaController : Singleton<RegenStaminaController>
    {
        private StaminaData.Stamina staminaConfig;

        private Coroutine coroutineCooldown;

        private void Awake()
        {
            staminaConfig = GameContainer.Instance.Get<StaminaDataBase>().GetConfig().GetDataConfig();

            EventManager.StartListening(GamePlayEvent.OnAddMoney(ResourceType.Money, MoneyType.Stamina),
                OnStaminaAdd);

            EventManager.StartListening(GamePlayEvent.OnSubMoney(ResourceType.Money, MoneyType.Stamina),
                OnStaminaSub);

            CheckRegenStamina();

            EventManager.StartListening(GamePlayEvent.OnLoadDataSuccess, CheckRegenStamina);
        }

        private void OnStaminaSub()
        {
            if (UserData.Instance.GetMoney(MoneyType.Stamina) < staminaConfig.maxStamina)
            {
                var regenStaminaData = UserData.Instance.RegenStamina;

                var numbStaminaSub =
                    EventManager.GetData<long>(GamePlayEvent.OnSubMoney(ResourceType.Money, MoneyType.Stamina));

                if (regenStaminaData.timeStart == 0)
                {
                    var timeNow = TimeManager.NowInSeconds;

                    var nextTimeRegenSeconds = timeNow + staminaConfig.timeRecovery;

                    var timeRegenFullSeconds = timeNow + (numbStaminaSub * staminaConfig.timeRecovery);

                    UserData.Instance.SetTimeStartRegenStamina(timeNow);

                    UserData.Instance.UpdateNextTimeRegenStamina(nextTimeRegenSeconds);

                    UserData.Instance.UpdateTimeFullRegenStamina(timeRegenFullSeconds);

                    if (coroutineCooldown == null)
                    {
                        coroutineCooldown = StartCoroutine(Cooldown(staminaConfig.timeRecovery));
                    }
                }
                else
                {
                    var newTime = regenStaminaData.timeRegenFullSeconds + (numbStaminaSub * staminaConfig.timeRecovery);

                    UserData.Instance.UpdateTimeFullRegenStamina(newTime);
                }
            }
        }

        private void OnStaminaAdd()
        {
            if (UserData.Instance.GetMoney(MoneyType.Stamina) >= staminaConfig.maxStamina)
            {
                if (coroutineCooldown != null)
                    StopCoroutine(coroutineCooldown);

                UserData.Instance.StopRegenStamina();
            }
            else
            {
                if (coroutineCooldown == null)
                {
                    coroutineCooldown = StartCoroutine(Cooldown(staminaConfig.timeRecovery));
                }
            }
        }

        private void CheckRegenStamina()
        {
            var regenStaminaData = UserData.Instance.RegenStamina;

            if (UserData.Instance.GetMoney(MoneyType.Stamina) >= staminaConfig.maxStamina)
            {
                UserData.Instance.StopRegenStamina();

                return;
            }

            if (regenStaminaData.timeStart > 0)
            {
                var timeNow = TimeManager.NowInSeconds;

                var timePass = timeNow - regenStaminaData.timeStart;

                var staminaReceive = timePass / staminaConfig.timeRecovery;

                if (staminaReceive > 0)
                {
                    Debug.Log($"staminaReceive = {staminaReceive}");

                    if ((UserData.Instance.GetMoney(MoneyType.Stamina) + staminaReceive) < staminaConfig.maxStamina)
                    {
                        UserData.Instance.AddMoney(MoneyType.Stamina, (long) staminaReceive,
                            AnalyticsConstants.SourceHome, "", false);
                    }
                    else
                    {
                        UserData.Instance.AddMoney(MoneyType.Stamina,
                            staminaConfig.maxStamina - UserData.Instance.GetMoney(MoneyType.Stamina),
                            AnalyticsConstants.SourceHome, "", false);
                    }
                }

                if (UserData.Instance.GetMoney(MoneyType.Stamina) < staminaConfig.maxStamina)
                {
                    var timeRegen1Stamina = staminaConfig.timeRecovery -
                                            (timePass - (staminaReceive * staminaConfig.timeRecovery));

                    var nextTimeRegenSeconds = timeNow + timeRegen1Stamina;

                    UserData.Instance.SetTimeStartRegenStamina(timeNow);

                    UserData.Instance.UpdateNextTimeRegenStamina(nextTimeRegenSeconds);

                    if (coroutineCooldown == null)
                    {
                        coroutineCooldown = StartCoroutine(Cooldown(timeRegen1Stamina));
                    }
                }
                else
                {
                    UserData.Instance.StopRegenStamina();
                }
            }
            else
            {
                Debug.Log("Run");

                if (UserData.Instance.GetMoney(MoneyType.Stamina) <= 0)
                    UserData.Instance.AddMoney(MoneyType.Stamina, staminaConfig.maxStamina,
                        AnalyticsConstants.SourceHome, "", false);
            }
        }

        private IEnumerator Cooldown(long timeRecovery)
        {
            // Debug.LogWarning("Run " + timeRecovery);

            yield return new WaitForSecondsRealtime(timeRecovery);

            // Debug.LogWarning("Add stamina");

            if (UserData.Instance.GetMoney(MoneyType.Stamina) + 1 < staminaConfig.maxStamina)
            {
                var nextTimeRegenSeconds = TimeManager.NowInSeconds + staminaConfig.timeRecovery;

                UserData.Instance.UpdateNextTimeRegenStamina(nextTimeRegenSeconds);

                UserData.Instance.SetTimeStartRegenStamina(TimeManager.NowInSeconds);
            }

            if (coroutineCooldown != null)
            {
                coroutineCooldown = null;
            }

            UserData.Instance.AddMoney(MoneyType.Stamina, 1, AnalyticsConstants.SourceHome, "", false);
        }
    }
}