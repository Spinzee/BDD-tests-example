using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Products.Repository.EmailTemplates
{
    [DebuggerDisplay("{Html}")]
    [Serializable]
    public class HtmlContent : IXmlSerializable
    {
        public HtmlContent()
        {
            Html = string.Empty;
        }

        public string Html { get; set; }

        public static implicit operator string(HtmlContent content)
        {
            return content == null ? string.Empty : content.Html;
        }

        public static HtmlContent Empty()
        {
            return new HtmlContent();
        }

        public override bool Equals(object obj)
        {
            HtmlContent other = obj as HtmlContent;
            return other != null && Equals(other.Html, this.Html);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Html = reader.ReadInnerXml();
        }

        public override string ToString()
        {
            return Html ?? string.Empty;
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteRaw(Html);
        }
    }
}
