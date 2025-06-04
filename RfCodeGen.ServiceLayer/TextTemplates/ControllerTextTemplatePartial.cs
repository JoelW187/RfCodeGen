using RfCodeGen.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.ServiceLayer.TextTemplates;

public partial class ControllerTextTemplate : ControllerTextTemplateBase
{
    private EntityDescriptorDto EntityDescriptor { get; set; }
    private string PluralizedEntityName { get; set; }

    public ControllerTextTemplate(EntityDescriptorDto entityDescriptor)
    {
        this.EntityDescriptor = entityDescriptor;

        Utils.Pluralizer.Pluralizer pluralizer = new();
        this.PluralizedEntityName = pluralizer.Pluralize(entityDescriptor.Name);
    }
}
