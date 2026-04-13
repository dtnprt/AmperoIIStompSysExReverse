using Pastel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

public class ConsoleHelper
{
    public enum OutputMode
    {
        Hex,
        Dec,
        Oct,
        Bin,
        Char,
        Auto,
        Hybrid,
        SysEx
    }

    static char[] HexChars = "0123456789abcdef".ToCharArray();
    static char nonPrintableChar = '·';
    static string spaceChar = " ";

    static int firstHexColumn = 12; // 8 characters for the address +  3 spaces
    static int firstCharColumn = 0;
    static int lineLength = 0;
    static int dynamicSteps = 8;
    static int bytesPerLine = 16;

    static int windowWidth = 120;
    static int windowHeight = 80;

    public enum ConfirmDefault
    {
        None, Yes, No
    }

    public static bool Confirm(string title, ConfirmDefault confirmDefault = ConfirmDefault.None)
    {
        ConsoleKey response;

        switch (confirmDefault)
        {
            case ConfirmDefault.Yes:
                do
                {
                    Console.Write($"{title} [Y/n] ");
                    response = Console.ReadKey(false).Key;
                    if (response == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        return true;
                    }
                } while (response != ConsoleKey.Y && response != ConsoleKey.N);
                break;
            case ConfirmDefault.No:
                do
                {
                    Console.Write($"{title} [y/N] ");
                    response = Console.ReadKey(false).Key;
                    if (response == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        return false;
                    }
                } while (response != ConsoleKey.Y && response != ConsoleKey.N);
                break;
            default:
                do
                {
                    Console.Write($"{title} [y/n] ");
                    response = Console.ReadKey(false).Key;
                    Console.WriteLine();
                } while (response != ConsoleKey.Y && response != ConsoleKey.N);
                break;

        }
        return (response == ConsoleKey.Y);
    }

    public static void Write(char input)
    {
        Console.Write(input);
    }

    public static void Write(string input)
    {
        Console.Write(input);
    }

    public static void WriteLine(string input)
    {
        Write(input + Environment.NewLine);
    }

    public static void WriteError(string message)
    {
        Console.Error.Write($"{message.Pastel(ColorTheme.Error1)}\n");
    }

    public static void WriteError(Exception ex)
    {
        Console.Error.Write($"{ex.Message.Pastel(Color.OrangeRed)}\n{ex.StackTrace}\n");
    }

    public static void WriteErrorDog(string message)
    {
        string doggo = "            \\\n             \\\n            /^-----^\\\n            V  o o  V\n             |  Y  |\n              \\ Q /\n              / - \\\n              |    \\\n              |     \\     )\n              || (___\\====";
        string msg = message.Length < 12 ? message.PadLeft(11) : message;
        Console.Error.Write($"\n   {msg.Pastel(Color.OrangeRed)}\n{doggo.Pastel(Color.White)}\n\n");
    }

    public static void WriteErrorTRex(string message)
    {
        string doggo = "                      \n           ████████   \n          ███▄███████ \n          ███████████ \n          ███████████ \n          ██████      \n          █████████   \n█       ███████       \n██    ████████████    \n███  ██████████  █    \n███████████████       \n███████████████       \n █████████████        \n  ███████████         \n    ████████          \n     ███  ██          \n     ██    █          \n     █     █          \n     ██    ██         \n                     ";
        string msg = message.Length < 12 ? message.PadLeft(11) : message;
        Console.Error.Write($"\n   {msg.Pastel(Color.OrangeRed)}\n{doggo.Pastel(Color.White)}\n\n");
    }

    public static string GetVersionString()
    {
        return GetVersionString(null, null);
    }
    public static string GetVersionString(string color1, string color2)
    {
        if (color1 == null || color2 == null)
            return "ConsoleUtils" + " (" + "https://github.com/j-zero/ConsoleUtils" + ")";
        else
            return "ConsoleUtils".Pastel(color1) + " (" + "https://github.com/j-zero/ConsoleUtils".Pastel(color2) + ")";
    }

