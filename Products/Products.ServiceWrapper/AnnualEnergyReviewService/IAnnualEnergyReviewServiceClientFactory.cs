namespace Products.ServiceWrapper.AnnualEnergyReviewService
{
    public interface IAnnualEnergyReviewServiceClientFactory
    {
        AnnualEnergyReviewServiceClient Create();

        messageHeader CreateMessageHeader();
    }
}
