using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KrillOrBeKrilled.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace KrillOrBeKrilled.UI {
  public class LobbyActorUI : MonoBehaviour {
    [SerializeField] private float _moveDelay = 5f;
    [FormerlySerializedAs("_actorImageRectTransform")] [SerializeField] private RectTransform _actorRectTransform;
    [SerializeField] private Animator _actorAnimator;
    [SerializeField] private List<RectTransform> _targets;

    private static readonly int IdleId = Animator.StringToHash("Idle");
    private static readonly int WalkId = Animator.StringToHash("Walk");

    private bool _doRoam = true;
    private bool _isMoving;
    private RectTransform _oldPos;
    
    private void Start() {
      this.StartCoroutine(this.Roam());
    }


    private IEnumerator Roam() {
      while (this._doRoam) {
        this.StartIdle();
        yield return new WaitForSeconds(this._moveDelay);
        
        this.StartWalk();
        this.MoveActor();
        yield return new WaitWhile(() => this._isMoving);
      }
    }


    private void MoveActor() {
      this._isMoving = true;
      RectTransform target;
      do {
        target = this._targets.GetRandomElement();
      } while (target == this._oldPos);

      this._oldPos = target;
      Sequence sequence = DOTween.Sequence();
      sequence.Append(this._actorRectTransform.DOAnchorPos(target.anchoredPosition, 3f));
      sequence.Join(this._actorRectTransform.DOSizeDelta(target.sizeDelta, 3f));
      sequence.SetEase(Ease.Linear);
      sequence.OnComplete(() => this._isMoving = false);
      sequence.Play();
    }
    

    private void StartIdle() {
      this._actorAnimator.ResetTrigger(LobbyActorUI.WalkId);
      this._actorAnimator.SetTrigger(LobbyActorUI.IdleId);
    }
    
    
    private void StartWalk() {
      this._actorAnimator.SetTrigger(LobbyActorUI.WalkId);
      this._actorAnimator.ResetTrigger(LobbyActorUI.IdleId);
    }
  }
}