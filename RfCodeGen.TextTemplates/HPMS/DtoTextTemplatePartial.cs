using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.TextTemplates.HPMS;

public partial class DtoTextTemplate(EntityDescriptorDto entityDescriptor) : ITextTemplate
{
    public EntityDescriptorDto EntityDescriptor { get; } = entityDescriptor;
}
