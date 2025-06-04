using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.TextTemplates;

public partial class ModelPartial(Entity entity) : ModelPartialBase
{
    private Entity Entity { get; } = entity;
}
