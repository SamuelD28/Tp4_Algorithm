using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static SDD.MotDePasse;

namespace Tests
{
    [TestClass]
    public class TesterMotDePasse
    {
        [TestMethod]
        public void _01_BytesToHex()
        {
            AreEqual("", BytesToHex(new byte[0]));
            AreEqual("0080fe", BytesToHex(new byte[] {0, 128, 254 }).ToLower());
        }

        [TestMethod]
        public void _02_HexToBytes()
        {
            AreEqual(0, HexToBytes("").Length);
            AreEqual(127, HexToBytes("7f")[0]);
            AreEqual("7f", BytesToHex(HexToBytes("7f")).ToLower());
            AreEqual("0080fe", BytesToHex(HexToBytes("0080fe")).ToLower());
            ThrowsException<ArgumentException>(()=>HexToBytes("xyz"));
        }

        [TestMethod]
        public void _03_EstAscii()
        {
            IsTrue(EstAscii(""));
            IsTrue(EstAscii(new string(Enumerable.Range(32, 96).Select(x => (char)x).ToArray())));
            IsFalse(EstAscii("é"));
            IsFalse(EstAscii("\t"));
        }

        [TestMethod]
        public void _04_GetHashAlgo()
        {
            AreEqual(Md5, GetHashAlgo(16));
            AreEqual(Md5, GetHashAlgo(nameof(Md5)));
            AreEqual(null, GetHashAlgo(15));
            AreEqual(null, GetHashAlgo("Md4"));
        }

        [TestMethod]
        public void _05_MaxEssais()
        {
            AreEqual(0, MaxEssais(0, 1, 10));
            AreEqual(10, MaxEssais(1, 1, 10));
            AreEqual(110, MaxEssais(2, 1, 10));
            AreEqual(1110, MaxEssais(3, 1, 10));
            AreEqual(111_111_110, MaxEssais(8, 1, 10));
            AreEqual(null, MaxEssais(30, 1, 10));
            AreEqual(10100, MaxEssais(2, 11, 110));
            AreEqual(100, MaxEssais(100, 10, 10));
        }

        [TestMethod]
        public void _10_DécoderMotDePasse_1()
        {
            var trouvé = DécoderMotDePasse("bbf3f11cb5b43e700273a78d12de55e4a7eab741ed2abf13787a4d2dc832b8ec",
            ' ', (char)127, out var motDePasse, out var nbEssais, out var taille);
            IsTrue(trouvé);
            AreEqual("%", motDePasse);
            IsTrue(nbEssais > 0);
            AreEqual(1, taille);
        }

        [TestMethod]
        public void _11_DécoderMotDePasse_2()
        {
            var trouvé = DécoderMotDePasse("a40e4cf3c7122bfd4a5631bf1e410f69",
            ' ', (char)127, out var motDePasse, out var nbEssais, out var taille);
            IsTrue(trouvé);
            AreEqual("Zz", motDePasse);
            IsTrue(nbEssais > 96);
            AreEqual(2, taille);
        }

        [TestMethod]
        public void _12_DécoderMotDePasse_MaxEssais()
        {
            var trouvé = DécoderMotDePasse("a40e4cf3c7122bfd4a5631bf1e410f69",
            ' ', (char)127, out var motDePasse, out var nbEssais, out var taille, 50);
            IsFalse(trouvé);
            AreEqual(null, motDePasse);
            AreEqual(50, nbEssais);
            AreEqual(1, taille);
        }

        [TestMethod]
        public void _13_DécoderMotDePasse_MaxTaille()
        {
            var trouvé = DécoderMotDePasse("a40e4cf3c7122bfd4a5631bf1e410f69",
            ' ', (char)127, out var motDePasse, out var nbEssais, out var taille, null, 1);
            IsFalse(trouvé);
            AreEqual(null, motDePasse);
            AreEqual(96, nbEssais);
            AreEqual(1, taille);
        }

        [TestMethod]
        public void _14a_DécoderMotDePasse_MaxTailleEtEssais()
        {
            var trouvé = DécoderMotDePasse("a40e4cf3c7122bfd4a5631bf1e410f69",
            ' ', (char)127, out var motDePasse, out var nbEssais, out var taille, 50, 1);
            IsFalse(trouvé);
            AreEqual(null, motDePasse);
            AreEqual(50, nbEssais);
            AreEqual(1, taille);
        }

        [TestMethod]
        public void _14b_DécoderMotDePasse_MaxTailleEtEssais()
        {
            var trouvé = DécoderMotDePasse("a40e4cf3c7122bfd4a5631bf1e410f69",
            ' ', (char)127, out var motDePasse, out var nbEssais, out var taille, 250, 1);
            IsFalse(trouvé);
            AreEqual(null, motDePasse);
            AreEqual(96, nbEssais);
            AreEqual(1, taille);
        }

        [TestMethod]
        public void _14c_DécoderMotDePasse_MaxTailleEtEssais()
        {
            var trouvé = DécoderMotDePasse("a40e4cf3c7122bfd4a5631bf1e410f69",
            ' ', (char)127, out var motDePasse, out var nbEssais, out var taille, 250, 2);
            IsFalse(trouvé);
            AreEqual(null, motDePasse);
            AreEqual(250, nbEssais);
            AreEqual(2, taille);
        }

        [TestMethod]
        public void _15_DécoderMotDePasse_Rapporter()
        {
            long sommeEssais = 0;
            long sommeTaille = 0;
            var trouvé = DécoderMotDePasse("a40e4cf3c7122bfd4a5631bf1e410f69",
            ' ', (char)127, out var motDePasse, out var nbEssais, out var taille, 97, null, (essai, t) => {
                sommeEssais += essai;
                sommeTaille += t;
            });
            IsFalse(trouvé);
            AreEqual(null, motDePasse);
            AreEqual(97, nbEssais);
            AreEqual(2, taille);
            AreEqual(98, sommeTaille);
            AreEqual(97 * 98 / 2, sommeEssais);
        }

    }
}
