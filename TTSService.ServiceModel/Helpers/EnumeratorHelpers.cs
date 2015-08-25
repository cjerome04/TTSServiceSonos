
using System.Collections.Generic;

namespace TTSService.ServiceModel.Helpers
{
    public static class EnumeratorHelpers
    {
        public static List<T> SaveRest<T>(this IEnumerator<T> e)
        {
            var list = new List<T>();
            while (e.MoveNext())
            {
                list.Add(e.Current);
            }
            return list;
        }
    }
}
