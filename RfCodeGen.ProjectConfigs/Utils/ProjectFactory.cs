using RfCodeGen.ProjectConfigs.CDMS.Dtos;
using RfCodeGen.ProjectConfigs.HPMS.Dtos;
using RfCodeGen.Shared.Dtos;

namespace RfCodeGen.ProjectConfigs.Utils;

public static class ProjectFactory
{
    public static IProjectDescriptor CreateProjectDescriptor(string projectId, string projectName, string projectPath)
    {
        return projectId switch
        {
            "HPMS" => new HpmsProjectDescriptorDto(projectName, projectPath),
            "CDMS" => new CdmsProjectDescriptorDto(projectName, projectPath),
            _ => throw new NotSupportedException($"Project type '{projectId}' is not supported.")
        };
    }
}
