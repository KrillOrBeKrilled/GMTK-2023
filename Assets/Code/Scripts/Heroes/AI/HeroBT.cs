using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// HeroBT
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// <see cref="BehaviourTree"/> subclass used to store data pertaining to all
    /// <see cref="HeroBT"/> Type trees and expose initialization methods to
    /// <see cref="Hero"/> for setup on prefab instantiation.
    /// </summary>
    [RequireComponent(typeof(FieldOfView), typeof(Rigidbody2D), typeof(Animator))]
    public class HeroBT : BehaviourTree.BehaviourTree  {
        protected HeroSoundsController SoundsController;
        protected FieldOfView HeroSight;
        protected Rigidbody2D Rigidbody;
        protected SpriteRenderer SpriteRenderer;
        
        protected Animator AnimController;
        protected readonly int JumpKey = Animator.StringToHash("jump");
        protected readonly int XSpeedKey = Animator.StringToHash("xSpeed");
        protected readonly int YSpeedKey = Animator.StringToHash("ySpeed");
        protected readonly int StunnedKey = Animator.StringToHash("is_stunned");
        
        private void Awake() {
            HeroSight = GetComponent<FieldOfView>();
            Rigidbody = GetComponent<Rigidbody2D>();
            AnimController = GetComponent<Animator>();
            SpriteRenderer = this.GetComponent<SpriteRenderer>();
        }
        
        protected override Node SetupTree() {
            return null;
        }

        /// <summary>
        /// Sets up all the required references to operate the <see cref="HeroBT"/> from <see cref="Hero"/> on
        /// instantiation.
        /// </summary>
        /// <param name="soundsController"> The controller used to execute all SFX associated with the hero. </param>
        internal void Initialize(HeroSoundsController soundsController) {
            this.SoundsController = soundsController;
        }
    }
}
