﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//*******************************************************************************************
// YarnCharacter
//*******************************************************************************************
namespace KrillOrBeKrilled.UI.Dialogue {
    /// <summary>
    /// Script for the 3D RPG sample project in YarnSpinner. DialogueRunner invokes
    /// <see cref="DialogueUI"/>, which locates the YarnCharacter that is speaking.
    /// </summary>
    /// <remarks> Put this script on your various NPC gameObjects. </remarks>
    public class YarnCharacter : MonoBehaviour {
        [Tooltip("This must match the character name used in Yarn dialogue scripts.")]
        [SerializeField] private string _characterName = "MyName";
        
        [Tooltip("Speech bubble offset from this character")]
        [SerializeField] private Vector2 _messageBubbleOffset = new(0f, 3f);

        public RectTransform RectTransform { get; private set; }
        public Vector2 PositionWithOffset => this._dampenedPosition + this._messageBubbleOffset;
        public string CharacterName => this._characterName;

        private readonly List<Vector2> _lastFourFramesPositions = new List<Vector2>();
        private Vector2 _dampenedPosition;
        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        // Start is called before the first frame update, but AFTER Awake()
        // ... this is important because YarnCharacterManager.Awake() must run before YarnCharacter.Start()
        private void Start() {
            if (DialogueUI.Instance is null) {
                Debug.LogError("DialogueUI not found");
                return;
            }

            DialogueUI.Instance.RegisterYarnCharacter(this);
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
            if (DialogueUI.Instance is not null) {
                DialogueUI.Instance.ForgetYarnCharacter(this);
            }
        }

        #endregion
    }
}
