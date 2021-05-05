namespace Products.Tests.Infrastructure.Extensions
{
    using Core.Enums;
    using NUnit.Framework;
    using Products.Infrastructure.Extensions;
    using Should;

    [TestFixture]
    public class EnumExtensionsTests
    {
        [Test]
        public void GetDescriptionReturnsCorrectValues()
        {
            // Arrange/Act/Assert
            BundlePackageType.FixAndFibre.GetDescription().ShouldEqual("Fix & Fibre");
            BundlePackageType.FixAndProtect.GetDescription().ShouldEqual("Fix & Protect");
            BundlePackageType.None.GetDescription().ShouldEqual("None");
        }

        [Test]
        public void ToDescriptionReturnsValues()
        {
            // Arrange/Act/Assert
            BundlePackageType.FixAndFibre.ToDescription().ShouldEqual("Fix & Fibre");
            BundlePackageType.FixAndProtect.ToDescription().ShouldEqual("Fix & Protect");
            BundlePackageType.None.ToDescription().ShouldEqual("None");
        }
    }
}
