using System.Diagnostics;
using System.Xml.Linq;

namespace RfCodeGen.Shared.Dtos;

[DebuggerDisplay("Name={Name}")]
public abstract record EntityDescriptorDto(EntityDto Entity, IEnumerable<EntityDescriptorDto> EntityDescriptors)
{
    public string Name => this.Entity?.Name ?? "";
    public List<EntityPropertyDescriptorDto> Properties { get; } = [];
    public string CamelCaseName => char.ToLowerInvariant(this.Name[0]) + this.Name[1..];
    public string PluralizedName { get; set; } = string.Empty;
    public bool HasChildren => this.Children.Any();

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
    public virtual IEnumerable<EntityDescriptorDto> Children { get; } = [];
    public virtual IEnumerable<string> Includes { get; } = [];
    public virtual string TInclude => string.Empty;
    public virtual string ChildDescriptor => string.Empty;
    public virtual string ChildDescriptors
    {
        get
        {
            return string.Join($",{Environment.NewLine}", this.Children.Select(v1 => v1.ChildDescriptor));
        }
    }
    public virtual bool IsLookupTable { get; }
    public virtual bool IsManyToManyTable { get; }
}

[DebuggerDisplay("Name={Name}")]
public abstract record EntityPropertyDescriptorDto
{
    public EntityPropertyDescriptorDto(EntityDescriptorDto entityDescriptor, string text)
    {
        this.EntityDescriptor = entityDescriptor;
        this.Text = text.Trim();

        ParseText(this.Text, out string modifiers, out string type, out string name, out bool get, out bool set, out string assignment);

        this.Modifiers = modifiers;
        this.Type = type;
        this.Name = name;
        this.Get = get;
        this.Set = set;
        this.Assignment = assignment;
    }

    public EntityDescriptorDto EntityDescriptor { get; }
    public string Text { get; }
    public string Modifiers { get; } = string.Empty;
    public string Type { get; } = string.Empty;
    public string Name { get; } = string.Empty;
    public bool Get { get; }
    public bool Set { get; }
    public string Assignment { get; } = string.Empty;

    public virtual bool Required { get; }
    public virtual bool IsPrimaryKey { get; }

    public virtual void ParseText(string line, out string modifiers, out string type, out string name, out bool get, out bool set, out string assignment)
    {
        line = line.Trim();

        var pieces = line.Split('=');
        assignment = string.Empty;
        if(pieces.Length == 2)
        {
            assignment = $" = {pieces[1].Trim()}";
            line = pieces[0].Trim(); // take the first part before the '='
        }

        var i = line.IndexOf('{');
        string definition = line[..i].Trim();
        string getset = line[i..].Trim();

        pieces = definition.Trim().Split(' ');
        name = pieces.Last();
        type = pieces[^2];
        modifiers = string.Join(' ', pieces.Take(pieces.Length - 2)); // take all but the last two pieces

        get = getset.Contains("get;");
        set = getset.Contains("set;");

        if(type.StartsWith("ICollection<"))
        {
            var collectionType = type[12..^1]; // e.g., ICollection<MyEntity>
            type = type.Replace($"<{collectionType}>", $"<{collectionType}Dto>");

            if(assignment != string.Empty)
                assignment = " = [];";
        }
    }
}