using System;

namespace Products.Infrastructure
{
    public static class Guard
    {
        public static void Against(bool assertion, string message)
        {
            if (!assertion)
            {
                return;
            }

            throw new InvalidOperationException(message);
        }

        public static void Against<TException>(bool assertion, string message) where TException : Exception
        {
            if (!assertion)
            {
                return;
            }

            throw (TException)Activator.CreateInstance(typeof(TException), message);
        }
    }
}
