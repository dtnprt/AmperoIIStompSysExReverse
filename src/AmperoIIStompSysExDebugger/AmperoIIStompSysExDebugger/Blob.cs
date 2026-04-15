using System.Drawing;

public class Blob
{

    public long Offset { get; set; }
    public byte[] Data { get; set; }
    public int Length
    {
        get
        {
            return Data.Length;
        }
    }
    public Blob()
    {

    }
    public Blob(byte[] Data)
    {
        this.Data = Data;
    }
    public Blob(long Offset, byte[] Data)
    {
        this.Offset = Offset;
        this.Data = Data;
    }
}

public class Selection
{
    public int Offset { get; set; }
    public int Length { get; set; }


    public int End
    {
        get
        {
            return Offset + Length;
        }
        set
        {
            this.Length = value - Offset;
        }
    }

    public Selection()
    {
    }
    public Selection(int Offset, int Length)
    {
        this.Offset = Offset;
        this.Length = Length;
    }
}

public class MarkerData 
{
    public string? ForegroundColor { get; set; }
    public string? BackgroundColor { get; set; }
    public string Description  { get; set; }

    public MarkerData()
    {
        this.ForegroundColor = null;
        this.BackgroundColor = null;
        this.Description = string.Empty;
    }
    public MarkerData(string? ForegroundColor, string Description = "")
    {
        this.ForegroundColor = ForegroundColor;
        this.BackgroundColor = null;
        this.Description = Description;
    }
    public MarkerData(string? ForegroundColor, string? BackgroundColor, string Description = "")
    {
        this.ForegroundColor = ForegroundColor;
        this.BackgroundColor = BackgroundColor;
        this.Description = Description;
    }

}

public class Marker
{
    Dictionary<long, MarkerData> markerDB = new Dictionary<long, MarkerData>();

    public Marker()
    {

    }

    public bool IsMarker(long Offset)
    {
        return markerDB.ContainsKey(Offset);
    }

    public string? GetForegroundColor(long Offset)
    {
        if (markerDB.ContainsKey(Offset))
            return markerDB[Offset].ForegroundColor;
        else
            return null;
    }

    public string? GetBackgroundColor(long Offset)
    {
        if (markerDB.ContainsKey(Offset))
            return markerDB[Offset].BackgroundColor;
        else
            return null;
    }

    public MarkerData GetColorInfo(long Offset)
    {
        if (markerDB.ContainsKey(Offset))
            return markerDB[Offset];
        else
            return new MarkerData();
    }

    public void Add(long Offset, string ForgroundColor)
    {
        this.Add(Offset, new MarkerData(ForgroundColor));
    }

    public void Add(long Offset, string ForgroundColor, string BackgroundColor)
    {
        this.Add(Offset, new MarkerData(ForgroundColor, BackgroundColor));
    }

    public void Add(long Offset, MarkerData ColorInfo)
    {
        if (markerDB.ContainsKey(Offset))
            markerDB[Offset] = ColorInfo;
        else
            markerDB.Add(Offset, ColorInfo);
    }
    public void AddRange(long StartOffset, long EndOffset, string ForgroundColor)
    {
        this.AddRange(StartOffset, EndOffset, new MarkerData(ForgroundColor, null));
    }

    public void AddRange(long StartOffset, long EndOffset, string ForgroundColor, string BackgroundColor)
    {
        this.AddRange(StartOffset, EndOffset, new MarkerData(ForgroundColor, BackgroundColor));
    }

    public void AddRange(long StartOffset, long EndOffset, MarkerData ColorInfo)
    {
        for (long i = StartOffset; i <= EndOffset; i++)
        {
            if (markerDB.ContainsKey(i))
                markerDB[i] = ColorInfo;
            else
                markerDB.Add(i, ColorInfo);
        }
    }

    public void AddRange(long[] Offsets, string ForgroundColor)
    {
        this.AddRange(Offsets, new MarkerData(ForgroundColor, null));
    }

    public void AddRange(long[] Offsets, string ForgroundColor, string BackgroundColor)
    {
        this.AddRange(Offsets, new MarkerData(ForgroundColor, BackgroundColor));
    }

    public void AddRange(long[] Offsets, MarkerData ColorInfo)
    {
        for (long i = 0; i < Offsets.Length; i++)
        {
            if (markerDB.ContainsKey(Offsets[i]))
                markerDB[Offsets[i]] = ColorInfo;
            else
                markerDB.Add(Offsets[i], ColorInfo);
        }
    }

}
