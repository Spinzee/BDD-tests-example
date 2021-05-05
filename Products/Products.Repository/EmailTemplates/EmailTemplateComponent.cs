namespace Products.Repository.EmailTemplates
{
    using System.Xml.Serialization;

    [System.Serializable()]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "root")]
    public class EmailTemplateComponent
    {
        public HtmlContent Content { get; set; }
    }
}