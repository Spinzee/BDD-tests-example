namespace Products.Service.Broadband.Managers
{
    using System.Collections.Generic;
    using Core;
    using Products.Model.Broadband;
    using Products.WebModel.ViewModels.Common;

    public interface IBroadbandManager
    {
        BroadbandType GetBroadbandType(string broadbandCode);

        double GetSurcharge();

        double GetConnectionFee();

        double GetInstallationFee();

        double GetEquipmentChargeFee();

        decimal GetEarlyTerminationChargeBroadbandFee();

        decimal GetEarlyTerminationChargeFibreAndFibrePlusFee();

        decimal GetTerminationChargeBroadbandFee();

        decimal GetTerminationChargeFibreAndFibrePlusFee();

        decimal GetEarlyExitFeeFixAndFibre();

        BroadbandProductGroup BroadbandProductGroup(string productCode);

        List<TermsAndConditionsPdfLink> GetTermsAndConditionPdfWithLinks(BroadbandProductGroup productGroup);
    }
}