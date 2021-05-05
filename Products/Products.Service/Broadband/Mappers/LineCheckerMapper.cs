namespace Products.Service.Broadband.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using Products.Model.Common;
    using Products.WebModel.ViewModels.Broadband;

    public static class LineCheckerMapper
    {
        public static List<AddressViewModel> MapBTAddressListToAddressViewModelList(List<BTAddress> addresses)
        {
            return addresses.Select((address, index) => new AddressViewModel
            {
                Id = index + 1,
                Postcode = address.Postcode,
                SubPremises = address.SubPremises,
                PremiseName = address.PremiseName,
                ThoroughfareNumber = address.ThoroughfareNumber,
                PostTown = address.PostTown,
                ThoroughfareName = address.ThoroughfareName,
                Locality = address.Locality,
                DistrictId = address.DistrictId,
                UPRN = address.UPRN
            }).ToList();
        }

        public static BTAddress MapAddressViewModelToBTAddress(AddressViewModel addressViewModel)
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
