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

//public interface IRfCodeGenerator<T>
//    where T : IProjectDescriptor
//{
//    Task<int> Generate(IEnumerable<EntityDto> entities, T projectDescriptor, IProgress<string> progress);
//}

public class RfCodeGenerator(IProjectDescriptor projectDescriptor) : RfCodeGeneratorBase   //, IRfCodeGenerator<T>
{
    private Pluralizer Pluralizer { get; } = new();
    private IProjectDescriptor ProjectDescriptor { get; } = projectDescriptor;

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

    public async Task<int> Generate(IEnumerable<EntityDto> entities, IProgress<string> progress)
    {
        int count = 0;

        List<EntityDescriptorDto> entityDescriptors = [];
        foreach(EntityDto entity in entities)
        {
            IEnumerable<string> lines = File.ReadAllLines(entity.FilePath);
            lines = lines.SkipWhile(v1 => !v1.StartsWith("public partial class"));

            EntityDescriptorDto entityDescriptor = this.ProjectDescriptor.GetEntityDescriptor(); // { Entity = entity, PluralizedName = Pluralizer.Pluralize(entity.Name) };
            entityDescriptor.Entity = entity;
            entityDescriptor.PluralizedName = this.Pluralizer.Pluralize(entity.Name);

            var propertyLines = lines.SkipWhile(v1 => v1 != "{").Skip(1).TakeWhile(v1 => v1 != "}").Where(v1 => !string.IsNullOrWhiteSpace(v1));
            foreach(string propertyLine in propertyLines)
            {
                RfCodeGenerator.ParsePropertyLine(propertyLine, out string modifiers, out string type, out string name, out bool get, out bool set, out string assignment);

                EntityPropertyDescriptorDto entityProperty = this.ProjectDescriptor.GetEntityPropertyDescriptor();   //new()
                entityProperty.Text = propertyLine.Trim();
                entityProperty.EntityDescriptor = entityDescriptor;
                entityProperty.Modifiers = modifiers;
                entityProperty.Type = type;
                entityProperty.Name = name;
                entityProperty.Get = get;
                entityProperty.Set = set;
                entityProperty.Assignment = assignment;
                
                entityDescriptor.Properties.Add(entityProperty);
            }

            entityDescriptors.Add(entityDescriptor);
        }

        var projectFolder = this.ProjectDescriptor.ProjectFolder;

        //Model (partial)
        foreach(var entityDescriptor in entityDescriptors)
        {
            var modelTemplate = this.ProjectDescriptor.GetModelTemplate(entityDescriptor);
            string modelPartialContent = modelTemplate.TransformText();
            string modelPartialFilePath = projectFolder.DataAccess.Models.Partials.GetFilePath($"{entityDescriptor.Entity.Name}.cs");
            await File.WriteAllTextAsync(modelPartialFilePath, modelPartialContent, this.ProjectDescriptor.Encoding);
            progress.Report($"Generated Model partial for {entityDescriptor.Entity.Name}");

            count++;
        }

        //Dto
        foreach(var entityDescriptor in entityDescriptors)
        {
            var dtoTemplate = this.ProjectDescriptor.GetDtoTemplate(entityDescriptor);
            string dtoContent = dtoTemplate.TransformText();
            string dtoFilePath = projectFolder.Shared.Dtos.GetFilePath($"{entityDescriptor.Name}Dto.cs");
            if(entityDescriptor.IsLookupTable)
                dtoFilePath = projectFolder.Shared.Dtos.Lookups.GetFilePath($"{entityDescriptor.Name}Dto.cs");
            await File.WriteAllTextAsync(dtoFilePath, dtoContent, this.ProjectDescriptor.Encoding);
            progress.Report($"Generated DTO for {entityDescriptor.Name}");

            count++;
        }

        //Domain
        foreach(var entityDescriptor in entityDescriptors.Where(v1 => !v1.IsLookupTable))
        {
            var domainTemplate = this.ProjectDescriptor.GetDomainTemplate(entityDescriptor);
            string domainContent = domainTemplate.TransformText();
            string domainFilePath = projectFolder.ServiceLayer.Domains.GetFilePath($"{entityDescriptor.Name}Domain.cs");
            await File.WriteAllTextAsync(domainFilePath, domainContent, this.ProjectDescriptor.Encoding);
            progress.Report($"Generated ServiceLayer domain for {entityDescriptor.Name}");

            count++;
        }

        //Controller
        foreach(var entityDescriptor in entityDescriptors.Where(v1 => !v1.IsLookupTable))
        {
            var controllerTemplate = this.ProjectDescriptor.GetControllerTemplate(entityDescriptor);
            string controllerContent = controllerTemplate.TransformText();
            string controllerFilePath = projectFolder.WebApi.Controllers.GetFilePath($"{this.Pluralizer.Pluralize(entityDescriptor.Name)}Controller.cs");
            await File.WriteAllTextAsync(controllerFilePath, controllerContent, this.ProjectDescriptor.Encoding);
            progress.Report($"Generated Controller for {entityDescriptor.Name}");

            count++;
        }

        return count;
    }
}
