using RfCodeGen.ServiceLayer;
using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;
using RfCodeGen.ProjectConfigs.CDMS.Dtos;
using System.Windows;
using RfCodeGen.ProjectConfigs.HPMS.Dtos;

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
        this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;

        this.ViewModel.ProjectDescriptors.Add(new HpmsProjectDescriptorDto("NJDOT HPMS", @"C:\Source\mbakerintlapps\NJDOT\NJDOT_HPMS\src\NJDOT_HPMS"));
        this.ViewModel.ProjectDescriptors.Add(new CdmsProjectDescriptorDto("CDMS Cost Recovery", @"C:\Source\mbakerintlapps\Alaska\CDMS\CostRecovery\WebApi\"));

        var selectedProjectId = Properties.Settings.Default.SelectedProjectId;

        this.ViewModel.SelectedProjectDescriptor = this.ViewModel.ProjectDescriptors.FirstOrDefault(v1 => v1.ProjectId == selectedProjectId) ?? this.ViewModel.ProjectDescriptors.FirstOrDefault();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {             
        case nameof(MainViewModel.SelectedProjectDescriptor):
            if (this.ViewModel.SelectedProjectDescriptor != null)
            {
                Properties.Settings.Default.SelectedProjectId = this.ViewModel.SelectedProjectDescriptor.ProjectId;
                Properties.Settings.Default.Save();
            }
            break;
        }
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

        var codeGenerator = new RfCodeGenerator(this.ViewModel.SelectedProjectDescriptor!);
        var count = await codeGenerator.Generate(selectedEntities, progress);

        this.ViewModel.Messages.Add($"Generated {count} files.");

        var domainServiceRegistrations = RfCodeGeneratorBase.GetDomainServiceRegistrations(selectedEntities);
        var autoMapperMappingProfiles = RfCodeGeneratorBase.GetAutoMapperMappingProfiles(selectedEntities);

        this.ViewModel.AuxGenCode = string.Join("\n\n", domainServiceRegistrations) + "\n\n" + string.Join("\n\n", autoMapperMappingProfiles);
    }
}






