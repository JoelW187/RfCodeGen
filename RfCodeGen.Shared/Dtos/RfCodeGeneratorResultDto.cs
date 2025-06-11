using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.Shared.Dtos;

public record RfCodeGeneratorResultDto(int GeneratedFileCount, IEnumerable<string> DomainServiceRegistrations, IEnumerable<string> AutoMapperMappingProfiles, IEnumerable<string> LookupTableEnums)
{
}
