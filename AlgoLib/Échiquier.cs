using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLib
{
	public class Échiquier
	{
		public static char[] ColonnesÉchiquier = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

		public Échiquier(int taille)
		{
			//Taille d'un equiquier equivant a taile * taille
			//Ici on limite a 26 pour le nombres de lettre dans l'alphabet
			if (taille >= 1 && taille <= ColonnesÉchiquier.Count())
			{
				Rangées = Enumerable.Range(1, taille);
				Colonnes = Enumerable.Range(ColonnesÉchiquier[0], taille).Select(i => (char)i);
				Reines = new List<Position>();
			}
			else
				throw new ArgumentException();
		}

		public int Taille => Rangées.Count();

		public IEnumerable<int> Rangées { get; private set; }

		public IEnumerable<char> Colonnes { get; private set; }

		public IEnumerable<Position> Reines { get; private set; }

		//Doit implementer
		public bool this[Position position] => true;

		public bool AjouterReine(Position postion)
		{
			throw new NotImplementedException();
		}

		public int AjouterReines(IEnumerable<Position> reines)
		{
			throw new NotImplementedException();
		}

		public int AjouterReines(string positions)
		{
			throw new NotImplementedException();
		}

		public bool SupprimerReine(Position reine)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public Échiquier Cloner()
		{
			throw new NotImplementedException();
		}

		public string EnDessin()
		{
			throw new NotImplementedException();
		}

		public bool EstÉgal(Échiquier autre)
		{
			throw new NotImplementedException();
		}

		public Échiquier BasculerHautBas()
		{
			throw new NotImplementedException();
		}

		public Échiquier BasculerGaucheDroite()
		{
			throw new NotImplementedException();
		}

		public Échiquier BasculerObliquement()
		{
			throw new NotImplementedException();
		}

		public Échiquier BasculerControbliquement()
		{
			throw new NotImplementedException();
		}

		public Échiquier Tourner90()
		{
			throw new NotImplementedException();
		}

		public Échiquier Tourner180()
		{
			throw new NotImplementedException();
		}

		public Échiquier Tourner270()
		{
			throw new NotImplementedException();
		}

		public bool EstSemblable(Échiquier autre)
		{
			throw new NotImplementedException();
		}


	}
}
