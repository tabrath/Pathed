using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathed
{
    public static class IListExtensions
    {
        public static void Add<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items.ToArray())
            {
                list.Add(item);
            }
        }

        public static void Remove<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items.ToArray())
            {
                list.Remove(item);
            }
        }

        public static void Update<T>(this IList<T> list, IEnumerable<T> items)
        {
            var removed = list.Where(x => !items.Contains(x)).ToArray();
            var added = items.Where(x => !list.Contains(x)).ToArray();

            list.Add(added);
            list.Remove(removed);
        }

        public static bool TryGetDifference<T>(this IList<T> list, IEnumerable<T> items, out IEnumerable<T> added, out IEnumerable<T> removed,
            Func<IEnumerable<T>, T, bool> predicate)
        {
            removed = list.Where(x => !predicate(items, x)).ToArray();
            added = items.Where(x => !predicate(list, x)).ToArray();

            return ((T[])added).Length > 0 || ((T[])removed).Length > 0;
        }
    }
}
