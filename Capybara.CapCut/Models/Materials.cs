using System.Text.Json.Serialization;
using Capybara.Json.Converters;

namespace Capybara.CapCut.Models;

public partial class Materials
{
    public abstract class Material : Base;

    public abstract class ConstantMaterial: Material, ICloneable<ConstantMaterial>
    {
        public Guid ConstantMaterialId { get; set; }

        public virtual ConstantMaterial DeepCopy()
        {
            var other = (ConstantMaterial)MemberwiseClone();
            other.ConstantMaterialId = ConstantMaterialId;
            return other;
        }
    }

    public class Text : Material, ICloneable<Text>
    {
        [JsonConverter(typeof(JsonEncodedStringToObjectConverter<ContentObj>))]
        public ContentObj Content { get; set; }

        public Text DeepCopy()
        {
            var other = (Text)MemberwiseClone();
            other.Content = Content?.DeepCopy();
            return other;
        }

        public class ContentObj: ICloneable<ContentObj>
        { 
            public string Text { get; set; }

            public class Style
            {
                public Fill Fill { get; set; }

                public Font Font { get; set; }

                public List<Stroke> Strokes { get; set; }

                public double Size { get; set; }

                public bool UseLetterColor { get; set; }

                public int[] Range { get; set; }

                public Style DeepCopy()
                {
                    var other = (Style)MemberwiseClone();
                    other.Fill = Fill?.DeepCopy();
                    other.Font = Font?.DeepCopy();
                    other.Strokes = Strokes?
                        .Select(s => s.DeepCopy())
                        .ToList();
                    other.Range = Range?.ToArray();

                    return other;
                }
            }

            public List<Style>? Styles { get; set; }

            public ContentObj DeepCopy()
            {
                var other = (ContentObj)MemberwiseClone();
                other.Styles = Styles?
                    .Select(s => s.DeepCopy())
                    .ToList();
                return other;
            }

            public void Highlight(Style background, Style highlight, int[] columnWidths, params (int row, int column)[] cells)
            {
                Styles = [];
                var thisStyle = background.DeepCopy();
                thisStyle.Range[0] = 0;
                void FinalizeStyle(int end)
                {
                    thisStyle.Range[1] = end;
                    Styles.Add(thisStyle);
                }
                foreach ((var row, var column) in cells
                    .OrderBy(c => c.row)
                    .ThenBy(c => c.column))
                {
                    var charIndex = (columnWidths.Sum() + columnWidths.Length) * (row + 1) + columnWidths[0..(column)].Sum() + column;
                    FinalizeStyle(charIndex);

                    thisStyle = highlight.DeepCopy();
                    thisStyle.Range[0] = charIndex;
                    charIndex += columnWidths[column];
                    thisStyle.Range[1] = charIndex;
                    Styles.Add(thisStyle);

                    thisStyle = background.DeepCopy();
                    thisStyle.Range[0] = charIndex;
                }
                FinalizeStyle(Text.Length);
            }
        }
    }

    public class Video : Material
    {
        //public Guid LocalMaterialId { get; set; }

        [JsonInclude]
        public string MaterialName { get; private set; }

        [JsonInclude]
        public string Path { get; private set; }

        public void ChangePath(string path)
        {
            Path = path;
            MaterialName = System.IO.Path.GetFileName(Path);
        }
    }

    public List<Text> Texts { get; set; }

    public List<Video> Videos { get; set; }

    public class Mask : ConstantMaterial, ICloneable<Mask>
    {
        //public string Category { get; set; }

        public MaskConfig Config { get; set; }
        
        Mask ICloneable<Mask>.DeepCopy()
        {
            var other = (Mask)DeepCopy();
            other.Config = Config.DeepCopy();
            return other;
        }
    }

    public List<HslItem> Hsl { get; set; }

    public class HslItem : ConstantMaterial
    {
    }

    [JsonPropertyName("common_mask")]
    public List<Mask> Masks { get; set; }
}
