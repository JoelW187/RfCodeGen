﻿using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.ProjectConfigs.HPMS.Dtos;

public record HpmsProjectDescriptorDto(string ProjectPath) : ProjectDescriptorDto("HPMS", ProjectPath, "HPMS.", "HPMS.")
{
    public static string Id => "HPMS";

    public override ITextTemplate GetModelTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.HPMS.TextTemplates.ModelTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDtoTemplate(EntityDescriptorDto entityDescriptor) => entityDescriptor.IsLookupTable ? new RfCodeGen.ProjectConfigs.HPMS.TextTemplates.DtoLookupTextTemplate(this, entityDescriptor) : new RfCodeGen.ProjectConfigs.HPMS.TextTemplates.DtoTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDomainTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.HPMS.TextTemplates.DomainTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetControllerTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.HPMS.TextTemplates.ControllerTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetRfControllerTestTemplate(EntityDescriptorDto entityDescriptor) => entityDescriptor.IsLookupTable ? new RfCodeGen.ProjectConfigs.HPMS.TextTemplates.LookupsControllerTestTextTemplate(this, entityDescriptor) : new RfCodeGen.ProjectConfigs.HPMS.TextTemplates.RfControllerTestTextTemplate(this, entityDescriptor);

    public override EntityDescriptorDto GetEntityDescriptor(EntityDto entity, IEnumerable<EntityDescriptorDto> EntityDescriptors)
    {
        return new HpmsEntityDescriptorDto(entity, EntityDescriptors);
    }
    public override EntityPropertyDescriptorDto GetEntityPropertyDescriptor(EntityDescriptorDto entityDescriptor, string text)
    {
        return new HpmsEntityPropertyDescriptorDto(entityDescriptor, text);
    }
}
