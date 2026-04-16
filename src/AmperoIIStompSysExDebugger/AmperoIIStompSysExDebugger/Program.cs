using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Tools;
using Pastel;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.ExceptionServices;

namespace InputDeviceExample
{
    class Program
    {

        

        private static IInputDevice _inputDevice;
        private static IOutputDevice _outputDevice;

        private static int _bytes_per_line = 16;

        private static bool _ready_to_print = true;

        private static ConsoleHelper.OutputMode _outputMode = ConsoleHelper.OutputMode.Hex;

        static long payload_offset_counter = 0;
        static long sysex_offset_counter = 0;

        static List<byte> payload_combined = new List<byte>();

        static void Main(string[] args)
        {

            if (!SelectInputDevice())
                return;

            SelectOutDevice();

            //_inputDevice = InputDevice.GetByName("7-Ampero II Stomp Audio MIDI");
            _inputDevice.EventReceived += OnEventReceived;
            _inputDevice.StartEventsListening();
            Console.Write($"(i) Input device is listening for events.\n\n".Pastel("ffffff"));

            string input = "";

            while(input != "q") {
                Console.Write("> ");
                input = Console.ReadLine();
                switch (input)
                {
                    case "clear":
                        Console.Clear();
                        break;
                    case "-":
                        Console.WriteLine("----------------------------------");
                        break;
                    case "hex":
                        _outputMode = ConsoleHelper.OutputMode.Hex;
                        Console.Write($"(i) set output mode to HEX\n".Pastel("ffffff"));
                        break;
                    case "bin":
                        _outputMode = ConsoleHelper.OutputMode.Bin;
                        Console.Write($"(i) set output mode to BIN\n".Pastel("ffffff"));
                        break;
                    case "dec":
                        _outputMode = ConsoleHelper.OutputMode.Dec;
                        Console.Write($"(i) set output mode to DEC\n".Pastel("ffffff"));
                        break;
                    case "char":
                        _outputMode = ConsoleHelper.OutputMode.Char;
                        Console.Write($"(i) set output mode to CHAR\n".Pastel("ffffff"));
                        break;
                    case "hybrid":
                        _outputMode = ConsoleHelper.OutputMode.Hybrid;
                        Console.Write($"(i) set output mode to HYBRID\n".Pastel("ffffff"));
                        break;

                    case "pp":
                        const int offeset_bank_patch = 0x0a;
                        const int offeset_preset_name = 0x2b;

                        Marker marker_payload = new Marker();
                        marker_payload.AddRange([offeset_bank_patch, offeset_bank_patch + 1], "ff00ff", "Patch Index");
                        marker_payload.AddRange(offeset_preset_name, offeset_preset_name + 16, "#00ffff", "Patch Name");

                        Console.Write($"\n(i) converted payload ({payload_combined.Count} bytes):\n".Pastel("f0c674"));
                        ConsoleHelper.HexDump(new Blob(payload_combined.ToArray()), _bytes_per_line, marker_payload, true, 0, false, -1, -1, _outputMode);
                        break;
                        
                    default:
                        SendMIDI(input);
                        break;
                }
            }

            (_inputDevice as IDisposable)?.Dispose();
        }

