using RfCodeGen.ServiceLayer.Utils.Pluralizer;
using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;
using System.Text;

namespace RfCodeGen.ServiceLayer;

public abstract class RfCodeGeneratorBase
{
    public static string GetDomainServiceRegistrations(IEnumerable<EntityDto> entities)
    {
        StringBuilder sb = new();
        foreach(EntityDto entity in entities)
        {
            sb.AppendLine($"builder.Services.AddScoped<I{entity.Name}Domain, {entity.Name}Domain>();");
        }

        return sb.ToString();
    }

    public static string GetAutoMapperMappingProfiles(IEnumerable<EntityDto> entities)
    {
        StringBuilder sb = new();
        foreach(EntityDto entity in entities)
        {
            sb.AppendLine($"CreateMap<{entity.Name}, {entity.Name}Dto>().ReverseMap();");
        }

        return sb.ToString();
    }
}

public interface IRfCodeGenerator<out TEntityDescriptor, out TEntityPropertyDescriptor>
    where TEntityDescriptor : EntityDescriptorDto, new()
    where TEntityPropertyDescriptor : EntityPropertyDescriptorDto, new()
{
    Task<int> Generate(IEnumerable<EntityDto> entities, ProjectDescriptorDto projectDescriptor, IProgress<string> progress);
}

public class RfCodeGenerator<TEntityDescriptor, TEntityPropertyDescriptor> : RfCodeGeneratorBase, IRfCodeGenerator<TEntityDescriptor, TEntityPropertyDescriptor>
    where TEntityDescriptor : EntityDescriptorDto, new()
    where TEntityPropertyDescriptor : EntityPropertyDescriptorDto, new()
{
    private Pluralizer Pluralizer { get; } = new();

    private static void ParsePropertyLine(string line, out string modifiers, out string type, out string name, out bool get, out bool set, out string assignment)
    {
        line = line.Trim();

        var pieces = line.Split('=');
        assignment = string.Empty;
        if(pieces.Length == 2)
        {
            assignment = $" = {pieces[1].Trim()}";
            line = pieces[0].Trim(); // take the first part before the '='
        }

        var i = line.IndexOf('{');
        string definition = line[..i].Trim();
        string getset = line[i..].Trim();

        pieces = definition.Trim().Split(' ');
        name = pieces.Last();
        type = pieces[^2];
        modifiers = string.Join(' ', pieces.Take(pieces.Length - 2)); // take all but the last two pieces

        get = getset.Contains("get;");
        set = getset.Contains("set;");

        if(type.StartsWith("ICollection<"))
        {
            var collectionType = type[12..^1]; // e.g., ICollection<MyEntity>
            type = type.Replace($"<{collectionType}>", $"<{collectionType}Dto>");

            if(assignment != string.Empty)
                assignment = " = [];";
        }
    }

    public async Task<int> Generate(IEnumerable<EntityDto> entities, ProjectDescriptorDto projectDescriptor, IProgress<string> progress)
    {
        int count = 0;

        List<TEntityDescriptor> entityDescriptors = [];
        foreach(EntityDto entity in entities)
        {
            IEnumerable<string> lines = File.ReadAllLines(entity.FilePath);
            lines = lines.SkipWhile(v1 => !v1.StartsWith("public partial class"));

            TEntityDescriptor entityDescriptor = new() { Entity = entity, PluralizedName = Pluralizer.Pluralize(entity.Name) };
            var propertyLines = lines.SkipWhile(v1 => v1 != "{").Skip(1).TakeWhile(v1 => v1 != "}").Where(v1 => !string.IsNullOrWhiteSpace(v1));
            foreach(string propertyLine in propertyLines)
            {
                RfCodeGenerator<TEntityDescriptor, TEntityPropertyDescriptor>.ParsePropertyLine(propertyLine, out string modifiers, out string type, out string name, out bool get, out bool set, out string assignment);

                TEntityPropertyDescriptor entityProperty = new()
                {
                    Text = propertyLine.Trim(),
                    EntityDescriptor = entityDescriptor,
                    Modifiers = modifiers,
                    Type = type,
                    Name = name,
                    Get = get,
                    Set = set,
                    Assignment = assignment
                };
                
                entityDescriptor.Properties.Add(entityProperty);
            }

            entityDescriptors.Add(entityDescriptor);
        }

        var projectFolder = projectDescriptor.ProjectFolder;

        //Model (partial)
        foreach(var entityDescriptor in entityDescriptors)
        {
            var modelTemplate = projectDescriptor.GetModelTemplate(entityDescriptor);
            string modelPartialContent = modelTemplate.TransformText();
            string modelPartialFilePath = projectFolder.DataAccess.Models.Partials.GetFilePath($"{entityDescriptor.Entity.Name}.cs");
            await File.WriteAllTextAsync(modelPartialFilePath, modelPartialContent, projectDescriptor.Encoding);
            progress.Report($"Generated Model partial for {entityDescriptor.Entity.Name}");

            count++;
        }

        //Dto
        foreach(var entityDescriptor in entityDescriptors)
        {
            var dtoTemplate = projectDescriptor.GetDtoTemplate(entityDescriptor);
            string dtoContent = dtoTemplate.TransformText();
            string dtoFilePath = projectFolder.Shared.Dtos.GetFilePath($"{entityDescriptor.Name}Dto.cs");
            if(entityDescriptor.IsLookupTable)
                dtoFilePath = projectFolder.Shared.Dtos.Lookups.GetFilePath($"{entityDescriptor.Name}Dto.cs");
            await File.WriteAllTextAsync(dtoFilePath, dtoContent, projectDescriptor.Encoding);
            progress.Report($"Generated DTO for {entityDescriptor.Name}");

            count++;
        }

        //Domain
        foreach(var entityDescriptor in entityDescriptors.Where(v1 => !v1.IsLookupTable))
        {
            var domainTemplate = projectDescriptor.GetDomainTemplate(entityDescriptor);
            string domainContent = domainTemplate.TransformText();
            string domainFilePath = projectFolder.ServiceLayer.Domains.GetFilePath($"{entityDescriptor.Name}Domain.cs");
            await File.WriteAllTextAsync(domainFilePath, domainContent, projectDescriptor.Encoding);
            progress.Report($"Generated ServiceLayer domain for {entityDescriptor.Name}");

            count++;
        }

        //Controller
        foreach(var entityDescriptor in entityDescriptors.Where(v1 => !v1.IsLookupTable))
        {
            var controllerTemplate = projectDescriptor.GetControllerTemplate(entityDescriptor);
            string controllerContent = controllerTemplate.TransformText();
            string controllerFilePath = projectFolder.WebApi.Controllers.GetFilePath($"{this.Pluralizer.Pluralize(entityDescriptor.Name)}Controller.cs");
            await File.WriteAllTextAsync(controllerFilePath, controllerContent, projectDescriptor.Encoding);
            progress.Report($"Generated Controller for {entityDescriptor.Name}");

            count++;
        }

        return count;
    }
}
