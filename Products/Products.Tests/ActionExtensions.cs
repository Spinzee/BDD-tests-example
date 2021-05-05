namespace Products.Tests
{
    using System;
    using NUnit.Framework;
    using Should;

    // ReSharper disable once UnusedMember.Global
    public static class ActionExtensions
    {
        public static void ShouldThrowArgumentExceptionWithMessage(this Action action, string message)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                ex.ShouldBeType(typeof(ArgumentException));
                ex.Message.ShouldEqual(message);
                return;
            }

            Assert.Fail("Expected exception not met");
        }
    }
}
