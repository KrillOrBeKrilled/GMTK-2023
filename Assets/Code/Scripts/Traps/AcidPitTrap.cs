using System.Collections;
using Traps;
using UnityEngine;
using UnityEngine.UI;

public class AcidPitTrap : InGroundTrap {
  [SerializeField] private MeshRenderer _acidLiquid;
  [SerializeField] private SpriteRenderer _heatEmanating;
  [SerializeField] private GameObject _bubbles;
  private Vector3 _bubblesStartPos;
  [SerializeField] private int _damageAmount;
  [SerializeField] private GameObject _pitAvoidanceJumpPad;
  
  private readonly WaitForSeconds _waitForOneSecond = new WaitForSeconds(1f);
  private Coroutine _intervalDamageCoroutine;

  public override void Construct(Vector3 spawnPosition, Canvas canvas, 
    Vector3Int[] tilePositions, PlayerSoundsController soundsController)
  {
    // Initialize all the bookkeeping structures we will need
    SpawnPosition = spawnPosition;
    TilePositions = tilePositions;
    SoundsController = soundsController;
            
    // Delete/invalidate all the tiles overlapping the trap
    TilemapManager.Instance.ClearLevelTiles(TilePositions);
  
    // Spawn a slider to indicate the progress on the build
    GameObject sliderObject = Instantiate(SliderBar, canvas.transform);
    sliderObject.transform.position = spawnPosition + AnimationOffset + Vector3.up;
    BuildCompletionBar = sliderObject.GetComponent<Slider>();
  
    // Trap deployment visuals
    transform.position = spawnPosition;
    _bubblesStartPos = _bubbles.transform.position;
    
    // Set the acid liquid level and heat haze intensity to 0
    _acidLiquid.material.SetFloat("_Depth", 0);
    _heatEmanating.material.SetFloat("_HazeRange", 0);
    _bubbles.SetActive(false);

    // Initiate the build time countdown
    ConstructionCompletion = 0;
  }
  
  public override Vector3 GetLeftSpawnPoint(Vector3 origin)
  {
    return origin + LeftSpawnOffset;
  }

  public override Vector3 GetRightSpawnPoint(Vector3 origin)
  {
    return origin + RightSpawnOffset;
  }
  
  protected override void SetUpTrap()
  {
      _pitAvoidanceJumpPad.SetActive(false);
  }
    
  protected override void DetonateTrap()
  {

  }

  protected override void OnEnteredTrap(Hero hero) {
    if (!IsReady) return;

    hero.HeroMovement.SetSpeedPenalty(0.8f);

    if (this._intervalDamageCoroutine != null) {
      this.StopCoroutine(this._intervalDamageCoroutine);
    }
    this._intervalDamageCoroutine = this.StartCoroutine(this.DealIntervalDamage(hero));
  }

  protected override void OnExitedTrap(Hero hero) {
    if (!IsReady) return;

    hero.HeroMovement.ResetSpeedPenalty();

    if (this._intervalDamageCoroutine != null)
      this.StopCoroutine(this._intervalDamageCoroutine);
  }

  // Animation to fill the pit with acid and heat haze
  protected override void BuildTrap()
  {
    // Clamp the acid depth to prevent the acid from looking strange around tile edges
    var targetDepth = Mathf.Clamp(ConstructionCompletion, 0, 0.8f);
    
    // Magic ratio to avoid making the haze too intense
    var targetHeatHazeRange = Mathf.Clamp(targetDepth / 3.8f, 0, 1);
    
    _acidLiquid.material.SetFloat("_Depth", targetDepth);
    _heatEmanating.material.SetFloat("_HazeRange", targetHeatHazeRange);

    if (!_bubbles.activeSelf && targetDepth > 0.05f)
    {
      _bubbles.SetActive(true);
    }

    _bubbles.transform.position = new Vector3(_bubblesStartPos.x, _bubblesStartPos.y + targetDepth * 1.8f,
                                              _bubblesStartPos.z);
  }

  private IEnumerator DealIntervalDamage(Hero hero) {
    while (hero.Health > 0) {
      hero.TakeDamage(this._damageAmount);
      yield return this._waitForOneSecond;
    }

    this._intervalDamageCoroutine = null;
  }
}
