using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.ProjectConfigs.HPMS.Dtos;

public record HpmsEntityPropertyDescriptorDto(EntityDescriptorDto EntityDescriptor, string Text) : EntityPropertyDescriptorDto(EntityDescriptor, Text)
{
    public override bool Required => this.Name.Equals("Sri", StringComparison.OrdinalIgnoreCase) || this.Name.Equals("MpStart", StringComparison.OrdinalIgnoreCase) || this.Name.Equals("MpEnd", StringComparison.OrdinalIgnoreCase);
    public override bool IsPrimaryKey
    {
        get
        {
            return this.Name.Equals("Id", StringComparison.OrdinalIgnoreCase);
        }
    }
}
