using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace SDD
{
    public struct MenuItem
    {
        public string Nom { get; }
        public Action Action { get; }
        public bool AvecPause { get; }

        public MenuItem(string nom, Action action = null, bool avecPause = true)
        {
            Nom = nom;
            Action = action;
            AvecPause = avecPause;
        }

        public bool IsSpacer => Action == null;
        public bool IsRien => Action == null && Nom == null;

        public static readonly MenuItem Spacer = new MenuItem("");
        public static readonly MenuItem Rien = new MenuItem(null);
    }

    public static class ConsoleMenu
    {
        public static char ExitChar = 'X';

        public static void Show(string titreMenu, params MenuItem[] menu)
            => Show(titreMenu, ConsoleColor.White, menu);

        public static void Show(string titreMenu, ConsoleColor foregroundMenu, params MenuItem[] menu)
            => Show(titreMenu, foregroundMenu, ConsoleColor.Black, menu);

        public static void Show(string titreMenu, ConsoleColor foregroundMenu, ConsoleColor backgroundMenu, params MenuItem[] menu) 
            => Show(titreMenu, foregroundMenu, backgroundMenu, null, null, menu);

        public static void Show(string titreMenu, ConsoleColor foregroundMenu, ConsoleColor backgroundMenu, ConsoleColor? foregroundSpacer, params MenuItem[] menu)
            => Show(titreMenu, foregroundMenu, backgroundMenu, foregroundSpacer, null, menu);

        public static void Show(string titreMenu, ConsoleColor foregroundMenu, ConsoleColor backgroundMenu, ConsoleColor? foregroundSpacer, Action état, params MenuItem[] menu)
        {
            var menuEffectif = menu.Where(item => !item.IsSpacer).ToArray();

            for (; ; )
            {
                AfficherMenu();
                Write("Votre choix? ");
                var choix = Char.ToUpper(ReadKey(true).KeyChar);
                var indexChoix = choix - 'A';
                Write(choix);
                if (choix == ExitChar)
                {
                    WriteLine();
                    break;
                }
                else if (0 <= indexChoix && indexChoix < menuEffectif.Length)
                {
                    WriteMenu(" : " + menuEffectif[indexChoix].Nom);
                    try 
                    {
                        menuEffectif[indexChoix].Action();
                    }
                    catch(Exception ex)
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine();
                        WriteLine(ex.Message);
                        ResetColor();
                    }
                    if(menuEffectif[indexChoix].AvecPause)
                    {
                        Write("\nAppuyez sur une touche pour poursuivre...");
                        ReadKey();
                        WriteLine();
                    }
                }
                else 
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine(" : choix invalide!");
                    ResetColor();
                }
            }

            void AfficherMenu()
            {
                Clear();
                Title = titreMenu;
                WriteMenu();
                WriteMenu(titreMenu);
                WriteMenu(new String('=', titreMenu.Length));
                WriteMenu();
                if(état != null)
                {
                    état();
                    WriteMenu();
                }
                int nbSpacers = 0;
                char lettre = 'A';
                for (var item = 0; item < menu.Length; item++)
                {
                    if (menu[item].IsSpacer)
                    {
                        if(!menu[item].IsRien)
                        {
                            nbSpacers++;
                            WriteMenu(menu[item].Nom, isSpacer: true);
                        }
                    }
                    else
                    {
                        WriteMenu($"  {lettre} - {menu[item].Nom}");
                        lettre++;
                    }
                }
                if (nbSpacers > 0) WriteMenu();
                WriteMenu($"  {ExitChar} - Exit");
                WriteMenu();
            }

            void WriteMenu(string line = "", bool isSpacer = false)
            {
                ForegroundColor = isSpacer ? foregroundSpacer ?? foregroundMenu : foregroundMenu;
                BackgroundColor = backgroundMenu;
                WriteLine("{0,-70}", line);
                ResetColor();
            }

        }
    }
}
