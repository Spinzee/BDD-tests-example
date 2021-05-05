namespace Products.Service.Common.Exceptions
{
    using System;

    public class DuplicateTariffTickUspFoundException : Exception
    {
        public DuplicateTariffTickUspFoundException(string servicePlanId)
            : base($"More than one instance of <tariffGroup> defined for a given 'Service PlanId: {servicePlanId}' in web.config.")
        { }
    }
}
