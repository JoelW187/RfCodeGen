// See https://aka.ms/new-console-template for more information
using RfCodeGen.ProjectConfigs.Utils;
using RfCodeGen.ServiceLayer;
using RfCodeGen.Shared.Dtos;

var progress = new Progress<string>(message =>
{
    Console.WriteLine(message);
});

IProjectDescriptor projectDescriptor = ProjectFactory.CreateProjectDescriptor("HPMS", "NJDOT HPMS", @"C:\Source\mbakerintlapps\NJDOT\NJDOT_HPMS\src\NJDOT_HPMS");
List<EntityDto> entities = [];

entities.Add(new(Path.Combine(projectDescriptor.ProjectFolder.DataAccess.Models.FullPath, "CrackingPercent.cs")));

var codeGenerator = new RfCodeGenerator(projectDescriptor);
var count = await codeGenerator.Generate(entities, progress);

Console.WriteLine($"Generated {count} files.");

Console.WriteLine();
var domainServiceRegistrations = RfCodeGeneratorBase.GetDomainServiceRegistrations(entities);
var autoMapperMappingProfiles = RfCodeGeneratorBase.GetAutoMapperMappingProfiles(entities);

string auxGenCode = string.Join("\r\n", domainServiceRegistrations) + "\r\n" + string.Join("\r\n", autoMapperMappingProfiles);
Console.WriteLine("Auxiliary Generated Code:");
Console.WriteLine(auxGenCode);

Console.ReadKey();
