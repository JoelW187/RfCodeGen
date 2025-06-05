using RfCodeGen.ServiceLayer.TextTemplates;
using RfCodeGen.ServiceLayer.Utils.Pluralizer;
using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.ServiceLayer;

public class RfCodeGenerator
{
    private Pluralizer Pluralizer { get; } = new();

    public async Task<int> Generate(IEnumerable<EntityDto> entities, ProjectFolder projectFolder, IProgress<string> progress)
    {
        int count = 0;

        List<EntityDescriptorDto> entityDescriptors = [];
        foreach(EntityDto entity in entities)
        {
            IEnumerable<string> lines = File.ReadAllLines(entity.FullName);
            lines = lines.SkipWhile(v1 => !v1.StartsWith("public partial class"));
            //string declaration = lines.First();
            var propertyLines = lines.SkipWhile(v1 => v1 != "{").Skip(1).TakeWhile(v1 => v1 != "}").Where(v1 => !string.IsNullOrWhiteSpace(v1));

            EntityDescriptorDto entityDescriptor = new(entity.Name);    //, declaration);
            foreach(string propertyLine in propertyLines)
            {
                var pieces = propertyLine.Trim().Split(' ');
                string modifier = pieces[0]; // e.g., public, private, protected, internal
                string type = pieces[1]; // e.g., string, int, DateTime, etc.
                string name = pieces[2]; // e.g., MyProperty
                bool get = propertyLine.Contains(" get; ");
                bool set = propertyLine.Contains(" set; ");
                EntityProperty entityProperty = new(modifier, type, name, get, set)
                {
                    Required = name.Equals("Sri", StringComparison.OrdinalIgnoreCase) || name.Equals("MpStart", StringComparison.OrdinalIgnoreCase) || name.Equals("MpEnd", StringComparison.OrdinalIgnoreCase),
                };
                entityDescriptor.Properties.Add(entityProperty);
            }

            entityDescriptor.IIdColumn = entityDescriptor.Properties.Any(v1 => v1.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.IParentSri = entityDescriptor.Properties.Any(v1 => v1.Name.Equals("ParentSri", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.ICheckout = entityDescriptor.Properties.Any(v1 => v1.Name.Equals("WrkId", StringComparison.OrdinalIgnoreCase) || v1.Name.Equals("ChoutWrkId", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.IInventory = entityDescriptor.Properties.Any(v1 => v1.Name.Equals("InvDate", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.IFeature = entityDescriptor.Properties.Any(v1 => v1.Name.Equals("Sri", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.IPointFeature = entityDescriptor.Properties.Any(v1 => entityDescriptor.IFeature && v1.Name.Equals("MpStart", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.ILinearFeature = entityDescriptor.Properties.Any(v1 => entityDescriptor.IFeature && v1.Name.Equals("MpEnd", StringComparison.OrdinalIgnoreCase));

            entityDescriptors.Add(entityDescriptor);
        }

        //Model (partial)
        foreach(EntityDto entity in entities)
        {
            ModelTextTemplate modelPartial = new(entity);
            string modelPartialContent = modelPartial.TransformText();
            string modelPartialFilePath = projectFolder.DataAccess.Models.Partials.GetFilePath($"{entity.Name}Partial.cs");   // Path.Combine(projectFolder.DataAccess.Models.Partials.FullPath, $"{entity.Name}Partial.cs");
            await File.WriteAllTextAsync(modelPartialFilePath, modelPartialContent, Encoding.UTF8);
            progress.Report($"Generated Model partial for {entity.Name}");

            count++;
        }

        //Dto
        foreach(var entityDescriptor in entityDescriptors)
        {
            DtoTextTemplate dto = new(entityDescriptor);
            string dtoContent = dto.TransformText();
            string dtoFilePath = projectFolder.Shared.Dtos.GetFilePath($"{entityDescriptor.Name}Dto.cs");   // Path.Combine(projectFolder.Shared.Dtos.FullPath, $"{entityDescriptor.Name}Dto.cs");
            await File.WriteAllTextAsync(dtoFilePath, dtoContent, Encoding.UTF8);
            progress.Report($"Generated DTO for {entityDescriptor.Name}");

            count++;
        }

        //ServiceLayer domain
        foreach(var entityDescriptor in entityDescriptors)
        {
            DomainTextTemplate domain = new(entityDescriptor);
            string domainContent = domain.TransformText();
            string domainFilePath = projectFolder.ServiceLayer.Domains.GetFilePath($"{entityDescriptor.Name}Domain.cs");
            await File.WriteAllTextAsync(domainFilePath, domainContent, Encoding.UTF8);
            progress.Report($"Generated ServiceLayer domain for {entityDescriptor.Name}");

            count++;
        }

        //Controller
        foreach(var entityDescriptor in entityDescriptors)
        {
            ControllerTextTemplate controller = new(entityDescriptor);
            string controllerContent = controller.TransformText();
            string controllerFilePath = projectFolder.WebApi.Controllers.GetFilePath($"{this.Pluralizer.Pluralize(entityDescriptor.Name)}Controller.cs"); // Path.Combine(projectFolder.WebApi.Controllers.FullPath, $"{this.Pluralizer.Pluralize(entityDescriptor.Name)}Controller.cs");
            await File.WriteAllTextAsync(controllerFilePath, controllerContent, Encoding.UTF8);
            progress.Report($"Generated Controller for {entityDescriptor.Name}");

            count++;
        }

        return count;
    }

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
