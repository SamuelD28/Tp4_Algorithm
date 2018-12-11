using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Console;
using static System.ConsoleColor;
using static SDD.MotDePasse;

namespace SDD
{
    public static partial class MenuMotDePasse
    {

        public static void Show()
        {
            ConsoleMenu.Show
            (
                "Mot de passe", Yellow, Black, Red, MenuItems.ToArray()
            );
        }

        static readonly string[] HashAlgorithms = new string[] {
            nameof(Md5), nameof(Sha1), nameof(Sha256), nameof(Sha384), nameof(Sha512)
        };

        static readonly List<MenuItem> MenuItems = new List<MenuItem>
        {
            MenuItem.Rien

            , new MenuItem("MaxEssais",
                () => {
                    MenuIO.Section("Minuscules");
                    MenuIO.RapporterPlusieursCalculs(
                        Enumerable.Range(1, 15),
                        x=>x.ToString().PadRight(2),
                        maxTaille => MaxEssais(maxTaille, 'a', 'z'),
                        format: maxEssais => (maxEssais?.ToString("N0") ?? "Trop grand").PadLeft(25));

                    MenuIO.Section("ASCII");
                    MenuIO.RapporterPlusieursCalculs(
                        Enumerable.Range(1, 15),
                        x=>x.ToString().PadRight(2),
                        maxTaille => MaxEssais(maxTaille, ' ', 127),
                        format: maxEssais => (maxEssais?.ToString("N0") ?? "Trop grand").PadLeft(25));

                })

            , new MenuItem("Encoder",
                () => {
                    string motDePasse = MenuIO.Lire("Mot de passe");
                    if(!EstAscii(motDePasse))
                    {
                        WriteLine("Chaîne ASCII pure attendue (incluant espaces)");
                        return;
                    }
                    // NB: Les résultats sont cachés, donc ca ne sert à rien de répéter...
                    int REPEAT = 1;
                    try
                    {
                        byte[] bytes = Encoding.Default.GetBytes(motDePasse);
                        MenuIO.RapporterBrut("Octets du mot de passe", BytesToHex(bytes));
                        
                        MenuIO.RapporterPlusieursCalculs(
                            HashAlgorithms,
                            x=>x.PadRight(6),
                            algo => GetHashAlgo(algo).ComputeHash(bytes), 
                            REPEAT,
                            ba => BytesToHex(ba));
                    }
                    catch (Exception ex)
                    {
                        WriteLine($"Un problème est survenu lors de l'encodage...\n{ex.Message}");
                    }
                })

            , new MenuItem("Afficher des combinaisons", () => AfficherDesCombinaisons())

            , new MenuItem(
                "Encoder et Décoder (minuscules)",
                () => EncoderEtDécoder('a', 'z', "minuscules"))

            , new MenuItem(
                "Encoder et Décoder (ASCII)",
                () => EncoderEtDécoder((char)32, (char)127, "ASCII"))

            , new MenuItem(
                "Décoder (HashString)", () => 
                {
                    if(!MenuIO.LireEntier("Nombre max de caractères dans le mot de passe", out int tailleMax, 4)) return;

                    WriteLine();
                    string hashString = MenuIO.Lire("Hash string");

                    Write("\nDécodage en cours...");
                    MenuIO.RapporterCalcul(
                        "\n\nMot de passe",
                        () => RapporterDécodage(hashString, tailleMax, out _));
                })

            , new MenuItem("Décoder et prédire (bonus)", () => DécoderEtPrédire())

        };

        static string RapporterDécodage(string hashString, int tailleMax, out string motDePasse)
        {
            int longueurPrécédente = 0;
            DécoderMotDePasse(
                hashString, (char)32, (char)127,
                out motDePasse,
                out var nbEssais,
                out var taille,
                null, tailleMax,
                (essai, longueur) => {
                    if (longueur != longueurPrécédente)
                    {
                        longueurPrécédente = longueur;
                        Write(longueur);
                    }
                    if (essai % 20000 == 0)
                    {
                        Write('.');
                    }
                });
            return (motDePasse ?? "Non trouvé")
                + $"  (taille: {taille}, essais: {nbEssais:N0})";
        }

        static void EncoderEtDécoder(char first, char last, string catégorie)
        {
            WriteLine();
            string motDePasse = MenuIO.Lire("Mot de passe");
            if (!motDePasse.All(c => c >= first && c <= last))
            {
                WriteLine($"Mot de passe invalide: {catégorie} svp");
                return;
            }

            WriteLine();
            string max = MenuIO.Lire("Nombre max d'essais (1_000_000 par défaut)", "1000000");
            if (!int.TryParse(max, out int nbEssaisMax) || nbEssaisMax < 0)
            {
                WriteLine("Entier invalide");
                return;
            }

            WriteLine();
            try
            {
                byte[] bytes = Encoding.Default.GetBytes(motDePasse);
                MenuIO.RapporterPlusieursCalculs(
                    HashAlgorithms,
                    x => x.PadRight(6),
                    algo => {
                        DécoderMotDePasse(
                            BytesToHex(GetHashAlgo(algo).ComputeHash(bytes)),
                            first, last, 
                            out motDePasse, out var nbEssais, out var taille,
                            nbEssaisMax);
                        return (motDePasse ?? "Non trouvé") 
                            + $"  (taille: {taille}, essais: {nbEssais:N0})";
                    });
            }
            catch (Exception ex)
            {
                WriteLine($"Un problème est survenu...\n{ex.Message}");
            }
        }

        static partial void AfficherDesCombinaisons();
        static partial void DécoderEtPrédire();
    }
}
