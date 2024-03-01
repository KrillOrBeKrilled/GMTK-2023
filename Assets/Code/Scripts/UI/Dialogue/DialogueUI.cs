using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;

//*******************************************************************************************
// YarnCharacterView
//*******************************************************************************************
namespace KrillOrBeKrilled.UI.Dialogue {
    /// <summary>
    /// Manager singleton that repositions DialogueUI window in 3D world space, based on
    /// whoever is speaking. Put this script on the same gameObject as your DialogueUI.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="DialogueViewBase"/> to receive data directly
    /// from <see cref="DialogueRunner"/>.
    /// </remarks>
    public class DialogueUI : DialogueViewBase {
        [SerializeField] private YarnCharacter _playerCharacter;
        [SerializeField] private RectTransform _canvasRectTransform;
        [SerializeField] private RectTransform _safeAreaRectTransform;
        [SerializeField] private Vector2 _screenEdgeOffset = new(20, 20);
        [SerializeField] private RectTransform _dialogueBubbleRect; 
        [SerializeField] private RectTransform _optionsBubbleRect;

        public static DialogueUI Instance { get; private set; }
        
        private DialogueSoundsController _soundsController;
        private readonly List<YarnCharacter> _allCharacters = new();
        private YarnCharacter _speakerCharacter;
        private Camera _mainCamera;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            if (DialogueUI.Instance is not null) {
                Destroy(this.gameObject);
                return;
            }
            
            DialogueUI.Instance = this;
            this.TryGetComponent(out this._soundsController);
            this._mainCamera = Camera.main;
        }

        private void FixedUpdate() {
            // this all in Update instead of RunLine because characters might walk around or move during the dialogue
            if (this._dialogueBubbleRect.gameObject.activeInHierarchy) {
                YarnCharacter character =
                    this._speakerCharacter is not null ? this._speakerCharacter : this._playerCharacter;
                this.PositionBubble(this._dialogueBubbleRect, character);
            }

            // put choice option UI above playerCharacter
            if (this._optionsBubbleRect.gameObject.activeInHierarchy) {
                this.PositionBubble(this._optionsBubbleRect, this._playerCharacter);
            }
        }

        #endregion

        //========================================
        // Public Methods
        //========================================

        #region Public Methods

        /// <summary>
        /// Unregisters a character to stop tracking dialogue lines spoken by this character if the
        /// character is currently registered.
        /// </summary>
        /// <param name="deletedCharacter"> The YarnCharacter to be unregistered. </param>
        /// <remarks> Automatically called by YarnCharacter.OnDestroy() to clean-up. </remarks>
        public void ForgetYarnCharacter(YarnCharacter deletedCharacter) {
            if (this._allCharacters.Contains(deletedCharacter)) {
                this._allCharacters.Remove(deletedCharacter);
            }
        }

        public void SetActorCharacter(YarnCharacter actor) {
            this._playerCharacter = actor;
        }

        /// <summary>
        /// Registers a character to track dialogue lines spoken by this character if the character is not
        /// currently registered.
        /// </summary>
        /// <param name="newCharacter"> The YarnCharacter to be registered. </param>
        /// <remarks> Automatically called by YarnCharacter.Start() so that YarnCharacterView knows they exist. </remarks>
        public void RegisterYarnCharacter(YarnCharacter newCharacter) {
            if (!this._allCharacters.Contains(newCharacter)) {
                this._allCharacters.Add(newCharacter);
            }
        }

        /// <summary>
        /// Extends the <see cref="DialogueViewBase.RunLine">RunLine</see> method from
        /// <see cref="DialogueViewBase"/> to play SFX associated with the in-game characters
        /// speaking their lines.
        /// </summary>
        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished) {
            // Try and get the character name from the line
            string characterName = dialogueLine.CharacterName;

            // if null, Update() will use the playerCharacter instead
            this._speakerCharacter = !string.IsNullOrEmpty(characterName) ? this.FindCharacter(characterName) : null;

            // Run Voice Events
            switch(characterName) {
                case "Hendall":
                    this._soundsController.OnHenSpeak();
                    break;
                case "Hero":
                    this._soundsController.OnHeroSpeak();
                    break;
                case "Dogan":
                    this._soundsController.OnBossSpeak();
                    break;
            }

            // IMPORTANT: we must mark this view as having finished its work, or else the DialogueRunner gets stuck forever
            onDialogueLineFinished();
        }

        #endregion

        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        private void PositionBubble(RectTransform bubbleRectTransform, YarnCharacter character) {
            // If Rect transform, prioritize it
            if (character._rectTransform != null) {
                Vector2 clampedPosition = this.ClampPositionToOnScreen(character._rectTransform.anchoredPosition);
                bubbleRectTransform.anchoredPosition = clampedPosition;
                return;
            }

            bubbleRectTransform.anchoredPosition = this.WorldToAnchoredPosition(character.PositionWithOffset);
        }

        /// <summary>
        /// Simple search through <see cref="_allCharacters"/> list for a matching name.
        /// </summary>
        /// <param name="searchName"> The name to identify a <see cref="YarnCharacter"/>. </param>
        /// <returns> The <see cref="YarnCharacter"/> with a name matching the provided name. </returns>
        /// <remarks> Returns <see langword="null"/> and LogWarning if no match is found. </remarks>
        private YarnCharacter FindCharacter(string searchName) {
            return this._allCharacters.FirstOrDefault(character => character.characterName == searchName);
        }

        /// <summary>
        /// Calculates where to put the dialogue bubble based on worldPos and any desired screen margins.
        /// </summary>
        /// <param name="worldPos"> The world space position to place the dialogue bubble. </param>
        /// <param name="containOnScreen"></param>
        /// <returns> The position to render the dialogue bubble in world space. </returns>
        private Vector2 WorldToAnchoredPosition(Vector3 worldPos, bool containOnScreen = true) {
            Vector2 viewportPosition = this._mainCamera.WorldToViewportPoint(worldPos);
            Vector2 sizeDelta = this._canvasRectTransform.sizeDelta;
            Vector2 worldObjectScreenPosition =
                new(viewportPosition.x * sizeDelta.x - sizeDelta.x * 0.5f,
                    viewportPosition.y * sizeDelta.y - sizeDelta.y * 0.5f);

            if (!containOnScreen)
                return worldObjectScreenPosition;

            return this.ClampPositionToOnScreen(worldObjectScreenPosition);
        }

        private Vector2 ClampPositionToOnScreen(Vector2 screenPosition) {
            Vector2 canvasSize = this._safeAreaRectTransform.rect.size;
            float canvasHalfWidth = canvasSize.x / 2;
            float canvasHalfHeight = canvasSize.y / 2;

            Rect bubbleRect = this._dialogueBubbleRect.rect;
            float bubbleHalfWidth = bubbleRect.width / 2;
            float bubbleHeight = bubbleRect.height;
            float minX = -canvasHalfWidth + bubbleHalfWidth + this._screenEdgeOffset.x;
            float maxX = -minX;
            float minY = -canvasHalfHeight + bubbleHeight + this._screenEdgeOffset.y;
            float maxY = canvasHalfHeight - this._screenEdgeOffset.y;

            screenPosition.x = Mathf.Clamp(screenPosition.x, minX, maxX);
            screenPosition.y = Mathf.Clamp(screenPosition.y, minY, maxY);
            return screenPosition;
        }

        #endregion
    }
}
