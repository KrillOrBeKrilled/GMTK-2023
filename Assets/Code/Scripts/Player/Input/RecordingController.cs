using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using Object = UnityEngine.Object;

namespace Player
{
    //*******************************************************************************************
    // RecordingController
    //*******************************************************************************************
    /// <summary>
    /// A type of PlayerController class that reads from files of recorded playtest sessions and
    /// recreates the scenario.
    /// </summary>
    public class RecordingController : PlayerController
    {
        public Object RecordingFile;
        [SerializeField] private string _filePath;

        private InputEventTrace.ReplayController _replayController;

        protected override void Awake()
        {
            base.Awake();
            this.ParseControls();
        }

        public override void StartSession()
        {
            this.StartCoroutine(this.ReplayDelay());
        }

        protected override void StopSession(string message)
        {
            // Make this blank to prevent the creation of the input recording script from the parent class
        }

        private void ParseControls()
        {
            this._inputRecorder = InputEventTrace.LoadFrom(this._filePath);

            if (this._inputRecorder.eventCount < 1)
            {
                print("File empty or parse failed!");
            }
        }

        private void Replay()
        {
            print("Start Replay");
            this._replayController = this._inputRecorder.Replay();
            this._replayController.PlayAllEventsAccordingToTimestamps();
        }

        private IEnumerator ReplayDelay()
        {
            // Slight delay to give more precision to combat a margin of error in the timing of the execution
            // of events when yarn triggers "start_level" in the GameManager.
            yield return new WaitForSeconds(0.02f);
            this.Replay();
        }
    }
}
