namespace Products.ControllerHelpers.Energy.Mappers
{
    using System.Text;
    using Model.Common;
    using WebModel.ViewModels.Energy;

    public sealed class QASMapper
    {
        public static QasAddress MapSelectAddressViewModelToQasAddress(SelectAddressViewModel viewModel)
        {
            var address = new QasAddress
            {
                HouseName = viewModel.PropertyNumber,
                AddressLine1 = viewModel.AddressLine1,
                AddressLine2 = viewModel.AddressLine2,
                Town = viewModel.Town,
                County = viewModel.County,
                Moniker = null,
                PicklistEntry = MapSelectAddressViewModelToPickListEntry(viewModel)
            };


            return address;
        }

        private static string MapSelectAddressViewModelToPickListEntry(SelectAddressViewModel viewModel)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(viewModel.PropertyNumber))
            {
                builder.Append($"{viewModel.PropertyNumber} ");
            }

            if (!string.IsNullOrEmpty(viewModel.AddressLine1))
            {
                builder.Append($"{viewModel.AddressLine1} ");
            }

            if (!string.IsNullOrEmpty(viewModel.AddressLine2))
            {
                builder.Append($"{viewModel.AddressLine2} ");
            }

            if (!string.IsNullOrEmpty(viewModel.Town))
            {
                builder.Append($"{viewModel.Town} ");
            }

            if (!string.IsNullOrEmpty(viewModel.County))
            {
                builder.Append($"{viewModel.County} ");
            }

            return builder.ToString();
        }
    }
}
