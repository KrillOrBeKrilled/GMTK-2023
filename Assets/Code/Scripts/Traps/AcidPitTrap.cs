using System.Collections;
using Traps;
using UnityEngine;
using UnityEngine.UI;

public class AcidPitTrap : Trap {
  [SerializeField] private int _damageAmount;
  [SerializeField] private GameObject _pitAvoidanceJumpPad;

  private readonly WaitForSeconds _waitForOneSecond = new WaitForSeconds(1f);
  private Coroutine _intervalDamageCoroutine;

  // public override void Construct(Vector3 spawnPosition, Canvas canvas, 
  //   Vector3Int[] tilePositions, PlayerSoundsController soundsController)
  // {
  //   // Initialize all the bookkeeping structures we will need
  //   SpawnPosition = spawnPosition;
  //   TilePositions = tilePositions;
  //   _soundsController = soundsController;
  //           
  //   // Delete/invalidate all the tiles overlapping the trap
  //   TilemapManager.Instance.ClearLevelTiles(TilePositions);
  //
  //   // Spawn a slider to indicate the progress on the build
  //   GameObject sliderObject = Instantiate(SliderBar, canvas.transform);
  //   sliderObject.transform.position = spawnPosition + AnimationOffset + Vector3.up;
  //   _buildCompletionBar = sliderObject.GetComponent<Slider>();
  //
  //   // Trap deployment visuals
  //   transform.position = spawnPosition + Vector3.up * 3f;
  //   transform.DOMove(spawnPosition + Vector3.up * AnimationOffset.y, 0.2f);
  //
  //   var sprite = GetComponent<SpriteRenderer>();
  //   var color = sprite.color;
  //   sprite.color = new Color(color.r, color.g, color.b, 0);
  //   sprite.DOFade(1, 0.4f);
  //
  //   // Initiate the build time countdown
  //   ConstructionCompletion = 0;
  // }
  
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

  private IEnumerator DealIntervalDamage(Hero hero) {
    while (hero.Health > 0) {
      hero.TakeDamage(this._damageAmount);
      yield return this._waitForOneSecond;
    }

    this._intervalDamageCoroutine = null;
  }
}
