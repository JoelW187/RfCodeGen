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

public class RfCodeGenerator(IProjectDescriptor projectDescriptor) : RfCodeGeneratorBase
{
    private Pluralizer Pluralizer { get; } = new();
    private IProjectDescriptor ProjectDescriptor { get; } = projectDescriptor;

    public async Task<int> Generate(IEnumerable<EntityDto> entities, IProgress<RfProgressUpdateDto> progress)
    {
        int count = 0;

        List<EntityDescriptorDto> entityDescriptors = [];
        foreach(EntityDto entity in entities)
        {
            IEnumerable<string> lines = File.ReadAllLines(entity.FilePath);
            lines = lines.SkipWhile(v1 => !v1.StartsWith("public partial class"));

            EntityDescriptorDto entityDescriptor = this.ProjectDescriptor.GetEntityDescriptor(entity);
            entityDescriptor.PluralizedName = this.Pluralizer.Pluralize(entity.Name);

            var propertyLines = lines.SkipWhile(v1 => v1 != "{").Skip(1).TakeWhile(v1 => v1 != "}").Where(v1 => !string.IsNullOrWhiteSpace(v1));
            foreach(string propertyLine in propertyLines)
            {
                EntityPropertyDescriptorDto entityProperty = this.ProjectDescriptor.GetEntityPropertyDescriptor(entityDescriptor, propertyLine);   //new()
                entityDescriptor.Properties.Add(entityProperty);
            }

            entityDescriptors.Add(entityDescriptor);
        }

        var projectFolder = this.ProjectDescriptor.ProjectFolder;

        foreach(var entityDescriptor in entityDescriptors)
        {
            //Model (partial)
            var modelTemplate = this.ProjectDescriptor.GetModelTemplate(entityDescriptor);
            string modelPartialContent = modelTemplate.TransformText();
            string modelPartialFilePath = projectFolder.DataAccess.Models.Partials.GetFilePath($"{entityDescriptor.Entity.Name}.cs");
            await File.WriteAllTextAsync(modelPartialFilePath, modelPartialContent, this.ProjectDescriptor.Encoding);
            progress.Report(new(entityDescriptor.Entity, "Model (partial)"));
            count++;

            //Dto
            var dtoTemplate = this.ProjectDescriptor.GetDtoTemplate(entityDescriptor);
            string dtoContent = dtoTemplate.TransformText();
            string dtoFilePath = projectFolder.Shared.Dtos.GetFilePath($"{entityDescriptor.Name}Dto.cs");
            if(entityDescriptor.IsLookupTable)
                dtoFilePath = projectFolder.Shared.Dtos.Lookups.GetFilePath($"{entityDescriptor.Name}Dto.cs");
            await File.WriteAllTextAsync(dtoFilePath, dtoContent, this.ProjectDescriptor.Encoding);
            progress.Report(new(entityDescriptor.Entity, "Dto"));
            count++;

            //Domain
            var domainTemplate = this.ProjectDescriptor.GetDomainTemplate(entityDescriptor);
            string domainContent = domainTemplate.TransformText();
            string domainFilePath = projectFolder.ServiceLayer.Domains.GetFilePath($"{entityDescriptor.Name}Domain.cs");
            await File.WriteAllTextAsync(domainFilePath, domainContent, this.ProjectDescriptor.Encoding);
            progress.Report(new(entityDescriptor.Entity, "Domain"));
            count++;

            //Controller
            var controllerTemplate = this.ProjectDescriptor.GetControllerTemplate(entityDescriptor);
            string controllerContent = controllerTemplate.TransformText();
            string controllerFilePath = projectFolder.WebApi.Controllers.GetFilePath($"{this.Pluralizer.Pluralize(entityDescriptor.Name)}Controller.cs");
            await File.WriteAllTextAsync(controllerFilePath, controllerContent, this.ProjectDescriptor.Encoding);
            progress.Report(new(entityDescriptor.Entity, "Controller"));
            count++;
        }

        return count;
    }
}
