namespace RfCodeGen.Shared.Dtos;

public record ProjectDescriptorDto(string ProjectId, string ProjectName, string ProjectPath, string ProjectPrefix)
{
    public ProjectFolder ProjectFolder => new(ProjectPath, ProjectPrefix);
}



