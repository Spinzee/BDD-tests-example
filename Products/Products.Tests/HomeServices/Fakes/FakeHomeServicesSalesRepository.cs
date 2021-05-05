namespace Products.Tests.HomeServices.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Products.Model.HomeServices;
    using Products.Repository.HomeServices;

    public class FakeHomeServicesSalesRepository : IHomeServicesSalesRepository
    {
        public int InsertSaleCount { get; set; }

        public Exception Exception { get; set; }

        public ApplicationData ApplicationData { get; set; }

        public async Task<List<int>> SaveApplication(ApplicationData applicationData)
        {
            await Task.Delay(1);

            InsertSaleCount = 0;

            if (Exception != null)
                throw Exception;

            ApplicationData = applicationData;
            // ReSharper disable once UnusedVariable
            foreach(ProductData productData in applicationData.ProductData)
            {
                InsertSaleCount++;
            }

            return new List<int>() { InsertSaleCount};
        }
    }
}
