namespace WinUIGallery.Helpers;

internal partial class Vector4Data
{
    public float W { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public static implicit operator System.Numerics.Vector4(Vector4Data d) => new(d.X, d.Y, d.Z, d.W);
    public static implicit operator Vector4Data(System.Numerics.Vector4 v) => new() { X = v.X, Y = v.Y, Z = v.Z, W = v.W };
}
internal partial class Vector3Data
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public static implicit operator System.Numerics.Vector3(Vector3Data d) => new(d.X, d.Y, d.Z);
    public static implicit operator Vector3Data(System.Numerics.Vector3 v) => new() { X = v.X, Y = v.Y, Z = v.Z };
}
internal partial class Vector2Data
{
    public float X { get; set; }
    public float Y { get; set; }

    public static implicit operator System.Numerics.Vector2(Vector2Data d) => new(d.X, d.Y);
    public static implicit operator Vector2Data(System.Numerics.Vector2 v) => new() { X = v.X, Y = v.Y };
}