using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.TextTemplates;

public partial class Domain(EntityDescriptorDto entityDescriptor) : DomainBase
{
    private EntityDescriptorDto EntityDescriptor { get; set; } = entityDescriptor;
}
