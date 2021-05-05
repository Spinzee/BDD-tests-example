namespace Products.ServiceWrapper.NewBTLineAvailabilityService
{
    using System;
    using Products.Model.Broadband;
    using Products.Model.Common;

    public interface INewBTLineAvailabilityServiceWrapper
    {
        OpenReachData Newbtlineavailability(BTAddress address, string CLI);
    }

    public class NewBTLineAvailabilityServiceWrapper : INewBTLineAvailabilityServiceWrapper
    {
        private readonly INewBTLineAvailabilityServiceFactory _newBtLineAvailabilityServiceFactory;

        public NewBTLineAvailabilityServiceWrapper(INewBTLineAvailabilityServiceFactory newBtLineAvailabilityServiceFactory)
        {
            _newBtLineAvailabilityServiceFactory = newBtLineAvailabilityServiceFactory;
        }

        public OpenReachData Newbtlineavailability(BTAddress address, string CLI)
        {
            using (var client = _newBtLineAvailabilityServiceFactory.Create())
            {
                var request = NewBTLineAvailabilityServiceMapper.CreateOpenreachRequest(address, CLI);
                request.messageHeader = _newBtLineAvailabilityServiceFactory.CreateMessageHeader();

                var newbtlineavailabilityResponse = ((NewBTLineAvailabilityServiceInterface)client).newbtlineavailability(request);

                if (newbtlineavailabilityResponse.flags.falloutFlag && newbtlineavailabilityResponse.evidence.reasondescription.ToLower().Contains("exception"))
                {
                    throw new Exception(newbtlineavailabilityResponse.evidence.reasondescription);
                }

                return NewBTLineAvailabilityServiceMapper.CreateOpenreachResponse(newbtlineavailabilityResponse);
            }
        }
    }
}
