﻿using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.ProjectConfigs.HPMS.TextTemplates;

public partial class DomainTextTemplate(ProjectDescriptorDto projectDescriptor, EntityDescriptorDto entityDescriptor) : ITextTemplate
{
    public ProjectDescriptorDto ProjectDescriptor { get; } = projectDescriptor;
    public EntityDescriptorDto EntityDescriptor { get; set; } = entityDescriptor;
}
