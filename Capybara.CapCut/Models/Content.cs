
namespace Capybara.CapCut.Models;

public class Content
{
    public string RenderType { get; set; }

    public Solid Solid { get; set; }

    public Content DeepCopy()
    {
        var other = (Content)MemberwiseClone();
        other.Solid = Solid?.DeepCopy();
        return other;
    }
}
