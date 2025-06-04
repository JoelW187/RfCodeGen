using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.Shared.Dtos;

public record EntityDescriptorDto(string Name)
{
    public List<EntityProperty> Properties { get; init; } = [];
    public bool IIdColumn { get; set; }
    public bool IParentSri { get; set; }
    public bool ICheckout { get; set; }
    public bool IInventory { get; set; }
    public bool IFeature { get; set; }
    public bool IPointFeature { get; set; }
    public bool ILinearFeature { get; set; }
    public string CamelCaseName => char.ToLowerInvariant(this.Name[0]) + this.Name[1..];

    public string DtoInterfaces
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

    public string DomainOrderBy
    {
        get
        {
            List<string> orderBy = [];

            if(this.IFeature)
            {
                orderBy.Add("q => q.OrderBy(entity => entity.Sri)");
                if(this.IPointFeature) orderBy.Add("ThenBy(entity => entity.MpStart)");
                if(this.ILinearFeature) orderBy.Add("ThenBy(entity => entity.MpEnd)");
            } else if(this.IIdColumn)
            {
                orderBy.Add("q => q.OrderBy(entity => entity.Id)");
            }

            if(orderBy.Count != 0)
                return string.Join(".", orderBy);
            else
                return "null";
        }
    }
}

public record EntityProperty(string Modifier, string Type, string Name, bool Get, bool Set)
{
    public bool Required { get; set; }
}