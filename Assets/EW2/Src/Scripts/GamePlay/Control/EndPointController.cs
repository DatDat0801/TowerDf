using Constants;
using Lean.Pool;
using UnityEngine;
using Zitga.Update;
using Zitga.ContextSystem;

namespace EW2
{
    public class EndPointController : MonoBehaviour, IUpdateSystem
    {
        public Location location;

        private WarningEndPointController warningEndPoint;

        private float timeCD;

        private Transform parentWarning;

        private Rect screenRect;

        public virtual void OnEnable()
        {
            if (Context.Current.GetService<GlobalUpdateSystem>() != null)
            {
                Context.Current.GetService<GlobalUpdateSystem>().Add(this);
            }
        }

        public virtual void OnDisable()
        {
            if (Context.Current != null)
                Context.Current.GetService<GlobalUpdateSystem>().Remove(this);
            if (warningEndPoint != null)
            {
                warningEndPoint.gameObject.SetActive(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(TagConstants.Enemy) &&
                other.gameObject.layer == LayerMask.NameToLayer(LayerConstants.EnemyBodyBox))
            {
                BodyCollider bodySpineCollider = other.GetComponent<BodyCollider>();

                if (bodySpineCollider != null)
                {
                    // need sub point before remove hero

                    var enemy = (EnemyBase)bodySpineCollider.Owner;

                    enemy.IsCompleteEndPoint = true;
                    GamePlayData.Instance.SubMoneyInGame(MoneyInGameType.LifePoint, enemy.EnemyData.life);

                    bodySpineCollider.Owner.Remove();
                    if (bodySpineCollider.Owner is EnemyBase)
                    {
                        ((EnemyBase)bodySpineCollider.Owner).CheckEnemyDie();
                    }

                    ShowWarning();

                    if (UserData.Instance.SettingData.shake)
                        Handheld.Vibrate();
                }
            }
        }

        private void ShowWarning()
        {
            timeCD = 3f;

            if (warningEndPoint)
            {
                warningEndPoint.ShowWarning();
            }
            else
            {
                if (!parentWarning)
                    parentWarning = FindObjectOfType<GamePlayWindowCommon>().transform;

                var go = ResourceUtils.GetUnitOther("warning_enemy_out",parentWarning,false);

                if (go)
                {
                    warningEndPoint = go.GetComponent<WarningEndPointController>();

                    warningEndPoint.InitWarning(this);
                }
            }
        }

        public void OnUpdate(float deltaTime)
        {
            if (timeCD > 0)
            {
                timeCD -= deltaTime;

                if (timeCD <= 0)
                {
                    if (warningEndPoint)
                        warningEndPoint.gameObject.SetActive(false);
                }
            }
        }

        public Vector3 GetPointSpawnWarning()
        {
            screenRect = new Rect(0f, 0f, Screen.width, Screen.height);

            var myCamera = GamePlayController.Instance.GetCameraController().MyCamera;

            var xMaxScreen = myCamera.ScreenToWorldPoint(new Vector2(screenRect.max.x, 0));

            var xMinScreen = myCamera.ScreenToWorldPoint(new Vector2(screenRect.min.x, 0));

            var yMaxScreen = myCamera.ScreenToWorldPoint(new Vector2(0, screenRect.max.y));

            var yMinScreen = myCamera.ScreenToWorldPoint(new Vector2(0, screenRect.min.y));

            Vector3 pos = transform.position;

            if (location == Location.Top)
            {
                return new Vector3(pos.x, yMaxScreen.y, 0);
            }
            else if (location == Location.Bottom)
            {
                return new Vector3(pos.x, yMinScreen.y, 0);
            }
            else if (location == Location.Left)
            {
                return new Vector3(xMinScreen.x, pos.y, 0);
            }
            else if (location == Location.Right)
            {
                return new Vector3(xMaxScreen.x, pos.y, 0);
            }

            return pos;
        }
    }
}