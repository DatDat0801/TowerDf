using System;
using EW2.Spell;
using UnityEngine;
using Zitga.UIFramework;

namespace EW2
{
    public class DirectionGoTo
    {
        public static void GotoProfile()
        {
            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            UIFrame.Instance.OpenWindow(ScreenIds.home);
            UIFrame.Instance.OpenWindow(ScreenIds.popup_profile);
        }

        public static void GotoGacha(GachaTabId tabId = GachaTabId.None)
        {
            if (!UnlockFeatureUtilities.IsRuneAvailable() && tabId == GachaTabId.Rune)
            {
                var titleNoti = String.Format(L.popup.spell_unlock_condition,
                    UnlockFeatureUtilities.RUNE_AVAILABLE_AT_STAGE_ID + 1);
                Ultilities.ShowToastNoti(titleNoti);
                return;
            }

            if (!UnlockFeatureUtilities.IsSpellAvailable() && tabId == GachaTabId.Spell)
            {
                var titleNoti = String.Format(L.popup.spell_unlock_condition,
                    UnlockFeatureUtilities.SPELL_AVAILABLE_AT_STAGE_ID + 1);
                Ultilities.ShowToastNoti(titleNoti);
                return;
            }

            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            GachaWindowProperties data = new GachaWindowProperties(tabId);
            UIFrame.Instance.OpenWindow(ScreenIds.gacha_scene, data);
        }

        public static void GotoHeroRoom()
        {
            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            UIFrame.Instance.OpenWindow(ScreenIds.hero_room_scene, new HeroRoomWindowProperties((int)HeroType.Jave));
        }

        public static void GotoSpellHeroRoom()
        {
            if (!UnlockFeatureUtilities.IsSpellAvailable())
            {
                var titleNoti = String.Format(L.popup.spell_unlock_condition,
                    UnlockFeatureUtilities.SPELL_AVAILABLE_AT_STAGE_ID + 1);
                Ultilities.ShowToastNoti(titleNoti);
                return;
            }
            
            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            UIFrame.Instance.OpenWindow(ScreenIds.hero_room_scene,
                new HeroRoomWindowProperties((int)HeroType.Jave, (int)HeroRoomTabId.Spell));
        }

        public static void GotoNighmareCampaign(int stageTarget = -1)
        {
            if (stageTarget >= 0)
            {
                var isUnlockStage =
                    UserData.Instance.CampaignData.IsUnlockedHardStage(0, stageTarget);

                if (!isUnlockStage)
                {
                    var content = string.Format(L.popup.locked_mode_notice, stageTarget + 1);
                    var dataInfo = new PopupInfoWindowProperties(L.popup.notice_txt, content);
                    UIFrame.Instance.OpenWindow(ScreenIds.popup_notice_text, dataInfo);
                    return;
                }
            }

            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            GamePlayControllerBase.playMode = PlayMode.ReplayCampaign;
            GamePlayControllerBase.gameMode = GameMode.CampaignMode;
            GamePlayControllerBase.CampaignId = stageTarget < 0
                ? UserData.Instance.CampaignData.GetStageNightmareCurrent()
                : MapCampaignInfo.GetCampaignId(0, stageTarget, (int)ModeCampaign.Nightmare);

            UIFrame.Instance.OpenWindow(ScreenIds.home);
        }

        public static void GotoNormalCampaign(int stageTarget = -1)
        {
            if (stageTarget >= 0)
            {
                var isUnlockStage = UserData.Instance.CampaignData.IsUnlockedStage(0, stageTarget);

                if (isUnlockStage)
                {
                    GamePlayControllerBase.CampaignId =MapCampaignInfo.GetCampaignId(0, stageTarget, (int)ModeCampaign.Normal);
                }
                else
                {
                    GamePlayControllerBase.CampaignId = UserData.Instance.CampaignData.GetStageCurrent();
                }
            }
            else
            {
                GamePlayControllerBase.CampaignId = UserData.Instance.CampaignData.GetStageCurrent();
            }
            
            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            GamePlayControllerBase.playMode = PlayMode.ReplayCampaign;
            GamePlayControllerBase.gameMode = GameMode.CampaignMode;

            UIFrame.Instance.OpenWindow(ScreenIds.home);
        }

        public static void GotoCheckinDaily()
        {
            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            UIFrame.Instance.OpenWindow(ScreenIds.home);
            UIFrame.Instance.OpenWindow(ScreenIds.daily_reward);
        }

