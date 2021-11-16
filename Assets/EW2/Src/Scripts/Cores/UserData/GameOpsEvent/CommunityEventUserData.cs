using System;
using System.Collections.Generic;

namespace EW2
{
    [Serializable]
    public class CommunityEventUserData
    {
        public List<JoinCommunityNotice> joinCommunityNotices;

        public CommunityEventUserData()
        {
            
        }

        public bool SetAsClaimedReward(CommunityChannel channel)
        {
            var index = this.joinCommunityNotices.FindIndex(o => o.channel == channel);
            if (index >= 0)
            {
                if (this.joinCommunityNotices[index].receivedReward == false)
                {
                    this.joinCommunityNotices[index] =
                        new JoinCommunityNotice() { channel = channel, receivedReward = true };
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        public bool IsReceivedReward(CommunityChannel channel)
        {
            if (this.joinCommunityNotices == null)
            {
                this.joinCommunityNotices = new List<JoinCommunityNotice>() {
                    new JoinCommunityNotice { channel = CommunityChannel.Facebook, receivedReward = false },
                    new JoinCommunityNotice { channel = CommunityChannel.Discord, receivedReward = false },
                    new JoinCommunityNotice { channel = CommunityChannel.FacebookGroup, receivedReward = false }
                };
            }
            if (channel == CommunityChannel.All)
            {
                for (var i = 0; i < this.joinCommunityNotices.Count; i++)
                {
                    if (this.joinCommunityNotices[i].receivedReward == false) return false;
                }
            }
            else
            {
                var index = this.joinCommunityNotices.FindIndex(o => o.channel == channel);
                if (index >= 0)
                {
                    return this.joinCommunityNotices[index].receivedReward;
                }
            }

            return true;

        }
    }

    [Serializable]
    public struct JoinCommunityNotice
    {
        public CommunityChannel channel;
        public bool receivedReward;
    }

    public enum CommunityChannel
    {
        Facebook = 0,
        Discord = 1,
        FacebookGroup = 2,
        All = 3
    }
}