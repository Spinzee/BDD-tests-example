namespace Products.Tests.Broadband.Fakes.Services
{
    using System;
    using Products.Model.Broadband;
    using Products.Model.Common;
    using ServiceWrapper.NewBTLineAvailabilityService;

    public class FakeNewBTLineAvailabilityServiceWrapper : INewBTLineAvailabilityServiceWrapper
    {
        public bool Fallout { get; set; }

        public bool InstallLine { get; set; }

        public bool BackOfficeFile { get; set; } = true;

        public string CLI { get; set; } = string.Empty;

        public bool NewBTLineAvailabilityServiceException { get; set; } = false;

        public OpenReachData Newbtlineavailability(BTAddress address, string phoneNumber)
        {
            if (NewBTLineAvailabilityServiceException)
            {
                throw new Exception("NewBTLineAvailabilityService Exception");
            }

            var openReachResponse = new newbtlineavailabilityResponse
            {
                evidence = new newbtlineavailabilityresponsedataEvidence
                {
                    Linestatus = newbtlineavailabilityresponsedataEvidenceLinestatus.NewConnection,
                    addresslinekey = "Ljk12345",
                    listofworkingline = new[]
                    {
                        new newbtlineavailabilityresponsedataEvidenceWorkingline
                        {
                            serviceid = CLI
                        }
                    }
                },
                flags = new newbtlineavailabilityresponsedataFlags
                {
                    boFileFlag = BackOfficeFile,
                    installFlag = InstallLine,
                    falloutFlag = Fallout
                }
            };

            return NewBTLineAvailabilityServiceMapper.CreateOpenreachResponse(openReachResponse);
        }
    }
}