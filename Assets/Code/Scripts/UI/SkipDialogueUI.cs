using KrillOrBeKrilled.Managers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//*******************************************************************************************
// SkipDialogueUI
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Handles the skip dialogue HUD hold timer logic and associated timer completion
    /// status animations. 
    /// </summary>
    public class SkipDialogueUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        [SerializeField] private Image _completionImage;
        private float _skipStartTime = -1f;
        private float _skipHoldDuration = 1f;
        private UnityAction _onSkipComplete;
        
        //========================================
        // Unity Methods
        //========================================
        
        #region Unity Methods
        
        private void Awake() {
            if (PlayerPrefsManager.ShouldSkipDialogue()) {
                this.gameObject.SetActive(false);
                return;
            }

            this.OnHoldStopped();
        }

        private void Update() {
            if (this._skipStartTime < 0f) {
                return;
            }

            float timeElapsed = Time.time - this._skipStartTime;
            float completionPercentage = timeElapsed / this._skipHoldDuration;
            this._completionImage.fillAmount = completionPercentage;

            if (completionPercentage >= 0.99f) {
                this._onSkipComplete?.Invoke();
                this._completionImage.fillAmount = 0f;
                this.OnHoldStopped();
                this.gameObject.SetActive(false);
            }
        }
        
        #endregion
        
        //========================================
        // Public Methods
        //========================================
        
        #region Public Methods
        
        /// <summary>
        /// Sets up references and listeners to notify observers of the skip dialogue completion and track
        /// when the level starts.
        /// </summary>
        /// <param name="onStartLevel"> Tracks when the level begins. </param>
        /// <param name="onSkipComplete"> Tracks when the skip dialogue timer is completed. </param>
        public void Initialize(UnityEvent onStartLevel, UnityAction onSkipComplete) {
            this._onSkipComplete = onSkipComplete;
            onStartLevel.AddListener(this.OnStartLevel);
        }
        
        /// <summary>
        /// Begins the skip dialogue timer.
        /// </summary>
        /// <param name="eventData"> The data associated with the image touch event. </param>
        public void OnPointerDown(PointerEventData eventData) {
            this._skipStartTime = Time.time;
            this._completionImage.gameObject.SetActive(true);
        }

        /// <summary>
        /// Resets the skip dialogue timer.
        /// </summary>
        /// <param name="eventData"> The data associated with the image touch event. </param>
        public void OnPointerUp(PointerEventData eventData) {
            this.OnHoldStopped();
        }
        
        #endregion
        
        //========================================
        // Private Methods
        //========================================
        
        #region Private Methods
        
        /// <summary>
        /// Helper method for resetting the skip dialogue timer and clearing the dialogue skip completion image.
        /// </summary>
        private void OnHoldStopped() {
            this._skipStartTime = -1f;
            this._completionImage.fillAmount = 0f;
            this._completionImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// Disables this GameObject.
        /// </summary>
        /// <remarks> Subscribed to the onStartLevel event provided upon <see cref="Initialize"/>. </remarks>
        private void OnStartLevel() {
            this.gameObject.SetActive(false);
        }
        
        #endregion
    }
}
