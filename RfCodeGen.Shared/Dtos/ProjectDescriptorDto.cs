namespace RfCodeGen.Shared.Dtos;

public abstract record ProjectDescriptorDto(string ProjectId, string ProjectName, string ProjectPath, string ProjectPrefix)
{
    public ProjectFolder ProjectFolder => new(ProjectPath, ProjectPrefix);

    //these must be implemented in derived classes to provide specific templates
    public abstract ITextTemplate GetModelTemplate(EntityDescriptorDto entityDescriptor);   // { throw new NotImplementedException(); }
    public abstract ITextTemplate GetDtoTemplate(EntityDescriptorDto entityDescriptor); // { throw new NotImplementedException(); }
    public abstract ITextTemplate GetDomainTemplate(EntityDescriptorDto entityDescriptor);  // { throw new NotImplementedException(); }
    public abstract ITextTemplate GetControllerTemplate(EntityDescriptorDto entityDescriptor);  // { throw new NotImplementedException(); }
}



