using System;
using UnityEngine;

namespace EW2
{
    public class BlockCollider : SpineBone<Dummy>
    {
        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (CompareTag(other.tag))
            {
                throw new Exception("Why are you the same tag? " + tag);
            }

            switch (other.tag)
            {
                case TagName.Hero:
                    break;
                case TagName.Tower:
                    break;
                case TagName.Enemy:
                    break;
            }
        }
    }
}