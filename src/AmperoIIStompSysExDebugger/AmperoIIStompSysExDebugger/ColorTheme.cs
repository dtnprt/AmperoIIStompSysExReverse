using System.Collections.Generic;
public class ColorTheme
{
    public static string Directory = "#569CD6";
    public static string Share = "#D69D85";
    //private string _colorExecutable = "";
    public static string File = "#DDDDDD";
    public static string Symlink = "#00CED1";
    public static string UnknownFileType = "#aaaaaa";
    public static string Error1 = "#e01e37"; // "#ff4500"
    public static string Error2 = "#a54242";
    public static string Null1 = "#8abeb7";
    public static string Null2 = "#5e8d87";
    public static string SpecialChar1 = "#b5db68";
    public static string SpecialChar2 = "#8c9440";
    public static string Red1 = "#cc6666";
    public static string Red2 = "#a54242";
    public static string Comment { get { return "666666"; } }
    //public static string Default1 { get { return "81a2be"; } }
    //public static string Default2 { get { return "5f819d"; } }
    public static string Default1 { get { return "cce6ff"; } }
    public static string Default2 { get { return "e6f3ff"; } }
    public static string DarkColor { get { return "268C96"; } }
    public static string HighLight2 { get { return "dea740"; } }
    public static string HighLight1 { get { return "f0c674"; } }
    public static string OffsetColor { get { return "808080"; } }
    public static string OffsetColor2 { get { return "A0A0A0"; } }
    public static string OffsetColorHighlight { get { return "f5eea9"; } }
    public static string OffsetColorHighlight2 { get { return "D2CC8D"; } }
    public static string Text { get { return "dddddd"; } }
    public static string DarkText { get { return "707070"; } }



    public static string GetColor(byte b, bool isOdd)
    {
        return GetColor((int)b, isOdd);
    }
    public static string GetColor(int b, bool isOdd)
    {
        string color = isOdd ? Default1 : Default2;    // default blue;

        if (b == 0x00)
            color = isOdd ? Null1 : Null2;
        //else if (b == 0x0d || b == 0x0a)    // CR LF
        //    color = isOdd ? SpecialChar1 : SpecialChar2;
        //else if (b < 32)
        //    color = isOdd ? HighLight2 : HighLight1;
        //else if (b > 127 && b <= 255)                   // US-ASCII
        //    color = isOdd ? OffsetColorHighlight : OffsetColor;
        else if (b > 0x7F)
            color = isOdd ? HighLight2 : HighLight1;

        return color;
    }
}