using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

//*******************************************************************************************
// YarnCharacterView
//*******************************************************************************************
namespace KrillOrBeKrilled.Dialogue {
    /// <summary>
    /// Manager singleton that repositions DialogueUI window in 3D world space, based on
    /// whoever is speaking. Put this script on the same gameObject as your DialogueUI.
    /// </summary>
    /// <remarks>
    /// Inherits from <see cref="DialogueViewBase"/> to receive data directly
    /// from <see cref="DialogueRunner"/>.
    /// </remarks>
    public class YarnCharacterView : DialogueViewBase {
        [Tooltip("Very minimal implementation of singleton manager (initialized lazily in Awake).")]
        public static YarnCharacterView instance;

        [Tooltip("List of all YarnCharacters in the scene, who register themselves in YarnCharacter.Start().")]
        [SerializeField] internal List<YarnCharacter> allCharacters = new List<YarnCharacter>();

        // This script assumes you are using a full-screen Unity UI canvas along with a full-screen game camera
        private Camera worldCamera;

        [Tooltip("Display dialogue choices for this character, and display any no-name dialogue here too.")]
        public YarnCharacter playerCharacter;
        YarnCharacter speakerCharacter;

        [SerializeField] internal Canvas canvas;
        private RectTransform _canvasRectTransform;

        [Tooltip("For best results, set the rectTransform anchors to middle-center, and make sure the " +
                 "rectTransform's pivot Y is set to 0.")]
        [SerializeField] internal RectTransform dialogueBubbleRect, optionsBubbleRect;

        private DialogueSoundsController _soundsController;

        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        private void Awake() {
            TryGetComponent(out _soundsController);

            // ... this is important because we must set the static "instance" here, before any YarnCharacter.Start() can use it
            instance = this;
            this.worldCamera = Camera.main;
            this._canvasRectTransform = this.canvas.GetComponent<RectTransform>();
        }

        private void FixedUpdate() {
            // this all in Update instead of RunLine because characters might walk around or move during the dialogue
            if (this.dialogueBubbleRect.gameObject.activeInHierarchy) {
                if (this.speakerCharacter is not null) {
                    this.PositionBubble(this.dialogueBubbleRect, this.speakerCharacter);
                } else {
                    // if no speaker defined, then display speech above playerCharacter as a default
                    this.PositionBubble(this.dialogueBubbleRect, this.playerCharacter);
                }
            }

            // put choice option UI above playerCharacter
            if (this.optionsBubbleRect.gameObject.activeInHierarchy) {
                this.PositionBubble(this.optionsBubbleRect, this.playerCharacter);
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
            if (instance.allCharacters.Contains(deletedCharacter)) {
                this.allCharacters.Remove(deletedCharacter);
            }
        }

        /// <summary>
        /// Registers a character to track dialogue lines spoken by this character if the character is not
        /// currently registered.
        /// </summary>
        /// <param name="newCharacter"> The YarnCharacter to be registered. </param>
        /// <remarks> Automatically called by YarnCharacter.Start() so that YarnCharacterView knows they exist. </remarks>
        public void RegisterYarnCharacter(YarnCharacter newCharacter) {
            if (!instance.allCharacters.Contains(newCharacter)) {
                this.allCharacters.Add(newCharacter);
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
            this.speakerCharacter = !string.IsNullOrEmpty(characterName) ? this.FindCharacter(characterName) : null;

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
                bubbleRectTransform.anchoredPosition = character._rectTransform.anchoredPosition;
                return;
            }

            bubbleRectTransform.anchoredPosition = this.WorldToAnchoredPosition(character.PositionWithOffset);
        }

        /// <summary>
        /// Simple search through <see cref="allCharacters"/> list for a matching name.
        /// </summary>
        /// <param name="searchName"> The name to identify a <see cref="YarnCharacter"/>. </param>
        /// <returns> The <see cref="YarnCharacter"/> with a name matching the provided name. </returns>
        /// <remarks> Returns <see langword="null"/> and LogWarning if no match is found. </remarks>
        private YarnCharacter FindCharacter(string searchName) {
            return this.allCharacters.FirstOrDefault(character => character.characterName == searchName);
        }

        /// <summary>
        /// Calculates where to put the dialogue bubble based on worldPos and any desired screen margins.
        /// </summary>
        /// <param name="worldPos"> The world space position to place the dialogue bubble. </param>
        /// <returns> The position to render the dialogue bubble in world space. </returns>
        private Vector2 WorldToAnchoredPosition(Vector3 worldPos) {
            Vector2 viewportPosition = this.worldCamera.WorldToViewportPoint(worldPos);
            Vector2 sizeDelta = this._canvasRectTransform.sizeDelta;
            Vector2 worldObjectScreenPosition =
                new Vector2(viewportPosition.x * sizeDelta.x - sizeDelta.x * 0.5f,
                    viewportPosition.y * sizeDelta.y - sizeDelta.y * 0.5f);

            return worldObjectScreenPosition;
        }

        #endregion
    }
}
