using System.Text.Json.Serialization;

namespace Capybara.CapCut.Models;

public abstract class Base
{
    public Guid Id { get; set; }

    public const string TemplateIdName = "$template_id";

    [JsonPropertyName(TemplateIdName)]
    public Guid? TemplateId { get; set; }
}
