using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.ProjectConfigs.CDMS.Dtos;

public record CdmsProjectDescriptorDto(string ProjectId, string ProjectPath, string ProjectPathPrefix, string ProjectNamespacePrefix) : ProjectDescriptorDto(ProjectId, ProjectPath, ProjectPathPrefix, ProjectNamespacePrefix)
{
    public static string Id => "CDMS";

    public override ITextTemplate GetModelTemplate(EntityDescriptorDto entityDescriptor) => entityDescriptor.IsLookupTable ? new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.ModelLookupTextTemplate(this, entityDescriptor) : new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.ModelTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDtoTemplate(EntityDescriptorDto entityDescriptor) => entityDescriptor.IsLookupTable ? new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.DtoLookupTextTemplate(this, entityDescriptor) : new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.DtoTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDomainTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.DomainTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetControllerTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.ControllerTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetRfControllerTestTemplate(EntityDescriptorDto entityDescriptor) => entityDescriptor.IsLookupTable ? new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.LookupsControllerTestTextTemplate(this, entityDescriptor) : new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.RfControllerTestTextTemplate(this, entityDescriptor);

    public override EntityDescriptorDto GetEntityDescriptor(EntityDto entity, IEnumerable<EntityDescriptorDto> EntityDescriptors)
    {
        return new CdmsEntityDescriptorDto(entity, EntityDescriptors);
    }
    public override EntityPropertyDescriptorDto GetEntityPropertyDescriptor(EntityDescriptorDto entityDescriptor, string text)
    {
        return new CdmsEntityPropertyDescriptorDto(entityDescriptor, text);
    }
}
