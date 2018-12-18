using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLib
{
	public struct Position : IEquatable<Position>
	{
		public Position(char colonne, int rangée)
		{
			if (colonne >= 'a' && colonne <= 'z')
				Colonne = colonne;
			else
				throw new ArgumentException();

			if (rangée >= 1)
				Rangée = rangée;
			else
				throw new ArgumentException();

		}

		public Position(string position)
		{
			if (position.Length != 2)
				throw new ArgumentException();

			if (char.IsLetter(position[0]) && position[1].IsHex())
			{
				//Il faut traiter les chiffre en hexadecimales
				Colonne = position[0];
				Rangée = Convert.ToInt32(position[1].ToString(), 16); //Doit optimiser

				if (Rangée <= 0)
					throw new ArgumentException();

			}
			else
				throw new ArgumentException();

		}

		public char Colonne;
		public int Rangée;

		public override int GetHashCode()
		{
			int hash = 1;
			hash = hash * 17 + Colonne.GetHashCode();
			hash = hash * 31 + Rangée.GetHashCode();
			return hash;
		}

		public override bool Equals(object obj)
		{
			if (obj is null || !(obj is Position))
				return false;

			return Equals((Position)obj);
		}

		public override string ToString()
		{
			string hex = Rangée.ToString("X");
			return Colonne.ToString() + hex;
		}

		public bool Equals(Position p)
		{
			if (ReferenceEquals(p, null))
				return false;

			if (GetType() != p.GetType())
				return false;

			// Optimization for a common success case.
			return ((Colonne == p.Colonne) && (Rangée == p.Rangée));
		}

		public static bool operator ==(Position lhs, Position rhs)
		{
			// Check for null on left side.
			if (ReferenceEquals(lhs, null))
			{
				if (ReferenceEquals(rhs, null))
				{
					// null == null = true.
					return true;
				}

				// Only the left side is null.
				return false;
			}
			// Equals handles case of null on right side.
			return lhs.Equals(rhs);
		}

		public static bool operator !=(Position lhs, Position rhs)
		{
			return !(lhs == rhs);
		}
	}

	public static class Extensions
	{
		public static bool IsHex(this char c)
		{
			return (c >= '0' && c <= '9') ||
					 (c >= 'a' && c <= 'f') ||
					 (c >= 'A' && c <= 'F');
		}
	}
}
