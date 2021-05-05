using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Service.HomeServices
{
    public interface IStepCounterService
    {
        string GetStepCounter(string actionName);
    }
}
