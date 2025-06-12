namespace RfCodeGen.Shared.Dtos;

public interface IProjectDescriptor
{
    static string Id => throw new NotImplementedException("ProjectId must be implemented in derived classes.");

    string ProjectId { get; }
    string ProjectName { get; }
    string ProjectPath { get; }
    string ProjectPathPrefix { get; }
    string ProjectNamespacePrefix { get; }
    ProjectFolder ProjectFolder { get; }
    System.Text.Encoding Encoding { get; }
    
    ITextTemplate GetModelTemplate(EntityDescriptorDto entityDescriptor);
    ITextTemplate GetDtoTemplate(EntityDescriptorDto entityDescriptor);
    ITextTemplate GetDomainTemplate(EntityDescriptorDto entityDescriptor);
    ITextTemplate GetControllerTemplate(EntityDescriptorDto entityDescriptor);
    ITextTemplate GetRfControllerTestTemplate(EntityDescriptorDto entityDescriptor);

    EntityDescriptorDto GetEntityDescriptor(EntityDto entity);
    EntityPropertyDescriptorDto GetEntityPropertyDescriptor(EntityDescriptorDto entityDescriptor, string text);
}

public abstract record ProjectDescriptorDto(string ProjectId, string ProjectName, string ProjectPath, string ProjectPathPrefix, string ProjectNamespacePrefix, System.Text.Encoding? TextEncoding = null) : IProjectDescriptor
{
    public ProjectFolder ProjectFolder => new(ProjectPath, ProjectPathPrefix);
    public System.Text.Encoding Encoding { get; } = TextEncoding ?? System.Text.Encoding.Default;

    //these must be implemented in derived classes to provide specific templates
    public abstract ITextTemplate GetModelTemplate(EntityDescriptorDto entityDescriptor);
    public abstract ITextTemplate GetDtoTemplate(EntityDescriptorDto entityDescriptor);
    public abstract ITextTemplate GetDomainTemplate(EntityDescriptorDto entityDescriptor);
    public abstract ITextTemplate GetControllerTemplate(EntityDescriptorDto entityDescriptor);
    public abstract ITextTemplate GetRfControllerTestTemplate(EntityDescriptorDto entityDescriptor);

    public abstract EntityDescriptorDto GetEntityDescriptor(EntityDto entity);
    public abstract EntityPropertyDescriptorDto GetEntityPropertyDescriptor(EntityDescriptorDto entityDescriptor, string text);
}



