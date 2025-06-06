namespace RfCodeGen.Shared.Dtos;

public abstract record ProjectDescriptorDto(string ProjectId, string ProjectName, string ProjectPath, string ProjectPathPrefix, string ProjectNamespacePrefix, System.Text.Encoding? encoding = null)
{
    public ProjectFolder ProjectFolder => new(ProjectPath, ProjectPathPrefix);
    public System.Text.Encoding Encoding { get; } = encoding ?? System.Text.Encoding.Default;

    //these must be implemented in derived classes to provide specific templates
    public abstract ITextTemplate GetModelTemplate(EntityDescriptorDto entityDescriptor);   // { throw new NotImplementedException(); }
    public abstract ITextTemplate GetDtoTemplate(EntityDescriptorDto entityDescriptor); // { throw new NotImplementedException(); }
    public abstract ITextTemplate GetDomainTemplate(EntityDescriptorDto entityDescriptor);  // { throw new NotImplementedException(); }
    public abstract ITextTemplate GetControllerTemplate(EntityDescriptorDto entityDescriptor);  // { throw new NotImplementedException(); }
}



