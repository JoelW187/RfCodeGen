﻿using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.ProjectConfigs.CDMS.TextTemplates;

public partial class ModelTextTemplate(ProjectDescriptorDto projectDescriptor, EntityDescriptorDto entityDescriptor) : ITextTemplate
{
    public ProjectDescriptorDto ProjectDescriptor { get; } = projectDescriptor;
    public EntityDescriptorDto EntityDescriptor { get; } = entityDescriptor;
}
