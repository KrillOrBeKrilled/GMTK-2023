using KrillOrBeKrilled.Heroes.BehaviourTree;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KrillOrBeKrilled.Heroes.AI {
    [RequireComponent(typeof(FieldOfView), typeof(Rigidbody2D), typeof(Animator))]
    public class HeroBT : BehaviourTree.BehaviourTree  {
        protected HeroSoundsController SoundsController;
        protected Tilemap GroundTilemap;
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

        internal void Initialize(HeroSoundsController soundsController, Tilemap groundTilemap) {
            SoundsController = soundsController;
            GroundTilemap = groundTilemap;
        }
    }
}
