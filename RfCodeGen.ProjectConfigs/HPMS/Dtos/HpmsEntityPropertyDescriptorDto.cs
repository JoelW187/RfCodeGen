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
    public override void ParseText(string line, out string modifiers, out string type, out string name, out bool get, out bool set, out string assignment)
    {
        base.ParseText(line, out modifiers, out type, out name, out get, out set, out assignment);

        //In the database the F_SYSTEM table has non-nullable MpStart and MpEnd but we need the to be nullable to support the ILinearFeature interface
        if(this.EntityDescriptor.Name=="FSystem" && (name.Equals("MpStart") || name.Equals("MpEnd")) && !type.EndsWith('?'))
        {
            type += "?";
        }
    }
}
