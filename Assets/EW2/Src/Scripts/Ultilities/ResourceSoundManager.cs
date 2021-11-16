using UnityEngine;

namespace EW2
{
    public static class ResourceSoundManager
    {
        public static AudioClip GetAudioMusic(string nameAudio)
        {
            string fullPath = $"Sounds/Music/{nameAudio}";

            return ResourceUtils.Get<AudioClip>(fullPath);
        }


        public static AudioClip GetSoundTower(int towerId, string nameAudio)
        {
            string fullPath = $"Sounds/GamePlay/Tower{towerId}/{nameAudio}";

            return ResourceUtils.Get<AudioClip>(fullPath);
        }

        public static string GetMusicMapByLvl(int campId)
        {
            var CampaignId = MapCampaignInfo.GetWorldMapModeId(campId);

            if (CampaignId.Item2 < 5)
            {
                return SoundConstant.MusicMapCampaign_1_5;
            }
            else if (CampaignId.Item2 >= 5 && CampaignId.Item2 <= 9)
            {
                return SoundConstant.MusicMapCampaign_6_10;
            }
            else if (CampaignId.Item2 >= 10 && CampaignId.Item2 <= 14)
            {
                return SoundConstant.MusicMapCampaign_11_15;
            }
            else
            {
                return SoundConstant.MusicMapCampaign_16;
            }
        }

        public static string GetAmbienceMusicNameByLevel(int id)
        {
            var campaignId = MapCampaignInfo.GetWorldMapModeId(id);

            if (campaignId.Item2 < 16)
            {
                return SoundConstant.AMBIENCE_1_16;
            }
            else
            {
                return SoundConstant.AMBIENCE_17_20;
            }
            
        }
    }
}