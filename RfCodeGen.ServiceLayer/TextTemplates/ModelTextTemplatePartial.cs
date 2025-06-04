using RfCodeGen.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.ServiceLayer.TextTemplates;

public partial class ModelTextTemplate(EntityDto entity) : ModelTextTemplateBase
{
    private EntityDto Entity { get; } = entity;
}
