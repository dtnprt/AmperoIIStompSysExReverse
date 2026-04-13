using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Tools;
using Pastel;
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace InputDeviceExample
{
    class Program
    {

        

        private static IInputDevice _inputDevice;
        

        static void Main(string[] args)
        {
            _inputDevice = InputDevice.GetByName("Ampero II Stomp Audio MIDI");
            _inputDevice.EventReceived += OnEventReceived;
            _inputDevice.StartEventsListening();
            Console.Write($"(i) Input device is listening for events. Press enter to exit...\n\n".Pastel("ffffff"));

            Console.Write("> ");
            string input = Console.ReadLine();
            while(input != "q") {
                Console.Write("> ");
                input = Console.ReadLine();
                switch (input)
                {
                    case "c":
                        Console.Clear();
                        break;
                    default:
                        break;
                }
            }

            (_inputDevice as IDisposable)?.Dispose();
        }

        private static void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            int payload_start = 0x24;
            int bpl = 16;
            var midiDevice = (MidiDevice)sender;
            if (e.Event.EventType == Melanchall.DryWetMidi.Core.MidiEventType.NormalSysEx)
            {

                byte[] sysex_input = ((SysExEvent)e.Event).Data;


                // Scene Information
                if (sysex_input[10] == 0x00 && sysex_input[11] == 0x00 && sysex_input[12] == 0x00 && sysex_input[13] == 0x01 && sysex_input[14] == 0x00 && sysex_input[15] == 0x00 && sysex_input[17] == 0x00)
                {
                    int scene_num = sysex_input[31] + 1;
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"Scene: {scene_num}\n");
                    Console.ResetColor();
                }

                // Patch Information (General)
                if (sysex_input[10] == 0x00 && sysex_input[11] == 0x00 && sysex_input[12] == 0x00 && sysex_input[13] == 0x00 && sysex_input[14] == 0x00 && sysex_input[15] == 0x00 && sysex_input[17] == 0x00)
                {

                    Console.Write($"(i) Found Patch Information Block #1:\n\n".Pastel("f0c674"));

                    int version = sysex_input[9]; // ???

                    Marker marker = new Marker();
                    marker.AddRange([10, 11, 12, 13, 14, 15], "f5eea9");
                    marker.AddRange([32, 33, 34, 35], "#cc6666"); // Patch Index
                   
                    marker.AddRange([100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132], "#569CD6"); // Patch Name

                    marker.AddRange([9], "#ffff00"); // version?

                    ConsoleHelper.HexDump(new Blob(sysex_input), bpl, marker, true);

                    
                    byte[] payload = sysex_input.Skip(payload_start).Take(sysex_input.Length - payload_start - 0x01).ToArray();
                    byte[] converted_payload = ConvertNibblePayload(payload);

                    Console.Write($"\n(i) Converted Payload:\n\n".Pastel("f0c674"));

                    Marker marker_payload = new Marker();
                    marker_payload.AddRange([0x0b, 0x0c], "cc6666");
                    marker_payload.AddRange(0x2c, 0x3c, "#569CD6");
                    ConsoleHelper.HexDump(new Blob(converted_payload), bpl, marker_payload, true);


                    

                    // Get 4 bytes Bank Information (33, 34, 35, 36), convert nibbelts to byets, shift and calculate bank/patch
                    const int offeset_bank_patch = 0x0b;



                    int preset_index = (converted_payload[offeset_bank_patch + 1] << 8) + offeset_bank_patch;
                    int preset_bank = preset_index / 3;
                    int preset_patch = preset_bank == 0 ? preset_index + 1 : (preset_index % preset_bank + 1);


                    // Get 32 Bytes from Offset 101, convert nibbles to bytes an get 16 chars to string
                    int offeset_preset_name = 0x2d;

                    int char_index = 0;
                    char[] preset_name_arr = new char[16];

                    for (int i = offeset_preset_name; i < (offeset_preset_name + 16); i++)
                        preset_name_arr[char_index++] = (char)converted_payload[i];
                    
                    var preset_name = new string(preset_name_arr);


                    //Console.Write($"\n(i) extracted information: ".Pastel("ffffff") + $"A{preset_bank:D2}-{preset_patch}".Pastel("#cc6666") + $" \"{preset_name.Pastel("#569CD6")}\"\n");

                }

                
                else
                {
                    Marker marker = new Marker();
                    marker.AddRange([10, 11, 12, 13, 14, 15], "f5eea9");
                    marker.AddRange([9], "#ffff00"); // version?

                    //Console.Write($"(i) Unknown Block:\n\n".Pastel("ffffff"));

                    //ConsoleHelper.HexDump(new Blob(sysex_input), bpl, marker, true);

                    byte[] payload = sysex_input.Skip(payload_start).Take(sysex_input.Length - payload_start - 0x01).ToArray();
                    byte[] convertedNibbles = ConvertNibblePayload(payload);

                    //Console.Write($"(i) Converted Payload:\n\n".Pastel("f0c674"));
                    ConsoleHelper.HexDump(new Blob(convertedNibbles), bpl, new Marker(), true);
                }

                Console.WriteLine("");
            }
        }

        public static bool Contains(Array a, object val)
        {
            return Array.IndexOf(a, val) != -1;
        }

        public static byte[] ConvertNibblePayload(byte[] payload)
        {
            if (payload.Length % 2 != 0)
                throw new Exception("Payload not dividable by 2!");

            byte[] result = new byte[payload.Length / 2];

            int counter = 0;
            for (int i = 0; i < payload.Length; i += 2)
            {
                byte byte1 = payload[i];
                byte byte2 = payload[i+1];

                result[counter++] = (byte)((byte1 << 4) + (byte2));

            }

            return result;
        }


    }
}
