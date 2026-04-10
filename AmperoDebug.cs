using System;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

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

            Console.WriteLine("Input device is listening for events. Press enter to exit...");
            Console.ReadLine();

            (_inputDevice as IDisposable)?.Dispose();
        }

        private static void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            var midiDevice = (MidiDevice)sender;
            if(e.Event.EventType == Melanchall.DryWetMidi.Core.MidiEventType.NormalSysEx)
            {
                //                         Bank/Patch     , Preset Name                                                                                                                                                        ...
                int[] markers = new int[] { 10, 11, 12, 13, 14, 15, 32, 33, 34, 35, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132 };
                //Console.WriteLine($"Event received from '{midiDevice.Name}' at {DateTime.Now}: {e.Event}");
                byte[] sysex_input = ((SysExEvent)e.Event).Data;

                for(int i = 0; i< sysex_input.Length; i++)
                {
                    Console.ResetColor();
                    if (Contains(markers, i))
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"{sysex_input[i]:X2} ");
                    Console.ResetColor();
                }
                Console.WriteLine();

                if (sysex_input.Length < 32)
                    return;


                if (sysex_input[10] == 0x00 && sysex_input[11] == 0x00 && sysex_input[12] == 0x00 && sysex_input[13] == 0x00 && sysex_input[14] == 0x00 && sysex_input[15] == 0x00) // Patch Information??
                {
                    // Get 4 bytes Bank Information (33, 34, 35, 36), convert nibbelts to byets, shift and calculate bank/patch
                    const int offeset_bank_patch = 32;
                    int preset_uun = sysex_input[offeset_bank_patch];
                    int preset_lun = sysex_input[offeset_bank_patch + 1];
                    int preset_uln = sysex_input[offeset_bank_patch + 2];
                    int preset_lln = sysex_input[offeset_bank_patch + 3];
                    int preset_index = (preset_lln << 8) + (preset_uun << 4) + (preset_lun);
                    int preset_bank = preset_index / 3;
                    int preset_patch = (preset_index % preset_bank + 1);

                    // Get 32 Bytes from Offset 101, convert nibbles to bytes an get 16 chars to string
                    const int offeset_preset_name = 100;
                    int char_index = 0;
                    char[] preset_name_arr = new char[16];
                    for (int i = offeset_preset_name; i < (offeset_preset_name + 32); i += 2)
                    {
                        char c = (char)((sysex_input[i] << 4) + sysex_input[i + 1]);
                        if (c == '\0')
                            break;
                        preset_name_arr[char_index++] = c;
                    }
                    var preset_name = new string(preset_name_arr);

                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"A{preset_bank:D2}-{preset_patch} \"{preset_name}\"");
                    Console.ResetColor();
                }
                Console.WriteLine("---");
            }
                
        }

        public static bool Contains(Array a, object val)
        {
            return Array.IndexOf(a, val) != -1;
        }
    }
}
