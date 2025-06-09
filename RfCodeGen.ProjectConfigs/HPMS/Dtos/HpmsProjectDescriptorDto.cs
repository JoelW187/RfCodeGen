using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.ProjectConfigs.HPMS.Dtos;

public record HpmsProjectDescriptorDto(string ProjectName, string ProjectPath) : ProjectDescriptorDto("HPMS", ProjectName, ProjectPath, "HPMS.", "HPMS.")
{
    public override ITextTemplate GetModelTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.HPMS.ModelTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDtoTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.HPMS.DtoTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDomainTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.HPMS.DomainTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetControllerTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.HPMS.ControllerTextTemplate(this, entityDescriptor);

    public override EntityDescriptorDto GetEntityDescriptor(EntityDto entity)
    {
        return new HpmsEntityDescriptorDto(entity);
    }
    public override EntityPropertyDescriptorDto GetEntityPropertyDescriptor(EntityDescriptorDto entityDescriptor, string text)
    {
        return new HpmsEntityPropertyDescriptorDto(entityDescriptor, text);
    }
}
