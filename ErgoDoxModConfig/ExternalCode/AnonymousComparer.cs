/*--------------------------------------------------------------------------
* AnonymousComparer - lambda compare selector for Linq
* ver 1.0.0.0 (Dec. 29th, 2009)
*
* created and maintained by neuecc <ils@neue.cc>
* licensed under Microsoft Public License(Ms-PL)
* http://neue.cc/
* http://linqcomparer.codeplex.com/
*--------------------------------------------------------------------------*/

using System.Collections.Generic;

namespace System.Linq
{
    public static class AnonymousComparer
    {
        /// <summary>Example:AnonymousComparer.Create((MyClass mc) => mc.MyProperty)</summary>
        public static IEqualityComparer<T> Create<T, TKey>(Func<T, TKey> compareKeySelector)
        {
            return new EqualityComparer<T, TKey>(compareKeySelector);
        }

        private class EqualityComparer<T, TKey> : IEqualityComparer<T>
        {
            private Func<T, TKey> compareKeySelector;

            public EqualityComparer(Func<T, TKey> compareKeySelector)
            {
                this.compareKeySelector = compareKeySelector;
            }

            public bool Equals(T x, T y)
            {
                return compareKeySelector(x).Equals(compareKeySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return compareKeySelector(obj).GetHashCode();
            }
        }

        #region Extensions for LINQ Standard Query Operators

        public static bool Contains<TSource, TCompareKey>(this IEnumerable<TSource> source, TSource value, Func<TSource, TCompareKey> compareKeySelector)
        {
            return source.Contains(value, AnonymousComparer.Create(compareKeySelector));
        }

        public static IEnumerable<TSource> Distinct<TSource, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TCompareKey> compareKeySelector)
        {
            return source.Distinct(AnonymousComparer.Create(compareKeySelector));
        }

        public static IEnumerable<TSource> Except<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
        {
            return first.Except(second, AnonymousComparer.Create(compareKeySelector));
        }

        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TCompareKey> compareKeySelector)
        {
            return source.GroupBy(keySelector, AnonymousComparer.Create(compareKeySelector));
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
        {
            return source.GroupBy(keySelector, resultSelector, AnonymousComparer.Create(compareKeySelector));
        }

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TCompareKey> compareKeySelector)
        {
            return source.GroupBy(keySelector, elementSelector, AnonymousComparer.Create(compareKeySelector));
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
        {
            return source.GroupBy(keySelector, elementSelector, resultSelector, AnonymousComparer.Create(compareKeySelector));
        }

        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult, TCompareKey>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
        {
            return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, AnonymousComparer.Create(compareKeySelector));
        }

        public static IEnumerable<TSource> Intersect<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
        {
            return first.Intersect(second, AnonymousComparer.Create(compareKeySelector));
        }

        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult, TCompareKey>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
        {
            return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, AnonymousComparer.Create(compareKeySelector));
        }

        public static bool SequenceEqual<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
        {
            return first.SequenceEqual(second, AnonymousComparer.Create(compareKeySelector));
        }

        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TCompareKey> compareKeySelector)
        {
            return source.ToDictionary(keySelector, AnonymousComparer.Create(compareKeySelector));
        }

        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TCompareKey> compareKeySelector)
        {
            return source.ToDictionary(keySelector, elementSelector, AnonymousComparer.Create(compareKeySelector));
        }

        public static ILookup<TKey, TSource> ToLookup<TSource, TKey, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TCompareKey> compareKeySelector)
        {
            return source.ToLookup(keySelector, AnonymousComparer.Create(compareKeySelector));
        }

        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TCompareKey> compareKeySelector)
        {
            return source.ToLookup(keySelector, elementSelector, AnonymousComparer.Create(compareKeySelector));
        }

        public static IEnumerable<TSource> Union<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
        {
            return first.Union(second, AnonymousComparer.Create(compareKeySelector));
        }

        #endregion
    }
}