        private static bool SelectInputDevice()
        {

            var all_devices = InputDevice.GetAll().ToList();
            int number = -1;
            bool device_chosen = false;

            if (all_devices.Count > 0)
            {

                while (!device_chosen)
                {
                    Console.Write($"(?) select input device:\n".Pastel("ffffff"));
                    for (int i = 0; i < all_devices.Count; i++)
                    {
                        Console.Write($"\t {i}: {all_devices[i].Name}\n".Pastel("ffffff"));
                    }
                    Console.Write("> ");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out number)) 
                    {
                        if(number >= 0 && number < all_devices.Count) // is valid device number
                        {
                            _inputDevice = all_devices[number];
                            Console.Write($"(i) selected input device \"{all_devices[number].Name}\"\n".Pastel("ffffff"));
                            return true;
                           
                        }
                    }
                }
            }
            else
            {
                Console.Write($"(!) no input devices found ".Pastel("ff0000"));
            }
            return false;
        }

        private static bool SelectOutDevice()
        {

            var all_devices = OutputDevice.GetAll().ToList();
            int number = -1;
            bool device_chosen = false;

            if (all_devices.Count > 0)
            {

                while (!device_chosen)
                {
                    Console.Write($"(?) select output device:\n".Pastel("ffffff"));
                    for (int i = 0; i < all_devices.Count; i++)
                    {
                        Console.Write($"\t {i}: {all_devices[i].Name}\n".Pastel("ffffff"));
                    }
                    Console.Write("> ");
                    string input = Console.ReadLine();

                    if (int.TryParse(input, out number))
                    {
                        if (number >= 0 && number < all_devices.Count) // is valid device number
                        {
                            _outputDevice = all_devices[number];
                            Console.Write($"(i) selected output device \"{all_devices[number].Name}\"\n".Pastel("ffffff"));
                            return true;

                        }
                    }
                }
            }
            else
            {
                Console.Write($"(!) no output devices found ".Pastel("ff0000"));
            }
            return false;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private static void SendMIDI(string input)
        {
            var formatted_input = input.Replace(" ", "").ToLower();
            try
            {
                // SysEx
                if (formatted_input.StartsWith("f0") && formatted_input.EndsWith("f7"))
                {
                    formatted_input = formatted_input.Substring(2, formatted_input.Length - 2);
                    var bytes = StringToByteArray(formatted_input);

                    Console.Write($"(i) Sending MIDI Message: \n".Pastel("ffffff"));
                    ConsoleHelper.HexDump(new Blob(bytes), 16, new Marker(), true);
                    _outputDevice.SendEvent(new NormalSysExEvent(bytes));
                }

                

            }
            catch(Exception ex)
            {
                Console.Error.Write($"(!) somethings fucked:\n".Pastel("f0c674"));
                Console.Error.Write(ex.Message.Pastel("f0c674"));
                Console.Error.Write(ex.StackTrace.Pastel("f0c674"));
            }

        }

        private static void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            int payload_start = 0xD;

            Marker sysex_marker = new Marker();
            //sysex_marker.AddRange([0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x12], "00ff00"); // command?

            sysex_marker.AddRange([0x08], "0000ff", "Request (0x11) / Answer (0x12)");        // Request (0x11) / Answer (0x12)
            sysex_marker.AddRange([0x09,0x0a], "00ff00", "Packet Length"); // Packet Length
            sysex_marker.AddRange([0x0b, 0x0c], "ffff00", "Offest?"); // Offset?

            

            var midiDevice = (MidiDevice)sender;
            if (e.Event.EventType == Melanchall.DryWetMidi.Core.MidiEventType.NormalSysEx)
            {

                byte[] sysex_input_without_start = ((SysExEvent)e.Event).Data;

                byte[] sysex_input = new byte[sysex_input_without_start.Length + 1];
                sysex_input_without_start.CopyTo(sysex_input, 1);
                sysex_input[0] = 0xF0;


                // Scene Information
                if (ByteArrayMatch(sysex_input, [0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x12], [0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00]))
                {


                    //marker.Add(0x20, "#ff00ff"); // scene number

                    byte[] payload = sysex_input.Skip(payload_start).Take(sysex_input.Length - payload_start - 0x01).ToArray();
                    byte[] converted_payload = ConvertNibblePayload(payload);

                    Console.Write($"(i) Scene Block:\n".Pastel("ffffff"));
                    ConsoleHelper.HexDump(new Blob(sysex_input), _bytes_per_line, sysex_marker, true, 0, false, -1, -1, _outputMode);

                    Marker marker_payload = new Marker();
                    marker_payload.Add(0x07, "ff00ff");

                    Console.Write($"\n(i) converted payload ({converted_payload.Length} bytes):\n".Pastel("f0c674"));
                    ConsoleHelper.HexDump(new Blob(converted_payload), _bytes_per_line, marker_payload, true, 0, false, -1, -1, _outputMode);

                    int scene_num = sysex_input[31] + 1;

                    Console.Write($"\n(i) extracted information: Scene ".Pastel("ffffff") + $"{scene_num}".Pastel("#ff00ff") + "\n");

                }

                // Patch Information Block
                else if (
                    ByteArrayMatch(sysex_input, [0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x10, 0x12], [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00])  // First Block
                    ||
                    ByteArrayMatch(sysex_input, [0x0b, 0x0c], [0x39, 0x01]) // Second Block
                    ||
                    ByteArrayMatch(sysex_input, [0x0b, 0x0c], [0x72, 0x02]) // Third Block
                    ||
                    ByteArrayMatch(sysex_input, [0x0b, 0x0c], [0x2b, 0x04]) // fourht Block??

                    )

                {

                    byte[] payload = sysex_input.Skip(payload_start).Take(sysex_input.Length - payload_start - 0x01).ToArray();
                    byte[] converted_payload = ConvertNibblePayload(payload);

                    


                    if (sysex_input[0x0b] == 0x00) // first block
                    {
                        payload_offset_counter = 0;
                        sysex_offset_counter = 0;
                        payload_combined = new List<byte>(); // clean payload block
                    }


                    payload_combined.AddRange(converted_payload);


                    Console.Write($"(i) found patch information block ({sysex_input.Length} bytes):\n".Pastel("f0c674"));

                    Marker marker = new Marker();

                    marker.AddRange(0x21, 0x24, "#ff00ff", "Patch Index"); // Patch Index
                    marker.AddRange(0x65, 0x85, "#00ffff", "Patch Name"); // Patch Name

                    ConsoleHelper.HexDump(new Blob(sysex_offset_counter, sysex_input), _bytes_per_line, sysex_marker, true, 0, false, -1, -1, _outputMode);




                    const int offeset_bank_patch = 0x0a;
                    const int offeset_preset_name = 0x2b;

                    Marker marker_payload = new Marker();
                    marker_payload.AddRange([offeset_bank_patch, offeset_bank_patch + 1], "ff00ff", "Patch Index");
                    marker_payload.AddRange(offeset_preset_name, offeset_preset_name + 16, "#00ffff", "Patch Name");

                    Console.Write($"\n(i) converted payload ({converted_payload.Length} bytes):\n".Pastel("f0c674"));
                    ConsoleHelper.HexDump(new Blob(payload_offset_counter, converted_payload), _bytes_per_line, marker_payload, true, 0, false, -1, -1, _outputMode);



                    payload_offset_counter += converted_payload.LongLength;
                    sysex_offset_counter += sysex_input.LongLength;



                    if (sysex_input[0x0b] == 0x00) // first part of the patch info
                    {


                        int preset_index = (converted_payload[offeset_bank_patch + 1] << 8) + converted_payload[offeset_bank_patch];
                        int preset_bank = preset_index / 3;
                        int preset_patch = preset_bank == 0 ? preset_index + 1 : (preset_index % preset_bank + 1);


                        // Get 32 Bytes from Offset 101, convert nibbles to bytes an get 16 chars to string
                        

                        int char_index = 0;
                        char[] preset_name_arr = new char[16];

                        for (int i = offeset_preset_name; i < (offeset_preset_name + 16); i++)
                            preset_name_arr[char_index++] = (char)converted_payload[i];

                        var preset_name = new string(preset_name_arr);


                        Console.Write($"\n(i) extracted information: Patch ".Pastel("ffffff") + $"A{preset_bank:D2}-{preset_patch}".Pastel("#cc6666") + $", preset name: \"{preset_name.Pastel("#569CD6")}\"\n");
                    }


                }

                // Other
                else
                {


                    byte[] payload = sysex_input.Skip(payload_start).Take(sysex_input.Length - payload_start - 0x01).ToArray();
                    byte[] converted_payload = ConvertNibblePayload(payload);

                    Console.Write($"(i) received unknown block:\n".Pastel("ffffff"));
                    ConsoleHelper.HexDump(new Blob(sysex_input), _bytes_per_line, sysex_marker, true, 0, false, -1, -1, _outputMode);

                    Console.Write($"\n(i) converted payload ({converted_payload.Length} bytes):\n".Pastel("f0c674"));
                    ConsoleHelper.HexDump(new Blob(converted_payload), _bytes_per_line, new Marker(), true, 0, false, -1, -1, _outputMode);
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
                throw new Exception("payload not dividable by 2!");

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

        static bool ByteArrayMatch(byte[] haystack, long[] offsets, byte[] needles)
        {
            if(offsets.Length != needles.Length)
                throw new Exception("offsets doesn't match needles");

            if (offsets.Length > haystack.Length)
                return false;

            for (int i = 0; i < offsets.Length; i++) {
                if (haystack[offsets[i]] != needles[i])
                    return false;
            }

            return true;
        }

    }
}
