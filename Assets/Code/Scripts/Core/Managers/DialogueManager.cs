using KrillOrBeKrilled.Core.Cameras;
using KrillOrBeKrilled.Model;
using UnityEngine;
using Yarn.Unity;

namespace KrillOrBeKrilled.Core.Managers {
  public class DialogueManager : MonoBehaviour {
    [Header("Dialogue References")]
    [SerializeField] private DialogueRunner _dialogueRunner;
    
    // /// <summary>
    // /// Triggers the sequence to make the hero enter the level.
    // /// </summary>
    // /// <remarks> Can be accessed as a YarnCommand. </remarks>
    // [YarnCommand("enter_hero_actor")]
    // public void EnterHero() {
    //   this._heroActor.EnterLevel();
    // }
    //
    // /// <summary>
    // /// Spawns a new hero from the level data at the corresponding spawn point.
    // /// </summary>
    // /// <remarks> Can be accessed as a YarnCommand. </remarks>
    // [YarnCommand("spawn_hero_actor")]
    // public void SpawnHeroActor() {
    //   // TODO: MOVE TO DIALOGUE MANAGER
    //   this._heroActor = this.WaveManager.SpawnHero(HeroData.DefaultHero, true);
    // }
  }
}