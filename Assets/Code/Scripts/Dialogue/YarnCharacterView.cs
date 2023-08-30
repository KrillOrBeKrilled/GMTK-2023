using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

//*******************************************************************************************
// YarnCharacterView
//*******************************************************************************************
namespace Dialogue {
    /// <summary>
    /// Manager singleton that repositions DialogueUI window in 3D world space, based on
    /// whoever is speaking. Put this script on the same gameObject as your DialogueUI.
    /// </summary>
    /// <remarks> Inherits from <see cref="DialogueViewBase"/> to receive data directly
    /// from <see cref="DialogueRunner"/>. </remarks>
    public class YarnCharacterView : DialogueViewBase {
        // Very minimal implementation of singleton manager (initialized lazily in Awake)
        public static YarnCharacterView instance;
        
        // List of all YarnCharacters in the scene, who register themselves in YarnCharacter.Start()
        public List<YarnCharacter> allCharacters = new List<YarnCharacter>();
        
        // This script assumes you are using a full-screen Unity UI canvas along with a full-screen game camera
        private Camera worldCamera;

        [Tooltip("Display dialogue choices for this character, and display any no-name dialogue here too.")]
        public YarnCharacter playerCharacter;
        YarnCharacter speakerCharacter;

        public Canvas canvas;
        public CanvasScaler canvasScaler;

        [Tooltip("For best results, set the rectTransform anchors to middle-center, and make sure the " +
                 "rectTransform's pivot Y is set to 0.")]
        public RectTransform dialogueBubbleRect, optionsBubbleRect;

        [Tooltip("Margin is 0-1.0 (0.1 means 10% of screen space)... -1 lets dialogue bubbles appear offscreen or get cutoff.")]
        public float bubbleMargin = 0.1f;

        // ------------------ SFX --------------------
        public AK.Wwise.Event HenDialogueEvent;
        public AK.Wwise.Event BossDialogueEvent;
        public AK.Wwise.Event HeroDialogueEvent;

        private void Awake() {
            // ... this is important because we must set the static "instance" here, before any YarnCharacter.Start() can use it
            instance = this;
            this.worldCamera = Camera.main;
        }
        
        private void FixedUpdate() {
            // this all in Update instead of RunLine because characters might walk around or move during the dialogue
            if (this.dialogueBubbleRect.gameObject.activeInHierarchy) {
                if (this.speakerCharacter is not null) {
                    this.dialogueBubbleRect.anchoredPosition = this.WorldToAnchoredPosition(
                        this.dialogueBubbleRect, this.speakerCharacter.positionWithOffset, this.bubbleMargin);
                }
                else
                {   // if no speaker defined, then display speech above playerCharacter as a default
                    this.dialogueBubbleRect.anchoredPosition = this.WorldToAnchoredPosition(
                        this.dialogueBubbleRect, this.playerCharacter.positionWithOffset, this.bubbleMargin);
                }
            }

            // put choice option UI above playerCharacter
            if (this.optionsBubbleRect.gameObject.activeInHierarchy) {
                this.optionsBubbleRect.anchoredPosition = this.WorldToAnchoredPosition(
                    this.optionsBubbleRect, this.playerCharacter.positionWithOffset, this.bubbleMargin);
            }
        }

        /// <summary>
        /// Registers a character to track dialogue lines spoken by this character if the character is not
        /// currently registered.
        /// </summary>
        /// <param name="newCharacter"> The YarnCharacter to be registered. </param>
        /// <remarks> Automatically called by YarnCharacter.Start() so that YarnCharacterView knows they exist. </remarks>
        public void RegisterYarnCharacter(YarnCharacter newCharacter) {
            if (!YarnCharacterView.instance.allCharacters.Contains(newCharacter)) {
                this.allCharacters.Add(newCharacter);
            }
        }

