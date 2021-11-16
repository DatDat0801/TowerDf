using Spine;
using UnityEngine;

namespace EW2
{
    public class Cavalry6050Spine : DummySpine
    {
        public string ready1Name, ready2Name;

        public Cavalry6050Spine(Unit owner) : base(owner)
        {
            ready1Name = "ready";

            ready2Name = "ready_2";
        }

        public void ReadyNormal()
        {
            SetAnimation(0, ready1Name, true);
        }

        public TrackEntry ReadySpecial(string nameReady)
        {
            return SetAnimation(0, nameReady, false);
        }
    }
}