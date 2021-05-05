namespace Products.Repository
{
    using EmailTemplates;

    public interface IContentRepository
    {
        EmailTemplate GetEmailTemplate(string emailTemplateName);

        EmailTemplateComponent GetEmailComponent(string emailComponentName);
    }
}
