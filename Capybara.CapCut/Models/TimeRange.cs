namespace Capybara.CapCut.Models;

public class TimeRange : ICloneable<TimeRange>
{
    public long Duration { get; set; }
    public long Start { get; set; }

    public TimeRange DeepCopy()
    {
        return (TimeRange)MemberwiseClone();
    }
}
