using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.TextTemplates.CDMS;

public partial class DomainTextTemplate(EntityDescriptorDto entityDescriptor) : ITextTemplate
{
    public EntityDescriptorDto EntityDescriptor { get; set; } = entityDescriptor;
}
