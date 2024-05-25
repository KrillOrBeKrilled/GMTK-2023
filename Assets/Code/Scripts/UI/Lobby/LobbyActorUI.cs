using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using KrillOrBeKrilled.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace KrillOrBeKrilled.UI {
  public class LobbyActorUI : MonoBehaviour {
    [SerializeField] private Vector2 _moveDelay = new(5f, 10f);
    [FormerlySerializedAs("_actorImageRectTransform")] [SerializeField] private RectTransform _actorRectTransform;
    [SerializeField] private Animator _actorAnimator;
    [SerializeField] private List<RectTransform> _targets;
    [SerializeField] private float _moveSpeed = 100;

    private static readonly int IdleId = Animator.StringToHash("Idle");
    private static readonly int WalkId = Animator.StringToHash("Walk");
    
    private bool _doRoam = true;
    private bool _isMoving;

    private RectTransform _currPos;
    private RectTransform _oldPos;
    
    private void Start() {
      this._actorRectTransform.anchoredPosition = this._targets[0].anchoredPosition;
      this._actorRectTransform.sizeDelta = this._targets[0].sizeDelta;
      this._oldPos = this._targets[0];
      this._currPos = this._oldPos;
      
      this.StartCoroutine(this.Roam());
    }


    private IEnumerator Roam() {
      while (this._doRoam) {
        this.StartIdle();
        float delay = Random.Range(this._moveDelay.x, this._moveDelay.y);
        yield return new WaitForSeconds(delay);
        
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
      } while (target == this._oldPos || target == this._currPos);

      bool lookLeft = target.anchoredPosition.x < this._currPos.anchoredPosition.x;
      this._actorRectTransform.localScale = new Vector3(lookLeft ? 1 : -1, 1, 1);
      
      float distance = Vector2.Distance(target.anchoredPosition, this._currPos.anchoredPosition);
      float moveDuration = distance / this._moveSpeed; 
      
      this._oldPos = this._currPos;
      this._currPos = target;
      
      Sequence sequence = DOTween.Sequence();
      sequence.Append(this._actorRectTransform.DOAnchorPos(target.anchoredPosition, moveDuration));
      sequence.Join(this._actorRectTransform.DOSizeDelta(target.sizeDelta, moveDuration));
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