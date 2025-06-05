using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.Shared.Dtos;

public abstract record EntityDescriptorDto()
{
    public string Name { get; set; } = string.Empty;
    public List<EntityPropertyDescriptorDto> Properties { get; init; } = [];
    public string CamelCaseName => char.ToLowerInvariant(this.Name[0]) + this.Name[1..];
    
    public abstract string DtoInterfaces { get; }

    public abstract string DefaultCollectionOrderBy { get; }
}

public abstract record EntityPropertyDescriptorDto()
{
    public string Modifier { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool Get { get; set; }
    public bool Set { get; set; }

    public abstract bool Required { get; }
}