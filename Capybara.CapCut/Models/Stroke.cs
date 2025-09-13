
namespace Capybara.CapCut.Models
{
    public class Stroke
    {
        public Content Content { get; set; }

        public float Width {  get; set; }

        public Stroke DeepCopy()
        {
            var other = (Stroke)MemberwiseClone();
            other.Content = Content?.DeepCopy();
            return other;
        }
    }
}