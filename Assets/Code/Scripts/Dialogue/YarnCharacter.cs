using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//*******************************************************************************************
// YarnCharacter
//*******************************************************************************************
namespace KrillOrBeKrilled.Dialogue {
    /// <summary>
    /// Script for the 3D RPG sample project in YarnSpinner. DialogueRunner invokes
    /// <see cref="YarnCharacterView"/>, which locates the YarnCharacter that is speaking.
    /// </summary>
    /// <remarks> Put this script on your various NPC gameObjects. </remarks>
    public class YarnCharacter : MonoBehaviour {
        [Tooltip("This must match the character name used in Yarn dialogue scripts.")]
        [SerializeField] internal string characterName = "MyName";

        [Tooltip("When positioning the message bubble in world space, YarnCharacterManager adds this additional " +
                 "offset to this gameObject's position. Taller characters should use taller offsets, etc.")]
        [SerializeField] internal Vector2 messageBubbleOffset = new Vector2(0f, 3f);

        [SerializeField] internal RectTransform _rectTransform;
        public Vector2 PositionWithOffset => this._dampenedPosition + this.messageBubbleOffset;

        private readonly List<Vector2> _lastFourFramesPositions = new List<Vector2>();
        private Vector2 _dampenedPosition;
        //========================================
        // Unity Methods
        //========================================

        #region Unity Methods

        // Start is called before the first frame update, but AFTER Awake()
        // ... this is important because YarnCharacterManager.Awake() must run before YarnCharacter.Start()
        private void Start() {
            if (YarnCharacterView.instance is null) {
                Debug.LogError("YarnCharacter can't find the YarnCharacterView instance! Is the 3D Dialogue " +
                               "prefab and YarnCharacterView script in the scene?");
                return;
            }

            YarnCharacterView.instance.RegisterYarnCharacter(this);

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
            if (YarnCharacterView.instance is not null) {
                YarnCharacterView.instance.ForgetYarnCharacter(this);
            }
        }

        #endregion
    }
}
