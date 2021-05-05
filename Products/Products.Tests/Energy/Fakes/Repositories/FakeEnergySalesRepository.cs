namespace Products.Tests.Energy.Fakes.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Repository.Energy;

    public class FakeEnergySalesRepository : IEnergySalesRepository
    {
        private List<SubProd> _subProds;

        public async Task<int> GetSubProductIdForFuelType(string product, int baseProductId, bool isPrePay)
        {
            await Task.Delay(1);

            if (_subProds != null)
            {
                SubProd first = null;
                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (SubProd p in _subProds)
                {
                    if (p.Name == product && p.BaseProductId == baseProductId)
                    {
                        first = p;
                        break;
                    }
                }

                if (first != null)
                {
                    return first.SubProductId;
                }
            }

            return 1;
        }


        public FakeEnergySalesRepository WithSubProducts()
        {
            _subProds = new List<SubProd>
            {
                new SubProd { Name = "SSE 1 Year Fixed v15", BaseProductId = 1, SubProductId = 910 },
                new SubProd { Name = "SSE 1 Year Fixed v15", BaseProductId = 2, SubProductId = 911 },
                new SubProd { Name = "SSE 1 Year Fixed v15", BaseProductId = 4, SubProductId = 912 },
                new SubProd { Name = "SSE 1 Year Fixed v15", BaseProductId = 5, SubProductId = 913 },
                new SubProd { Name = "Standard Energy", BaseProductId = 5, SubProductId = 193 },
                new SubProd { Name = "Standard Energy", BaseProductId = 4, SubProductId = 194 },
                new SubProd { Name = "Standard Energy", BaseProductId = 2, SubProductId = 195 },
                new SubProd { Name = "Standard Energy", BaseProductId = 1, SubProductId = 196 },
                new SubProd { Name = "SSE 1 Year Fix And Fibre", BaseProductId = 1, SubProductId = 123 },
                new SubProd { Name = "SSE 1 Year Fix And Fibre", BaseProductId = 2, SubProductId = 124 },
                new SubProd { Name = "SSE 1 Year Fix And Fibre", BaseProductId = 4, SubProductId = 125 },
                new SubProd { Name = "SSE 1 Year Fix And Fibre", BaseProductId = 5, SubProductId = 126 }
            };

            return this;
        }
    }

    public class SubProd
    {
        public string Name { get; set; }
        public int SubProductId { get; set; }
        public int BaseProductId { get; set; }
    }
}