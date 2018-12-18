using System;
using System.Linq;
using AlgoLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace SDD
{
    [TestClass]
    public class TesterÉchiquier
    {
        Échiquier New(int taille)
            => new Échiquier(taille);

        Échiquier New(string reines)
        {
            var taille = reines.Split(new[] { ' ' }).Length;
            return New(reines, taille, taille);
        }

        Échiquier New(string reines, int taille, int? attendus = null)
        {
            var e = New(taille);
            var nb = e.AjouterReines(reines);
            if(attendus.HasValue)
                AreEqual(attendus, nb);
            return e;
        }

        [TestMethod]
        public void _01_Construction()
        {
            var e = New(4);
            AreEqual(4, e.Taille);
            AreEqual(1, e.Rangées.First());
            AreEqual(4, e.Rangées.Last());
            AreEqual('a', e.Colonnes.First());
            AreEqual('d', e.Colonnes.Last());
            AreEqual(0, e.Reines.Count());
        }

        static Position
            a1 = new Position('a', 1),
            a2 = new Position('a', 2),
            a3 = new Position('a', 3),
            a4 = new Position('a', 4),
            b1 = new Position('b', 1),
            b2 = new Position('b', 2),
            b3 = new Position('b', 3),
            b4 = new Position('b', 4),
            c1 = new Position('c', 1),
            c2 = new Position('c', 2),
            c3 = new Position('c', 3),
            c4 = new Position('c', 4),
            d1 = new Position('d', 1),
            d2 = new Position('d', 2),
            d3 = new Position('d', 3),
            d4 = new Position('d', 4),
            e4 = new Position('e', 4),
            d5 = new Position('d', 5),
            e5 = new Position('e', 5);


        [TestMethod]
        public void _02_Ajouter()
        {
            var e = New(4);

            // Ajouter premier élément
            IsTrue(e.AjouterReine(b4));
            AreEqual(1, e.Reines.Count());
            AreEqual(b4, e.Reines.First());

            // Impossible d'ajouter
            IsFalse(e.AjouterReine(b4), "Peu pas ajouter au même endroit");
            IsFalse(e.AjouterReine(b1), "Peu pas ajouter sur la même colonne");
            IsFalse(e.AjouterReine(d4), "Peu pas ajouter sur la même rangée");
            IsFalse(e.AjouterReine(a3), "Peu pas ajouter en diagonale");
            IsFalse(e.AjouterReine(d2), "Peu pas ajouter en diagonale");

            // Ajouter deuxieme élément
            IsTrue(e.AjouterReine(a2));
            AreEqual(2, e.Reines.Count());
            AreEqual(a2, e.Reines.First());
            AreEqual(b4, e.Reines.Last());

            // Ajouter deux derniers
            IsTrue(e.AjouterReine(c1));
            IsTrue(e.AjouterReine(d3));
            AreEqual(4, e.Reines.Count());
            AreEqual(a2, e.Reines.First());
            AreEqual(b4, e.Reines.ElementAt(1));
            AreEqual(c1, e.Reines.ElementAt(2));
            AreEqual(d3, e.Reines.Last());

            // Coordonnées invalides
            ThrowsException<ArgumentException>(
                ()=>e.AjouterReine(e4));
            ThrowsException<ArgumentException>(
                () => e.AjouterReine(new Position('A', 4)));
            ThrowsException<ArgumentException>(
                () => e.AjouterReine(new Position('a', 0)));
            ThrowsException<ArgumentException>(
                () => e.AjouterReine(new Position('a', 5)));
        }

        [TestMethod]
        public void _03_ToString()
        {
            var e = New(4);
            AreEqual("", e.ToString());

            IsTrue(e.AjouterReine(b4));
            AreEqual("b4", e.ToString());

            IsTrue(e.AjouterReine(a2));
            AreEqual("a2 b4", e.ToString());
            
            IsTrue(e.AjouterReine(d3));
            AreEqual("a2 b4 d3", e.ToString());

            IsTrue(e.AjouterReine(c1));
            AreEqual("a2 b4 c1 d3", e.ToString());
        }

        [TestMethod]
        public void _04_AjouterPlusieurs()
        {
            var e = New(4);
            AreEqual(2, e.AjouterReines(new[] {d3, c1, c2, b4, a2, a3, a4 }));
            AreEqual("c1 d3", e.ToString());

            e = New(4);
            AreEqual(2, e.AjouterReines("d3 c1 c2 b4 a2 a3 a4"));
            AreEqual("c1 d3", e.ToString());

            ThrowsException<ArgumentException>(() => e.AjouterReines("d33"));
            ThrowsException<ArgumentException>(() => e.AjouterReines(new[] {d5}));
        }

        [TestMethod]
        public void _05_Supprimer()
        {
            var e = New("a2 b4 c1 d3");

            // On ne peut pas supprimer une reine qui n'existe pas
            IsFalse(e.SupprimerReine(a1));
            IsFalse(e.SupprimerReine(b2));
            IsFalse(e.SupprimerReine(c3));
            IsFalse(e.SupprimerReine(d4));

            IsTrue(e.SupprimerReine(b4));
            IsFalse(e.SupprimerReine(b4));
            AreEqual("a2 c1 d3", e.ToString());

            IsTrue(e.SupprimerReine(d3));
            IsFalse(e.SupprimerReine(d3));
            AreEqual("a2 c1", e.ToString());

            IsTrue(e.SupprimerReine(a2));
            AreEqual("c1", e.ToString());

            IsTrue(e.SupprimerReine(c1));
            AreEqual("", e.ToString());

            ThrowsException<ArgumentException>(() => e.SupprimerReine(d5));
            ThrowsException<ArgumentException>(() => e.SupprimerReine(e4));
        }

        [TestMethod]
        public void _06_Indexation()
        {
            var e = New("a2 b4 c1 d3");

            IsTrue(e[a2]);
            IsTrue(e[b4]);
            IsTrue(e[c1]);
            IsTrue(e[d3]);

            IsFalse(e[a3]);
            IsFalse(e[b2]);
            IsFalse(e[c4]);
            IsFalse(e[d1]);

            ThrowsException<ArgumentException>(() => e[d5]);
            ThrowsException<ArgumentException>(() => e[e4]);
        }

        [TestMethod]
        public void _07_Cloner()
        {
            var e = New("a2 b4 c1 d3");
            var f = e.Cloner();

            AreEqual("a2 b4 c1 d3", e.ToString());
            AreEqual("a2 b4 c1 d3", f.ToString());

            e.SupprimerReine(b4);
            f.SupprimerReine(a2);

            AreEqual("a2 c1 d3", e.ToString(), "Copie non profonde");
            AreEqual("b4 c1 d3", f.ToString(), "Copie non profonde");
        }

        [TestMethod]
        public void _08_EstÉgal()
        {
            var e = New(4);
            var f = e.Cloner();
            IsTrue(e.EstÉgal(f));
            IsTrue(f.EstÉgal(e));

            e = New("a2 b4 c1 d3");
            f = e.Cloner();
            IsTrue(e.EstÉgal(f));
            IsTrue(f.EstÉgal(e));

            IsTrue(f.SupprimerReine(a2));
            IsFalse(f.EstÉgal(e));
            IsFalse(e.EstÉgal(f));

            e = New("a1", 2);
            f = New("a1", 3);
            IsFalse(f.EstÉgal(e));
            IsFalse(e.EstÉgal(f));
        }

        [TestMethod]
        public void _09_EnDessin()
        {
            var e = New("a2 b4 c1 d3");
            var dessin = e.EnDessin().Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            AreEqual(5, dessin.Length);

            AreEqual(" . R . .", dessin[0]);
            AreEqual(" . . . R", dessin[1]);
            AreEqual(" R . . .", dessin[2]);
            AreEqual(" . . R .", dessin[3]);
        }

        private void TesterTransformation(
            int taille,
            string reinesAvant, 
            string reinesAprès, 
            Func<Échiquier, Échiquier> transformer)
        {
            var e = New(reinesAvant, taille);
            AreEqual(reinesAprès, transformer(e).ToString(), "Transformation inexacte");
            AreEqual(reinesAvant, e.ToString(), "L'échiquier de départ doit rester intact");
        }

        [TestMethod]
        public void _20_BasculerHautBas()
        {
            TesterTransformation(4, "a2 b4", "a3 b1", e => e.BasculerHautBas());
        }

        [TestMethod]
        public void _21_BasculerGaucheDroite()
        {
            TesterTransformation(4, "a2 b4", "c4 d2", e => e.BasculerGaucheDroite());
        }

        [TestMethod]
        public void _22_BasculerObliquement()
        {
            TesterTransformation(4, "a2 b4", "b1 d2", e => e.BasculerObliquement());
        }

        [TestMethod]
        public void _23_BasculerControbliquement()
        {
            TesterTransformation(4, "a2 b4", "a3 c4", e => e.BasculerControbliquement());
        }

        [TestMethod]
        public void _24_Tourner90()
        {
            TesterTransformation(4, "a2 b4", "b4 d3", e => e.Tourner90());
        }

        [TestMethod]
        public void _25_Tourner180()
        {
            TesterTransformation(4, "a2 b4", "c1 d3", e => e.Tourner180());
        }

        [TestMethod]
        public void _26_Tourner270()
        {
            TesterTransformation(4, "a2 b4", "a2 c1", e => e.Tourner270());
        }

        [TestMethod]
        public void _27_EstSemblable()
        {
            var set = new[]{
                New("a2 b4", 4),
                New("a2 c1", 4),
                New("c1 d3", 4),
                New("b4 d3", 4),
                New("a3 c4", 4),
                New("b1 d2", 4),
                New("b1 d2", 4),
                New("c4 d2", 4)
            };

            foreach (var e in set)
            {
                foreach (var f in set)
                {
                    var g = e.Cloner();
                    var h = f.Cloner();
                    IsTrue(f.EstSemblable(e));
                    AreEqual(g.ToString(), e.ToString());
                    AreEqual(h.ToString(), f.ToString());
                }
            }
        }

        [TestMethod]
        public void _28_EstNonSemblable()
        {
            var set = new[]{
                New(4),
                New("a1", 4),
                New("a2", 4),
                New("a2 b4", 4),
                New("a2 b4 c1", 4),
                New("a2 b4 c1 d3", 4),
                New(5)
            };

            foreach (var e in set)
            {
                foreach (var f in set)
                {
                    var g = e.Cloner();
                    var h = f.Cloner();
                    if(!f.EstÉgal(e))
                        IsFalse(f.EstSemblable(e));
                    AreEqual(g.ToString(), e.ToString());
                    AreEqual(h.ToString(), f.ToString());
                }
            }

        }

    }
}
