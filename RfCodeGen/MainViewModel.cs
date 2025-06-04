using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace RfCodeGen;

public class MainViewModel : INotifyPropertyChanged
{
    public MainViewModel()
    {
        //string modelsPath = Path.Combine(this.SourceCodeRoot, @"HPMS.DataAccess\Models");
        //string modelsPartialsPath = Path.Combine(modelsPath, @"Partials");

        var entityFileNames = Directory.EnumerateFiles(this.ProjectFolder.DataAccess.Models.FullName, "*.cs", SearchOption.TopDirectoryOnly).OrderBy(v1 => v1);
        this._entities = new ObservableCollection<Entity>(entityFileNames.Select(fullName => new Entity(fullName, File.Exists(Path.Combine(this.ProjectFolder.DataAccess.Models.Partials.FullName, $"{Path.GetFileNameWithoutExtension(fullName)}Partial.cs")))));
    }

    private ProjectFolder _projectFolder = new(@"C:\Source\mbakerintlapps\NJDOT\NJDOT_HPMS\src\NJDOT_HPMS");
    public ProjectFolder ProjectFolder
    {
        get { return _projectFolder; }
        set
        {
            if(_projectFolder == value) return;

            _projectFolder = value;
            OnPropertyChanged();
        }
    }

    //private string _sourceCodeRoot = @"C:\Source\mbakerintlapps\NJDOT\NJDOT_HPMS\src\NJDOT_HPMS";
    //public string SourceCodeRoot
    //{
    //    get { return _sourceCodeRoot; }
    //    set
    //    {
    //        if(_sourceCodeRoot == value) return;

    //        _sourceCodeRoot = value;
    //        OnPropertyChanged();
    //    }
    //}

    private ObservableCollection<Entity> _entities;
    public ObservableCollection<Entity> Entities
    {
        get { return _entities; }
        set
        {
            if(_entities == value) return;

            _entities = value;
            OnPropertyChanged();
        }
    }

    //public Task Initialized { get; private set; }

    //private async Task InitializeAsync()
    //{
    //    var entityNames = Directory.EnumerateFiles(Path.Combine(this.SourceCodeRoot, @"HPMS.DataAccess\Models"), "*.*", SearchOption.TopDirectoryOnly).tol
    //}

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public record Entity(string FullName, bool HasPartial = false, bool IsSelected = false) : EntityDto(FullName)
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

    //public string FileName => Path.GetFileName(this.FullName);

    //public string Name => Path.GetFileNameWithoutExtension(this.FullName);

    public override string ToString() => this.FullName;

    //public event PropertyChangedEventHandler? PropertyChanged;
    //protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    //{
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //}
}
