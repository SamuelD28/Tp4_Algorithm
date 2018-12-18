using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDD
{
	public static class MotDePasse
	{
		public static Dictionary<int, KeyValuePair<int, string>> HashAlgorithms = new Dictionary<int, KeyValuePair<int, string>>() {
			{ 0,new KeyValuePair<int, string>(16, "md5")},
			{ 1,new KeyValuePair<int, string>(128, "sha1")},
			{ 2,new KeyValuePair<int, string>(256, "sha256")},
			{ 3,new KeyValuePair<int, string>(384, "sha384")},
			{ 4,new KeyValuePair<int, string>(512, "sha512")},
		};

		public static HashAlgorithm GetHashAlgo(int byteSize)
		{
			switch (byteSize)
			{
				case 16: return MD5.Create();
				case 128: return SHA1.Create();
				case 256: return SHA256.Create();
				case 384: return SHA384.Create();
				case 512: return SHA512.Create();
				default: return null;
			}
		}

		public static HashAlgorithm GetHashAlgo(string name)
		{
			switch (name.ToLower())
			{
				case "md5": return MD5.Create();
				case "sha1": return SHA1.Create();
				case "sha256": return SHA256.Create();
				case "sha384": return SHA384.Create();
				case "sha512": return SHA512.Create();
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
			char[] rangeChars = Enumerable.Range(premier, dernier - premier + 1).Select(i => (char)i).ToArray();
			long essaisMax = (p_nbEssaisMax != null) ? (long)p_nbEssaisMax : 1_000_000;
			long tailleMax = (p_tailleMax != null) ? (long)MaxEssais((int)p_tailleMax, premier, dernier) : 1_000_000;

			string motDePasse = null;
			bool limiteAtteinte = false;
			int taille = 1;

			long[] essais = new long[HashAlgorithms.Count()];
			for (; ; taille++)
			{
				var tokenSource = new CancellationTokenSource();
				ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();
				foreach (var hashAlgorithm in HashAlgorithms)
				{
					HashAlgorithm hash = GetHashAlgo(hashAlgorithm.Value.Key);
					int numeroThread = hashAlgorithm.Key;
					Task t = Task.Factory.StartNew(() =>
					{
						Permutation.ForAllCombinations(rangeChars, taille, seq =>
						{
							essais[numeroThread]++;

							if (essais[numeroThread] >= essaisMax || essais[numeroThread] >= tailleMax)
								tokenSource.Cancel();
							else if (CompareHashString(seq, hashString, hash))
							{
								motDePasse = new string(seq);
								tokenSource.Cancel();
							}
							report?.Invoke(essais[numeroThread], taille);

							return tokenSource.IsCancellationRequested;
						});
					}, tokenSource.Token);
					tasks.Add(t);
				}
				Task.WaitAll(tasks.ToArray());

				if (motDePasse != null || limiteAtteinte) break;
			}

			p_tailleDernierEssai = taille;
			p_nbEssaisEffectués = essais.Sum();
			p_motDePasseTrouvé = motDePasse;

			return p_motDePasseTrouvé != null;
		}



		public static object _locker = new object();
		public static bool CompareHashString(char[] sequenceToHash, string targetHash, HashAlgorithm hashAlogrithm)
		{
			byte[] bytes = Encoding.Default.GetBytes(sequenceToHash);
			string sequenceHashed = BytesToHex(hashAlogrithm.ComputeHash(bytes));
			return sequenceHashed == targetHash;
		}
	}
}
