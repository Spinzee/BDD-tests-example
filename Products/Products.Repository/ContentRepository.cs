namespace Products.Repository
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;
    using EmailTemplates;
    using Infrastructure;

    public class ContentRepository : IContentRepository
    {
        private const string TemplatePathFormat = "Products.Repository.{0}.{1}.xml";
        private const string EmailTemplateFolderName = "EmailTemplates";

        public EmailTemplate GetEmailTemplate(string emailTemplateName)
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(emailTemplateName), $"{nameof(emailTemplateName)} is null or empty");

            return GetDeserializedContent<EmailTemplate>(emailTemplateName, EmailTemplateFolderName);
        }

        public EmailTemplateComponent GetEmailComponent(string emailComponentName)
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(emailComponentName), $"{nameof(emailComponentName)} is null or empty");
            return GetDeserializedContent<EmailTemplateComponent>(emailComponentName, EmailTemplateFolderName);
        }

        private static T GetDeserializedContent<T>(string templateName, string folderName) where T : class, new()
        {
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(templateName), $"{nameof(templateName)} is null or empty");
            Guard.Against<ArgumentException>(string.IsNullOrEmpty(folderName), $"{nameof(folderName)} is null or empty");

            var serializer = new XmlSerializer(typeof(T));
            Assembly assembly = typeof(ContentRepository).Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(string.Format(TemplatePathFormat, folderName, templateName)))
            {
                T deserializedContent = null;

                if (stream != null)
                {
                    using (XmlReader reader = XmlReader.Create(stream))
                    {
                        deserializedContent = (T)serializer.Deserialize(reader);
                    }
                }

                return deserializedContent;
            }
        }
    }
}
