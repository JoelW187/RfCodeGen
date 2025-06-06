namespace RfCodeGen.Shared.Dtos;

public record EntityDescriptorDto()
{
    public string Name => this.Entity?.Name ?? "";
    public List<EntityPropertyDescriptorDto> Properties { get; } = [];
    public string CamelCaseName => char.ToLowerInvariant(this.Name[0]) + this.Name[1..];
    public string PluralizedName { get; set; } = string.Empty;
    public EntityDto Entity { get; set; } = null!;

    public virtual string PkColumnName
    {
        get
        {
            return this.Properties.FirstOrDefault(v1 => v1.IsPrimaryKey)?.Name ?? "";
        }
    }
    public virtual string DebuggerDisplay
    {
        get
        {
            List<string> values = [];

            if(this.PkColumnName != string.Empty)
                values.Add($"{this.PkColumnName}={{{this.PkColumnName}}}");

            string? desc = this.Properties.FirstOrDefault(v1 => v1.Type.Equals("string", StringComparison.OrdinalIgnoreCase) || v1.Type.Equals("string?", StringComparison.OrdinalIgnoreCase))?.Name;
            if(desc != null)
                values.Add($"{desc}={{{desc}}}");

            desc = string.Join(",", values);

            if(desc != string.Empty)
                desc = $"[DebuggerDisplay(\"{desc}\")]";

            return desc;
        }
    }
    public virtual List<EntityPropertyDescriptorDto> DtoProperties => this.Properties;
    public virtual string DtoInterfaces { get; } = string.Empty;
    public virtual string DefaultCollectionOrderBy { get; } = string.Empty;
    public List<string> Includes { get; set; } = [];
    public virtual bool IsLookupTable { get; }
    public virtual bool IsManyToManyTable { get; }
}

public record EntityPropertyDescriptorDto()
{
    public EntityDescriptorDto EntityDescriptor { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public string Modifiers { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Get { get; set; }
    public bool Set { get; set; }
    public string Assignment { get; set; } = string.Empty;

    public virtual bool Required { get; }
    public virtual bool IsPrimaryKey { get; }
}