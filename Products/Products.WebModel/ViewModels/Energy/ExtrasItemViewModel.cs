namespace Products.WebModel.ViewModels.Energy
{
    using System;
    using System.Collections.Generic;
    using Infrastructure.Extensions;
    using Resources.Energy;

    public class ExtrasItemViewModel : IEquatable<ExtrasItemViewModel>
    {
        public ExtrasItemViewModel(
            string name,
            double price,
            string productCode,
            string tagLine,
            List<string> bulletList1,
            List<string> bulletList2,
            bool isAddedToBasket,
            string baseUrl)
        {
            Name = name;
            Price = price.ToCurrency();
            ProductCode = productCode;
            TagLine = tagLine;
            BulletList1 = bulletList1;
            BulletList2 = bulletList2;
            IsAddedToBasket = isAddedToBasket;
            IsAddedToBasketStr = isAddedToBasket.ToString().ToLowerInvariant();
            AddRemoveButtonText = isAddedToBasket ? Extras_Resources.RemoveButtonText : Extras_Resources.AddButtonText;
            AddRemoveButtonAltText = isAddedToBasket ? Extras_Resources.RemoveButtonAlt : Extras_Resources.AddButtonTextAlt;
            RemoveButtonAltText = Extras_Resources.RemoveButtonAlt;
            AddButtonAltText = Extras_Resources.AddButtonTextAlt;
            RemoveButtonIconUrl = $"{baseUrl}/Content/Svgs/icons/trashcan-white.svg";
            AddedCssClass = isAddedToBasket ? "added" : "";
            Description = Extras_Resources.ElecWiringCoverDescription;
            YouWillGetHeaderText = string.Format(Extras_Resources.YouWillGetHeaderText, name);
            YouWillGetList = new List<string>
            {
                Extras_Resources.ElecWiringCoverGet1,
                Extras_Resources.ElecWiringCoverGet2
            };

            MoreInformationModalId = $"extraMoreInformation-{productCode}";
            MoreInformationModalDataTargetId = $"#{MoreInformationModalId}";
            ModalAddRemoveButtonText = isAddedToBasket ? Extras_Resources.ModalRemoveButtonText : Extras_Resources.ModalAddButtonText;
            ModalAddRemoveButtonCssClass = isAddedToBasket ? "button-secondary" : "button-primary";
            ExtraContainerId = $"extra-container-{productCode}";
            ButtonGroup = $"button-extra-{productCode}";
            ModalExtraPriceHeaderId = $"extra-modal-priceheader-{productCode}";
        }

        public string Name { get; }

        public string Price { get; }

        public string ProductCode { get; }

        public string TagLine { get; }

        public List<string> BulletList1 { get; }

        public List<string> BulletList2 { get; }

        public bool IsAddedToBasket { get; }

        public string IsAddedToBasketStr { get; }

        public string AddRemoveButtonText { get; }

        public string ModalAddRemoveButtonText { get; }

        public string ModalAddRemoveButtonCssClass { get; }

        public string AddRemoveButtonAltText { get; }
        
        public string RemoveButtonAltText { get; }
        
        public string AddButtonAltText { get; }

        public string RemoveButtonIconUrl { get; }

        public string AddedCssClass { get; }

        public string Description { get; }

        public string YouWillGetHeaderText { get; }

        public List<string> YouWillGetList { get; }

        public string MoreInformationModalId { get; }

        public string MoreInformationModalDataTargetId { get; }

        public string ExtraContainerId { get; }

        public string ModalExtraPriceHeaderId { get; }

        public string ButtonGroup { get; }

        public bool Equals(ExtrasItemViewModel other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ProductCode == other.ProductCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ExtrasItemViewModel) obj);
        }

        public override int GetHashCode()
        {
            return ProductCode != null ? ProductCode.GetHashCode() : 0;
        }
    }
}