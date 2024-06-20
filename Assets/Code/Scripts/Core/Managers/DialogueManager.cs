using System.Linq;
using KrillOrBeKrilled.Common;
using UnityEngine;
using Yarn.Unity;

namespace KrillOrBeKrilled.Core.Managers {
  public class DialogueManager : MonoBehaviour {
    [SerializeField] private DialogueRunner _dialogueRunner;
    [SerializeField] private GameEvent _onDialogueStart;
    [SerializeField] private GameEvent _onDialogueOver;

    private void OnEnable() {
      this._dialogueRunner.onDialogueComplete.AddListener(this.TriggerDialogueOver);
    }
    
    private void OnDisable() {
      this._dialogueRunner.onDialogueComplete.RemoveListener(this.TriggerDialogueOver);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dialogueName"></param>
    public void StartDialogue(string dialogueName) {
      if (!this._dialogueRunner.yarnProject.NodeNames.Contains(dialogueName)) {
        Debug.LogError("Missing or Incorrect Dialogue Name, make sure provided dialogue name value is correct");
        return;
      }
      
      this._onDialogueStart.Raise();
      this._dialogueRunner.StartDialogue(dialogueName);
    }

    /// <summary>
    /// Aborts the dialogue playback if the dialogue is actively running and starts the level.
    /// </summary>
    public void StopDialogue() {
      if (this._dialogueRunner.IsDialogueRunning) {
        this._dialogueRunner.Stop();
      }
    }
    
    /// <summary>
    /// 
    /// </summary>
    private void TriggerDialogueOver() {
      this._onDialogueOver.Raise();  
    }
  }
}