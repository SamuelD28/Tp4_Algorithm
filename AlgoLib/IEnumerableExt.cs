using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDD
{
    public static class IEnumerableExt
    {
		public static IList<T> Shuffle<T>(this Random r, IList<T> array)
		{
			int n = array.Count;
			while(n > 1)
			{
				int k = r.Next(n--);
				T tempArray = array[n];
				array[n] = array[k];
				array[k] = tempArray;
			}
			return array;
		}

		public static IEnumerable<char> RangeOfChars(char start, char end)
		{
			return Enumerable.Range((int)start, (int)end - (int)start + 1).Select(i => (char)i);
		}

		public static bool EstOrdonné<T>(this IEnumerable<T> items)
        {
            bool IsOrdered = true;
            for (int i = 0; i < items.Count(); i++)
            {
                if (i < items.Count() - 1)
                {
                    int differenceComparator = Comparer<T>.Default.Compare(items.ElementAt(i), items.ElementAt(i + 1));
                    if (differenceComparator > 0)
                        IsOrdered = false;
                }
            }
            return IsOrdered;
        }

        public static bool EstStrictementOrdonné<T>(this IEnumerable<T> items)
        {
            bool IsOrdered = true;
            for (int i = 0; i < items.Count(); i++)
            {
                if (i < items.Count() - 1)
                {
                    int differenceComparator = Comparer<T>.Default.Compare(items.ElementAt(i), items.ElementAt(i + 1));
                    if (differenceComparator > 0)
                        IsOrdered = false;
                    else if (differenceComparator == 0)
                        IsOrdered = false;
                }
            }
            return IsOrdered;
        }

        public static int NbDoublons<T>(this IEnumerable<T> items)
        {
            int NbDoublons = 0;
            HashSet<T> set = new HashSet<T>();
            for (int i = 0; i < items.Count(); i++)
            {
                if (set.Add(items.ElementAt(i)))
                    set.Add(items.ElementAt(i));
                else
                    NbDoublons++;
            }
            return NbDoublons;
        }

        public static bool EstSansDoublons<T>(this IEnumerable<T> items)
        {
            if (items.NbDoublons() == 0)
                return true;
            else
                return false;
        }

        public static ISet<T> Doublons<T>(this IEnumerable<T> items)
        {
            HashSet<T> Doublons = new HashSet<T>();
            HashSet<T> set = new HashSet<T>();
            for (int i = 0; i < items.Count(); i++)
            {
                if (set.Add(items.ElementAt(i)))
                    set.Add(items.ElementAt(i));
                else
                    Doublons.Add(items.ElementAt(i));
            }
            return Doublons;
        }

        public static IDictionary<T, int> DoublonsComptés<T>(this IEnumerable<T> items)
        {
            Dictionary<T, int> Doublons = new Dictionary<T, int>();
            for (int i = 0; i < items.Count(); i++)
            {
                if (!Doublons.ContainsKey(items.ElementAt(i)))
                    Doublons.Add(items.ElementAt(i), 1);
                else
                    Doublons[items.ElementAt(i)] = Doublons[items.ElementAt(i)] + 1;
            }
            return Doublons.Where(x => x.Value > 1).ToDictionary(x => x.Key, x => x.Value);        
        }

        public static KeyValuePair<T, int> DoublonMax<T>(this IEnumerable<T> items)
        {
            Dictionary<T, int> Doublons = (Dictionary<T, int>)items.DoublonsComptés();

            if (Doublons.Count == 0)
                throw new InvalidOperationException();

            return Doublons.MaxBy(x => x.Value);
        }

        public static IDictionary<T, IList<int>> DoublonsPositionnés<T>(this IEnumerable<T> items)
        {
            Dictionary<T, IList<int>> Doublons = new Dictionary<T, IList<int>>();
            for (int i = 0; i < items.Count(); i++)
            {
                if (!Doublons.ContainsKey(items.ElementAt(i)))
                    Doublons.Add(items.ElementAt(i), new List<int>(){i});
                else
                    Doublons[items.ElementAt(i)].Add(i);
            }
            return Doublons.Where(x => x.Value.Count > 1 ).ToDictionary(x => x.Key, x=> x.Value);
        }

        static readonly Random m_Random = new Random();

        public static string EnTexte<T>(this IEnumerable<T> en, string séparateur = " ", bool showCount = false, int? nbMax = null)
        {
            int max = nbMax ?? int.MaxValue;
            int count = en.Count();
            var sb = new StringBuilder();
            if (showCount)
                sb.Append($"[{count}] ");
            bool firstPass = true;
            foreach (var item in en.Take(max))
            {
                if (firstPass)
                {
                    firstPass = false;
                }
                else
                {
                    sb.Append(séparateur);
                }
                sb.Append(item.ToString());
            }
            if (count > max)
                sb.Append($" ... ({count - max} autres)");
            return sb.ToString();
        }

        public static T MaxBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
        {
            return en.Select(t => new Tuple<T, R>(t, evaluate(t)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) > 0 ? next : max).Item1;
        }

        public static T MinBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
        {
            return en.Select(t => new Tuple<T, R>(t, evaluate(t)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) < 0 ? next : max).Item1;
        }

        public static IOrderedEnumerable<T> Sorted<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.OrderBy(elem => elem);
        }

        public static double StdDev<T>(this IEnumerable<T> list, Func<T, double> values = null)
        {
            /*  Références: 
             *  https://stackoverflow.com/questions/2253874/linq-equivalent-for-standard-deviation 
             *  http://warrenseen.com/blog/2006/03/13/how-to-calculate-standard-deviation/
            */

            var mean = 0.0;
            var sum = 0.0;
            var stdDev = 0.0;
            var n = 0;
            foreach (var value in list.Select(values ?? (v => Convert.ToDouble(v))))
            {
                n++;
                var delta = value - mean;
                mean += delta / n;
                sum += delta * (value - mean);
            }
            if (1 < n)
                stdDev = Math.Sqrt(sum / (n - 1));

            return stdDev;

        }
    }
}
