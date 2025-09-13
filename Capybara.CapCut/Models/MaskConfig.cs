using Capybara;
using System.Text.Json.Serialization;

namespace Capybara.CapCut.Models;

public class MaskConfig: ICloneable<MaskConfig>
{
    //public double AspectRatio { get; set; }
    [JsonPropertyName("centerX")]
    public double CenterX { get; set; }
    /*
    public double CenterY { get; set; }
    public double Expansion { get; set; }
    public double Feather { get; set; }
    public double Height { get; set; }
    public bool Invert { get; set; }
    public double Rotation { get; set; }
    public double RoundCorner { get; set; }
    public double Width { get; set; }
    */
    public MaskConfig DeepCopy()
    {
        var other = (MaskConfig)MemberwiseClone();
        return other;
    }
}