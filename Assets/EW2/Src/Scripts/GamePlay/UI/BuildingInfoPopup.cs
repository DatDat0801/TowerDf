using UnityEngine;
using UnityEngine.UI;

namespace EW2
{
    public class BuildingInfoContent
    {
        public string title;
        public string content;
        public string damage;
        public string atkSpeed;
        public string atkType;
    }

    public class BuildingInfoPopup : MonoBehaviour
    {
        [SerializeField] private Text txtTitle;
        [SerializeField] private Text txtContent;
        [SerializeField] private Text txtDamage;
        [SerializeField] private Text txtAtkSpeed;
        [SerializeField] private Text txtAtkType;

        private const int defaultPos = 545;

        private RectTransform rect;

        private void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        public void SetInfo(BuildingInfoContent info)
        {
            txtTitle.text = info.title;

            txtContent.text = info.content;

            if (!string.IsNullOrEmpty(info.damage))
            {
                txtDamage.text = info.damage;
                
                txtDamage.gameObject.SetActive(true);
            }
            else
            {
                txtDamage.gameObject.SetActive(false);
            }
            
            if (!string.IsNullOrEmpty(info.atkSpeed))
            {
                txtAtkSpeed.text = info.atkSpeed;
                
                txtAtkSpeed.gameObject.SetActive(true);
            }
            else
            {
                txtAtkSpeed.gameObject.SetActive(false);
            }
            
            if (!string.IsNullOrEmpty(info.atkType))
            {
                txtAtkType.text = info.atkType;
                
                txtAtkType.gameObject.SetActive(true);
            }
            else
            {
                txtAtkType.gameObject.SetActive(false);
            }
            
        }

        public void UpdatePosition()
        {
            var pixelWidth = GamePlayController.Instance.GetCameraController().MyCamera.pixelWidth;
            
            var width = pixelWidth / 2;

            var rightPost = transform.parent.localPosition.x + defaultPos * pixelWidth / 1920.0f + rect.rect.width / 2;

            transform.localPosition = new Vector3(width > rightPost ? defaultPos : -defaultPos, 0, 0);
        }
    }
}