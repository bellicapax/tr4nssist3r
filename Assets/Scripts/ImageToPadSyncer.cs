using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Messages;

public class ImageToPadSyncer : MonoBehaviour
{
    private const string PUSH_OUT_NAME = "MIDIOUT2 (Ableton Push) ";
    private const string PUSH_IN_NAME = "MIDIIN2 (Ableton Push) ";
    private const string ON_COLOR_CHANNEL_PREFIX = "OnColor";
    private const string OFF_COLOR_CHANNEL_PREFIX = "OffColor";
    private const string STATUS_CHANNEL_PREFIX = "Status";
    private const string OP_CODE_CHANNEL = "OpCode";
    private const int NUM_BUTTONS = 64;
    private const int NOTE_OFFSET = 36;
    private readonly byte[] SYSEX_CONSTANTS = new byte[]
    {
        240, // code for sysex?
        71, // manufacturer
        0, // device ID
        21 // product ID
    };
    private readonly byte[] LED_CONSTANTS = new byte[]
    {
        4,
        0,
        8
    };

    private IMidiOutputDevice _midiOutput;
    private IMidiInputDevice _midiInput;


    [SerializeField] private Texture2D _image;
    [SerializeField] private Button _button;
    [SerializeField] private CsoundUnity _cSound;

    // Start is called before the first frame update
    void Start()
    {
        if(_image != null && _image.height == 8 && _image.width == 8)
        {
            _button.onClick.AddListener(ShowImage);
        }
        else
        {
            UnityEngine.Debug.LogError("Image is null or not a perfect 8x8 - cannot use");
            _button.onClick.AddListener(ShowRandomMapRtMidi);
        }
        foreach (var outputDeviceInfo in MidiDeviceManager.Default.OutputDevices)
        {
            if (outputDeviceInfo.Name == PUSH_OUT_NAME)
            {

                Debug.Log("Creating device!");
                _midiOutput = outputDeviceInfo.CreateDevice();
                _midiOutput.Open();
            }
        }
        //foreach (var inputDeviceInfo in MidiDeviceManager.Default.InputDevices)
        //{
        //    //Debug.Log("In Device " + inputDeviceInfo.Name);
        //}
    }

    private void ShowImage()
    {
        var colors = _image.GetPixels();
        for (int i = 0; i < colors.Length; i++)
        {
            var msg = CreateLEDMessage((byte)i, colors[i], false);
            _midiOutput.Send(msg);
        }
    }

    private void ShowRandomMapRtMidi()
    {
        for (int i = 0; i < NUM_BUTTONS; i++)
        {
            var msg = CreateLEDMessage((byte)i, new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255)), true);
            //var msg = new NoteOnMessage(RtMidi.Core.Enums.Channel.Channel1, (RtMidi.Core.Enums.Key)(NOTE_OFFSET + i), OffColors[Random.Range(0, OffColors.Length)]);
            _midiOutput.Send(msg);
        }
    }

    private SysExMessage CreateLEDMessage(byte pad, Color color, bool isIntColor)
    {
        var bytes = new List<byte>(SYSEX_CONSTANTS);
        bytes.AddRange(LED_CONSTANTS);
        bytes.Add(pad);
        bytes.Add(0); // Another constant
        bytes.AddRange(isIntColor ? IntColorToBytes(color) : FloatColorToBytes(color));
        bytes.Add(247); // End of message
        return new SysExMessage(bytes.ToArray());
    }

    private void ClearPadLEDs()
    {
        for (int i = 0; i < NUM_BUTTONS; i++)
        {
            var msg = CreateLEDMessage((byte)i, Color.black, true);
            //var msg = new NoteOnMessage(RtMidi.Core.Enums.Channel.Channel1, (RtMidi.Core.Enums.Key)(NOTE_OFFSET + i), OffColors[Random.Range(0, OffColors.Length)]);
            _midiOutput.Send(msg);
        }
    }

    private byte[] FloatColorToBytes(Color color)
    {
        int r = Mathf.FloorToInt(color.r * 255);
        int g = Mathf.FloorToInt(color.g * 255);
        int b = Mathf.FloorToInt(color.b * 255);
        return new byte[]
        {
            (byte)(r / 16),
            (byte)(r % 16),
            (byte)(g / 16),
            (byte)(g % 16),
            (byte)(b / 16),
            (byte)(b % 16),
        };
    }

    private byte[] IntColorToBytes(Color color)
    {
        return new byte[]
        {
            (byte)(color.r / 16),
            (byte)(color.r % 16),
            (byte)(color.g / 16),
            (byte)(color.g % 16),
            (byte)(color.b / 16),
            (byte)(color.b % 16),
        };
    }

    private void OnDestroy()
    {
        if (_midiOutput != null)
        {
            ClearPadLEDs();
            _midiOutput.Close();
            _midiOutput.Dispose();
        }
        if (_midiInput != null)
        {
            _midiInput.Close();
            _midiInput.Dispose();
        }
    }
}
