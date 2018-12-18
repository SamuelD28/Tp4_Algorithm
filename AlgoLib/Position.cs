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
			if (position.Length == 2)
				throw new ArgumentException();

			if (char.IsLetter(position[0]) && char.IsDigit(position[1]))
			{
				Colonne = position[0];
				Rangée = (int)position[1];
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
			if (obj is null)
				return false;

			return Equals((Position)obj);
		}

		public override string ToString()
		{
			return base.ToString();
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
}
