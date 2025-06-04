using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen;

public class MainViewModel : INotifyPropertyChanged
{
    public MainViewModel()
    {
        //string modelsPath = Path.Combine(this.SourceCodeRoot, @"HPMS.DataAccess\Models");
        //string modelsPartialsPath = Path.Combine(modelsPath, @"Partials");

        var entityFileNames = Directory.EnumerateFiles(this.ProjectFolder.DataAccess.Models.FullName, "*.cs", SearchOption.TopDirectoryOnly).OrderBy(v1 => v1);
        this._entities = new ObservableCollection<EntityDto>(entityFileNames.Select(fullName => new EntityDto(fullName, File.Exists(Path.Combine(this.ProjectFolder.DataAccess.Models.Partials.FullName, Path.GetFileName(fullName))))));
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

    private ObservableCollection<EntityDto> _entities;
    public ObservableCollection<EntityDto> Entities
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

public record EntityDto(string FullName, bool HasPartial = false, bool IsSelected = false) : INotifyPropertyChanged
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

    public string FileName => Path.GetFileName(this.FullName);

    public string Name => Path.GetFileNameWithoutExtension(this.FullName);

    public override string ToString() => this.FullName;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public abstract record SourceCodeFolderBase(string FullName)
{
    public string FileName => Path.GetFileName(FullName);
    public string Name => Path.GetFileNameWithoutExtension(FullName);
    public override string ToString() => FullName;
}

public record ProjectFolder(string FullName) : SourceCodeFolderBase(FullName)
{
    public DataAccessFolder DataAccess { get; } = new(Path.Combine(FullName, "HPMS.DataAccess"));
    public SharedFolder Shared { get; } = new(Path.Combine(FullName, "HPMS.Shared"));
    public ServiceLayerFolder ServiceLayer { get; } = new(Path.Combine(FullName, "HPMS.ServiceLayer"));
    public WebApiFolder WebApi { get; } = new(Path.Combine(FullName, "HPMS.WebApi"));
}

//DataAccess
public record DataAccessFolder(string FullName) : SourceCodeFolderBase(FullName)
{
    public DataAccessModelsFolder Models { get; } = new(Path.Combine(FullName, "Models"));
}

public record DataAccessModelsFolder(string FullName) : SourceCodeFolderBase(FullName)
{
    public DataAccessModelsPartialsFolder Partials { get; } = new(Path.Combine(FullName, "Partials"));
}

public record DataAccessModelsPartialsFolder(string FullName) : SourceCodeFolderBase(FullName) { }

//Shared
public record SharedFolder(string FullName) : SourceCodeFolderBase(FullName)
{
    public SharedDtosFolder Dtos { get; } = new(Path.Combine(FullName, "Dtos"));
}

public record SharedDtosFolder(string FullName) : SourceCodeFolderBase(FullName) { }

//ServiceLayer
public record ServiceLayerFolder(string FullName) : SourceCodeFolderBase(FullName)
{
    public ServiceLayerDomainFolder Domains { get; } = new(Path.Combine(FullName, "Domains"));
}

public record ServiceLayerDomainFolder(string FullName) : SourceCodeFolderBase(FullName) { }

//Controller
public record WebApiFolder(string FullName) : SourceCodeFolderBase(FullName)
{
    public WebApiControllerFolder Controllers { get; } = new(Path.Combine(FullName, "Controllers"));
}

public record WebApiControllerFolder(string FullName) : SourceCodeFolderBase(FullName) { }
