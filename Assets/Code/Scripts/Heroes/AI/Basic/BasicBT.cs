using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

namespace KrillOrBeKrilled.Heroes.AI {
    public class BasicBT : BehaviourTree.BehaviourTree  {
        [Header("Debug")]
        public bool Debug = false;
        
        // --------------- Animation -----------------
        [Header("Animation")]
        private Animator _animController;
        private int _jumpKey = Animator.StringToHash("jump");
        private int _xSpeedKey = Animator.StringToHash("xSpeed");
        private int _ySpeedKey = Animator.StringToHash("ySpeed");

        // ---------------- Movement -----------------
        [Header("Movement")]
        private Rigidbody2D _rigidbody;
        [SerializeField] internal float MovementSpeed = 4f;
        
        // Examples
        // - 0.2 is 20% speed reduction
        // - 0.7 is 70% speed reduction
        [Tooltip("Clamped between [0,1] as a speed reduction percentage.")]
        private float _speedPenalty = 0f;

        private void Awake() {
            _animController = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        protected override Node SetupTree() {
            var root = new Selector(new List<Node> {
                new Run(_rigidbody, _animController, MovementSpeed),
                new Idle(_rigidbody, _animController)
            });
            
            root.SetData("JumpKey", _jumpKey);
            root.SetData("XSpeedKey", _xSpeedKey);
            root.SetData("YSpeedKey", _ySpeedKey);
            
            root.SetData("IsMoving", true);
            root.SetData("SpeedPenalty", 0f);

            return root;
        }
    }
}
