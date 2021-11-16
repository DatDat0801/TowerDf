using System.Collections;
using System.Collections.Generic;
using Zitga.UIFramework;
#if UNITY_ANDROID
using Google.Play.Review;

#elif UNITY_IOS
using UnityEngine.iOS;
#endif

namespace EW2
{
    public class RatingController : Singleton<RatingController>
    {
        private RatingData _ratingData;
        private List<int> _mapIdTrigger = new List<int>();

#if UNITY_ANDROID
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;
#endif

        private void Awake()
        {
            GetData();
        }

        private void GetData()
        {
            this._ratingData = GameContainer.Instance.GetRatingDatabase();

            if (this._ratingData)
            {
                foreach (var mapIdTrigger in this._ratingData.mapIdTriggers)
                {
                    var mapConvert = MapCampaignInfo.GetCampaignId(0, mapIdTrigger.mapId, mapIdTrigger.difficulty);
                    this._mapIdTrigger.Add(mapConvert);
                }
            }
        }

        public void ShowPopupRating()
        {
            UIFrame.Instance.OpenWindow(ScreenIds.popup_rating);
        }

        public bool CheckCanShowRating(int mapId)
        {
            var userDataBase = UserData.Instance.AccountData;
            return !userDataBase.isCompleteRating && this._mapIdTrigger.Contains(mapId) &&
                   !userDataBase.mapCompleteShowRating.Contains(mapId);
        }

        public void HandleRatingCallback()
        {
#if UNITY_ANDROID
            StartCoroutine(RequestReviews());
#elif UNITY_IOS
            Device.RequestStoreReview();
#endif
        }

#if UNITY_ANDROID
        IEnumerator RequestReviews()
        {
            this._reviewManager = new ReviewManager();

            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                Log.Error(requestFlowOperation.Error.ToString());
                yield break;
            }

            _playReviewInfo = requestFlowOperation.GetResult();

            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                Log.Error(launchFlowOperation.Error.ToString());
                yield break;
            }
        }
#endif
    }
}