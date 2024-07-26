using System.Collections.Generic;
using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// BasicBT
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// A <see cref="HeroBT"/> subclass that performs the functionality of a basic hero
    /// Type, including nodes to handle idle, running, pit tracking, and jumping
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
            var heroTransform = this.transform;

            var jumping = new Sequence(new List<Node> {
                new LookForObstacle(),
                new ApproachTarget(heroTransform, this.Rigidbody, this.AnimController, this.MovementSpeed, this.SpeedBlendDuration),
                new DecideJumpForce(heroTransform, this.Rigidbody, this.MinJumpForce, this.MaxJumpForce),
                new Jump(this.Rigidbody, this.AnimController, this.SoundsController)
            });

            var jumpAndFall = new Selector(new List<Node> {
                new Fall(heroTransform, this.Rigidbody, this.AnimController, this.GroundToSight),
                jumping
            });

            var groundJumping = new Sequence(new List<Node> {
                new LookForGround(heroTransform, this.HeroSight, this.GroundToSight, this.ObstaclesToSight),
                jumpAndFall
            });

            var root = new Selector(new List<Node> {
                new Freeze(this.Rigidbody, this.AnimController),
                new Stun(this.AnimController),
                groundJumping,
                new Run(this.Rigidbody, this.SpriteRenderer, this.AnimController, this.MovementSpeed),
                new Idle(this.Rigidbody, this.AnimController)
            });

            root.SetData("JumpKey", this.JumpKey);
            root.SetData("XSpeedKey", this.XSpeedKey);
            root.SetData("YSpeedKey", this.YSpeedKey);
            root.SetData("StunnedKey", this.StunnedKey);
            root.SetData("IsFrozen", false);
            root.SetData("IsMoving", true);
            root.SetData("SpeedMultiplier", 1f);
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
