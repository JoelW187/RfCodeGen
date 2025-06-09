using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.ProjectConfigs.HPMS.Dtos;

public record HpmsEntityDescriptorDto(EntityDto Entity) : EntityDescriptorDto(Entity)
{
    private bool IIdColumn => this.Properties.Any(v1 => v1.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
    private bool IParentSri => this.Properties.Any(v1 => v1.Name.Equals("ParentSri", StringComparison.OrdinalIgnoreCase));
    private bool ICheckout => this.Properties.Any(v1 => v1.Name.Equals("WrkId", StringComparison.OrdinalIgnoreCase) || v1.Name.Equals("ChoutWrkId", StringComparison.OrdinalIgnoreCase));
    private bool IInventory => this.Properties.Any(v1 => v1.Name.Equals("InvDate", StringComparison.OrdinalIgnoreCase));
    private bool IFeature => this.Properties.Any(v1 => v1.Name.Equals("Sri", StringComparison.OrdinalIgnoreCase));
    private bool IPointFeature => IFeature && this.Properties.Any(v1 => v1.Name.Equals("MpStart", StringComparison.OrdinalIgnoreCase));
    private bool ILinearFeature => IFeature && this.Properties.Any(v1 => v1.Name.Equals("MpEnd", StringComparison.OrdinalIgnoreCase));

    public override string DtoInterfaces
    {
        get
        {
            List<string> interfaces = [];

            if(this.IIdColumn) interfaces.Add("IIdColumn");
            if(this.IParentSri) interfaces.Add("IParentSri");
            if(this.ICheckout) interfaces.Add("ICheckout");
            if(this.IInventory) interfaces.Add("IInventory");
            if(this.IPointFeature) interfaces.Add("IPointFeature");
            if(this.ILinearFeature) interfaces.Add("ILinearFeature");

            return string.Join(", ", interfaces);
        }
    }
    public override string DefaultCollectionOrderBy
    {
        get
        {
            List<string> orderBy = [];

            if(this.IFeature)
            {
                orderBy.Add("q => q.OrderBy(entity => entity.Sri)");
                if(this.IPointFeature) orderBy.Add("ThenBy(entity => entity.MpStart)");
                if(this.ILinearFeature) orderBy.Add("ThenBy(entity => entity.MpEnd)");
            }
            else if(this.IIdColumn)
            {
                orderBy.Add("q => q.OrderBy(entity => entity.Id)");
            }

            if(orderBy.Count != 0)
                return string.Join(".", orderBy);
            else
                return "null";
        }
    }
    public override string DebuggerDisplay
    {
        get
        {
            List<string> values = [];

            if(this.PkColumnName != string.Empty)
                values.Add($"{this.PkColumnName}={{{this.PkColumnName}}}");

            if(this.Properties.Any(v1 => v1.Name.Equals("Sri", StringComparison.CurrentCultureIgnoreCase)))
                values.Add($"Sri={{Sri}}");
            if(this.Properties.Any(v1 => v1.Name.Equals("MpStart", StringComparison.CurrentCultureIgnoreCase)))
                values.Add($"MpStart={{MpStart}}");
            if(this.Properties.Any(v1 => v1.Name.Equals("MpEnd", StringComparison.CurrentCultureIgnoreCase)))
                values.Add($"MpEnd={{MpEnd}}");

            string? desc = this.Properties.FirstOrDefault(v1 => v1.Name.StartsWith(this.Name, StringComparison.OrdinalIgnoreCase))?.Name;
            desc ??= this.Properties.FirstOrDefault(v1 => v1.Type.Equals("string", StringComparison.OrdinalIgnoreCase) || v1.Type.Equals("string?", StringComparison.OrdinalIgnoreCase))?.Name;

            if(desc != null)
                values.Add($"{desc}={{{desc}}}");

            desc = string.Join(",", values);

            if(desc != string.Empty)
                desc = $"[DebuggerDisplay(\"{desc}\")]";

            return desc;
        }
    }
}
