using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.Shared.Dtos;

public record EntityDescriptorDto()
{
    public string Name { get; set; } = string.Empty;
    public List<EntityPropertyDescriptorDto> Properties { get; init; } = [];
    public string CamelCaseName => char.ToLowerInvariant(this.Name[0]) + this.Name[1..];
    
    public virtual string DtoInterfaces { get; } = string.Empty;

    public virtual string DefaultCollectionOrderBy { get; } = string.Empty;
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