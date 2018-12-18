using System;
using System.Collections.Generic;
using AlgoLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SDD
{
    [TestClass]
    public class TesterPosition
    {
        [TestMethod]
        public void _01_ConstructionColonneRangée()
        {
            var p = new Position('c', 4);
            AreEqual('c', p.Colonne);
            AreEqual(4, p.Rangée);

            ThrowsException<ArgumentException>(() => new Position('a', 0));
            ThrowsException<ArgumentException>(() => new Position(' ', 1));
        }

        [TestMethod]
        public void _02_ConstructionString()
        {
            var p = new Position("c4");
            AreEqual('c', p.Colonne);
            AreEqual(4, p.Rangée);

            p = new Position("cE");
            AreEqual('c', p.Colonne);
            AreEqual(14, p.Rangée);

            ThrowsException<ArgumentException>(() => new Position("a"));
            ThrowsException<ArgumentException>(() => new Position("a14"));
            ThrowsException<ArgumentException>(() => new Position("a0"));
            ThrowsException<ArgumentException>(() => new Position("@1"));
        }

        [TestMethod]
        public void _03_ToString()
        {
            AreEqual("e2", new Position('e', 2).ToString());
            AreEqual("cE", new Position('c', 14).ToString());
        }

        [TestMethod]
        public void _04_Equals()
        {
            var p = new Position('c', 4);
            var q = new Position('c', 4);
            var r = new Position('c', 5);
            var s = new Position('d', 4);
            
            // Equals
            IsTrue(p.Equals(q));
            IsFalse(p.Equals(r));
            IsFalse(p.Equals(s));
            IsFalse(p.Equals(4));
            IsFalse(p.Equals(null));

            // Opérateur ==
            IsTrue(p == q);
            IsFalse(p == r);
            IsFalse(p == s);

            // Opérateur !=
            IsFalse(p != q);
            IsTrue(p != r);
            IsTrue(p != s);
        }

        [TestMethod]
        public void _05_GetHashCode()
        {
            var hashCodes = new HashSet<int>();
            for(var rang = 1; rang <= 26; rang++)
                for(var col = 'a'; col <= 'z'; col++)
                    hashCodes.Add(new Position(col, rang).GetHashCode());
            AreEqual(26 * 26, hashCodes.Count);
        }

    }
}
