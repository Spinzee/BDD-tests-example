
namespace Products.Repository.EmailTemplates
{
    using System.Xml.Serialization;

    [System.Serializable()]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "root")]
    public class EmailTemplate
    {
        public string FromEmail { get; set; }

        public string ToEmail { get; set; }

        public string Subject { get; set; }

        public HtmlContent Body { get; set; }
    }
}

