using System;
using System.Collections;
using System.Collections.Generic;
using Constants;
using Invoke;
using UnityEngine;
using UnityEngine.EventSystems;
using Zitga.UIFramework;

namespace EW2
{
    public enum CameraState
    {
        Free,
        Move
    }

    public class CameraController : MonoBehaviour
    {
        // private const float TopBoundDefault = 5.76f;
        //
        // private const float BottomBoundDefault = -5.76f;
        //
        // private const float LeftBoundDefault = -10.24f;
        //
        // private const float RightBoundDefault = 10.24f;

        private const float DistancePerfect = 2.3f;

        private const float Offset = 0.3f;

        private const float MaxOrtho = 5.76f;

        private Camera myCamera;

        public Camera MyCamera
        {
            get
            {
                if (myCamera == null)
                    myCamera = GetComponent<Camera>();

                return myCamera;
            }
        }

        public LayerMask terrain;

        public LayerMask rallyRange;

        private float maxTimeForTap = 1f;

        private bool isClickable = true;

        private bool canClick = true;
        private bool canActiveUI = true;

        private bool onUI;

        private float ratio;

        private float topLimit, bottomLimit, leftLimit, rightLimit;

        private Vector3 previousMousePos;

        private Vector3 mouseDownPos;

        private CameraState cameraState;

        private float maxDeltaDistance = 15f;

        private float sqrMaxDeltaDistance;

        private Rect screenRect;

        private float sizeDefault;
        private Vector3 positionDefault;

        public const float SPEED_RESET_DEFAULT = 2;

        #region Camera Event

        public delegate void CameraTargetEvent(bool success, Vector3 position);

        public event CameraTargetEvent OnTap;

        public delegate void CameraEvent();

        public event CameraEvent OnCameraMove;

        #endregion

        public void Setup(float topBound, float bottomBound, float leftBound, float rightBound)
        {
            screenRect = new Rect(0f, 0f, Screen.width, Screen.height);

            var screenRatio = (float) Screen.width / Screen.height;

            ratio = (float) Screen.height / Screen.width;

            ratio = ratio / 9f * 16f;

            maxDeltaDistance *= ratio;

            sqrMaxDeltaDistance = maxDeltaDistance * maxDeltaDistance;

            MyCamera.orthographicSize = MaxOrtho * ratio;

            var orthographicSize = MyCamera.orthographicSize;

            topLimit = topBound - orthographicSize;
            
            bottomLimit = bottomBound + orthographicSize;
            
            leftLimit = leftBound + orthographicSize * screenRatio;
            
            rightLimit = rightBound - orthographicSize * screenRatio;

            sizeDefault = MyCamera.orthographicSize;
            positionDefault = transform.position;
            Debug.LogWarning(
            $"topLimit {topLimit}, bottomLimit {bottomLimit}, leftLimit {leftLimit}, rightLimit {rightLimit}");
        }

        // Update is called once per frame
        void Update()
        {
            UpdateManually();
        }

        #region Handle action camera

        public void ResetDefault()
        {
            ExecuteFocusPoint(sizeDefault,positionDefault,SPEED_RESET_DEFAULT);
        }
        public void UpdateManually()
        {
            if (!canClick)
            {
                return;
            }
            switch (cameraState)
            {
                case CameraState.Free:
                    if (Input.GetMouseButtonDown(0))
                    {
                        PointerEventData ped = new PointerEventData(null);

                        ped.position = Input.mousePosition;

                        List<RaycastResult> results = new List<RaycastResult>();

                        UIFrame.Instance.Raycast(ped, results);
                        if (results.Count==0)
                        {
                            StartWaveButtonController.Instance.Raycast(ped, results);
                        }
                        // print( ped.position + " " + results.Count);

                        if (results.Count > 0)
                        {
                            onUI = true;
                        }
                        else
                        {
                            onUI = false;
                        }

                        previousMousePos = Input.mousePosition;

                        mouseDownPos = previousMousePos;

                        isClickable = true;

                        InvokeProxy.Iinvoke.Invoke(this, SetEnableMouse, maxTimeForTap);
                    }
                    else if (Input.GetMouseButton(0) && !onUI)
                    {
                        var deltaPos = Input.mousePosition - previousMousePos;

                        if (deltaPos.sqrMagnitude > sqrMaxDeltaDistance ||
                            !MathUtils.IsSmallerThanRange(mouseDownPos, Input.mousePosition, maxDeltaDistance * 2))
                        {
                            if (canActiveUI)
                            {
                                cameraState = CameraState.Move;
                            
                                if (OnCameraMove != null)
                                {
                                    OnCameraMove.Invoke();
                                }
                            
                                previousMousePos = MyCamera.ScreenToWorldPoint(Input.mousePosition);
                            }
                        }
                        else
                        {
                            previousMousePos = Input.mousePosition;
                        }
                    }
                    else if (Input.GetMouseButtonUp(0))
                    {
                        if (!onUI && canActiveUI)
                        {
                            if (isClickable)
                            {
                                Ray ray = MyCamera.ScreenPointToRay(Input.mousePosition);

                                var hits = Physics2D.RaycastAll(ray.origin, ray.direction);

                                if (OnTap != null)
                                {
                                    var result = CheckPointTargetSuccess(Input.mousePosition);

                                    OnTap(result.Item1, result.Item2);
                                }

                                CheckSelectUnit(ray, hits);
                            }
                        }
                    }

                    break;

                case CameraState.Move:
                    if (Input.GetMouseButton(0))
                    {
                        if(this.topLimit <= 0 && this.bottomLimit >= 0) return;

                        var dis = MyCamera.ScreenToWorldPoint(Input.mousePosition) - previousMousePos;

                        transform.position -= dis;

                        previousMousePos = MyCamera.ScreenToWorldPoint(Input.mousePosition);

                        var pos = transform.position;
                        
                        if (pos.x < leftLimit)
                            pos.x = leftLimit;
                        else if (pos.x > rightLimit)
                            pos.x = rightLimit;

                        if (pos.y < bottomLimit)
                            pos.y = bottomLimit;
                        else if (pos.y > topLimit)
                            pos.y = topLimit;
                        
                        pos.z = -10;

                        transform.position = pos;
                    }
                    else
                    {
                        cameraState = CameraState.Free;
                    }

                    break;
            }
        }

