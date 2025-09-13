using Capybara.CapCut.Json.Converters;
using Capybara.CapCut.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Capybara.CapCut;

public class Context(string folder)
{
    const string projectFile = "draft_content.json";

    public static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters = { new PropperCaseGuidJsonConverter() }
    };
    private Project? Project;

    FileInfo ProjectFileInfo => new(Path.Combine(folder, projectFile));

    public async ValueTask<Project> GetProjectAsync()
    {
        return Project ??= await JsonSerializer
            .DeserializeAsync<Project>(ProjectFileInfo.OpenRead(), jsonSerializerOptions);
    }

    public async Task SaveChangesAsync()
    {
        if (Project == null) throw new InvalidOperationException("No changes");

        var thisJson = JsonSerializer.Serialize(Project, jsonSerializerOptions);
        var thisNode = JsonNode.Parse(thisJson);
        JsonNode fileJson;
        using (var readStream = ProjectFileInfo.OpenRead())
        {
            fileJson = await JsonNode.ParseAsync(readStream);
        }
        Update(thisNode.AsObject(), fileJson.AsObject());
        
        var newFolder = $"{folder} {DateTime.UtcNow.Ticks}";
        Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(folder, newFolder);
        var target = Path.Combine(newFolder, projectFile);
        await System.IO.File.WriteAllTextAsync(target, fileJson.ToJsonString());

        /* //CapCut discovers new projects by folder on it's own, so we don't need to update root_meta_info.json
        var rootFile = Directory.GetParent(Folder).EnumerateFiles("root_meta_info.json").Single();
        JsonNode rootJson;
        using(var readStream = rootFile.OpenRead())
        {
            rootJson = (await JsonNode.ParseAsync(readStream))!;
        }
        var store = rootJson["all_draft_store"]!.AsArray();
        var newLink = store
            .Single(n => Common.File.ArePathsEquals(
                n!["draft_fold_path"]!.GetValue<string>(),
                Folder))!
            .DeepClone();
        store.Add(newLink);
        await File.WriteAllTextAsync(rootFile.FullName, rootJson.ToJsonString());
        */
    }

    static void Update(JsonObject source, JsonObject target)
    {
        foreach (var sourceProperty in source.Where(p => p.Key != Base.TemplateIdName))
        {
            var targetProperty = target[sourceProperty.Key];
            var sourcePropertValueKind = sourceProperty.Value?.GetValueKind();
            if (sourceProperty.Key == "content" && sourcePropertValueKind == JsonValueKind.String)
            {
                var sourceNode = JsonNode.Parse(sourceProperty.Value.GetValue<string>()).AsObject();
                var targetNode = JsonNode.Parse(targetProperty.GetValue<string>()).AsObject();
                Update(sourceNode, targetNode);
                target[sourceProperty.Key] = targetNode.ToJsonString();
            }
            else
            {
                switch (sourcePropertValueKind)
                {
                    case JsonValueKind.Object:
                        Update(sourceProperty.Value.AsObject(), targetProperty.AsObject());
                        break;
                    case JsonValueKind.Array:
                        Update(sourceProperty.Value.AsArray(), targetProperty.AsArray());
                        break;
                    case JsonValueKind.String:
                    case JsonValueKind.Number:
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                    case JsonValueKind.Null:
                    case null:
                        target[sourceProperty.Key] = sourceProperty.Value?.DeepClone();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

        }
    }

    static void Update(JsonArray source, JsonArray target)
    {
        var templates = new List<JsonObject>();
        for (var i = 0; i < source.Count; i++)
        {
            var sourceNode = source[i];
            JsonNode targetNode = target.Count > i
                ? target[i]
                : null;

            switch (sourceNode?.GetValueKind())
            {
                case JsonValueKind.Object:
                    var templateId = sourceNode[Base.TemplateIdName]?.GetValue<Guid>();
                    if (templateId == null)
                    {
                        var itemId = sourceNode["id"];
                        //Upsert
                        if (itemId != null)
                        {
                            targetNode = target
                                .SingleOrDefault(n => n["id"].GetValue<Guid>() == itemId.GetValue<Guid>());
                        }

                        //Insert
                        if (targetNode == null)
                        {
                            targetNode = sourceNode.DeepClone();
                            target.Add(targetNode);
                        }
                    }
                    else
                    {
                        var template = target
                            .Single(n => n["id"].GetValue<Guid>() == templateId)
                            .AsObject();
                        templates.Add(template);
                        targetNode = template.DeepClone();
                        target.Add(targetNode);
                    }
                    Update(sourceNode.AsObject(), targetNode.AsObject());
                    break;
                case JsonValueKind.Array:
                    Update(sourceNode.AsArray(), targetNode.AsArray());
                    break;
                case JsonValueKind.String:
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                case null:
                    target[i] = sourceNode?.DeepClone();
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        
        //Remove templates
        foreach (var template in templates)
        {
            target.Remove(template);
        }

        //Delete all deleted
        foreach (var targetNode in target.ToArray())
        {
            switch (targetNode.GetValueKind())
            {
                case JsonValueKind.Object:
                    var targetObj = targetNode.AsObject();
                    if (targetObj.TryGetPropertyValue("id", out var idNode))
                    {
                        var id = idNode.GetValue<Guid>();
                        if(!source.Any(n => n["id"].GetValue<Guid>() == id))
                        {
                            target.Remove(targetNode);
                        }
                    }
                    break;
            }
        }
    }
}
