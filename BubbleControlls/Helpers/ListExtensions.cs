using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BubbleControlls.Helpers
{
    public static class ListExtensions
    {
        public static T? FindDeep<T>(this IEnumerable<T> list, Guid id, Func<T, Guid> idSelector, Func<T, IEnumerable<T>?> childSelector)
        {
            foreach (var item in list)
            {
                if (idSelector(item) == id)
                    return item;

                var children = childSelector(item);
                if (children != null)
                {
                    var found = children.FindDeep(id, idSelector, childSelector);
                    if (found != null)
                        return found;
                }
            }
            return default;
        }
        public static T? FindDeep<T>(this IEnumerable<T> list, string name, Func<T, string> nameSelector, Func<T, IEnumerable<T>?> childSelector)
        {
            foreach (var item in list)
            {
                if (nameSelector(item) == name)
                    return item;

                var children = childSelector(item);
                if (children != null)
                {
                    var found = children.FindDeep(name, nameSelector, childSelector);
                    if (found != null)
                        return found;
                }
            }
            return default;
        }
    }
}
