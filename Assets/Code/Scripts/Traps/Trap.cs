using System.Collections.Generic;
using DG.Tweening;
using Input;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Traps
{
    // Parent trap class
    public abstract class Trap : MonoBehaviour
    {
        [SerializeField] protected List<Vector3Int> LeftGridPoints, RightGridPoints;
        [SerializeField] protected int ValidationScore;
        [SerializeField] protected Vector3 LeftSpawnOffset, RightSpawnOffset;
        [SerializeField] protected float BuildingDuration;
        [SerializeField] protected GameObject SliderBar;

        private Vector3 _spawnPosition;
        private Slider _buildCompletionBar;
        private AK.Wwise.Event _startBuildEvent, _stopBuildEvent, _buildCompleteEvent;
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
                    _stopBuildEvent.Post(gameObject);
                    _buildCompleteEvent.Post(gameObject);
                    Destroy(_buildCompletionBar.gameObject);
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
            AK.Wwise.Event startBuild, AK.Wwise.Event stopBuild, AK.Wwise.Event buildComplete)
        {
            // Initialize all the bookkeeping structures we will need
            _spawnPosition = spawnPosition;
            _startBuildEvent = startBuild;
            _stopBuildEvent = stopBuild;
            _buildCompleteEvent = buildComplete;
            
            // Spawn a slider to indicate the progress on the build
            GameObject sliderObject = Instantiate(SliderBar, canvas.transform);
            sliderObject.transform.position = spawnPosition + (Vector3.up * 2f);
            _buildCompletionBar = sliderObject.GetComponent<Slider>();
            
            // Trap deployment visuals
            transform.position = spawnPosition + Vector3.up * 3f;
            transform.DOMove(spawnPosition, 0.2f);
            
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

        protected abstract void OnEnteredTrap(Hero hero);
        protected abstract void OnExitedTrap(Hero hero);

        private void BuildTrap()
        {
            // Randomly shake the trap along the x and y-axis
            Vector3 targetPosition = new Vector3(_spawnPosition.x + Random.Range(-0.5f, 0.5f), 
                _spawnPosition.y + Random.Range(0.01f, 0.2f));
            
            // Shake out then back
            transform.DOMove(targetPosition, 0.05f);
            transform.DOMove(_spawnPosition, 0.05f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) return;

        this.OnEnteredTrap(other.GetComponent<Hero>());
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !IsReady)
            {
                PlayerController playerController;
                IPlayerState playerState;
                
                if (other.TryGetComponent<PlayerController>(out playerController))
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
                    _startBuildEvent.Post(gameObject);
                }
            }
                
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Player") && IsBuilding)
            {
                IsBuilding = false;
                _stopBuildEvent.Post(gameObject);
                return;
            }
            
            this.OnExitedTrap(other.GetComponent<Hero>());
        }
    }
}
