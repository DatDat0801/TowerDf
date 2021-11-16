using UnityEngine;

namespace EW2
{
    public class SoldierRallyController
    {
        private readonly Vector3 firstDiff = new Vector3(-0.433f, 0.25f, 0.025f);
        private readonly Vector3 secondDiff = new Vector3(0.433f, 0.25f, 0.025f);
        private readonly Vector3 thirdDiff = new Vector3(0, -0.3f, -0.03f);
        private readonly Vector3 fourthDiff = new Vector3(-0.433f, -0.8f, -0.03f);
        private readonly Vector3 fifthDiff = new Vector3(0.433f, -0.8f, -0.03f);
        public int Id { get; private set; }
        public Vector3 RallyPoint { get; private set; }

        public SoldierRallyController()
        {
        }

        public SoldierRallyController(int idSoldier)
        {
            Init(idSoldier);
        }

        public void Init(int id)
        {
            this.Id = id;
        }

        public void Rally(Vector3 point)
        {
            switch (Id)
            {
                case 0:
                    RallyPoint = point + firstDiff;
                    break;

                case 1:
                    RallyPoint = point + secondDiff;
                    break;

                case 2:
                    RallyPoint = point + thirdDiff;
                    break;
                case 3:
                    RallyPoint = point + fourthDiff;
                    break;
                case 4:
                    RallyPoint = point + fifthDiff;
                    break;
                default:
                    RallyPoint = point;
                    break;
            }
        }

        public void Upgrade(Vector3 point)
        {
            RallyPoint = point;
        }
    }
}