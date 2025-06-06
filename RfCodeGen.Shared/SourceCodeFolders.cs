using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.Shared;

public abstract record SourceCodeFolderBase(string FullPath)
{
    public string FileName => Path.GetFileName(FullPath);
    public string Name => Path.GetFileNameWithoutExtension(FullPath);

    public string GetFilePath(string filename) => Path.Combine(FullPath, filename);
    public override string ToString() => FullPath;
}

public record ProjectFolder(string FullPath, string Prefix) : SourceCodeFolderBase(FullPath)
{
    //public ProjectFolder(ProjectDescriptorDto projectDescriptor) : this(projectDescriptor.ProjectPath, projectDescriptor.ProjectPrefix) { }

    public DataAccessFolder DataAccess { get; } = new(Path.Combine(FullPath, $"{Prefix}DataAccess"));
    public SharedFolder Shared { get; } = new(Path.Combine(FullPath, $"{Prefix}Shared"));
    public ServiceLayerFolder ServiceLayer { get; } = new(Path.Combine(FullPath, $"{Prefix}ServiceLayer"));
    public WebApiFolder WebApi { get; } = new(Path.Combine(FullPath, $"{Prefix}WebApi"));
}

//DataAccess
public record DataAccessFolder(string FullPath) : SourceCodeFolderBase(FullPath)
{
    public DataAccessModelsFolder Models { get; } = new(Path.Combine(FullPath, "Models"));
}

public record DataAccessModelsFolder(string FullPath) : SourceCodeFolderBase(FullPath)
{
    public DataAccessModelsPartialsFolder Partials { get; } = new(Path.Combine(FullPath, "Partials"));
}

public record DataAccessModelsPartialsFolder(string FullPath) : SourceCodeFolderBase(FullPath) { }

//Shared
public record SharedFolder(string FullPath) : SourceCodeFolderBase(FullPath)
{
    public SharedDtosFolder Dtos { get; } = new(Path.Combine(FullPath, "Dtos"));
}

public record SharedDtosFolder(string FullPath) : SourceCodeFolderBase(FullPath)
{
    public SharedDtosLookupsFolder Lookups { get; } = new(Path.Combine(FullPath, "Lookups"));
}

public record SharedDtosLookupsFolder(string FullPath) : SourceCodeFolderBase(FullPath) { }

//ServiceLayer
public record ServiceLayerFolder(string FullPath) : SourceCodeFolderBase(FullPath)
{
    public ServiceLayerDomainsFolder Domains { get; } = new(Path.Combine(FullPath, "Domains"));
}

public record ServiceLayerDomainsFolder(string FullPath) : SourceCodeFolderBase(FullPath) { }

//Controller
public record WebApiFolder(string FullPath) : SourceCodeFolderBase(FullPath)
{
    public WebApiControllersFolder Controllers { get; } = new(Path.Combine(FullPath, "Controllers"));
}

public record WebApiControllersFolder(string FullPath) : SourceCodeFolderBase(FullPath) { }
