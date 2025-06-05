using RfCodeGen.ServiceLayer;
using RfCodeGen.Shared.Dtos;
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

        RfCodeGenerator<HPMSEntityDescriptorDto, HPMSEntityPropertyDescriptorDto> codeGenerator = new();
        var count = await codeGenerator.Generate(selectedEntities, this.ViewModel.ProjectFolder, progress);

        this.ViewModel.Messages.Add($"Generated {count} files.");

        var domainServiceRegistrations = codeGenerator.GetDomainServiceRegistrations(selectedEntities);
        var autoMapperMappingProfiles = codeGenerator.GetAutoMapperMappingProfiles(selectedEntities);

        this.ViewModel.AuxGenCode = string.Join("\n\n", domainServiceRegistrations) + "\n\n" + string.Join("\n\n", autoMapperMappingProfiles);
    }

    private record HPMSEntityDescriptorDto : EntityDescriptorDto
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
    }

    private record HPMSEntityPropertyDescriptorDto : EntityPropertyDescriptorDto
    {
        public override bool Required => this.Name.Equals("Sri", StringComparison.OrdinalIgnoreCase) || this.Name.Equals("MpStart", StringComparison.OrdinalIgnoreCase) || this.Name.Equals("MpEnd", StringComparison.OrdinalIgnoreCase);
    }
}
