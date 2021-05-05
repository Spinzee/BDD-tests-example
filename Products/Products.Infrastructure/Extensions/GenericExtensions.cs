namespace Products.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class GenericExtensions
    {
        public static void Replace<T>(this List<T> list, Predicate<T> oldItemSelector, T newItem)
        {
            int oldItemIndex = list.FindIndex(oldItemSelector);
            list[oldItemIndex] = newItem;
        }

        public static T ToEnum<T>(this string enumValue)
        {
            return (T)Enum.Parse(typeof(T), enumValue, true);
        }
    }
}
