using RfCodeGen.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.ProjectConfigs.CDMS.Dtos;

public record CdmsEntityPropertyDescriptorDto : EntityPropertyDescriptorDto
{
    public override bool IsPrimaryKey
    {
        get
        {
            return this.Name.Equals($"{this.EntityDescriptor.Name}Id", StringComparison.OrdinalIgnoreCase);
        }
    }
}
