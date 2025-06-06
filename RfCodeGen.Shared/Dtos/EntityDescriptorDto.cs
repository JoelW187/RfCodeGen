namespace RfCodeGen.Shared.Dtos;

public record EntityDescriptorDto()
{
    public string Name => this.Entity?.Name ?? "";
    public List<EntityPropertyDescriptorDto> Properties { get; } = [];
    public string CamelCaseName => char.ToLowerInvariant(this.Name[0]) + this.Name[1..];
    public string PluralizedName { get; set; } = string.Empty;
    public EntityDto Entity { get; set; } = null!;

    //these can be overridden in derived classes to provide specific behavior
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

    public virtual string DtoInterfaces { get; } = string.Empty;
    public virtual string DefaultCollectionOrderBy { get; } = string.Empty;
    public virtual string Includes { get; } = string.Empty;
    public virtual bool IsLookupTable { get; }

    //these must be implemented in derived classes to provide specific templates
    public virtual ITextTemplate GetModelTemplate() { throw new NotImplementedException(); }
    public virtual ITextTemplate GetDtoTemplate() { throw new NotImplementedException(); }
    public virtual ITextTemplate GetDomainTemplate() { throw new NotImplementedException(); }
    public virtual ITextTemplate GetControllerTemplate() { throw new NotImplementedException(); }
}

public record EntityPropertyDescriptorDto()
{
    public string Modifier { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Get { get; set; }
    public bool Set { get; set; }

    public virtual bool Required { get; }
    public virtual bool IsPrimaryKey { get; }
}