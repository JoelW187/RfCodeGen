using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.TextTemplates;

public partial class Dto : DtoBase
{
    private EntityDefinitionDto EntityDefinition { get; }

    public Dto(EntityDefinitionDto entityDefinition)
    {
        this.EntityDefinition = entityDefinition;
    }
}
