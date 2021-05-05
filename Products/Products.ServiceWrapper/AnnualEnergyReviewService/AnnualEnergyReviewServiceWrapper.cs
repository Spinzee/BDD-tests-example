using System;

namespace Products.ServiceWrapper.AnnualEnergyReviewService
{
    public class AnnualEnergyReviewServiceWrapper :IAnnualEnergyReviewServiceWrapper
    {
        private readonly IAnnualEnergyReviewServiceClientFactory _annualEnergyReviewServiceClientFactory;

        public AnnualEnergyReviewServiceWrapper(IAnnualEnergyReviewServiceClientFactory annualEnergyReviewServiceClientFactory)
        {
            _annualEnergyReviewServiceClientFactory = annualEnergyReviewServiceClientFactory;
        }

        public actionAERResponse actionAER(actionAERRequest request)
        {
            actionAERResponse response;

            using (var client = _annualEnergyReviewServiceClientFactory.Create())
            {
                actionAERRequest actionAerRequest = new actionAERRequest
                {
                    messageHeader = _annualEnergyReviewServiceClientFactory.CreateMessageHeader(),
                    aerRequestCollection = request.aerRequestCollection,
                    csrUserID = request.csrUserID
                };

                response = client.actionAER(actionAerRequest);
            }

            return response;
        }

        public cancelAERResponse cancelAER(cancelAERRequest request)
        {
            throw new NotImplementedException();
        }

        public checkAERResponse checkAER(string[] customerAccountNumbers)
        {
            checkAERResponse response;

            using (var client = _annualEnergyReviewServiceClientFactory.Create())
            {
                checkAERRequest request = new checkAERRequest
                {
                    messageHeader = _annualEnergyReviewServiceClientFactory.CreateMessageHeader(),
                    customerAccountCollection = customerAccountNumbers

                };

                response = client.checkAER(request);
            }

            return response;
        }

        public getEnergyDataResponse getEnergyData(string[] customerAccountNumbers)
        {
            getEnergyDataResponse response;

            using (var client = _annualEnergyReviewServiceClientFactory.Create())
            {
                getEnergyDataRequest request = new getEnergyDataRequest
                {
                    messageHeader = _annualEnergyReviewServiceClientFactory.CreateMessageHeader(),
                    customerAccountCollection = customerAccountNumbers
                };

                response = client.getEnergyData(request);
            }

            return response;
        }

        public getNextReviewDateResponse getNextReviewDate(getNextReviewDateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
