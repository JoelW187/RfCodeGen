using RfCodeGen.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.ServiceLayer.TextTemplates;

public partial class ControllerTextTemplate(EntityDescriptorDto entityDescriptor) : ControllerTextTemplateBase
{
    private EntityDescriptorDto EntityDescriptor { get; set; } = entityDescriptor;
}