        void SetEnableMouse()
        {
            isClickable = false;
        }

        public void ExecuteFocusPoint(float sizeTarget,Vector3 positionTarget,float speed)
        {
            StartCoroutine(ChangeSizeToTarget(sizeTarget, speed));
            StartCoroutine(MoveToTarget(positionTarget, speed));
        }

        private IEnumerator ChangeSizeToTarget(float sizeTarget, float speed)
        {
            var currentSize = MyCamera.orthographicSize;
            var totalTime = 0f;
            while (true)
            {
                totalTime += speed*Time.deltaTime;
                if (totalTime>=1)
                {
                    MyCamera.orthographicSize = sizeTarget;
                   yield break; 
                }
                var calculatedSize = Mathf.Lerp(currentSize, sizeTarget, totalTime);
                MyCamera.orthographicSize = calculatedSize;
                yield return null;
            }
        }
        
        private IEnumerator MoveToTarget(Vector3 positionTarget, float speed)
        {
            var currentPosition = transform.position;
            var currentZCoordinate = currentPosition.z;
            var totalTime = 0f;
            while (true)
            {
                totalTime += speed*Time.deltaTime;
                if (totalTime>=1)
                {
                    var finalPosition = positionTarget;
                    finalPosition.z = currentZCoordinate;
                    transform.position = finalPosition;
                    yield break; 
                }
                var calculatedPosition = Vector3.Lerp(currentPosition, positionTarget, totalTime);
                calculatedPosition.z = currentZCoordinate;
                transform.position = calculatedPosition;
                yield return null;
            }
        }

        public void EnableCanClick() => canClick = true;
        public void DisableCanClick() => canClick = false;

