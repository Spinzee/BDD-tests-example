using System.ServiceModel;

namespace Products.ServiceWrapper.AnnualEnergyReviewService
{
    public class AnnualEnergyReviewServiceClient : ClientBase<IAnnualEnergyReviewServiceClient>, IAnnualEnergyReviewServiceClient
    {
        public actionAERResponse actionAER(actionAERRequest request)
        {
            var result = Channel.actionAER(request);
            return result;
        }

        public cancelAERResponse cancelAER(cancelAERRequest request)
        {
            var result = Channel.cancelAER(request);
            return result;
        }

        public checkAERResponse checkAER(checkAERRequest request)
        {
            var result = Channel.checkAER(request);
            return result;
        }

        public getEnergyDataResponse getEnergyData(getEnergyDataRequest request)
        {
            var result = Channel.getEnergyData(request);
            return result;
        }

        public getNextReviewDateResponse getNextReviewDate(getNextReviewDateRequest request)
        {
            var result = Channel.getNextReviewDate(request);
            return result;
        }
    }
}
