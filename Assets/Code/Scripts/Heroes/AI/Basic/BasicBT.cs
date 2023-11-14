using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// BasicBT
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A <see cref="HeroBT"/> subclass that performs the functionality of a basic hero
    /// type, including nodes to handle idle, running, pit tracking, and jumping
    /// behaviours.
    /// </summary>
    public class BasicBT : HeroBT  {

        // --------------- Hero Sight ----------------
        [Header("Eyesight")]
        [SerializeField] internal LayerMask ObstaclesToSight;
        [SerializeField] internal LayerMask GroundToSight;

        // ---------------- Movement -----------------
        [Header("Movement")]
        [Range(0, 7)]
        [SerializeField] internal float MovementSpeed = 4f;
        [SerializeField] internal float SpeedBlendDuration = 0.5f;
        
        // ---------------- Jumping ------------------
        [Header("Jumps")]
        [SerializeField] internal float MinJumpForce = 0f;
        [SerializeField] internal float MaxJumpForce = 80f;

        protected override Node SetupTree() {
            var heroTransform = transform;
            
            var jumping = new Sequence(new List<Node> {
                new LookForObstacle(),
                new ApproachTarget(heroTransform, Rigidbody, AnimController, MovementSpeed, SpeedBlendDuration),
                new DecideJumpForce(heroTransform, Rigidbody, MinJumpForce, MaxJumpForce),
                new Jump(Rigidbody, AnimController, SoundsController)
            });

            var jumpAndFall = new Selector(new List<Node> {
                new Fall(heroTransform, Rigidbody, AnimController, GroundToSight),
                jumping
            });
                
            var groundJumping = new Sequence(new List<Node> {
                new LookForGround(heroTransform, HeroSight, GroundToSight, ObstaclesToSight),
                jumpAndFall
            });
            
            var root = new Selector(new List<Node> {
                new Stun(),
                groundJumping,
                new Run(Rigidbody, AnimController, MovementSpeed),
                new Idle(Rigidbody, AnimController)
            });
            
            root.SetData("JumpKey", JumpKey);
            root.SetData("XSpeedKey", XSpeedKey);
            root.SetData("YSpeedKey", YSpeedKey);
            root.SetData("IsMoving", true);
            root.SetData("IsStunned", false);
            root.SetData("StunDuration", 0f);
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
