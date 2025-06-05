namespace RfCodeGen.Shared.Dtos;

public record EntityDescriptorDto()
{
    public string Name => this.Entity?.Name ?? "";
    public List<EntityPropertyDescriptorDto> Properties { get; } = [];
    public string CamelCaseName => char.ToLowerInvariant(this.Name[0]) + this.Name[1..];
    public string PluralizedName { get; set; } = string.Empty;
    public EntityDto Entity { get; set; } = null!;

    public virtual ITextTemplate GetModelTemplate() { throw new NotImplementedException(); }
    public virtual ITextTemplate GetDtoTemplate() { throw new NotImplementedException(); }
    public virtual ITextTemplate GetDomainTemplate() { throw new NotImplementedException(); }
    public virtual ITextTemplate GetControllerTemplate() { throw new NotImplementedException(); }

    public virtual string DtoInterfaces { get; } = string.Empty;
    public virtual string DefaultCollectionOrderBy { get; } = string.Empty;
    public virtual string Includes { get; } = string.Empty;
    public virtual bool IsLookupTable { get; }
}

public record EntityPropertyDescriptorDto()
{
    public string Modifier { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Get { get; set; }
    public bool Set { get; set; }

    public virtual bool Required { get; }
}