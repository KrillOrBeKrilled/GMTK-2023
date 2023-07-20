using System;
using System.Collections;
using System.IO;
using UnityEngine;
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

        public override void StartSession()
        {
            StartCoroutine(ReplayDelay());
        }

        protected override void StopSession(string message)
        {
            // Make this blank to prevent the creation of the input recording script from the parent class
        }

        private void ParseControls()
        {
            _inputRecorder = InputEventTrace.LoadFrom("Assets\\InputRecordings\\" + FilePath + ".txt");
            
            if (_inputRecorder.eventCount < 1)
            {
                print("File empty or parse failed!");
            }
        }

        private void Replay()
        {
            print("Start Replay");
            _replayController = _inputRecorder.Replay();
            _replayController.PlayAllEventsAccordingToTimestamps();
        }

        private IEnumerator ReplayDelay()
        {
            // Slight delay to give more precision to combat a margin of error in the timing of the execution
            // of events when yarn triggers "start_level" in the GameManager.
            yield return new WaitForSeconds(0.02f);
            Replay();
        }
    }
}

