// See https://aka.ms/new-console-template for more information
using RfCodeGen.ProjectConfigs.Utils;
using RfCodeGen.ServiceLayer;
using RfCodeGen.Shared.Dtos;
using System.Runtime.CompilerServices;

IProjectDescriptor projectDescriptor;

if(args.Length < 3 || args.Length > 4)
{
    Exit("Invalid number of arguments");
}

string projectId = args[0].Trim();
string projectName = args[1].Trim();
string projectPath = args[2].Trim();
List<string> entityNames = [];
if(args.Length == 4)
{
    entityNames.AddRange(args[3].Trim().Split(',').Select(e => e.Trim()));
}

if(!Directory.Exists(projectPath))
{
    Exit($"Project path '{projectPath}' does not exist.");
}

try
{
    projectDescriptor = ProjectFactory.CreateProjectDescriptor(projectId, projectName, projectPath);

    if(entityNames.Count == 0)
    {
        entityNames.AddRange(Directory.EnumerateFiles(projectDescriptor.ProjectFolder.DataAccess.Models.FullPath, "*.cs", SearchOption.TopDirectoryOnly).Select(v1 => Path.GetFileName(v1)));
    }

    List<EntityDto> entities = [];
    foreach(string entityName in entityNames)
    {
        string filename = Path.ChangeExtension(entityName, "cs");

        string filePath = Path.Combine(projectDescriptor.ProjectFolder.DataAccess.Models.FullPath, filename);
        if (!File.Exists(filePath))
            Exit($"Entity file '{filePath}' does not exist.");

        entities.Add(new(Path.Combine(projectDescriptor.ProjectFolder.DataAccess.Models.FullPath, filename)));
    }

    var codeGenerator = new RfCodeGenerator(projectDescriptor);
    var count = await codeGenerator.Generate(entities, new Progress<RfProgressUpdateDto>((update) => PrintProgress(update)));

    Console.WriteLine($"Generated {count} files for {entities.Count} entities.");

    Console.WriteLine();
    var domainServiceRegistrations = RfCodeGeneratorBase.GetDomainServiceRegistrations(entities);
    var autoMapperMappingProfiles = RfCodeGeneratorBase.GetAutoMapperMappingProfiles(entities);

    string auxGenCode = string.Join("\r\n", domainServiceRegistrations) + "\r\n" + string.Join("\r\n", autoMapperMappingProfiles);
    Console.WriteLine("Auxiliary Generated Code:");
    Console.WriteLine(auxGenCode);
}
catch (Exception ex)
{
    Exit($"Error creating project: {ex.Message}");
    return;
}

#if DEBUG
Console.ReadKey();
#endif

static void PrintProgress(RfProgressUpdateDto update)
{
    Console.WriteLine($"{update.Entity.Name} - {update.Step}");
}

static void Exit(string? message)
{
    if(!string.IsNullOrWhiteSpace(message))
        PrintUsage(message);

#if DEBUG
    Console.ReadKey();
#endif

    Environment.Exit(1);
}

static void PrintUsage(string? message = null)
{
    if (!string.IsNullOrEmpty(message))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
        Console.WriteLine();
    }

    Console.WriteLine("RfCodeGen.Cli - Command Line Interface for RfCodeGen Code Generation Tool");
    Console.WriteLine();

    var projectIds = ProjectFactory.GetProjectIds();
    Console.WriteLine("Usage: RfCodeGen.Cli <projectId> <projectName> <projectPath> [<entityNames>]");
    Console.WriteLine("Available project IDs:");
    foreach (var id in projectIds)
    {
        Console.WriteLine($"- {id}");
    }
    Console.WriteLine($"projectName is user defined and can be any string.");
    Console.WriteLine($"projectPath is the path to the project folder where the generated code will be placed. Generally this will be the folder where the .sln file lives.");
    Console.WriteLine($"entityNames is an optional comma-separated list of entity names to generate code for. If not provided, all entities in the DataAccess.Models folder will be processed.");
    Console.WriteLine();
    Console.WriteLine($"Example: RfCodeGen.Cli HPMS 'NJDOT HPMS' 'C:\\Source\\mbakerintlapps\\NJDOT\\NJDOT_HPMS\\src\\NJDOT_HPMS'");
}