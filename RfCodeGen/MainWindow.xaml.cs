using RfCodeGen.ServiceLayer;
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

        RfCodeGenerator codeGenerator = new();
        var count = await codeGenerator.Generate(selectedEntities, this.ViewModel.ProjectFolder, progress);

        this.ViewModel.Messages.Add($"Generated {count} files.");

        var domainServiceRegistrations = RfCodeGenerator.GetDomainServiceRegistrations(selectedEntities);
        var autoMapperMappingProfiles = RfCodeGenerator.GetAutoMapperMappingProfiles(selectedEntities);

        this.ViewModel.AuxGenCode = string.Join("\n\n", domainServiceRegistrations) + "\n\n" + string.Join("\n\n", autoMapperMappingProfiles);
    }
}
