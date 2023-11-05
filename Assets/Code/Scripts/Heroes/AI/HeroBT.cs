using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;

//*******************************************************************************************
// HeroBT
//*******************************************************************************************
namespace KrillOrBeKrilled.Heroes.AI {
    /// <summary>
    /// <see cref="BehaviourTree"/> subclass used to store data pertaining to all
    /// <see cref="HeroBT"/> type trees and expose initialization methods to
    /// <see cref="Hero"/> for setup on prefab instantiation.
    /// </summary>
    [RequireComponent(typeof(FieldOfView), typeof(Rigidbody2D), typeof(Animator))]
    public class HeroBT : BehaviourTree.BehaviourTree  {
        protected HeroSoundsController SoundsController;
        protected FieldOfView HeroSight;
        protected Rigidbody2D Rigidbody;
        
        protected Animator AnimController;
        protected int JumpKey = Animator.StringToHash("jump");
        protected int XSpeedKey = Animator.StringToHash("xSpeed");
        protected int YSpeedKey = Animator.StringToHash("ySpeed");
        
        private void Awake() {
            HeroSight = GetComponent<FieldOfView>();
            Rigidbody = GetComponent<Rigidbody2D>();
            AnimController = GetComponent<Animator>();
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
            SoundsController = soundsController;
        }
    }
}
