using UnityEngine;

namespace EW2
{
    public class UnitSizeConstants
    {
        private const float Tiny = 0.8f;
        
        private const float Small = 1.2f;
        
        private const float MiniBoss = 2.2f;
        
        private const float Boss = 3.3f;

        public static float GetUnitSize(UnitSize unitSize)
        {
            switch (unitSize)
            {
                case UnitSize.Tiny:
                    return Tiny;

                case UnitSize.Hero:
                case UnitSize.Small:
                    return Small;
                
                case UnitSize.MiniBoss:
                    return MiniBoss;

                case UnitSize.Boss:
                    return Boss;

                default:
                    return 1f;
            }
        }
        
        public static Vector3 GetUnitPosEffect(UnitSize unitSize)
        {
            switch (unitSize)
            {
                case UnitSize.Tiny:
                    return new Vector3(0f,0.12f,0f );

                case UnitSize.Hero:
                case UnitSize.Small:
                    return new Vector3(0f,0.25f,0f );
                
                case UnitSize.MiniBoss:
                    return new Vector3(0f,0.53f,0f );

                case UnitSize.Boss:
                    return new Vector3(0f,1.38f,0f );

                default:
                    return Vector3.zero;
            }
        }
    }
}