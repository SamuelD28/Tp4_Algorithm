using System;
using System.Runtime.CompilerServices;

namespace SDD
{
    public static class Permutation
    {
        /// <summary>
        /// Génère toutes les combinaisons de N entiers compris 
        /// entre first et last, inclusivement.
        /// 
        /// <para>Exemple 1:</para>
        /// 
        /// <code>
        ///     ForAllCombinations(1, 3, 2, seq => { 
        ///         WriteLine(string.Join("", seq));
        ///         return false;
        ///     });
        /// </code>    
        /// 
        /// <para>Sortie: </para>
        /// 
        ///     11
        ///     21
        ///     31
        ///     12
        ///     22
        ///     32
        ///     13
        ///     23
        ///     33
        ///     
        /// <para>Exemple 2:</para>
        /// 
        /// <code>
        ///     int[] réponse = null;
        ///     ForAllCombinations(1, 3, 2, seq => {
        ///         if( seq.Average() == 2 )
        ///         {
        ///             // On a trouvé la combinaison recherchée
        ///             réponse = seq;
        ///             return true;
        ///         }
        ///         return false;
        ///     });
        ///     WriteLine(string.Join("", réponse));
        /// </code>    
        /// 
        /// <para>Sortie: </para>
        /// 
        ///     31
        ///     
        /// </summary>
        /// <param name="first">Premier entier</param>
        /// <param name="last">Dernier entier</param>
        /// <param name="size">Taille des séquences à générer</param>
        /// <param name="visit">Callback appelée pour chaque combinaison qui est générée. La combinaison est passée en argument. Retourner vrai pour terminer immédiatement la génération, sinon faux.</param>
        /// <returns>Vrai si la séquence se termine prématurément, à cause du callback</returns>        
        public static bool 
            ForAllCombinations(int first, int last, int size, Func<int[], bool> visit)
        {
            int nbItems = Math.Max(last - first + 1, 0);
            
            if (size <= 0)
                return false;
            
            if (nbItems == 0)
                throw new ArgumentException($"There is no items in the interval [{first}..{last}]");
            
            // Initialize combination
            int[] combination = new int[size];
            for (int i = 0; i < size; i++)
                combination[i] = first;
            
            for (; ; )
            {
                if (visit(combination))
                    return true;
                
                // Increment combination
                int level = 0;
                while (level < size)
                {
                    combination[level]++;
                    if (combination[level] > last)
                    {
                        combination[level] = first;
                        level++;
                        continue;
                    }
                    break;
                }
                if (level >= size)
                    break;
            }

            return false;
        }

        /// <summary>
        /// Génère toutes les combinaisons de N valeurs comprises 
        /// entre map(first) et map(last), inclusivement.
        /// 
        /// <para>Exemple:</para>
        /// 
        /// <code>
        ///     ForAllCombinations('A', 'C', 2, i => (char)i, seq => {
        ///         WriteLine(string.Join("", seq));
        ///         return false;
        ///     });
        /// </code>    
        /// 
        /// <para>Sortie:</para>
        ///     
        ///     AA
        ///     BA
        ///     CA
        ///     AB
        ///     BB
        ///     CB
        ///     CA
        ///     CB
        ///     CC
        ///     
        /// </summary>
        /// <param name="first">Premier entier</param>
        /// <param name="last">Dernier entier</param>
        /// <param name="size">Taille des séquences</param>
        /// <param name="map">Fonction à  appliquer à chaque entier entre first et last pour produire les valeurs de la séquence</param>
        /// <param name="visit">Callback appelée pour chaque combinaison qui est générée. Retourner vrai pour terminer immédiatement la génération, sinon faux.</param>
        /// <returns>Vrai si la séquence se termine prématurément</returns>        
        public static bool ForAllCombinations<T>(int first, int last, int size, Func<int, T> map, Func<T[], bool> visit)
        {

            if (size <= 0)
                return false;

            int nbItems = last - first + 1;
            if (nbItems <= 0)
                throw new ArgumentException($"There is no items in the interval [{first}..{last}]");

            // Precompute mappings
            T mapFirst = map(first);
            T[] maps = new T[nbItems];
            for(int i = first; i <= last; i++)
            {
                maps[i-first] = map(i);
            }

            // Initialize counts and combination
            int[] counts = new int[size];
            T[] combination = new T[size];
            for (int i = 0; i < size; i++)
            {
                counts[i] = first;
                combination[i] = mapFirst;
            }

            // Generate and visit all combinations
            for (; ; )
            {
                if (visit(combination))
                    return true;

                // Increment combination
                int level = 0;
                while (level < size)
                {
                    counts[level]++;
                    if (counts[level] > last)
                    {
                        counts[level] = first;
                        combination[level] = mapFirst;
                        level++;
                        continue;
                    }
                    else
                    {
                        combination[level] = maps[counts[level]-first];
                    }
                    break;
                }
                if (level >= size)
                    break;
            }

            return false;
        }