        public static void GotoDailyQuest()
        {
            var questScene = GameObject.FindObjectOfType<QuestWindowController>();
            if (questScene)
            {
                questScene.TabsManager.SetSelected(0);
            }
            else
            {
                UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
                UIFrame.Instance.OpenWindow(ScreenIds.quest_scene);
            }
        }

        public static void GotoUpgradeSpell()
        {
            if (!UnlockFeatureUtilities.IsSpellAvailable())
            {
                var titleNoti = String.Format(L.popup.spell_unlock_condition,
                    UnlockFeatureUtilities.SPELL_AVAILABLE_AT_STAGE_ID + 1);
                Ultilities.ShowToastNoti(titleNoti);
                return;
            }

            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            var listSpell = UserData.Instance.GetListSpell();
            var checkAllSpellLvlMax = true;

            foreach (var spell in listSpell)
            {
                if (!spell.IsLevelMax())
                {
                    checkAllSpellLvlMax = false;
                    break;
                }
            }

            if (checkAllSpellLvlMax)
            {
                UIFrame.Instance.OpenWindow(ScreenIds.gacha_scene);
            }
            else
            {
                UIFrame.Instance.OpenWindow(ScreenIds.hero_room_scene,
                    new HeroRoomWindowProperties((int)HeroType.Jave, (int)HeroRoomTabId.Spell));
            }
        }

        public static void GotoMapHaveBoss(int bossId)
        {
            if (bossId == (int)BossId.Boss10)
            {
                if (!UserData.Instance.CampaignData.IsUnlockedStage(0, 9))
                {
                    var titleNoti = String.Format(L.popup.defeat_boss_achievement_direction, "10");
                    Ultilities.ShowToastNoti(titleNoti);
                    return;
                }
            }
            else if (bossId == (int)BossId.Boss20)
            {
                if (!UserData.Instance.CampaignData.IsUnlockedStage(0, 19))
                {
                    var titleNoti = String.Format(L.popup.defeat_boss_achievement_direction, "20");
                    Ultilities.ShowToastNoti(titleNoti);
                    return;
                }
            }


            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            if (bossId == (int)BossId.Boss10)
            {
                GamePlayControllerBase.playMode = PlayMode.ReplayCampaign;
                GamePlayControllerBase.gameMode = GameMode.CampaignMode;
                GamePlayControllerBase.CampaignId = MapCampaignInfo.GetCampaignId(0, 9, 0);
            }
            else if (bossId == (int)BossId.Boss20)
            {
                GamePlayControllerBase.playMode = PlayMode.ReplayCampaign;
                GamePlayControllerBase.gameMode = GameMode.CampaignMode;
                GamePlayControllerBase.CampaignId = MapCampaignInfo.GetCampaignId(0, 19, 0);
            }

            UIFrame.Instance.OpenWindow(ScreenIds.home);
        }

        public static void GotoUpgradeTower()
        {
            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            UIFrame.Instance.OpenWindow(ScreenIds.home);
            UIFrame.Instance.OpenWindow(ScreenIds.tower_upgrade_system);
        }

        public static void GotoRuneHeroRoom()
        {
            if (!UnlockFeatureUtilities.IsRuneAvailable())
            {
                var titleNoti = String.Format(L.popup.spell_unlock_condition,
                    UnlockFeatureUtilities.RUNE_AVAILABLE_AT_STAGE_ID + 1);
                Ultilities.ShowToastNoti(titleNoti);
                return;
            }

            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            UIFrame.Instance.OpenWindow(ScreenIds.hero_room_scene,
                new HeroRoomWindowProperties((int)HeroType.Jave, (int)HeroRoomTabId.Rune));
        }

        public static void GotoWatchVideo()
        {
            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            UIFrame.Instance.OpenWindow(ScreenIds.rewarded_ads);
        }

        public static void GotoBuyNow()
        {
            UIFrame.Instance.CloseWindow(UIFrame.Instance.GetCurrentScreenId());
            UIFrame.Instance.OpenWindow(ScreenIds.popup_buy_now);
        }

        public static void GoToGloryRoad()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.home);
            var data = new HeroAcademyWindowProperties(HeroAcademyTab.GloryRoad);
            UIFrame.Instance.OpenWindow(ScreenIds.hero_academy_event, data);
        }
    }
}