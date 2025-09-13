namespace Capybara.CapCut.Models;

public class Track : Base
{
    public string Type { get; set; }

    public abstract class MaterialDependent: Base
    {
        public Guid MaterialId { get; set; }
    }

    public class Segment : MaterialDependent, ICloneable<Segment>
    {
        public TimeRange SourceTimerange { get; set; }

        public TimeRange TargetTimerange { get; set; }
        
        public class CommonKeyFrame : MaterialDependent, ICloneable<CommonKeyFrame>
        {
            public Guid MaterialId { get; set; }

            public string PropertyType { get; set; }

            public class KeyFrame : Base, ICloneable<KeyFrame>
            {
                /*
                [JsonPropertyName("curveType")]
                public string CurveType { get; set; }

                [JsonPropertyName("graphID")]
                public string GraphId { get; set; }

                public PointF LeftControl { get; set; }

                public PointF RightControl { get; set; }

                public string StringValue { get; set; }
                */

                public long TimeOffset { get; set; }

                public List<double>? Values { get; set; }

                public KeyFrame DeepCopy()
                {
                    var other = (KeyFrame)MemberwiseClone();
                    other.Values = Values?
                        .ToList();
                    return other;
                }
            }

            public List<KeyFrame>? KeyframeList { get; set; }

            public CommonKeyFrame DeepCopy()
            {
                var other = (CommonKeyFrame)MemberwiseClone();
                other.KeyframeList = KeyframeList?
                    .Select(kf => kf.DeepCopy())
                    .ToList();
                return other;
            }
        }

        public List<CommonKeyFrame> CommonKeyframes { get; set; }

        public List<Guid> ExtraMaterialRefs { get; set; }

        public Segment DeepCopy()
        {
            var other = (Segment)MemberwiseClone();
            other.SourceTimerange = SourceTimerange?.DeepCopy();
            other.TargetTimerange = TargetTimerange?.DeepCopy();
            other.CommonKeyframes = CommonKeyframes?
                .Select(kf => kf.DeepCopy())
                .ToList();
            other.ExtraMaterialRefs = ExtraMaterialRefs?
                .ToList();
            return other;
        }

        public void SetHorizontalAppearance(params (long Time, double Value)[] keyFrames)
        {
            var kfListX = CommonKeyframes
                .Single(kf => kf.PropertyType == "KFTypeCommonMaskPositionX")
                .KeyframeList!;

            Track.Segment.CommonKeyFrame.KeyFrame? prevKF = default; 
            for(var i=0; i <3; i++)
            {
                var (Time, Value) = keyFrames[i];
                var kfOut = kfListX[i];
                kfOut.TimeOffset = Time;
                if(prevKF != default)
                {
                    kfOut.TimeOffset += prevKF.TimeOffset;
                }
                kfOut.Values = [Value];
                prevKF = kfOut;
            }
        }
    }

    public List<Segment> Segments { get; set; }
}
