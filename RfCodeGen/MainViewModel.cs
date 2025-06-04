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

    private string _auxGenCode;
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

    public override string ToString() => this.FullName;
}
