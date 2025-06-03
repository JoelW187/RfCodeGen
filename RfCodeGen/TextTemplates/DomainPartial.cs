using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.TextTemplates;

public partial class Domain : DomainBase
{
    private EntityDefinitionDto EntityDefinition { get; set; }

    public Domain(EntityDefinitionDto entityDefinition)
    {
        this.EntityDefinition = entityDefinition;
    }
}
