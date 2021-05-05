namespace Products.Service.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Model.Energy;
    using Products.Model.Common.CMSResponse;

    public class ContentManagementServiceMapper
    {
        private const string ContentStackDomain = "assets.contentstack.io";
        private const string SSEDomain = "sse.co.uk";

        public static List<CMSEnergyContent> MapEntryListToCMSEnergyContentList(List<Entry> entries)
        {
            return entries.Select(MapEntryToCMSEnergyContent).ToList();
        }

        public static CMSEnergyContent MapEntryToCMSEnergyContent(Entry entry)
        {
            return new CMSEnergyContent
            {
                TariffName = entry.Name,
                TagLine = entry.Description,
                TickUsps = entry.KeyPoints?.Select(y => new TariffTickUsp(y.Heading, y.Description, y.DisplayOrder)).ToList() ?? new List<TariffTickUsp>(),
                PDFList = entry.Documents?.Select(z => 
                              new PDFContent{
                                  Name = z.DocumentDisplayName,
                                  Title = z.DocumentAltText,
                                  Category = z.DocumentType,
                                  Path = z.DocumentUrl?.Url.Replace(ContentStackDomain, SSEDomain)
                              }).ToList() ?? new List<PDFContent>()
            };
        }

        public static string GetFileNameFromWebPath(string webPath)
        {
            return Path.GetFileName(Uri.UnescapeDataString(webPath).Replace("/", "\\"));
        }
    }
}
