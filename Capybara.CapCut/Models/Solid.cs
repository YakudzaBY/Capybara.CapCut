
namespace Capybara.CapCut.Models;

public class Solid
{
    //TODO refactor for System.Drawing.Color
    public List<float> Color { get; set; }

    internal Solid DeepCopy()
    {
        var other = (Solid)MemberwiseClone();
        other.Color = Color?.ToList();
        return other;
    }
}
