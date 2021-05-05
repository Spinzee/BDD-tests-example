using System;

namespace Infrastructure
{
    public static class Guard
    {
        // type of exception defined at runtime (generic)
        public static void Against<TException>(bool assertion, string message) where TException : Exception
        {
            if (!assertion)
            {
                return;
            }

            // Activator, dynamically construct objects at run time
            throw (TException)Activator.CreateInstance(typeof(TException), message);
        }
    }
}
