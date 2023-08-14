using System.Collections.Generic;
using DG.Tweening;
using Input;
using UnityEngine;
using UnityEngine.UI;

namespace Traps
{
    // Parent trap class
    public abstract class Trap : MonoBehaviour {
        [SerializeField] public int Cost;
        [SerializeField] protected List<Vector3Int> LeftGridPoints, RightGridPoints;
        [SerializeField] protected int ValidationScore;
        [SerializeField] protected Vector3 LeftSpawnOffset, RightSpawnOffset, AnimationOffset;
        [SerializeField] protected float BuildingDuration;
        [SerializeField] protected GameObject SliderBar;

        protected Vector3 SpawnPosition;
        protected Vector3Int[] TilePositions;
        private Slider _buildCompletionBar;
        private PlayerSoundsController _soundsController;
        protected float ConstructionCompletion, t;
        protected bool IsBuilding, IsReady;

        public void Update()
        {
            if (!IsReady && IsBuilding)
            {
                ConstructionCompletion = Mathf.Lerp(ConstructionCompletion, 1f, t / BuildingDuration);
                t += Time.deltaTime;

                _buildCompletionBar.DOValue(ConstructionCompletion, 0.1f);

                // Make construction animations
                BuildTrap();

                if (ConstructionCompletion >= 0.99f)
                {
                    IsReady = true;
                    IsBuilding = false;
                    _soundsController.OnStopBuild();
                    _soundsController.OnBuildComplete();
                    Destroy(_buildCompletionBar.gameObject);

                    // Play trap set up animation
                    SetUpTrap();
                }
            }
        }

        public List<Vector3Int> GetLeftGridPoints()
        {
            return LeftGridPoints;
        }

        public List<Vector3Int> GetRightGridPoints()
        {
            return RightGridPoints;
        }

        public bool IsValidScore(int score)
        {
            return score >= ValidationScore;
        }

        public void Construct(Vector3 spawnPosition, Canvas canvas, 
            Vector3Int[] tilePositions, PlayerSoundsController soundsController)
        {
            // Initialize all the bookkeeping structures we will need
            SpawnPosition = spawnPosition;
            TilePositions = tilePositions;
            _soundsController = soundsController;
            
            // Delete/invalidate all the tiles overlapping the trap
            TilemapManager.Instance.ClearLevelTiles(TilePositions);

            // Spawn a slider to indicate the progress on the build
            GameObject sliderObject = Instantiate(SliderBar, canvas.transform);
            sliderObject.transform.position = spawnPosition + AnimationOffset + Vector3.up;
            _buildCompletionBar = sliderObject.GetComponent<Slider>();

            // Trap deployment visuals
            transform.position = spawnPosition + Vector3.up * 3f;
            transform.DOMove(spawnPosition + Vector3.up * AnimationOffset.y, 0.2f);

            var sprite = GetComponent<SpriteRenderer>();
            var color = sprite.color;
            sprite.color = new Color(color.r, color.g, color.b, 0);
            sprite.DOFade(1, 0.4f);

            // Initiate the build time countdown
            ConstructionCompletion = 0;
        }

        // Adjusts the trap spawn position relative to an origin
        public abstract Vector3 GetLeftSpawnPoint(Vector3 origin);
        public abstract Vector3 GetRightSpawnPoint(Vector3 origin);

        // Animation to ready the trap for detonation
        protected abstract void SetUpTrap();
        protected abstract void DetonateTrap();
        protected abstract void OnEnteredTrap(Hero hero);
        protected abstract void OnExitedTrap(Hero hero);

        protected virtual void OnDetonateTrapAnimationCompete() {

        }

        private void BuildTrap()
        {
            // Randomly shake the trap along the x and y-axis
            Vector3 targetPosition = new Vector3(SpawnPosition.x + Random.Range(-0.5f, 0.5f),
                SpawnPosition.y + Random.Range(0.01f, 0.2f));

            // Shake out then back
            transform.DOMove(targetPosition, 0.05f);
            transform.DOMove(SpawnPosition + Vector3.up * AnimationOffset.y, 0.05f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) return;

            if (other.CompareTag("Hero"))
                this.OnEnteredTrap(other.GetComponent<Hero>());
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !IsReady)
            {
                PlayerController playerController;
                IPlayerState playerState;

                if (other.TryGetComponent(out playerController))
                {
                    playerState = playerController.GetPlayerState();
                }
                else
                {
                    playerState = other.GetComponentInParent<PlayerController>().GetPlayerState();
                }

                if (playerState is IdleState)
                {
                    IsBuilding = true;
                    _soundsController.OnStartBuild();
                }
            }

        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Player") && IsBuilding)
            {
                IsBuilding = false;
                _soundsController.OnStopBuild();
                return;
            }

            if (other.CompareTag("Hero")) {
                this.OnExitedTrap(other.GetComponent<Hero>());
            }
        }
    }
}