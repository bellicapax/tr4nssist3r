using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Messages;

public class ShowMap : MonoBehaviour
{
    private const string PUSH_OUT_NAME = "MIDIOUT2 (Ableton Push) ";
    private const string PUSH_IN_NAME = "MIDIIN2 (Ableton Push) ";
    private const string ON_COLOR_CHANNEL_PREFIX = "OnColor";
    private const string OFF_COLOR_CHANNEL_PREFIX = "OffColor";
    private const string STATUS_CHANNEL_PREFIX = "Status";
    private const string OP_CODE_CHANNEL = "OpCode";
    private const int NUM_BUTTONS = 64;
    private const int NOTE_OFFSET = 36;

    private IMidiOutputDevice _midiOutput;
    private IMidiInputDevice _midiInput;

    private readonly int[] OffColors = new int[]
        {
            1, 7, 10, 15, 23, 35, 47, 51, 55
        };


    [SerializeField] private Button _button;
    [SerializeField] private CsoundUnity _cSound;

    // Start is called before the first frame update
    void Start()
    {
        _button.onClick.AddListener(ShowRandomMapRtMidi);
        foreach (var outputDeviceInfo in MidiDeviceManager.Default.OutputDevices)
        {
            Debug.Log("Out Device " + outputDeviceInfo.Name);
            if (outputDeviceInfo.Name == PUSH_OUT_NAME)
            {

                Debug.Log("Creating device!");
                _midiOutput = outputDeviceInfo.CreateDevice();
                _midiOutput.Open();
            }
        }
        foreach (var inputDeviceInfo in MidiDeviceManager.Default.InputDevices)
        {
            Debug.Log("In Device " + inputDeviceInfo.Name);
        }
    }

    private void ShowRandomMapRtMidi()
    {
        for (int i = 0; i < NUM_BUTTONS; i++)
        {
            var msg = new NoteOnMessage(RtMidi.Core.Enums.Channel.Channel1, (RtMidi.Core.Enums.Key)(NOTE_OFFSET + i), OffColors[Random.Range(0, OffColors.Length)]);
            _midiOutput.Send(msg);
        }
    }

    private void ShowRandomMapForCsound()
    {
        _cSound.setChannel(OP_CODE_CHANNEL, 1);
        for (int i = 0; i < NUM_BUTTONS; i++)
        {
            _cSound.setChannel(OFF_COLOR_CHANNEL_PREFIX + i, OffColors[Random.Range(0, OffColors.Length)]);
            _cSound.setChannel(STATUS_CHANNEL_PREFIX + i, 144);
        }
    }

    private void OnDestroy()
    {
        if (_midiOutput != null)
        {
            _midiOutput.Close();
            _midiOutput.Dispose();
        }
        if(_midiInput != null)
        {
            _midiInput.Close();
            _midiInput.Dispose();
        }
    }
}
