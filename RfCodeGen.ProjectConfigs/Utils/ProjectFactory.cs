using RfCodeGen.ProjectConfigs.CDMS.Dtos;
using RfCodeGen.ProjectConfigs.HPMS.Dtos;
using RfCodeGen.Shared.Dtos;
using System.Reflection;

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

    public static IEnumerable<string> GetProjectIds()
    {
        List<string> projectIds = [];

        var projectDescriptors = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsAssignableTo(typeof(IProjectDescriptor)));

        foreach (var projectDescriptor in projectDescriptors)
        {
            var p = projectDescriptor.GetProperty("Id", BindingFlags.Static | BindingFlags.Public);
            try
            {
                string? id = p!.GetValue(projectDescriptor) as string;
                if(!string.IsNullOrEmpty(id) && !projectIds.Contains(id))
                    projectIds.Add(id);
            }
            catch(Exception) { }
        }

        return projectIds;
    }
}
