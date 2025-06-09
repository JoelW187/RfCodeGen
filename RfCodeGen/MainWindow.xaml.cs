using RfCodeGen.ServiceLayer;
using RfCodeGen.ServiceLayer.Utils.Pluralizer;
using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;
using System.Net.WebSockets;
using System.Windows;

namespace RfCodeGen;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainViewModel ViewModel { get; set; } = null!;

    public MainWindow()
    {
        this.DataContextChanged += MainWindow_DataContextChanged;
        InitializeComponent();
    }

    private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if(this.DataContext is not MainViewModel vm) return;

        this.ViewModel = vm;

        this.ViewModel.ProjectDescriptors.Add(new HpmsProjectDescriptorDto("HPMS", "NJDOT HPMS", @"C:\Source\mbakerintlapps\NJDOT\NJDOT_HPMS\src\NJDOT_HPMS", "HPMS.", "HPMS."));
        //this.ViewModel.ProjectDescriptors.Add(new CdmsProjectDescriptorDto("CDMS", "CDMS Cost Recovery", @"C:\Source\mbakerintlapps\Alaska\CDMS\CostRecovery\WebApi\", "CostRecovery.", "CDMS.CostRecovery."));
        this.ViewModel.SelectedProjectDescriptor = this.ViewModel.ProjectDescriptors.FirstOrDefault();
    }

    private void SelectAll_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel.Entities.ToList().ForEach(entity => entity.IsSelected = true);
    }

    private void DeselectAll_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel.Entities.ToList().ForEach(entity => entity.IsSelected = false);
    }

    private void SelectNew_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel.Entities.Where(v1 => !v1.HasPartial).ToList().ForEach(entity => entity.IsSelected = true);
    }

    private void SelectExisting_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel.Entities.Where(v1 => v1.HasPartial).ToList().ForEach(entity => entity.IsSelected = true);
    }

    private void SelectInvert_Click(object sender, RoutedEventArgs e)
    {
        this.ViewModel.Entities.ToList().ForEach(entity => entity.IsSelected = !entity.IsSelected);
    }

    private async void Generate_Click(object sender, RoutedEventArgs e)
    {
        var progress = new Progress<string>(message =>
        {
            this.ViewModel.Messages.Add(message);
        });

        var selectedEntities = this.ViewModel.Entities.Where(v1 => v1.IsSelected).OrderBy(v1 => v1.Name).ToList();

        var codeGenerator = RfCodeGeneratorFactory.Create(this.ViewModel.SelectedProjectDescriptor!.ProjectId);
        var count = await codeGenerator.Generate(selectedEntities, this.ViewModel.SelectedProjectDescriptor!, progress);

        this.ViewModel.Messages.Add($"Generated {count} files.");

        var domainServiceRegistrations = RfCodeGeneratorBase.GetDomainServiceRegistrations(selectedEntities);
        var autoMapperMappingProfiles = RfCodeGeneratorBase.GetAutoMapperMappingProfiles(selectedEntities);

        this.ViewModel.AuxGenCode = string.Join("\n\n", domainServiceRegistrations) + "\n\n" + string.Join("\n\n", autoMapperMappingProfiles);
    }
}

public static class RfCodeGeneratorFactory
{
    public static IRfCodeGenerator<EntityDescriptorDto, EntityPropertyDescriptorDto> Create(string projectId)
    {
        return projectId switch
        {
            "HPMS" => new RfCodeGenerator<HpmsEntityDescriptorDto, HpmsEntityPropertyDescriptorDto>(),
            "CDMS" => new RfCodeGenerator<CdmsEntityDescriptorDto, CdmsEntityPropertyDescriptorDto>(),
            _ => throw new NotSupportedException($"Project ID '{projectId}' is not supported."),
        };
    }
}

internal record CdmsProjectDescriptorDto(string ProjectId, string ProjectName, string ProjectPath, string ProjectPathPrefix, string ProjectNamespacePrefix) : ProjectDescriptorDto(ProjectId, ProjectName, ProjectPath, ProjectPathPrefix, ProjectNamespacePrefix)
{
    public override ITextTemplate GetModelTemplate(EntityDescriptorDto entityDescriptor) => entityDescriptor.IsLookupTable ? new RfCodeGen.ProjectConfigs.CDMS.ModelLookupTextTemplate(this, entityDescriptor) : new RfCodeGen.ProjectConfigs.CDMS.ModelTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDtoTemplate(EntityDescriptorDto entityDescriptor) => entityDescriptor.IsLookupTable ? new RfCodeGen.ProjectConfigs.CDMS.DtoLookupTextTemplate(this, entityDescriptor) : new RfCodeGen.ProjectConfigs.CDMS.DtoTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDomainTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.CDMS.DomainTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetControllerTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.CDMS.ControllerTextTemplate(this, entityDescriptor);
}

internal record CdmsEntityDescriptorDto : EntityDescriptorDto
{
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
    public override List<string> Includes
    {
        get
        {
            Pluralizer pluralizer = new();
            List<string> list = [];

            var names = this.Properties.Where(v1 => (v1.Modifiers.Contains("virtual") && (v1.Type.StartsWith("ICollection<") && !v1.Name.EndsWith("Navigation") && !v1.Name.EndsWith("Navigations")))).Select(v1 => v1.Name);
            foreach(var name in names)
            {
                if(name.EndsWith(this.PluralizedName))
                    list.Add(pluralizer.Pluralize(name[..^this.PluralizedName.Length])); //remove the pluralized name from the end
                else
                    list.Add(name); //keep the name as is
            }

            return list;
        }
    }
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

internal record CdmsEntityPropertyDescriptorDto : EntityPropertyDescriptorDto
{
    public override bool IsPrimaryKey
    {
        get
        {
            return this.Name.Equals($"{this.EntityDescriptor.Name}Id", StringComparison.OrdinalIgnoreCase);
        }
    }
}

internal record HpmsProjectDescriptorDto(string ProjectId, string ProjectName, string ProjectPath, string ProjectPathPrefix, string ProjectNamespacePrefix) : ProjectDescriptorDto(ProjectId, ProjectName, ProjectPath, ProjectPathPrefix, ProjectNamespacePrefix)
{
    public override ITextTemplate GetModelTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.HPMS.ModelTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDtoTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.HPMS.DtoTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDomainTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.HPMS.DomainTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetControllerTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.HPMS.ControllerTextTemplate(this, entityDescriptor);
}

internal record HpmsEntityDescriptorDto : EntityDescriptorDto
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

internal record HpmsEntityPropertyDescriptorDto : EntityPropertyDescriptorDto
{
    public override bool Required => this.Name.Equals("Sri", StringComparison.OrdinalIgnoreCase) || this.Name.Equals("MpStart", StringComparison.OrdinalIgnoreCase) || this.Name.Equals("MpEnd", StringComparison.OrdinalIgnoreCase);
    public override bool IsPrimaryKey   
    {
        get
        {
            return this.Name.Equals("Id", StringComparison.OrdinalIgnoreCase);
        }
    }
}
