namespace Products.Tests.Common.Helpers
{
    using System.Collections.Generic;
    using Model.Common.CMSResponse;
    using Model.Energy;

    public class FakeContentManagementStub
    {
        public static List<CMSEnergyContent> GetCMSEnergyContentList()
        {
            return new List<CMSEnergyContent>
            {
                new CMSEnergyContent { TariffName = "Standard tariff" }
            };
        }

        public static Entry GetDummyEntry()
        {
            return new Entry
            {
                Name = "Standard tariff",
                Description = "Our simple energy tariff with no ties",
                KeyPoints = new List<TickUsp>
                {
                    new TickUsp { Heading = "Flexible energy", Description = "Energy prices may go up or down", DisplayOrder = 1 }
                },
                Documents = new List<Document>
                {
                    new Document
                    {
                        DocumentUrl = new DocumentUrl { Url = "http://mypdfpath/mypdf.pdf" },
                        DocumentAltText = "My PDF Alt Text",
                        DocumentDisplayName = "My PDF Display Name",
                        DocumentType = "PDF"
                    }
                }
            };
        }

        public static CMSResponseModel GetDummyCMSResponseModelForProducts()
        {
            return new CMSResponseModel
            {
                Entries = new List<Entry>
                {
                    new Entry
                    {
                        Name = "Standard tariff",
                        Description = "Our simple energy tariff with no ties",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Flexible energy", Description = "Energy prices may go up or down", DisplayOrder = 1 },
                            new TickUsp { Heading = "No early exit fee", Description = "", DisplayOrder = 2 }
                        }
                    },
                    new Entry
                    {
                        Name = "SSE 1 Year Fixed v14 tariff",
                        Description = "Fix your energy prices for 1 year",
                        KeyPoints = new List<TickUsp>()
                    },
                    new Entry
                    {
                        Name = "SSE 1 Year Fixed v15 tariff",
                        Description = "Fix your energy prices for 1 year",
                        KeyPoints = new List<TickUsp>()
                    },
                    new Entry
                    {
                        Name = "SSE 1 Year Fixed v16 tariff",
                        Description = "Fix your energy prices for 1 year",
                        KeyPoints = new List<TickUsp>()
                    },
                    new Entry
                    {
                        Name = "Smart Tariff tariff",
                        Description = "Smart tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Smart", Description = "Smart", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "SSE 1 Year Fix and Protect tariff",
                        Description = "SSE 1 Year Fix and Protect tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Fix and Protect", Description = "Fix and Protect", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "Tariff 1 tariff",
                        Description = "Tariff 1 tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Tariff 1", Description = "Tariff 1", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "Tariff 2 tariff",
                        Description = "Tariff 2 tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Tariff 2", Description = "Tariff 2", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "Tariff 3 tariff",
                        Description = "Tariff 3 tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Tariff 3", Description = "Tariff 3", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "Fix and Fibre Bundle tariff",
                        Description = "Fix and Fibre Bundle tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Fix and Fibre Bundle", Description = "Fix and Fibre Bundle", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "Standard Economy 7 tariff",
                        Description = "Standard Economy 7",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Standard Economy 7", Description = "Standard Economy 7", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "Fixed 2020 tariff",
                        Description = "Fixed 2020 tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Fixed 2020", Description = "Fixed 2020", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "Fixed Three Years Economy tariff",
                        Description = "Fixed Three Years Economy tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Fixed Three Years Economy", Description = "Fixed Three Years Economy", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "1 Year Fixed tariff",
                        Description = "1 Year Fixed  tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "1 Year Fixed", Description = "1 Year Fixed", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "2 Year Fixed tariff",
                        Description = "2 Year Fixed  tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "2 Year Fixed", Description = "2 Year Fixed", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "SMART Fixed (Elec) tariff",
                        Description = "SMART Fixed (Elec) tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "SMART Fixed (Elec)", Description = "SMART Fixed (Elec)", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "SMART Fixed (Gas) tariff",
                        Description = "SMART Fixed (Gas) tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "SMART Fixed (Gas)", Description = "SMART Fixed (Gas)", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "1 Year Fixed (Gas) tariff",
                        Description = "1 Year Fixed (Gas) tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "1 Year Fixed (Gas)", Description = "1 Year Fixed (Gas)", DisplayOrder = 1 }
                        }
                    },
                    new Entry
                    {
                        Name = "T1 tariff",
                        Description = "T1 tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "T1", Description = "T1", DisplayOrder = 1 }
                        }
                    }
                }
            };
        }

        public static CMSResponseModel GetDummyCMSResponseModel()
        {
            return new CMSResponseModel
            {
                Entries = new List<Entry>
                {
                    new Entry
                    {
                        Name = "Standard tariff",
                        Description = "Our simple energy tariff with no ties",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Flexible energy", Description = "Energy prices may go up or down", DisplayOrder = 1 },
                            new TickUsp { Heading = "No early exit fee", Description = "", DisplayOrder = 2 }
                        }
                    },
                    new Entry
                    {
                        Name = "Smart Tariff tariff",
                        Description = "Our simple energy tariff with no ties",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Flexible energy", Description = "Energy prices may go up or down", DisplayOrder = 1 },
                            new TickUsp { Heading = "No early exit fee", Description = "", DisplayOrder = 2 }
                        }
                    },
                    new Entry
                    {
                        Name = "2 Year Fix tariff",
                        Description = "Fix your energy prices for 2 years",
                        KeyPoints = new List<TickUsp>()
                    }
                }
            };
        }

        public static CMSResponseModel GetDummyCMSResponseModelWithPdfs()
        {
            return new CMSResponseModel
            {
                Entries = new List<Entry>
                {
                    new Entry
                    {
                        Name = "Tariff v1",
                        Description = "Dummy tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Flexible energy", Description = "Energy prices may go up or down", DisplayOrder = 1 },
                            new TickUsp { Heading = "No early exit fee", Description = "", DisplayOrder = 2 }
                        },
                        Documents = new List<Document>
                        {
                            new Document
                            {
                                DocumentUrl = new DocumentUrl { Url = "http://mypdfpath/tariff_v1.pdf" },
                                DocumentAltText = "My PDF Alt Text",
                                DocumentDisplayName = "tariff_v1.pdf",
                                DocumentType = "PDF"
                            }
                        }
                    },
                    new Entry
                    {
                        Name = "Tariff v2",
                        Description = "Dummy tariff",
                        KeyPoints = new List<TickUsp>
                        {
                            new TickUsp { Heading = "Flexible energy", Description = "Energy prices may go up or down", DisplayOrder = 1 },
                            new TickUsp { Heading = "No early exit fee", Description = "", DisplayOrder = 2 }
                        },
                        Documents = new List<Document>
                        {
                            new Document
                            {
                                DocumentUrl = new DocumentUrl { Url = "http://mypdfpath/tariff_v1.pdf" },
                                DocumentAltText = "My PDF Alt Text",
                                DocumentDisplayName = "tariff_v1.pdf",
                                DocumentType = "PDF"
                            },
                            new Document
                            {
                                DocumentUrl = new DocumentUrl { Url = "http://mypdfpath/tariff_v2.pdf" },
                                DocumentAltText = "My PDF Alt Text",
                                DocumentDisplayName = "tariff_v2.pdf",
                                DocumentType = "PDF"
                            }
                        }
                    }
                }
            };
        }
    }
}
