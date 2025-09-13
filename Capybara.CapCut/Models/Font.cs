
namespace Capybara.CapCut.Models;

public class Font
{
    public string Path { get; set; }

    public string Id { get; set; }

    public Font DeepCopy()
    {
        return (Font)MemberwiseClone();
    }
}