    // Enumerate by nearest space
    // Split String value by closest to length spaces
    // e.g. for length = 3 
    // "abcd efghihjkl m n p qrstsf" -> "abcd", "efghihjkl", "m n", "p", "qrstsf" 
    public static IEnumerable<String> SplitByNearestSpace(String value, int length)
    {
        if (String.IsNullOrEmpty(value))
            yield break;

        int bestDelta = int.MaxValue;
        int bestSplit = -1;

        int from = 0;

        for (int i = 0; i < value.Length; ++i)
        {
            var Ch = value[i];

            if (Ch != ' ')
                continue;

            int size = (i - from);
            int delta = (size - length > 0) ? size - length : length - size;

            if ((bestSplit < 0) || (delta < bestDelta))
            {
                bestSplit = i;
                bestDelta = delta;
            }
            else
            {
                yield return value.Substring(from, bestSplit - from);

                i = bestSplit;

                from = i + 1;
                bestSplit = -1;
                bestDelta = int.MaxValue;
            }
        }

        // String's tail
        if (from < value.Length)
        {
            if (bestSplit >= 0)
            {
                if (bestDelta < value.Length - from)
                    yield return value.Substring(from, bestSplit - from);

                from = bestSplit + 1;
            }

            if (from < value.Length)
                yield return value.Substring(from);
        }
    }


    public static void BinDump(byte[] bytes, int lineLength = 0)
    {
        if (lineLength == 0)
            lineLength = Console.WindowWidth - (Console.WindowWidth % 2) - 1;
        if (bytes == null) return;
        for (int i = 0; i < bytes.Length; i += lineLength)
        {
            string line = "";
            for (int j = 0; j < lineLength; j++)
            {
                if (!(i + j >= bytes.Length))
                {
                    byte b = bytes[i + j];
                    string color = ColorTheme.GetColor(b, j % 2 == 0);

                    line += ("" + (b < 32 ? nonPrintableChar : (char)b)).Pastel(color);
                }
                else
                {
                    line += spaceChar;
                }
            }
            Console.WriteLine(line);
        }
    }

    public static void VerySimpleHexDump(byte[] bytes, int bytesPerLine = 16)
    {
        if (bytes == null)
        {
            Console.WriteLine("<null>");
            return;
        }
        int bytesLength = bytes.Length;

        char[] HexChars = "0123456789ABCDEF".ToCharArray();

        int firstHexColumn =
              8                   // 8 characters for the address
            + 3;                  // 3 spaces

        int firstCharColumn = firstHexColumn
            + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
            + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
            + 2;                  // 2 spaces 

        int lineLength = firstCharColumn
            + bytesPerLine           // - characters to show the ascii value
            + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

        char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
        int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
        StringBuilder result = new StringBuilder(expectedLines * lineLength);

        for (int i = 0; i < bytesLength; i += bytesPerLine)
        {
            line[0] = HexChars[(i >> 28) & 0xF];
            line[1] = HexChars[(i >> 24) & 0xF];
            line[2] = HexChars[(i >> 20) & 0xF];
            line[3] = HexChars[(i >> 16) & 0xF];
            line[4] = HexChars[(i >> 12) & 0xF];
            line[5] = HexChars[(i >> 8) & 0xF];
            line[6] = HexChars[(i >> 4) & 0xF];
            line[7] = HexChars[(i >> 0) & 0xF];

            int hexColumn = firstHexColumn;
            int charColumn = firstCharColumn;

            for (int j = 0; j < bytesPerLine; j++)
            {
                if (j > 0 && (j & 7) == 0) hexColumn++;
                if (i + j >= bytesLength)
                {
                    line[hexColumn] = ' ';
                    line[hexColumn + 1] = ' ';
                    line[charColumn] = ' ';
                }
                else
                {
                    byte b = bytes[i + j];
                    line[hexColumn] = HexChars[(b >> 4) & 0xF];
                    line[hexColumn + 1] = HexChars[b & 0xF];
                    line[charColumn] = (b < 32 ? '·' : (char)b);
                }
                hexColumn += 3;
                charColumn++;
            }
            result.Append(line);
        }
        Console.WriteLine(result.ToString());
    }

