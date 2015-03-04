using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathed
{
    public static class IListExtensions
    {
        public static void Add<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items.ToArray())
            {
                list.Add(item);
            }
        }

        public static void Remove<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (items == null)
                throw new ArgumentNullException("items");

            foreach (var item in items.ToArray())
            {
                list.Remove(item);
            }
        }

        public static void Update<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (items == null)
                throw new ArgumentNullException("items");

            var removed = list.Where(x => !items.Contains(x)).ToArray();
            var added = items.Where(x => !list.Contains(x)).ToArray();

            list.Add(added);
            list.Remove(removed);
        }

        public static bool TryGetDifference<T>(this IList<T> list, IEnumerable<T> items, out IEnumerable<T> added, out IEnumerable<T> removed,
            Func<IEnumerable<T>, T, bool> predicate)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (items == null)
                throw new ArgumentNullException("items");

            removed = list.Where(x => !predicate(items, x)).ToArray();
            added = items.Where(x => !predicate(list, x)).ToArray();

            return ((T[])added).Length > 0 || ((T[])removed).Length > 0;
        }
    }
}
