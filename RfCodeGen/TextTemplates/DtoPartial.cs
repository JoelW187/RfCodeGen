using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.TextTemplates;

public partial class Dto(EntityDescriptorDto entityDescriptor) : DtoBase
{
    private EntityDescriptorDto EntityDescriptor { get; } = entityDescriptor;
}
