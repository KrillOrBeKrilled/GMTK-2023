using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using KrillOrBeKrilled.Heroes;
using UnityEngine;
using Yarn.Unity;

//*******************************************************************************************
// YarnCharacterView
//*******************************************************************************************
namespace KrillOrBeKrilled.UI {
    /// <summary>
    /// Manager singleton that repositions StoryUI window in 3D world space, based on
    /// whoever is speaking. Put this script on the same gameObject as your StoryUI.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="DialogueViewBase"/> to receive data directly
    /// from <see cref="DialogueRunner"/>.
    /// </remarks>
    public class StoryUI : DialogueViewBase {
        [SerializeField] private ComicsUI _comicsUI;
        [SerializeField] private List<GameObject> _hideOnComicShow;
        [SerializeField] private DialogueCharacter _playerCharacter;
        [SerializeField] private RectTransform _canvasRectTransform;
        [SerializeField] private RectTransform _safeAreaRectTransform;
        [SerializeField] private Vector2 _screenEdgeOffset = new(20, 20);
        [SerializeField] private RectTransform _dialogueBubbleRect; 
        [SerializeField] private RectTransform _optionsBubbleRect;
        [SerializeField] private DialogueSoundsController _soundsController;
        
        public static StoryUI Instance { get; private set; }
        
        private readonly List<DialogueCharacter> _allCharacters = new();
        private DialogueCharacter _speakerCharacter;
        private Camera _mainCamera;
        private bool _isShowingComics;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            if (StoryUI.Instance is not null) {
                Destroy(this.gameObject);
                return;
            }
            
            StoryUI.Instance = this;
            this._mainCamera = Camera.main;
        }

        private void FixedUpdate() {
            if (this._dialogueBubbleRect.gameObject.activeInHierarchy) {
                DialogueCharacter character =
                    this._speakerCharacter is not null ? this._speakerCharacter : this._playerCharacter;
                this.PositionBubble(this._dialogueBubbleRect, character);
            }
            
            if (this._optionsBubbleRect.gameObject.activeInHierarchy) {
                this.PositionBubble(this._optionsBubbleRect, this._playerCharacter);
            }
        }

