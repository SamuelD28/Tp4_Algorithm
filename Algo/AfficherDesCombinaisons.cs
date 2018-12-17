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
			Permutation.ForAllCombinations(0, 1, 4, seq =>
			{
				Console.Write(string.Join("", seq) + " ");
				return false;
			});

			Console.WriteLine("\n");

			IList<char[]> listeCroissante = new List<char[]>();
			Permutation.ForAllCombinations('A', 'C', 3, 
					i => (char)i, 
					seq => {

						int currentSum = seq.Sum(b => b);
						int lastSum = (listeCroissante.Count() != 0)?listeCroissante.Last().Sum(b => b): -1;

						if (currentSum > lastSum)
						{
							char[] newReference = new char[seq.Count()];
							Array.Copy(seq, newReference, seq.Count());
							listeCroissante.Add(newReference);
							Console.Write(string.Join("", seq) + " ");
						}

					return false;
			});

			Console.WriteLine("\n");

			int compteur = 0;
			Permutation.ForAllCombinations(new[] { 3, 6, 9 }, 3, seq =>
			{
				double moyenne = seq.Average();
				if (moyenne == 6.0)
				{
					compteur++;
					Console.Write(string.Join("", seq) + " ");
				}

				if (compteur == 5)
					return true;

				return false;
			});
		}
	}
}