        /// <summary>
        /// Génère toutes les combinaisons de N valeurs prises dans l'ensemble donné. 
        /// 
        /// <para>Exemple:</para>
        /// 
        /// <code>
        ///     ForAllCombinations(new[]{'x', 'y', 'z'}, 2, seq => {
        ///         WriteLine(string.Join("", seq));
        ///         return false;
        ///     });
        /// </code>    
        /// 
        /// <para>Sortie:</para>
        ///     
        ///     xx
        ///     yx
        ///     zx
        ///     yx
        ///     yy
        ///     zy
        ///     xz
        ///     yz
        ///     zz
        ///     
        /// </summary>
        /// <param name="items">Ensembles de valeurs dans lesquels on pige</param>
        /// <param name="size">Taille des séquences à générer</param>
        /// <param name="visit">Callback appelée pour chaque combinaison qui est générée. Retourner vrai pour terminer immédiatement la génération, sinon faux.</param>
        /// <returns>Vrai si la séquence se termine prématurément</returns>        
        public static bool 
            ForAllCombinations<T>(T[] items, int size, Func<T[], bool> visit)
        {
            int nbItems = items.Length;
            if (size <= 0) 
                return false;
            if (nbItems == 0)
                throw new ArgumentException("Items should contain at least one item");
            T[] combination = new T[size];
            for (int i = 0; i < size; i++)
                combination[i] = items[0];
            int[] counts = new int[size];
            for(;;)
            {
                if (visit(combination))
                    return true;
                // Increment counts and update combination by the way
                int level = 0;
                while(level < size)
                {
                    counts[level]++;
                    if (counts[level] >= nbItems)
                    {
                        counts[level] = 0;
                        combination[level] = items[0];
                        level++;
                        continue;
                    }
                    break;
                }
                if (level >= size)
                    break;
                else
                    combination[level] = items[counts[level]];
            }
            return false;
        }

        /// <summary>
        /// Heap's algorithm to find all pmermutations. 
        /// Non recursive, more efficient.
        /// 
        /// <para>
        ///     See: https://en.wikipedia.org/wiki/Heap%27s_algorithm#cite_note-3
        /// </para>
        /// <para>
        ///     Code from : https://stackoverflow.com/questions/11208446/generating-permutations-of-a-set-most-efficiently
        /// </para>
        /// 
        /// <para>NB: Les permutations ne sont pas générées en ordre croissant.</para>
        /// 
        /// <para>================== Example =======================</para> 
        /// 
        /// <code>
        ///     ForAllPermutations(new[]{'a', 'b', 'c'}, perm => {
        ///         WriteLine(string.Join("", perm));
        ///         return false;
        ///     });
        /// </code>
        /// 
        /// <para>=================== Sortie =======================</para> 
        ///        
        ///     abc
        ///     acb
        ///     bac
        ///     bca
        ///     cab
        ///     cba
        /// 
        /// <para>NB: Pas nécessairement dans cet ordre...</para> 
        /// </summary>
        /// <param name="items">Items to permute in each possible ways</param>
        /// <param name="visit">Function to call with each permutation. May return true to cancell the rest of the visit.</param>
        /// <returns>Return true if process was cancelled</returns> 
        public static bool ForAllPermutations<T>(T[] items, Func<T[], bool> visit)
        {
            int countOfItem = items.Length;

            if (countOfItem <= 0)
            {
                return false;
            }

            if (countOfItem == 1)
            {
                return visit(items);
            }

            var indexes = new int[countOfItem];
            for (int i = 0; i < countOfItem; i++)
            {
                indexes[i] = 0;
            }

            if (visit(items))
            {
                return true;
            }

            for (int i = 1; i < countOfItem;)
            {
                if (indexes[i] < i)
                { // On the web there is an implementation with a multiplication which should be less efficient.
                    if ((i & 1) == 1) // if (i % 2 == 1)  ... more efficient ??? At least the same.
                    {
                        Swap(ref items[i], ref items[indexes[i]]);
                    }
                    else
                    {
                        Swap(ref items[i], ref items[0]);
                    }

                    if (visit(items))
                    {
                        return true;
                    }

                    indexes[i]++;
                    i = 1;
                }
                else
                {
                    indexes[i++] = 0;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

    }
}
