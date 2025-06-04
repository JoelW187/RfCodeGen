using RfCodeGen.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.TextTemplates;

public partial class Controller : ControllerBase
{
    private EntityDescriptorDto EntityDescriptor { get; set; }
    private string PluralizedEntityName{ get; set; }

    public Controller(EntityDescriptorDto entityDescriptor)
    {
        this.EntityDescriptor = entityDescriptor;

        var pluralizer = new RfCodeGen.ServiceLayer.Utils.Pluralizer.Pluralizer();
        this.PluralizedEntityName = pluralizer.Pluralize(entityDescriptor.Name);
    }
}
