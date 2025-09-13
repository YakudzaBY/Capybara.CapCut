namespace Capybara.CapCut.Models;

public class Project
{

    public long Duration { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Might be improved to rely on project.Duration")]
    public long ToProjectTime(TimeSpan timeSpan) => (long)timeSpan.TotalMicroseconds;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Might be improved to rely on project.Duration")]
    public TimeSpan ToTimeSpan(long projectTime) => TimeSpan.FromMicroseconds(projectTime);

    public Materials Materials { get; set; } = default!;

    public List<Track> Tracks { get; set; } = default!;

    public Template<Track.Segment>? GetSegmentTemplate<T>(T material)
        where T : Materials.Material
    {
        return Tracks
            .Where(t => t.Segments.Count == 1
                && t.Segments[0].MaterialId == material.Id)
            .Select(t => new Template<Track.Segment>(t.Segments[0], t.Segments))
            .SingleOrDefault();
    }

    public static (T material, Track.Segment segment) CreateMaterialWithSegment<T>(Template<T> materialTemplate, Template<Track.Segment> segmentTemplate, long start)
        where T: Materials.Material, new()
    {
        var material = materialTemplate.Copy();

        var segment = segmentTemplate.Copy();
        segment.MaterialId = material.Id;
        segment.TargetTimerange = new TimeRange
        {
            Start = start,
        };
        return (material, segment);
    }
}