using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//*******************************************************************************************
// YarnCharacter
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// DialogueRunner invokes
    /// <see cref="StoryUI"/>, which locates the YarnCharacter that is speaking.
    /// </summary>
    /// <remarks> Put this script on your various NPC gameObjects. </remarks>
    public class DialogueCharacter : MonoBehaviour {
        [Tooltip("This must match the character name used in Yarn dialogue scripts.")]
        [SerializeField] private string _characterName = "MyName";
        
        [Tooltip("Speech bubble offset from this character")]
        [SerializeField] private Vector2 _messageBubbleOffset = new(0f, 3f);

        public RectTransform RectTransform { get; private set; }
        public Vector2 PositionWithOffset => this._dampenedPosition + this._messageBubbleOffset;
        public string CharacterName => this._characterName;

        private readonly List<Vector2> _lastFourFramesPositions = new();
        private Vector2 _dampenedPosition;
        
        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Start() {
            if (StoryUI.Instance is null) {
                Debug.LogError("StoryUI not found");
                return;
            }

            StoryUI.Instance.RegisterCharacter(this);
            if (this.TryGetComponent(out RectTransform rectTransform))
                this.RectTransform = rectTransform;

            Vector3 position = this.transform.position;
            this._lastFourFramesPositions.Add(position);
            this._lastFourFramesPositions.Add(position);
            this._lastFourFramesPositions.Add(position);
            this._lastFourFramesPositions.Add(position);
        }
        
        private void FixedUpdate() {
            this._lastFourFramesPositions.Add(this.transform.position);
            this._lastFourFramesPositions.RemoveAt(0);
            this._dampenedPosition = this._lastFourFramesPositions.Aggregate((acc, val) => acc + val) / this._lastFourFramesPositions.Count;
        }

        /// <summary>
        /// Unregisters this character from the YarnCharacterView.
        /// </summary>
        private void OnDestroy() {
            if (StoryUI.Instance is not null) {
                StoryUI.Instance.ForgetDialogueCharacter(this);
            }
        }

        #endregion
    }
}
