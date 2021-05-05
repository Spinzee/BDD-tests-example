namespace Products.ServiceWrapper.NewBTLineAvailabilityService
{
    using System.Linq;
    using Core;
    using Model.Broadband;
    using Model.Common;

    public static class NewBTLineAvailabilityServiceMapper
    {
        public static newbtlineavailabilityRequest CreateOpenreachRequest(BTAddress address, string cli)
        {
            return new newbtlineavailabilityRequest
            {
                addressdata = new address
                {
                    locality = address.Locality,
                    postTown = address.PostTown,
                    postcode = address.Postcode,
                    premiseName = address.PremiseName,
                    subPremises = address.SubPremises,
                    thoroughfareName = address.ThoroughfareName,
                    thoroughfareNumber = address.ThoroughfareNumber
                },
                cli = cli
            };
        }

        public static OpenReachData CreateOpenreachResponse(newbtlineavailabilityResponse response)
        {
            var lineStatus = LineStatus.Other;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (response.evidence?.Linestatus)
            {
                case newbtlineavailabilityresponsedataEvidenceLinestatus.StartofstoppedLine:
                    lineStatus = LineStatus.StartofstoppedLine;
                    break;
                case newbtlineavailabilityresponsedataEvidenceLinestatus.MPFConversion:
                    lineStatus = LineStatus.MPFConversion;
                    break;
                case newbtlineavailabilityresponsedataEvidenceLinestatus.NewConnection:
                    lineStatus = LineStatus.NewConnection;
                    break;
            }

            return new OpenReachData
            {
                AddressLineKey = response.evidence?.addresslinekey,
                LineavailabilityFlags = new LineAvailability
                {
                    BackOfficeFile = response.flags.boFileFlag,
                    Fallout = response.flags.falloutFlag ,
                    InstallLine = response.flags.installFlag
                },
                LineStatus = lineStatus,
                CLI = response.evidence?.listofworkingline?.FirstOrDefault()?.serviceid
            };
        }
    }
}
