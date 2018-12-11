using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static System.Console;
using static System.ConsoleColor;

namespace SDD
{
    public static class MenuIO
    {
        public static void RapporterBrut<T> (
            string propriété, 
            T valeur, 
            string eol = "\n", 
            ConsoleColor? color = null, 
            int offset = 0, 
            Func<T, string> format = null)
        {
            ForegroundColor = Cyan;
            if(!String.IsNullOrWhiteSpace(propriété))
                Write("{0," + offset + "} : ", propriété);
            else 
                Write("  ");
            ForegroundColor = color ?? Magenta;
            format = format ?? (v => v.ToString());
            Write(format(valeur).PadRight(10));
            ResetColor();
            Write(eol);
        }

        //public static void RapporterBrut<T>(
        //    string propriété, 
        //    T valeur, 
        //    string durée = null,
        //    Func<T, string> valFormat = null,
        //    int? propOffset = null,
        //    int? valOffset = null,
        //    int? duréeOffset = null)
        //{
                         
        //}

        public static double RapporterCalcul<T> (
            string propriété, 
            Func<T> calcul, 
            int benchmarks = 1,
            Func<T, string> format = null)
        {
            try {
                T valeur = default(T);
                long totalTicks = 0;
                int loops = Math.Max(benchmarks, 1);
                for (int i = 0; i < loops; i++)
                {
                    var watch = Stopwatch.StartNew();
                    valeur = calcul();
                    watch.Stop();
                    totalTicks += watch.ElapsedTicks;
                }
                
                RapporterBrut(propriété, valeur, eol: benchmarks > 0 ? "" : "\n", format:format);
                
                if (benchmarks < 1) return 0;

                if (valeur?.ToString() != "") Write("  ");
                var elapsedSeconds = 1.0 * totalTicks / benchmarks / Stopwatch.Frequency;
                ForegroundColor = DarkYellow;
                WriteLine($"({FormatTime(elapsedSeconds, 1)})");
                ResetColor();
                return elapsedSeconds;
            }
            catch(Exception ex)
            {
                RapporterBrut(propriété, ex.Message, color:Red);
                return 0;
            }
        }

        public static string FormatTime(double seconds, int digits = 1)
        {
            return ConvertTime(seconds, out string unit)
                .ToString("N"+digits) + " " + unit;
        }

        static double ConvertTime(double seconds, out string unit)
        {
            if (seconds >= 3600 * 24 * 365.25)
            {
                unit = "ans"; return seconds / (3600 * 24 * 365.25);
            }
            else if (seconds >= 3600*24)
            {
                unit = "jr"; return seconds / (3600*24);
            }
            else if (seconds >= 3600)
            {
                unit = "hr"; return seconds / 3600;
            }
            else if (seconds >= 60)
            {
                unit = "min"; return seconds / 60;
            }
            else if (seconds >= 1)
            {
                unit = "sec"; return seconds;
            }
            else if (seconds >= 0.001)
            {
                unit = "ms"; return seconds * 1000;
            }
            else if (seconds >= 0.000_001)
            {
                unit = "us"; return seconds * 1_000_000;
            }
            else
            {
                unit = "ns"; return seconds * 1_000_000_000;
            }
        }

        public static void RapporterAction (
            string description, 
            Action action, 
            int benchmarks = 1 )
        => RapporterCalcul(description, () => { action(); return ""; }, benchmarks);

        public static void 
            RapporterPlusieursCalculs<T, U> (
                IEnumerable<T> items, 
                Func<T, string> description, 
                Func<T, U> calcul, 
                int benchmarks = 1,
                Func<U, string> format = null)
        {
            foreach (var item in items)
            {
                RapporterCalcul(description(item), () => calcul(item), benchmarks, format);
            }
        }

        public static void 
            RapporterPlusieursActions<T> (
                IEnumerable<T> items, 
                Func<T, string> description, 
                Action<T> action, 
                int benchmarks = 1)
        {
            RapporterPlusieursCalculs(items, description, item => { action(item); return ""; }, benchmarks);
        }

        public static string 
            Lire(string propriété, string defaut = null)
        {
            WriteLine();
            var lecture = "";
            if (defaut != null)
                propriété = $"{propriété} ([{defaut}])";
            while (lecture == "")
            {
                BackgroundColor = White;
                ForegroundColor = Black;
                Write("> " + propriété + " ? ");
                ResetColor();
                Write(" ");
                ForegroundColor = White;
                lecture = ReadLine().Trim();
                ResetColor();
                if (defaut != null) break;
            }
            return lecture == "" ? defaut : lecture;
        }

        public static bool LireEntier(string propriété, out int entier, int défaut, int min = 1, int max = int.MaxValue)
        {
            string strTaille = Lire(propriété, $"{défaut}");
            if (!int.TryParse(strTaille, out entier) || entier < min || entier > max)
            {
                WriteLine($"{propriété} invalide");
                return false;
            }
            return true;
        }

        public static int SectionOffset = 0;

        public static void Section(string titre, int? offset = null)
        {
            int sectionOffset = offset ?? SectionOffset;
            WriteLine();
            if (sectionOffset < 0)
            {
                WriteLine(titre.PadLeft(-sectionOffset));
                WriteLine(new String('=', titre.Length).PadLeft(-sectionOffset));
            }
            else
            {
                Write(new String(' ', sectionOffset));
                WriteLine(titre);
                Write(new String(' ', sectionOffset));
                WriteLine(new String('=', titre.Length));
            }
            WriteLine();
        }

        public static T MuteExceptions<T>(Func<T> fn, T defaulter = default(T))
        {
            try
            {
                return fn();
            }
            catch
            {
                return defaulter;
            }
        }

    }
}