        private void OnDestroy() {
            StoryUI.Instance = null;
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
        public void ForgetDialogueCharacter(DialogueCharacter deletedCharacter) {
            if (this._allCharacters.Contains(deletedCharacter)) {
                this._allCharacters.Remove(deletedCharacter);
            }
        }

        /// <summary>
        /// Sets a custom actor.
        /// </summary>
        /// <param name="actor"> the new Actor.</param>
        public void SetActorCharacter(DialogueCharacter actor) {
            this._playerCharacter = actor;
        }

        /// <summary>
        /// Registers a character to track dialogue lines spoken by this character if the character is not
        /// currently registered.
        /// </summary>
        /// <param name="newCharacter"> The YarnCharacter to be registered. </param>
        /// <remarks> Called by <code>YarnCharacter.Start()</code> so that YarnCharacterView knows they exist. </remarks>
        public void RegisterCharacter(DialogueCharacter newCharacter) {
            if (!this._allCharacters.Contains(newCharacter)) {
                this._allCharacters.Add(newCharacter);
            }
        }

        /// <summary>
        /// Registers the hero with the dialogue system.
        /// </summary>
        /// <param name="actor">The new <see cref="Hero"/> with the dialogue system. </param>
        public void OnHeroActorSpawn(Hero actor) {
            if (!actor.TryGetComponent(out DialogueCharacter newYarnCharacter)) {
                return;
            }
            
            this.RegisterCharacter(newYarnCharacter);
            this.SetActorCharacter(newYarnCharacter);
        }

        /// <summary>
        /// Unregisters the associated <see cref="Hero"/> from the dialogue system.
        /// </summary>
        /// <param name="actor"> The <see cref="Hero"/> actor to unregister. </param>
        public void OnHeroActorDestroy(Hero actor) {
            if (actor.TryGetComponent(out DialogueCharacter diedCharacter)) {
                StoryUI.Instance.ForgetDialogueCharacter(diedCharacter);
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

        [YarnCommand("show_comics")]
        public void ShowComics() {
            this._hideOnComicShow.ForEach((toHide) => toHide.SetActive(false));
            this.StartCoroutine(this.ComicsCoroutine());
        }
        
        public void OnComicsComplete() {
            this._isShowingComics = false;
            this._hideOnComicShow.ForEach((toHide) => toHide.SetActive(true));
        }

        #endregion

        //========================================
        // Private Methods
        //========================================

        #region Private Methods

        private IEnumerator ComicsCoroutine() {
            this._isShowingComics = true;
            this._comicsUI.ShowComics();
            yield return new WaitWhile(() => this._isShowingComics);
        }
        
        /// <summary>
        /// Positions bubble on screen near the target <see cref="DialogueCharacter"/>.
        /// Takes into consideration the Offset given in the <see cref="DialogueCharacter"/>.
        /// </summary>
        /// <param name="bubbleRectTransform"> the Dialogue speech bubble to position. </param>
        /// <param name="character"> the speaker character. </param>

        private void PositionBubble(RectTransform bubbleRectTransform, DialogueCharacter character) {
            bubbleRectTransform.pivot = new Vector2(0.5f, 0.5f);
            bubbleRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            bubbleRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            
            // If Rect transform, prioritize it
            if (character.RectTransform is not null) {
                Vector2 clampedPosition = this.ClampPositionToOnScreen(character.RectTransform.anchoredPosition);
                bubbleRectTransform.pivot = character.RectTransform.pivot;
                bubbleRectTransform.anchorMin = character.RectTransform.anchorMin;
                bubbleRectTransform.anchorMax = character.RectTransform.anchorMax;
                bubbleRectTransform.anchoredPosition = clampedPosition;
                return;
            }

            bubbleRectTransform.anchoredPosition = this.WorldToAnchoredPosition(character.PositionWithOffset);
        }

        /// <summary>
        /// Simple search through <see cref="_allCharacters"/> list for a matching name.
        /// </summary>
        /// <param name="searchName"> The name to identify a <see cref="DialogueCharacter"/>. </param>
        /// <returns> The <see cref="DialogueCharacter"/> with a name matching the provided name. </returns>
        /// <remarks> Returns <see langword="null"/> and LogWarning if no match is found. </remarks>
        private DialogueCharacter FindCharacter(string searchName) {
            return this._allCharacters.FirstOrDefault(character => character.CharacterName == searchName);
        }

        /// <summary>
        /// Calculates where to put the dialogue bubble based on worldPos and any desired screen margins.
        /// </summary>
        /// <param name="worldPos"> The world space position to place the dialogue bubble. </param>
        /// <param name="containOnScreen"> Whether to clamp the position to remain on screen. </param>
        /// <returns> The position to render the dialogue bubble in world space. </returns>
        private Vector2 WorldToAnchoredPosition(Vector3 worldPos, bool containOnScreen = true) {
            Vector2 viewportPosition = this._mainCamera.WorldToViewportPoint(worldPos);
            Vector2 sizeDelta = this._canvasRectTransform.sizeDelta;
            Vector2 worldObjectScreenPosition =
                new(viewportPosition.x * sizeDelta.x - sizeDelta.x * 0.5f,
                    viewportPosition.y * sizeDelta.y - sizeDelta.y * 0.5f);

            return containOnScreen
                       ? this.ClampPositionToOnScreen(worldObjectScreenPosition)
                       : worldObjectScreenPosition;
        }

        /// <summary>
        /// Clamps the given position to remain on screen.
        /// </summary>
        /// <param name="screenPosition"> The screen position coordinate to clamp. </param>
        /// <returns> Clamped value of the given coordinate. </returns>
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
