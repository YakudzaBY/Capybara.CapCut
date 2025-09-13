using Capybara.CapCut.Models;

namespace Capybara.CapCut;

public record Template<T>(T Example, ICollection<T> Container)
    : ITemplate
    where T : Base, new()
{
    public T Copy()
    {
        //TODO - make all cloneable
        T result = typeof(ICloneable<T>).IsAssignableFrom(typeof(T))
            ? ((ICloneable<T>)Example).DeepCopy()
            : new();

        result.Id = Guid.NewGuid();
        result.TemplateId = Example.Id;
        Container.Add(result);
        return result;
    }

    public void Detach()
    {
        Container.Remove(Example);
    }
}

public interface ITemplate
{
    void Detach();
}