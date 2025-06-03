using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.TextTemplates;

public partial class ModelPartial : ModelPartialBase
{
    private EntityDto Entity { get; }

    public ModelPartial(EntityDto entity)
    {
        this.Entity = entity;
    }
}
