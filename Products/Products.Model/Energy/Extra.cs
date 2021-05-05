namespace Products.Model.Energy
{
    using System;
    using System.Collections.Generic;
    using Core.Enums;

    public class Extra : IEquatable<Extra>
    {
        public Extra(string name, 
            double bundlePrice, 
            double originalPrice, 
            string productCode, 
            double id, 
            double contractLength,
            string tagLine,
            List<string> bulletList1,
            List<string> bulletList2,
            ExtraType type)
        {
            Name = name;
            BundlePrice = bundlePrice;
            OriginalPrice = originalPrice;
            ProductCode = productCode;
            Id = id;
            ContractLength = contractLength;
            TagLine = tagLine;
            BulletList1 = bulletList1;
            BulletList2 = bulletList2;
            Type = type;
        }

        public string Name { get; }

        public double BundlePrice { get; }

        public double OriginalPrice { get; }

        public string ProductCode { get;}

        public double Id { get; }

        public double ContractLength { get; }

        public string TagLine { get; }

        public List<string> BulletList1 { get; }

        public List<string> BulletList2 { get; }

        public ExtraType Type { get; }

        public bool Equals(Extra other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(ProductCode, other.ProductCode);
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

            return Equals((Extra) obj);
        }

        public override int GetHashCode()
        {
            return ProductCode != null ? ProductCode.GetHashCode() : 0;
        }
    }
}