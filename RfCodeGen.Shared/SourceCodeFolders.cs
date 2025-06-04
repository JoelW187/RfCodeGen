using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.Shared;


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
    public ServiceLayerDomainsFolder Domains { get; } = new(Path.Combine(FullName, "Domains"));
}

public record ServiceLayerDomainsFolder(string FullName) : SourceCodeFolderBase(FullName) { }

//Controller
public record WebApiFolder(string FullName) : SourceCodeFolderBase(FullName)
{
    public WebApiControllersFolder Controllers { get; } = new(Path.Combine(FullName, "Controllers"));
}

public record WebApiControllersFolder(string FullName) : SourceCodeFolderBase(FullName) { }
