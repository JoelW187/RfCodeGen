using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.ProjectConfigs.CDMS.TextTemplates;

public partial class ControllerTextTemplate(ProjectDescriptorDto projectDescriptor, EntityDescriptorDto entityDescriptor) : ITextTemplate
{
    public ProjectDescriptorDto ProjectDescriptor { get; } = projectDescriptor;
    public EntityDescriptorDto EntityDescriptor { get; } = entityDescriptor;
}
