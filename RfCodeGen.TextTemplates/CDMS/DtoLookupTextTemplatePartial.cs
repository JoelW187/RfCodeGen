using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.TextTemplates.CDMS;

public partial class DtoLookupTextTemplate(ProjectDescriptorDto projectDescriptor, EntityDescriptorDto entityDescriptor) : ITextTemplate
{
    public ProjectDescriptorDto ProjectDescriptor { get; } = projectDescriptor;
    public EntityDescriptorDto EntityDescriptor { get; } = entityDescriptor;
}
