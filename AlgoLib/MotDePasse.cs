using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SDD
{
	public static class MotDePasse
	{
		public static readonly MD5 Md5 = MD5.Create();
		public static readonly SHA1 Sha1 = SHA1.Create();
		public static readonly SHA256 Sha256 = SHA256.Create();
		public static readonly SHA384 Sha384 = SHA384.Create();
		public static readonly SHA512 Sha512 = SHA512.Create();
		public static readonly HashAlgorithm[] HashAlgorithms = new HashAlgorithm[] { Md5, Sha1, Sha256, Sha384, Sha512 };

		public static HashAlgorithm GetHashAlgo(int byteSize)
		{
			switch (byteSize)
			{
				case 16: return Md5;
				case 128: return Sha1;
				case 256: return Sha256;
				case 384: return Sha384;
				case 512: return Sha512;
				default: return null;
			}
		}

		public static HashAlgorithm GetHashAlgo(string name)
		{
			switch (name.ToLower())
			{
				case "md5": return Md5;
				case "sha1": return Sha1;
				case "sha256": return Sha256;
				case "sha384": return Sha384;
				case "sha512": return Sha512;
				default: return null;
			}
		}

		public static byte[] HexToBytes(string hexString)
		{
			int hexLength = hexString.Length;

			//String length must be a whole number
			if (hexLength % 2 != 0)
				throw new ArgumentException();

			byte[] bytes = new byte[hexLength / 2];
			for (int i = 0; i < hexLength; i += 2)
			{
				bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);

			}
			return bytes;
		}

		public static string BytesToHex(byte[] bytes)
		{
			StringBuilder hex = new StringBuilder(bytes.Length * 2);
			foreach (byte b in bytes)
				hex.AppendFormat("{0:x2}", b);
			return hex.ToString();
		}

		public static bool EstAscii(string s)
		{
			foreach (char c in s)
			{
				if (c < 32 || c > 127)
					return false;
			}
			return true;
		}

		public static long? MaxEssais(int tailleMax, int premier, int dernier)
		{
			long? totalEssais = 0;
			int range = (dernier - premier) + 1;

			for (int i = 1; i <= tailleMax; i++)
			{
				try
				{
					totalEssais = checked(totalEssais + (long)Math.Pow(range, i));
				}
				catch (OverflowException o)
				{
					totalEssais = null;
				}
			}

			return totalEssais;
		}

		public static bool DécoderMotDePasse(
			string hashString,
			char premier,
			char dernier,
			out string p_motDePasseTrouvé,
			out long p_nbEssaisEffectués,
			out int p_tailleDernierEssai,
			long? p_nbEssaisMax = null,
			int? p_tailleMax = null,
			Action<long, int> report = null
			)
		{
			IEnumerable<char> rangeChars = Enumerable.Range((int)premier, (int)dernier - (int)premier + 1).Select(i => (char)i);
			string motDePasse = null;
			long essaisMax = (p_nbEssaisMax != null) ? (long)p_nbEssaisMax : 1_000_000;
			long tailleMax = (p_tailleMax != null) ? (long)MaxEssais((int)p_tailleMax, premier, dernier) : 1_000_000;

			int taille = 1;
			long essais = 0;

			bool limiteAtteinte = false;
			for (; ; taille++)
			{
				Permutation.ForAllCombinations(rangeChars.ToArray(), taille, seq =>
				{
					essais++;
					foreach (HashAlgorithm hashAlgorithm in HashAlgorithms)
					{
						if (essais >= essaisMax || essais >= tailleMax)
						{
							limiteAtteinte = true;
							break;
						}

						if (CompareHashString(seq, hashString, hashAlgorithm))
							motDePasse = new string(seq);

					}

					report?.Invoke(essais, taille);
					return limiteAtteinte;
				});

				/***/
				if (motDePasse != null || limiteAtteinte) break;
				/***/
			}

			p_tailleDernierEssai = taille;
			p_nbEssaisEffectués = essais;
			p_motDePasseTrouvé = motDePasse;

			return p_motDePasseTrouvé != null;
		}

		public static bool CompareHashString(char[] sequenceToHash, string targetHash, HashAlgorithm hashAlogrithm)
		{
			byte[] bytes = Encoding.Default.GetBytes(sequenceToHash);
			string sequenceHashed = BytesToHex(hashAlogrithm.ComputeHash(bytes));

			return sequenceHashed == targetHash;
		}
	}
}