        public void EnableCanActiveUI() => canActiveUI = true;
        public void DisableCanActiveUI() => canActiveUI = false;
        private void CheckSelectUnit(Ray ray, RaycastHit2D[] hits)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit && hit.collider)
                {
                    var go = hit.collider.gameObject;

                    if (go.CompareTag(TagConstants.TowerPoint))
                    {
                        var towerPoint = go.GetComponentInParent<TowerPointController>();

                        if (towerPoint)
                        {
                            HandlePointTowerOption(towerPoint);
                            break;
                        }
                    }
                    else if (go.CompareTag(TagConstants.Hero))
                    {
                        var selectCollider = go.GetComponent<SelectCollider>();
                        if (selectCollider != null)
                            if (selectCollider.Owner != null)
                            {
                                HeroBase control = selectCollider.Owner as HeroBase;
                                control.SelectHero();
                                break;
                            }
                    }
                    else if (go.CompareTag(TagConstants.TreasureSoldier))
                    {
                        var selectCollider = go.GetComponent<TreasureSoldierController>();
                        if (selectCollider != null)
                        {
                            selectCollider.ClaimGold();
                        }
                    }
                    else if (go.CompareTag(TagConstants.Cavalry))
                    {
                        var selectCollider = go.GetComponent<GroupCavalryController>();
                        if (selectCollider != null)
                        {
                            selectCollider.OnCavalryClick();
                        }
                    }
                }
            }
        }

        #endregion

        #region Ultilities

        public (bool, Vector3) CheckPointTargetSuccess(Vector3 position)
        {
            Vector3 pos = MyCamera.ScreenToWorldPoint(position);
            pos.z = UnitPositionUtils.ComputeZCoordinateFollowYCoordinate(pos.y);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 1000f, terrain.value);
            if (hit.collider != null)
            {
                return (true, pos);
            }

            return (false, pos);
        }

        public bool CheckPointTargetRallySuccess(Vector3 position)
        {
            RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, 1000f, rallyRange.value);

            RaycastHit2D hit1 = Physics2D.Raycast(position, Vector2.zero, 1000f, terrain.value);

            if (hit.collider != null && hit1.collider != null)
            {
                return true;
            }

            return false;
        }

        public Vector2 HandleCheckNearestPosition(Vector2 pointCheck, Vector2 pointTarget)
        {
            RaycastHit2D hit;
            Vector2 pointResult = pointTarget;
            Vector2 checkPoint;
            float journeyLength = Vector2.Distance(pointCheck, Vector2.zero);
            float distCovered = 0;
            float fracJourney = 0;

            while (distCovered < 25)
            {
                checkPoint = Vector2.Lerp(pointCheck, pointTarget, fracJourney);
                hit = Physics2D.Raycast(checkPoint, Vector2.zero, 1000f, terrain.value);
                if (hit.collider != null)
                {
                    pointResult = checkPoint;
                    break;
                }

                distCovered++;
                fracJourney += 0.05f;
            }

            return pointResult;
        }

        public Vector2 HandleCheckNearestPositionInRange(Vector2 pointCheck, Vector2 posRange)
        {
            RaycastHit2D hit, hit1;
            Vector2 pointResult = posRange;
            Vector2 checkPoint;
            float journeyLength = Vector2.Distance(pointCheck, Vector2.zero);
            float distCovered = 0;
            float fracJourney = 0;

            while (distCovered < 30)
            {
                checkPoint = Vector2.Lerp(pointCheck, posRange, fracJourney);

                hit = Physics2D.Raycast(checkPoint, Vector2.zero, 200f, rallyRange.value);

                hit1 = Physics2D.Raycast(checkPoint, Vector2.zero, 1000f, terrain.value);

                if (hit.collider != null && hit1.collider != null)
                {
                    pointResult = checkPoint;
                    break;
                }

                distCovered++;
                fracJourney += 0.05f;
            }

            return pointResult;
        }

        private void HandlePointTowerOption(TowerPointController towerPoint)
        {
            var checkPoint = towerPoint.transform.position;

            var xMaxScreen = MyCamera.ScreenToWorldPoint(new Vector2(screenRect.max.x, 0));

            xMaxScreen.y = checkPoint.y;

            var xMinScreen = MyCamera.ScreenToWorldPoint(new Vector2(screenRect.min.x, 0));

            xMinScreen.y = checkPoint.y;

            var yMaxScreen = MyCamera.ScreenToWorldPoint(new Vector2(0, screenRect.max.y));

            yMaxScreen.x = checkPoint.x;

            var yMinScreen = MyCamera.ScreenToWorldPoint(new Vector2(0, screenRect.min.y));

            yMinScreen.x = checkPoint.x;

            var disLeft = Vector2.Distance(checkPoint, xMinScreen);

            var disRight = Vector2.Distance(checkPoint, xMaxScreen);

            var disTop = Vector2.Distance(checkPoint, yMaxScreen);

            var disDown = Vector2.Distance(checkPoint, yMinScreen);

            var posCamera = transform.position;

            if (disLeft < DistancePerfect)
            {
                posCamera.x = Mathf.Clamp(posCamera.x - (DistancePerfect - disLeft) * 1.8f + Offset, leftLimit,
                    rightLimit);
            }
            else if (disRight < DistancePerfect)
            {
                posCamera.x = Mathf.Clamp(posCamera.x + (DistancePerfect - disRight) + Offset, leftLimit, rightLimit);
            }

            if (disTop < DistancePerfect)
            {
                posCamera.y = Mathf.Clamp(posCamera.y + (DistancePerfect - disTop) + Offset, bottomLimit, topLimit);
            }
            else if (disDown < DistancePerfect)
            {
                posCamera.y = Mathf.Clamp(posCamera.y - (DistancePerfect - disDown) + Offset, bottomLimit, topLimit);
            }

            transform.position = posCamera;

          //  InvokeProxy.Iinvoke.Invoke(this, () => { towerPoint.OnPressed(); }, 0.1f);
            StartCoroutine(PressTowerPoint(towerPoint));
        }


        private IEnumerator PressTowerPoint(TowerPointController towerPoint)
        {
            yield return new WaitForSecondsRealtime(0.02f);
            towerPoint.OnPressed();
        }

        #endregion
    }
}