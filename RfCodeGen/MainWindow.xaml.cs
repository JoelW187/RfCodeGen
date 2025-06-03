using RfCodeGen.TextTemplates;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

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
        List<EntityDefinitionDto> entityDefinitions = [];
        foreach(EntityDto entity in this.ViewModel.Entities.Where(v1 => v1.IsSelected))
        {
            IEnumerable<string> lines = File.ReadAllLines(entity.FullName);
            lines = lines.SkipWhile(v1 => !v1.StartsWith("public partial class"));
            //string declaration = lines.First();
            var propertyLines = lines.SkipWhile(v1 => v1 != "{").Skip(1).TakeWhile(v1 => v1 != "}").Where(v1 => !string.IsNullOrWhiteSpace(v1));

            EntityDefinitionDto entityDefinition = new(entity.Name);    //, declaration);
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
                entityDefinition.Properties.Add(entityProperty);
            }

            entityDefinition.IParentSri = entityDefinition.Properties.Any(v1 => v1.Name.Equals("ParentSri", StringComparison.OrdinalIgnoreCase));
            entityDefinition.ICheckout = entityDefinition.Properties.Any(v1 => v1.Name.Equals("WrkId", StringComparison.OrdinalIgnoreCase) || v1.Name.Equals("ChoutWrkId", StringComparison.OrdinalIgnoreCase));
            entityDefinition.IInventory = entityDefinition.Properties.Any(v1 => v1.Name.Equals("InvDate", StringComparison.OrdinalIgnoreCase));
            entityDefinition.IFeature = entityDefinition.Properties.Any(v1 => v1.Name.Equals("Sri", StringComparison.OrdinalIgnoreCase));
            entityDefinition.IPointFeature = entityDefinition.Properties.Any(v1 => entityDefinition.IFeature && v1.Name.Equals("MpStart", StringComparison.OrdinalIgnoreCase));
            entityDefinition.ILinearFeature = entityDefinition.Properties.Any(v1 => entityDefinition.IFeature && v1.Name.Equals("MpEnd", StringComparison.OrdinalIgnoreCase));

            entityDefinitions.Add(entityDefinition);
        }

        foreach(var entityDefinition in entityDefinitions)
        {
            Dto dto=new(entityDefinition);
            string dtoContent = dto.TransformText();
            string dtoFileName = Path.Combine(this.ViewModel.ProjectFolder.Shared.Dtos.FullName, $"{entityDefinition.Name}Dto.cs");
            File.WriteAllText(dtoFileName, dtoContent, Encoding.UTF8);
        }
        //var testEntity = this.ViewModel.Entities.Single(v1 => v1.FileName == "AadtSingleUnit.cs");

        //ModelPartial modelPartial = new(testEntity);
        //string modelPartialContent = modelPartial.TransformText();
        //File.WriteAllText(@"c:\junk\partialtest.cs", modelPartialContent, Encoding.UTF8);
    }
}

public record EntityDefinitionDto(string Name)
{
    public List<EntityProperty> Properties { get; init; } = [];
    public bool IParentSri { get; set; }
    public bool ICheckout { get; set; }
    public bool IInventory { get; set; }
    public bool IFeature { get; set; }
    public bool IPointFeature { get; set; }
    public bool ILinearFeature { get; set; }

    public string Interfaces
    {
        get
        {
            List<string> interfaces = [];
            if(this.IParentSri) interfaces.Add("IParentSri");
            if(this.ICheckout) interfaces.Add("ICheckout");
            if(this.IInventory) interfaces.Add("IInventory");
            if(this.IPointFeature) interfaces.Add("IPointFeature");
            if(this.ILinearFeature) interfaces.Add("ILinearFeature");
            return string.Join(", ", interfaces);
        }
    }
}

public record EntityProperty(string Modifier, string Type, string Name, bool Get, bool Set)
{
    public bool Required { get; set; }
}