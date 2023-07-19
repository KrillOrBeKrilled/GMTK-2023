using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace Code.Scripts.Player.Input
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
        public string FilePath;
        private InputEventTrace.ReplayController _replayController;

        protected override void Awake()
        {
            base.Awake();
            ParseControls();
        }

        protected override void StartRecording()
        {
            Replay();
        }

        protected override void StopRecording()
        {
            // Make this blank to prevent the creation of the input recording script from the parent class
        }

        private void ParseControls()
        {
            _inputRecorder = InputEventTrace.LoadFrom("Assets\\InputRecordings\\" + FilePath + ".txt");
        }

        private void Replay()
        {
            print("Start Replay");
            _replayController = _inputRecorder.Replay();
            _replayController.PlayAllEventsAccordingToTimestamps();
        }
    }
}

