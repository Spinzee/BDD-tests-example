namespace Products.ControllerHelpers.Energy.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using Model.Common;
    using WebModel.ViewModels.Energy;

    public sealed class BTMapper
    {
        public static List<BTAddressViewModel> MapBTAddressListToBTAddressViewModelList(List<BTAddress> addresses)
        {
            return addresses.Select((address, index) => new BTAddressViewModel
            {
                Id = index + 1,
                Postcode = address.Postcode,
                SubPremises = address.SubPremises,
                PremiseName = address.PremiseName,
                ThoroughfareNumber = address.ThoroughfareNumber,
                PostTown = address.PostTown,
                ThoroughfareName = address.ThoroughfareName,
                Locality = address.Locality,
                DistrictId = address.DistrictId
            }).ToList();
        }

        public static BTAddress MapBTAddressViewModelToBTAddress(BTAddressViewModel addressViewModel)
        {
            return new BTAddress
            {
                DistrictId = addressViewModel.DistrictId,
                Id = addressViewModel.Id,
                Locality = addressViewModel.Locality,
                ParentUPRN = addressViewModel.ParentUPRN,
                Postcode = addressViewModel.Postcode,
                PostTown = addressViewModel.PostTown,
                PremiseName = addressViewModel.PremiseName,
                SubPremises = addressViewModel.SubPremises,
                ThoroughfareName = addressViewModel.ThoroughfareName,
                ThoroughfareNumber = addressViewModel.ThoroughfareNumber,
                UPRN = addressViewModel.UPRN
            };
        }
    }
}