    public static bool IsPrintable(char c)
    {
        // The set of Unicode character categories containing non-rendering,
        // unknown, or incomplete characters.
        // !! Unicode.Format and Unicode.PrivateUse can NOT be included in
        // !! this set, because they may (private-use) or do (format)
        // !! contain at least *some* rendering characters.
        var nonRenderingCategories = new UnicodeCategory[] {
            UnicodeCategory.Control,
            UnicodeCategory.OtherNotAssigned,
            UnicodeCategory.Surrogate, UnicodeCategory.OtherSymbol, UnicodeCategory.PrivateUse,
        };

        // Char.IsWhiteSpace() includes the ASCII whitespace characters that
        // are categorized as control characters. Any other character is
        // printable, unless it falls into the non-rendering categories.
        var isPrintable = (c == 0x0a && c == 0x09) || !nonRenderingCategories.Contains(Char.GetUnicodeCategory(c));


        return isPrintable;
    }

    public static void HexDump(byte[] bytes, int bytesPerLine = 16)
    {
        HexDump(new Blob(bytes), bytesPerLine, new Marker());
    }

    public static void HexDump(string str, int bytesPerLine = 16)
    {
        HexDump(new Blob(Encoding.UTF8.GetBytes(str)), bytesPerLine, new Marker());
    }

    static int SetBytesPerLine(int bPerLine)
    {
        return SetBytesPerLine(bPerLine, false);
    }
    static int SetBytesPerLine(int bPerLine, bool noAscii)
    {
        bytesPerLine = bPerLine;

        firstCharColumn = firstHexColumn
                            + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                            + (bytesPerLine - 1) / dynamicSteps // - 1 extra space every 8 characters from the 9th
                            + 2;                  // 2 spaces 

        lineLength = firstCharColumn + (!noAscii ? bytesPerLine : 0) + Environment.NewLine.Length;       // - characters to show the ascii value + Carriage return and line feed (should normally be 2)

        return lineLength;
    }


