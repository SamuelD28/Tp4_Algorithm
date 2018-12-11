using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDD
{
    public static partial class MenuMotDePasse
    {
        static partial void AfficherDesCombinaisons()
        {
            Permutation.ForAllCombinations(0, 1, 4, seq => {
                        Console.Write(string.Join("", seq) + " ");
                        return false;
                     });
            }
    }
}
