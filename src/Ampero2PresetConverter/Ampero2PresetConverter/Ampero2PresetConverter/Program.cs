if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: AmperoPresetConverter.exe filename.prst");
    return;
}

List<byte> result = new List<byte>();
var filename = Path.GetFullPath(args[0]);
var path = Path.GetDirectoryName(filename);

var new_filename_bin = Path.Combine(path, Path.GetFileNameWithoutExtension(filename) + ".bin");
var new_filename_arr = Path.Combine(path, Path.GetFileNameWithoutExtension(filename) + ".arr");

var base64_data = System.IO.File.ReadAllText(filename);
var data = Convert.FromBase64String(base64_data);
var byte_array_string = System.Text.Encoding.UTF8.GetString(data);


try
{
    File.WriteAllText(new_filename_arr, byte_array_string);



    var decimal_array = byte_array_string.Replace("[", "").Replace("]", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    foreach (string num in decimal_array)
    {
        byte b = 0;
        if (Byte.TryParse(num, out b))
        {
            result.Add(b);
        }
        else
        {
            Console.Error.WriteLine($"Something f*cked up during byte parsing from \"{new_filename_arr}\"!");
            return;
        }
    }
    File.WriteAllBytes(new_filename_bin, result.ToArray());

}
catch (Exception ex)
{
    Console.Error.WriteLine($"Something completly f*cked up!");
    Console.Error.WriteLine(ex.ToString());
    Console.Error.WriteLine(ex.StackTrace.ToString());
}

