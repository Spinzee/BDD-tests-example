namespace Products.ServiceWrapper.AnnualEnergyReviewService
{
    public interface IAnnualEnergyReviewServiceWrapper
    {
        checkAERResponse checkAER(string[] customerAccountNumbers);

        getEnergyDataResponse getEnergyData(string[] customerAccountNumbers);

        actionAERResponse actionAER(actionAERRequest request);
    }
}
