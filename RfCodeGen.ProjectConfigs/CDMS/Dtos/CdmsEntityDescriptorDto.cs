using RfCodeGen.ProjectConfigs.Utils.Pluralizer;
using RfCodeGen.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.ProjectConfigs.CDMS.Dtos;

public record CdmsEntityDescriptorDto(EntityDto Entity, IEnumerable<EntityDescriptorDto> EntityDescriptors) : EntityDescriptorDto(Entity, EntityDescriptors)
{
    public Pluralizer Pluralizer { get; } = new();

    public override List<EntityPropertyDescriptorDto> DtoProperties
    {
        get
        {
            if(!this.IsLookupTable)
            {
                return [.. this.Properties.Where(v1 => (!v1.Modifiers.Contains("virtual") || (v1.Type.StartsWith("ICollection<") && !v1.Name.EndsWith("Navigation") && !v1.Name.EndsWith("Navigations")))
                    && !v1.Name.Equals("CreatedDate", StringComparison.CurrentCultureIgnoreCase)
                    && !v1.Name.Equals("CreatedBy", StringComparison.CurrentCultureIgnoreCase)
                    && !v1.Name.Equals("ModifiedDate", StringComparison.CurrentCultureIgnoreCase)
                    && !v1.Name.Equals("ModifiedBy", StringComparison.CurrentCultureIgnoreCase)
                )];
            }
            else
            {
                return [.. this.Properties.Where(v1 => !v1.Modifiers.Contains("virtual")
                    && !v1.Name.Equals("CreatedDate", StringComparison.CurrentCultureIgnoreCase)
                    && !v1.Name.Equals("CreatedBy", StringComparison.CurrentCultureIgnoreCase)
                    && !v1.Name.Equals("ModifiedDate", StringComparison.CurrentCultureIgnoreCase)
                    && !v1.Name.Equals("ModifiedBy", StringComparison.CurrentCultureIgnoreCase)
                    && !v1.Name.Equals("Description", StringComparison.CurrentCultureIgnoreCase)
                    && !v1.Name.Equals("SortOrder", StringComparison.CurrentCultureIgnoreCase)
                    && !v1.Name.Equals("ActiveInd", StringComparison.CurrentCultureIgnoreCase)
                )];
            }
        }
    }
    public override IEnumerable<EntityDescriptorDto> Children
    {
        get
        {
            List<EntityDescriptorDto> list = [];
            string entityName;

            var names = this.Properties.Where(v1 => (v1.Modifiers.Contains("virtual") && (v1.Type.StartsWith("ICollection<") && !v1.Name.EndsWith("Navigation") && !v1.Name.EndsWith("Navigations")))).Select(v1 => v1.Name);
            foreach(var name in names)
            {
                entityName = name;
                if(entityName.EndsWith(this.PluralizedName))
                    entityName = name[..^this.PluralizedName.Length]; //remove the pluralized name from the end
                
                entityName = this.Pluralizer.Singularize(entityName); //singularize the name
                var childEntity = this.EntityDescriptors.FirstOrDefault(v1 => v1.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidOperationException($"Entity '{this.Name}' has a collection property '{name}' that does not match any entity in the project. Expected singular name: '{entityName}'.");
                list.Add(childEntity);
            }

            return list;
        }
    }
    public override IEnumerable<string> Includes
    {
        get
        {
            return [.. this.Children.Select(v1 => this.Pluralizer.Pluralize(v1.Name))];
            //Pluralizer pluralizer = new();
            //List<string> list = [];

            //var names = this.Properties.Where(v1 => (v1.Modifiers.Contains("virtual") && (v1.Type.StartsWith("ICollection<") && !v1.Name.EndsWith("Navigation") && !v1.Name.EndsWith("Navigations")))).Select(v1 => v1.Name);
            //foreach(var name in names)
            //{
            //    if(name.EndsWith(this.PluralizedName))
            //        list.Add(pluralizer.Pluralize(name[..^this.PluralizedName.Length])); //remove the pluralized name from the end
            //    else
            //        list.Add(name); //keep the name as is
            //}

            //return list;
        }
    }
    public override string TInclude => !this.Includes.Any() ? "string" : $"{this.Name}Include";
    public override string ChildDescriptor => $"ChildDescriptor.Create<CdmsContext, ICdmsRepository<{this.Name}>, {this.Name}, {this.Name}Dto, {this.Name}Domain, {this.PluralizedName}Controller, {this.TInclude}>(typeof(I{this.Name}Domain))";
    public override bool IsLookupTable
    {
        get
        {
            return this.Properties.Any(v1 => v1.Name.Equals($"{this.Name}Id", StringComparison.OrdinalIgnoreCase))
                && this.Properties.Any(v1 => v1.Name.Equals($"{this.Name}Ak", StringComparison.OrdinalIgnoreCase))
                && this.Properties.Any(v1 => v1.Name.Equals("Description", StringComparison.OrdinalIgnoreCase))
                && this.Properties.Any(v1 => v1.Name.Equals("SortOrder", StringComparison.OrdinalIgnoreCase))
                && this.Properties.Any(v1 => v1.Name.Equals("ActiveInd", StringComparison.OrdinalIgnoreCase));
        }
    }
    public override bool IsManyToManyTable
    {
        get
        {
            int i = this.Name.IndexOf("And");
            if(i < 1) return false; //the table name can't start with "And", so we check for index < 1
            if(this.Name.EndsWith("And") && this.Name.LastIndexOf("And") == i) return false; //the table name can't end with "And" if it's the only one

            return true;
        }
    }
    public override string DebuggerDisplay
    {
        get
        {
            if(!this.IsLookupTable)
                return base.DebuggerDisplay;

            string dd = $"[DebuggerDisplay(\"{this.Name}Id={{{this.Name}Id}},{this.Name}Ak={{{this.Name}Ak}},Description={{Description}}\")]\r\n";

            return dd;
        }
    }
}
