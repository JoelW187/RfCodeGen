using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace RfCodeGen;

public class MainViewModel : INotifyPropertyChanged
{
    public MainViewModel()
    {
        this.PropertyChanged += MainViewModel_PropertyChanged;
    }

    private void MainViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
        case nameof(SelectedProjectDescriptor):
            if(this.SelectedProjectDescriptor == null)
            {
                //this.ProjectFolder = null;
                this.Entities.Clear();
                this.Messages.Clear();
                this.AuxGenCode = "";
                return;
            }

            //this.ProjectFolder = new ProjectFolder(this.SelectedProjectDescriptor);
            var entityFileNames = Directory.EnumerateFiles(this.SelectedProjectDescriptor.ProjectFolder.DataAccess.Models.FullPath, "*.cs", SearchOption.TopDirectoryOnly).OrderBy(v1 => v1);
            var entities = entityFileNames.Select(fullName => new Entity(fullName, File.Exists(this.SelectedProjectDescriptor.ProjectFolder.DataAccess.Models.Partials.GetFilePath($"{Path.GetFileNameWithoutExtension(fullName)}.cs")))).ToList();
            entities.ForEach(entity =>
            {
                this.Entities.Add(entity);
            });
            break;
        }
    }

    public ObservableCollection<ProjectDescriptorDto> ProjectDescriptors { get; } = [];

    private ProjectDescriptorDto? _selectedProjectDescriptor;
    public ProjectDescriptorDto? SelectedProjectDescriptor
    {
        get { return _selectedProjectDescriptor; }
        set
        {
            if(_selectedProjectDescriptor == value) return;

            _selectedProjectDescriptor = value;
            OnPropertyChanged();
        }
    }

    //private ProjectFolder? _projectFolder;
    //public ProjectFolder? ProjectFolder
    //{
    //    get { return _projectFolder; }
    //    set
    //    {
    //        if(_projectFolder == value) return;

    //        _projectFolder = value;
    //        OnPropertyChanged();
    //    }
    //}

    public ObservableCollection<Entity> Entities { get; } = [];

    private ObservableCollection<string> _messages = [];
    public ObservableCollection<string> Messages
    {
        get { return _messages; }
        set
        {
            if(_messages == value) return;

            _messages = value;
            OnPropertyChanged();
        }
    }

    private string _auxGenCode = "";
    public string AuxGenCode
    {
        get { return _auxGenCode; }
        set
        {
            if(_auxGenCode == value) return;

            _auxGenCode = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

[DebuggerDisplay("Name={Name},HasPartial={HasPartial},IsSelected={IsSelected},FilePath={FilePath}")]
public record Entity(string FilePath, bool HasPartial = false, bool IsSelected = false) : EntityDto(FilePath), INotifyPropertyChanged
{
    private bool _hasPartial = HasPartial;
    public bool HasPartial
    {
        get { return _hasPartial; }
        set
        {
            if(_hasPartial == value) return;

            _hasPartial = value;
            OnPropertyChanged();
        }
    }

    private bool _isSelected = IsSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            if(_isSelected == value) return;

            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public override string ToString() => this.FilePath;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
