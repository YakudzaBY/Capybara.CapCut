
namespace Capybara.CapCut.Models;

public class Fill
{
    public Content Content { get; set; }

    public Fill DeepCopy()
    {
        var other = (Fill)MemberwiseClone();
        other.Content = Content?.DeepCopy();
        return other;
    }
}
