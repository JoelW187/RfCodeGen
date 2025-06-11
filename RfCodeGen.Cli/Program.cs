// See https://aka.ms/new-console-template for more information
using RfCodeGen.ProjectConfigs.Utils;
using RfCodeGen.ServiceLayer;
using RfCodeGen.Shared.Dtos;
using System.Runtime.CompilerServices;

IProjectDescriptor projectDescriptor;

if(args.Length != 3)
{
    Exit("Invalid number of arguments");
}

string projectId = args[0].Trim();
string projectName = args[1].Trim();
string projectPath = args[2].Trim();

if(!Directory.Exists(projectPath))
{
    Exit($"Project path '{projectPath}' does not exist.");
}

try
{
    projectDescriptor = ProjectFactory.CreateProjectDescriptor(projectId, projectName, projectPath);

    List<EntityDto> entities = [];
    entities.Add(new(Path.Combine(projectDescriptor.ProjectFolder.DataAccess.Models.FullPath, "CrackingPercent.cs")));

    var codeGenerator = new RfCodeGenerator(projectDescriptor);
    var count = await codeGenerator.Generate(entities, new Progress<string>((message) => PrintProgress(message)));

    Console.WriteLine($"Generated {count} files.");

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

static void PrintProgress(string message)
{
    Console.WriteLine(message);
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
    Console.WriteLine("Usage: RfCodeGen.Cli <projectId> <projectName> <projectPath>");
    Console.WriteLine("Available project IDs:");
    foreach (var id in projectIds)
    {
        Console.WriteLine($"- {id}");
    }
    Console.WriteLine($"projectName is user defined and can be any string.");
    Console.WriteLine($"projectPath is the path to the project folder where the generated code will be placed. Generally this will be the folder where the .sln file lives.");
    Console.WriteLine();
    Console.WriteLine($"Example: RfCodeGen.Cli HPMS 'NJDOT HPMS' 'C:\\Source\\mbakerintlapps\\NJDOT\\NJDOT_HPMS\\src\\NJDOT_HPMS'");
}