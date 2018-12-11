using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;
using static System.ConsoleColor;

namespace SDD
{
    public static partial class MenuGénéral
    {

        public static void Main()
        {
            InitPhase2();
            SetWindowSize(LargestWindowWidth, LargestWindowHeight);
            Show();
        }

        static void Show()
        {
            // ConsoleMenu.ExitChar = 'Z';
            ConsoleMenu.Show
            (
                "Algorithmes", Yellow, Black, Red, MenuItems.ToArray()
            );
        }

        static readonly List<MenuItem> MenuItems = new List<MenuItem>
        {
            MenuItem.Rien

            , new MenuItem("Mots de passe",
                () => MenuMotDePasse.Show(), false)


        };

        static partial void InitPhase2();

    }
}
