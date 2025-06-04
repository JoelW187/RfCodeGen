using RfCodeGen.Shared.Dtos;
using RfCodeGen.TextTemplates;
using System.IO;
using System.Text;
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

    private void Generate_Click(object sender, RoutedEventArgs e)
    {
        //Model Partial
        foreach(EntityDto entity in this.ViewModel.Entities.Where(v1 => v1.IsSelected))
        {
            ModelPartial modelPartial = new(entity);
            string modelPartialContent = modelPartial.TransformText();
            string partialFileName = Path.Combine(this.ViewModel.ProjectFolder.DataAccess.Models.Partials.FullName, entity.FileName);
            File.WriteAllText(partialFileName, modelPartialContent, Encoding.UTF8);
        }

        //Dto
        List<EntityDescriptorDto> entityDescriptors = [];
        foreach(EntityDto entity in this.ViewModel.Entities.Where(v1 => v1.IsSelected))
        {
            IEnumerable<string> lines = File.ReadAllLines(entity.FullName);
            lines = lines.SkipWhile(v1 => !v1.StartsWith("public partial class"));
            //string declaration = lines.First();
            var propertyLines = lines.SkipWhile(v1 => v1 != "{").Skip(1).TakeWhile(v1 => v1 != "}").Where(v1 => !string.IsNullOrWhiteSpace(v1));

            EntityDescriptorDto entityDescriptor = new(entity.Name);    //, declaration);
            foreach(string propertyLine in propertyLines)
            {
                var pieces = propertyLine.Trim().Split(' ');
                string modifier = pieces[0]; // e.g., public, private, protected, internal
                string type = pieces[1]; // e.g., string, int, DateTime, etc.
                string name = pieces[2]; // e.g., MyProperty
                bool get = propertyLine.Contains(" get; ");
                bool set = propertyLine.Contains(" set; ");
                EntityProperty entityProperty = new(modifier, type, name, get, set)
                {
                    Required = name.Equals("Sri", StringComparison.OrdinalIgnoreCase) || name.Equals("MpStart", StringComparison.OrdinalIgnoreCase) || name.Equals("MpEnd", StringComparison.OrdinalIgnoreCase),
                };
                entityDescriptor.Properties.Add(entityProperty);
            }

            entityDescriptor.IIdColumn = entityDescriptor.Properties.Any(v1 => v1.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.IParentSri = entityDescriptor.Properties.Any(v1 => v1.Name.Equals("ParentSri", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.ICheckout = entityDescriptor.Properties.Any(v1 => v1.Name.Equals("WrkId", StringComparison.OrdinalIgnoreCase) || v1.Name.Equals("ChoutWrkId", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.IInventory = entityDescriptor.Properties.Any(v1 => v1.Name.Equals("InvDate", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.IFeature = entityDescriptor.Properties.Any(v1 => v1.Name.Equals("Sri", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.IPointFeature = entityDescriptor.Properties.Any(v1 => entityDescriptor.IFeature && v1.Name.Equals("MpStart", StringComparison.OrdinalIgnoreCase));
            entityDescriptor.ILinearFeature = entityDescriptor.Properties.Any(v1 => entityDescriptor.IFeature && v1.Name.Equals("MpEnd", StringComparison.OrdinalIgnoreCase));

            entityDescriptors.Add(entityDescriptor);
        }

        foreach(var entityDescriptor in entityDescriptors)
        {
            Dto dto=new(entityDescriptor);
            string dtoContent = dto.TransformText();
            string dtoFileName = Path.Combine(this.ViewModel.ProjectFolder.Shared.Dtos.FullName, $"{entityDescriptor.Name}Dto.cs");
            File.WriteAllText(dtoFileName, dtoContent, Encoding.UTF8);
        }

        //ServiceLayer domain
        foreach(var entityDescriptor in entityDescriptors)
        {
            Domain domain = new(entityDescriptor);
            string domainContent = domain.TransformText();
            string domainFileName = Path.Combine(this.ViewModel.ProjectFolder.ServiceLayer.Domain.FullName, $"{entityDescriptor.Name}Domain.cs");
            File.WriteAllText(domainFileName, domainContent, Encoding.UTF8);
        }
    }
}
