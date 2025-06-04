using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.TextTemplates;

public partial class ModelPartial(EntityDto entity) : ModelPartialBase
{
    private EntityDto Entity { get; } = entity;
}