        /// <summary>
        /// Unregisters a character to stop tracking dialogue lines spoken by this character if the
        /// character is currently registered.
        /// </summary>
        /// <param name="deletedCharacter"> The YarnCharacter to be unregistered. </param>
        /// <remarks> Automatically called by YarnCharacter.OnDestroy() to clean-up. </remarks>
        public void ForgetYarnCharacter(YarnCharacter deletedCharacter) {
            if (YarnCharacterView.instance.allCharacters.Contains(deletedCharacter)) {
                this.allCharacters.Remove(deletedCharacter);
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
            switch (characterName) {
                case "Hero":
                    this.HeroDialogueEvent.Post(this.gameObject);
                    break;
                case "Hendall":
                    this.HenDialogueEvent.Post(this.gameObject);
                    break;
                case "Dogan":
                    this.BossDialogueEvent.Post(this.gameObject);
                    break;
            }

            // IMPORTANT: we must mark this view as having finished its work, or else the DialogueRunner gets stuck forever
            onDialogueLineFinished();
        }

        /// <summary> Simple search through <see cref="allCharacters"/> list for a matching name. </summary>
        /// <param name="searchName"> The name to identify a <see cref="YarnCharacter"/>. </param>
        /// <returns> The <see cref="YarnCharacter"/> with a name matching the provided name. </returns>
        /// <remarks> Returns <see langword="null"/> and LogWarning if no match is found. </remarks>
        private YarnCharacter FindCharacter(string searchName) {
            foreach (var character in this.allCharacters) {
                if (character.characterName == searchName) {
                    return character;
                }
            }

            Debug.LogWarningFormat("YarnCharacterView couldn't find a YarnCharacter named {0}!", searchName );
            return null;
        }
        
        /// <summary>
        /// Calculates where to put the dialogue bubble based on worldPos and any desired screen margins.
        /// </summary>
        /// <param name="bubble"> The dialogue bubble to hold dialogue text. </param>
        /// <param name="worldPos"> The world space position to place the dialogue bubble. </param>
        /// <param name="constrainToViewportMargin"> Screen margins to confine the dialogue bubble. </param>
        /// <returns> The position to render the dialogue bubble in world space. </returns>
        /// <remarks> Ensure <see cref="constrainToViewportMargin"/> is between <b>0.0f-1.0f (% of screen)</b> to
        /// constrain to screen; the value of <b>-1</b> lets the bubble go off-screen. </remarks>
        private Vector2 WorldToAnchoredPosition(RectTransform bubble, Vector3 worldPos, float constrainToViewportMargin = -1f) {
            Camera canvasCamera = this.worldCamera;
            // Canvas "Overlay" mode is special case for ScreenPointToLocalPointInRectangle (see the Unity docs)
            if (this.canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                canvasCamera = null;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                bubble.parent.GetComponent<RectTransform>(), // calculate local point inside parent... NOT inside the dialogue bubble itself
                this.worldCamera.WorldToScreenPoint(worldPos),
                canvasCamera,
                out Vector2 screenPos
            );

            // to force the dialogue bubble to be fully on screen, clamp the bubble rectangle within the screen bounds
            if (constrainToViewportMargin >= 0f) {
                // because ScreenPointToLocalPointInRectangle is relative to a Unity UI RectTransform,
                // it may not necessarily match the full screen resolution (i.e. CanvasScaler)

                // it's not really in world space or screen space, it's in a RectTransform "UI space"
                // so we must manually convert our desired screen bounds to this UI space

                bool useCanvasResolution = this.canvasScaler is not null && 
                                           this.canvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ConstantPixelSize;
                Vector2 screenSize = Vector2.zero;
                screenSize.x = useCanvasResolution ? this.canvasScaler.referenceResolution.x : Screen.width;
                screenSize.y = useCanvasResolution ? this.canvasScaler.referenceResolution.y : Screen.height;

                // calculate "half" values because we are measuring margins based on the center, like a radius
                var halfBubbleWidth = bubble.rect.width / 2;
                var halfBubbleHeight = bubble.rect.height / 2;

                // to calculate margin in UI-space pixels, use a % of the smaller screen dimension
                var margin = screenSize.x < screenSize.y ? 
                    screenSize.x * constrainToViewportMargin 
                    : screenSize.y * constrainToViewportMargin;

                // finally, clamp the screenPos fully within the screen bounds, while accounting for the bubble's rectTransform anchors
                screenPos.x = Mathf.Clamp(
                    screenPos.x,
                    margin + halfBubbleWidth - bubble.anchorMin.x * screenSize.x,
                    -(margin + halfBubbleWidth) - bubble.anchorMax.x * screenSize.x + screenSize.x
                );

                screenPos.y = Mathf.Clamp(
                    screenPos.y,
                    margin + halfBubbleHeight - bubble.anchorMin.y * screenSize.y,
                    -(margin + halfBubbleHeight) - bubble.anchorMax.y * screenSize.y + screenSize.y
                );
            }

            return screenPos;
        }
    }
}
