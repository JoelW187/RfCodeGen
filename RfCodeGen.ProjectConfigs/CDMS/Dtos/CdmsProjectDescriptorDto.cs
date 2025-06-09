using RfCodeGen.Shared;
using RfCodeGen.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfCodeGen.ProjectConfigs.CDMS.Dtos;

public record CdmsProjectDescriptorDto(string ProjectName, string ProjectPath) : ProjectDescriptorDto("CDMS", ProjectName, ProjectPath, "CostRecovery.", "CDMS.CostRecovery.")
{
    public override ITextTemplate GetModelTemplate(EntityDescriptorDto entityDescriptor) => entityDescriptor.IsLookupTable ? new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.ModelLookupTextTemplate(this, entityDescriptor) : new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.ModelTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDtoTemplate(EntityDescriptorDto entityDescriptor) => entityDescriptor.IsLookupTable ? new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.DtoLookupTextTemplate(this, entityDescriptor) : new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.DtoTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetDomainTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.DomainTextTemplate(this, entityDescriptor);
    public override ITextTemplate GetControllerTemplate(EntityDescriptorDto entityDescriptor) => new RfCodeGen.ProjectConfigs.CDMS.TextTemplates.ControllerTextTemplate(this, entityDescriptor);
}
