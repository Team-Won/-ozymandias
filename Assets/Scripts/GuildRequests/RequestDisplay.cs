using System;
using DG.Tweening;
using GuildRequests.Templates;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace GuildRequests
{
    public class RequestDisplay : UiUpdater
    {
        [Serializable]
        private struct Display {
            public Slider slider;
            public TextMeshProUGUI description, count;
        }

        [SerializeField] private GameObject notification;
        private Display _bookDisplay;
        private Display _notificationDisplay;
        private CanvasGroup _notificationCanvasGroup;
        private int _oldCompleted;
        
        public Request Request { get; set; }

        private void Start()
        {
            _bookDisplay = new Display
            {
                slider = GetComponentInChildren<Slider>(),
                description = transform.Find("Description").GetComponent<TextMeshProUGUI>(),
                count = transform.Find("Count").GetComponent<TextMeshProUGUI>(),
            };
            
            _notificationDisplay = new Display
            {
                slider = notification.GetComponentInChildren<Slider>(),
                description = notification.transform.Find("Description").GetComponent<TextMeshProUGUI>(),
                count = notification.transform.Find("Count").GetComponent<TextMeshProUGUI>(),
            };
            
            _notificationCanvasGroup = notification.GetComponent<CanvasGroup>();
        }
        
        protected override void UpdateUi()
        {
            if (Request == null)
            {
                _bookDisplay.description.text = "Nothings here yet!";
                _bookDisplay.count.text = "";
                _bookDisplay.slider.gameObject.SetActive(false);
            }
            else
            {
                _bookDisplay.description.text = Request.Description;
                _bookDisplay.count.text = Request.completed + "/" + Request.required;
                _bookDisplay.slider.gameObject.SetActive(true);
                _bookDisplay.slider.value = (float)Request.completed / Request.required;

                if (Request.completed == 0) {
                    _oldCompleted = 0; // Reset for new requests
                    _notificationDisplay.slider.value = 0;
                }
                
                if (Request.completed <= _oldCompleted) return;
                _oldCompleted = Request.completed;
                
                // Cancel any current tweens in case updates trigger twice in quick succession
                DOTween.Kill(_notificationDisplay);
                DOTween.Kill(_notificationCanvasGroup);
                
                notification.SetActive(true);
                _notificationCanvasGroup.alpha = 1;

                _notificationDisplay.description.text = Request.Description;
                _notificationDisplay.count.text = Request.completed + "/" + Request.required;

                _notificationDisplay.slider
                    .DOValue((float)Request.completed / Request.required, 0.5f)
                    .OnComplete(() =>
                    {
                        _notificationCanvasGroup.DOFade(0, 2f)
                            .OnComplete(() => notification.SetActive(false));
                    });
            }
        }
    }
}