    public static void HexDump(Blob bytes, int BytesPerLine, Marker marker, bool header = true, ulong largestOffset = 0, bool zeroOffset = false, int highlightOffset = -1, int highlightLength = -1, OutputMode outputMode = OutputMode.Hex, bool noOffset = false, bool noText = false)
    {


        if (!Console.IsOutputRedirected)
        {
            windowHeight = Console.WindowHeight;
            windowWidth = Console.WindowWidth;
        }

        if (bytes == null) return;


        string spacer = "   ";
        string offsetPrefix = "0x";
        int bytesLength = bytes.Length;

        int offsetLength = largestOffset != 0 ? (largestOffset).ToString("X").Length : (bytes.Offset + bytes.Length - 1).ToString("X").Length;

        if (offsetLength % 2 != 0) offsetLength++;

        int padding = 2;
        if (outputMode == OutputMode.Hex || outputMode == OutputMode.Hybrid)
            padding = 2;
        else if (outputMode == OutputMode.Dec || outputMode == OutputMode.Oct)
            padding = 3;
        else if (outputMode == OutputMode.Bin)
            padding = 8;

        noText = noText | outputMode == OutputMode.Hybrid;

        if (BytesPerLine == 0)  // dynamic
        {
            int dynWidth = windowWidth;
            int minWidth = 32;
            while (lineLength < dynWidth - dynamicSteps - Environment.NewLine.Length)
            {
                SetBytesPerLine(bytesPerLine += dynamicSteps, noText);
                if (lineLength > dynWidth)
                {
                    SetBytesPerLine(bytesPerLine -= dynamicSteps, noText);
                    break;
                }

            }
            if (lineLength < minWidth)
                SetBytesPerLine(dynamicSteps, noText);
        }
        else
        {
            SetBytesPerLine(BytesPerLine, noText);
        }



        //Console.WriteLine(bytes.Length.ToString("X"));

        // header
        if (header)
        {
            for (int i = 0; i < offsetLength + offsetPrefix.Length; i++)
                Write(spaceChar);
            Write(spacer); // spacer
            for (int j = 0; j < bytesPerLine; j++)
            {
                if (j > 0 && (j & (dynamicSteps - 1)) == 0)
                    Write(spaceChar);

                string c = j % 2 == 0 ? ColorTheme.OffsetColor : ColorTheme.OffsetColor2;

                Write(j.ToString("X").ToLower().PadLeft(padding, '0').Pastel(c) + spaceChar);
            }
            Write(spacer); // spacer

            if (!noText)
                for (int j = 0; j < bytesPerLine; j++)
                {
                    Write((j % 16).ToString("X").ToLower().Pastel(ColorTheme.OffsetColor));
                }
            Write(Environment.NewLine);
        }

        int linecounter = 0;

        for (int i = 0; i < bytesLength; i += bytesPerLine)
        {
            linecounter++;
            string offsetPart = string.Empty;
            string hexPart = string.Empty;
            string asciiPart = string.Empty;

            offsetPart = (i + (zeroOffset ? 0 : Math.Abs(bytes.Offset))).ToString("X").ToLower().PadLeft(offsetLength, '0');

            for (int j = 0; j < bytesPerLine; j++)
            {
                int pos = i + j;
                int relativePos = pos + (int)bytes.Offset;

                if (j > 0 && (j & (dynamicSteps - 1)) == 0)
                    hexPart += spaceChar;

                if (pos >= bytesLength)
                {
                    for (int s = 0; s <= padding; s++) hexPart += spaceChar; // Spaces before ascii-part 
                }
                else
                {

                    byte b = bytes.Data[pos];
                    int first = (b >> 4) & 0xF;
                    int second = b & 0xF;

                    string newHexPart = string.Empty;


                    if (outputMode == OutputMode.Dec)
                    {
                        newHexPart += b.ToString().PadLeft(3, '0');
                    }
                    else if (outputMode == OutputMode.Oct)
                    {
                        newHexPart += Convert.ToString(b, 8).PadLeft(3, '0');
                    }

                    else if (outputMode == OutputMode.Bin)
                    {
                        newHexPart += Convert.ToString(first, 2).PadLeft(4, '0');
                        newHexPart += Convert.ToString(second, 2).PadLeft(4, '0');
                    }
                    else if (outputMode == OutputMode.Hybrid)
                    {
                        if (b < 32 || b > 0x7f)
                        {
                            newHexPart += HexChars[first];
                            newHexPart += HexChars[second];
                        }
                        else
                        {
                            newHexPart += ((char)b).ToString().PadLeft(2, ' ');
                        }


                        //newHexPart += " (" +b.ToString().PadLeft(3, '0') + ")";
                    }
                    else //(outputMode == OutputMode.Hex)
                    {

                        newHexPart += HexChars[first];
                        newHexPart += HexChars[second];

                        //newHexPart += " (" +b.ToString().PadLeft(3, '0') + ")";
                    }



                    string color = ColorTheme.GetColor(b, j % 2 == 0);

                    if (marker.IsMarker(pos) && marker.GetForegroundColor(pos) != null)
                        color = marker.GetForegroundColor(pos);

                    ;

                    if ((highlightOffset != -1 && highlightLength != -1))
                    {
                        if ((relativePos < highlightOffset) || (relativePos >= (highlightOffset + highlightLength)))
                            color = "#666666";
                    }

                    hexPart += newHexPart.Pastel(color);
                    hexPart += spaceChar;

                    asciiPart += ("" + (b < 32 ? nonPrintableChar : (char)b)).Pastel(color);

                }
            }
            WriteLine((noOffset ? string.Empty : (offsetPrefix + offsetPart).Pastel(linecounter % 2 == 0 ? ColorTheme.OffsetColor : ColorTheme.OffsetColor2) + spacer) + hexPart + (noText ? string.Empty : spacer + asciiPart));
        }
    }




}

