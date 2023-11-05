using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

namespace KrillOrBeKrilled.Heroes.AI {
    public class BasicBT : HeroBT  {
        [Header("Debug")]
        public bool Debug = false;
        
        // ------------ External Systems -------------
        [Header("External Systems")]

        // --------------- Hero Sight ----------------
        [Header("Eyesight")]
        [SerializeField] internal LayerMask ObstaclesToSight;
        [SerializeField] internal LayerMask GroundToSight;

        // ---------------- Movement -----------------
        [Header("Movement")]
        [SerializeField] internal float MovementSpeed = 4f;
        [SerializeField] internal float DashSpeed = 6f;
        
        // Examples
        // - 0.2 is 20% speed reduction
        // - 0.7 is 70% speed reduction
        [Tooltip("Clamped between [0,1] as a speed reduction percentage.")]
        private float _speedPenalty = 0f;
        
        // ---------------- Jumping ------------------
        [Header("Jumps")]
        [SerializeField] internal float MinJumpForce = 0f;
        [SerializeField] internal float MaxJumpForce = 80f;
        [Range(1, 89)]
        [SerializeField] internal float JumpAngle = 88f;

        protected override Node SetupTree() {
            var heroTransform = transform;
            
            var jumping = new Sequence(new List<Node> {
                new LookForObstacle(HeroSight, ObstaclesToSight),
                new ApproachTarget(heroTransform, Rigidbody, AnimController, DashSpeed),
                new DecideJumpForce(heroTransform, MinJumpForce, MaxJumpForce, JumpAngle, DashSpeed),
                new Jump(Rigidbody, AnimController, SoundsController)
            });

            var jumpAndFall = new Selector(new List<Node> {
                new Fall(heroTransform, Rigidbody, GroundToSight),
                jumping
            });
                
            var groundJumping = new Sequence(new List<Node> {
                new LookForGround(heroTransform, HeroSight, GroundToSight, ObstaclesToSight),
                jumpAndFall
            });
            
            var root = new Selector(new List<Node> {
                groundJumping,
                new Run(Rigidbody, AnimController, MovementSpeed),
                new Idle(Rigidbody, AnimController)
            });
            
            root.SetData("JumpKey", JumpKey);
            root.SetData("XSpeedKey", XSpeedKey);
            root.SetData("YSpeedKey", YSpeedKey);
            root.SetData("IsMoving", true);
            root.SetData("SpeedPenalty", 0f);

            jumping.SetData("JumpLaunchPoint", Vector3.zero);
            jumping.SetData("JumpVelocity", Vector3.zero);
            
            jumpAndFall.SetData("JumpLandPoint", Vector3.zero);
            
            groundJumping.SetData("PitList", new List<(Vector3, Vector3)>());
            groundJumping.SetData("IsFalling", false);

            return root;
        }
    }
}
