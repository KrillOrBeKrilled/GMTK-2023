using UnityEngine;

//*******************************************************************************************
// YarnCharacter
//*******************************************************************************************
namespace Dialogue {
    /// <summary>
    /// Script for the 3D RPG sample project in YarnSpinner. DialogueRunner invokes
    /// <see cref="YarnCharacterView"/>, which locates the YarnCharacter that is speaking.
    /// </summary>
    /// <remarks> Put this script on your various NPC gameObjects. </remarks>
    public class YarnCharacter : MonoBehaviour {
        [Tooltip("This must match the character name used in Yarn dialogue scripts.")]
        public string characterName = "MyName";

        [Tooltip("When positioning the message bubble in world space, YarnCharacterManager adds this additional " +
                 "offset to this gameObject's position. Taller characters should use taller offsets, etc.")]
        public Vector3 messageBubbleOffset = new Vector3(0f, 3f, 0f);

        [Tooltip("If true, then apply messageBubbleOffset relative to this transform's rotation and scale.")]
        public bool offsetUsesRotation = false;

        // bwaaaah ugly ugly ugly
        private Vector3 _prevPosition;
        private Vector3 _prevPrevPosition;
        private Vector3 _prevPrevPrevPosition;
        private Vector3 _dampenedPosition;

        /// <summary>
        /// Calculates the positioning of dialogue message bubbles with respect to the specified
        /// <see cref="messageBubbleOffset"/> in local or world space depending on
        /// <see cref="offsetUsesRotation"/>.
        /// </summary>
        /// <returns> The position for the dialogue message bubble associated with this character to
        /// be anchored to. </returns>
        public Vector3 positionWithOffset {
            get {
                if (!this.offsetUsesRotation)
                    return this._dampenedPosition + this.messageBubbleOffset;
                
                // convert offset into local space
                return this._dampenedPosition + this.transform.TransformPoint(this.messageBubbleOffset);
            }
        }

        // Start is called before the first frame update, but AFTER Awake()
        // ... this is important because YarnCharacterManager.Awake() must run before YarnCharacter.Start()
        private void Start() {
            if (YarnCharacterView.instance is null) {
                Debug.LogError("YarnCharacter can't find the YarnCharacterView instance! Is the 3D Dialogue " +
                               "prefab and YarnCharacterView script in the scene?");
                return;
            }

            YarnCharacterView.instance.RegisterYarnCharacter(this);

            var position = this.transform.position;
            this._prevPosition = position;
            this._prevPrevPosition = position;
            this._prevPrevPrevPosition = position;
        }


        private void FixedUpdate() {
            var position = this.transform.position;

            this._dampenedPosition = (position + this._prevPosition + this._prevPrevPosition + this._prevPrevPrevPosition) / 4f;

            this._prevPrevPrevPosition = this._prevPrevPosition;
            this._prevPrevPosition = this._prevPosition;
            this._prevPosition = position;
        }

        /// <summary> Unregisters this character from the YarnCharacterView. </summary>
        private void OnDestroy() {
            if (YarnCharacterView.instance is not null) {
                YarnCharacterView.instance.ForgetYarnCharacter(this);
            }
        }
    }
}
