﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dama_v1
{
    class ChessBoard
    {
        public List<Square> squares = new List<Square>();
        public Hrac hrac1 = new Hrac();
        public Hrac hrac2 = new Hrac();
        List<Square> dostupePolicka = new List<Square>();
        
        public int sumKaminky_before = 24;
        public int sumKaminky_after = 0;
 

        //------------------------------------------------------
        //Pro Damu
        List<Square> all_diagonals = new List<Square>(); // Vsechny policka ve dvou diagonalu
        List<Square> diagonal_topLeft = new List<Square>();
        List<Square> diagonal_topRight = new List<Square>();
        List<Square> diagonal_botRight = new List<Square>();
        List<Square> diagonal_botLeft = new List<Square>();

        public void createChessBoard()
        {
            int index = 0; // Hodnota indexu cerneho pole
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
                    {
                        Square square = new Square(j * Square.width, i * Square.width);
                        square.Color = Color.DarkSlateGray;
                        square.Souradnice = new Point(j, i);
                        square.Index = index;
                        index++;
                        squares.Add(square);

                    }
                }
            }
        }

        public void vykreslitChessBoard(Graphics g)
        {
            foreach (Square square in squares)
            {
                square.Vykresli_Square(g);
            }
        }

        public void createKaminky()
        {
            for (int i = 0; i < 12; i++)
            {
                Kaminek k1 = new Kaminek(Square.width - 10, squares[i].X + 5, squares[i].Y + 5, 1);
                k1.Color = "#000000";
                k1.Index = i;
                squares[i].Kamen = k1;
                hrac1.kaminky.Add(k1);

                Kaminek k2 = new Kaminek(Square.width - 10, squares[32 - i - 1].X + 5, squares[32 - i - 1].Y + 5, 2);
                k2.Color = "#FFFFFF";
                k2.Index = 32 - i - 1;
                squares[32 - i - 1].Kamen = k2;
                hrac2.kaminky.Add(k2);
            }
            hrac1.kaminky[9].IsDama = true;
            hrac2.kaminky[9].IsDama = true;
            //hrac2.kaminky[10].IsDama = true;
        }

        public void vykreslitKaminky(Graphics g)
        {
            foreach (Kaminek k in hrac1.kaminky)
            {
                k.vykresli_Kaminek(g);
            }
            foreach (Kaminek k in hrac2.kaminky)
            {
                k.vykresli_Kaminek(g);
            }
        }

        public Kaminek selectKaminek(int turnHrace, int x, int y)
        {
            foreach (Kaminek k in (turnHrace == 1) ? hrac1.kaminky : hrac2.kaminky)
            {
                if (
                    (k.X <= x && k.X + k.Velikost >= x) &&
                    (k.Y <= y && k.Y + k.Velikost >= y)
                   )
                {
                    return k;
                }
            }
            return null;
        }

        Square originSquare = null;


        public void ukazatDostupnePole(Kaminek k, int hrac)
        {
            //Smazat kamen z pole
            if (k.X == squares[k.Index].X + 5 && k.Y == squares[k.Index].Y + 5) // podle souradnice mysi na Canvasu
            {
                originSquare = squares[k.Index]; // Pole obsahuje puvodni pozici origin_kaminka
            }

            if (hrac == 2)
            {
                if (k.IsDama)
                {
                    naplnitDiagonali(k);
                }
                else
                {
                    if (squares[k.Index].Souradnice.Y % 2 == 0)
                    {
                        dostupnePole2(k.Index, k.Index - 3, k.Index - 7);
                        dostupnePole2(k.Index, k.Index - 4, k.Index - 9);
                    }
                    else if (squares[k.Index].Souradnice.Y % 2 == 1)
                    {
                        dostupnePole2(k.Index, k.Index - 4, k.Index - 7);
                        dostupnePole2(k.Index, k.Index - 5, k.Index - 9);
                    }
                }
            }
            else if (hrac == 1)
            {
                if (k.IsDama)
                {
                    naplnitDiagonali(k);
                }
                else
                {
                    if (squares[k.Index].Souradnice.Y % 2 == 0)
                    {
                        dostupnePole1(k.Index, k.Index + 4, k.Index + 7);
                        dostupnePole1(k.Index, k.Index + 5, k.Index + 9);
                    }
                    else if (squares[k.Index].Souradnice.Y % 2 == 1)
                    {
                        dostupnePole1(k.Index, k.Index + 3, k.Index + 7);
                        dostupnePole1(k.Index, k.Index + 4, k.Index + 9);
                    }
                }
            }
        }


        void dostupnePole1(int indexOrigin, int index1, int index2)
        {
            if (
                index1 >= 0 && index1 < squares.Count
                && squares[index1].Souradnice.Y == squares[indexOrigin].Souradnice.Y + 1)
            {
                if (squares[index1].Kamen == null && squares[indexOrigin].Kamen.MultiSkok == false)
                {
                    oznacitDostupnePole(squares[index1]);
                }
                else
                {
                    if (squares[index1].Kamen?.BelongToHrac == 2
                     && index2 >= 0 && index2 < squares.Count
                     && squares[index2].Souradnice.Y == squares[indexOrigin].Souradnice.Y + 2)
                    {
                        oznacitDostupnePole(squares[index2]);
                    }
                }
            }
        }
        void dostupnePole2(int indexOrigin, int index1, int index2)
        {
            if (
               index1 >= 0 && index1 < squares.Count
               && squares[index1].Souradnice.Y == squares[indexOrigin].Souradnice.Y - 1)
            {
                if (squares[index1].Kamen == null && squares[indexOrigin].Kamen.MultiSkok == false)
                {
                    oznacitDostupnePole(squares[index1]);
                }
                else
                {
                    if (squares[index1].Kamen?.BelongToHrac == 1
                     && index2 >= 0 && index2 < squares.Count
                     && squares[index2].Souradnice.Y == squares[indexOrigin].Souradnice.Y - 2)
                    {
                        oznacitDostupnePole(squares[index2]);
                    }
                }
            }
        }

        // Ukládá odstupné pole do dostupných polích
        // Bude volaná i pro černý i bílé, rozhoduje to podle parametrů
        public void oznacitDostupnePole(Square policko)
        // Polícko o 1 řádek výš, polícko o 2 řádek výš, souřadnice o 1 řádek výš, souřadnice o 2 řádek výš
        {
            if (policko.Kamen == null)
            {
                policko.Color = Color.Red;
                dostupePolicka.Add(policko);
            }
        }

        public int umistitKamen(Kaminek k)
        {
            foreach (Square sq in dostupePolicka)
            {
                if (sq.X < k.X + k.Velikost / 2
                && sq.X + Square.width > k.X + k.Velikost / 2
                && sq.Y < k.Y + k.Velikost / 2
                && sq.Y + Square.width > k.Y + k.Velikost / 2)
                {
                    if (k.IsDama)
                    {
                        Dama_SebratKamen(originSquare.Index, sq.Index, k);
                        k.X = sq.X + 5;
                        k.Y = sq.Y + 5;
                        k.Index = sq.Index;
                        sq.Kamen = k;
                        originSquare.Kamen = null;
                        return 3;
                    }
                    else if (Math.Abs(sq.Index - originSquare.Index) > 5)
                    {
                        SebratKamen(originSquare.Index, sq.Index);
                        k.X = sq.X + 5;
                        k.Y = sq.Y + 5;
                        k.Index = sq.Index;
                        sq.Kamen = k;
                        originSquare.Kamen = null;
                        ChangeToDamaIfPossible(sq, k);
                        return 2;
                    }
                    else
                    {
                        k.X = sq.X + 5;
                        k.Y = sq.Y + 5;
                        k.Index = sq.Index;
                        sq.Kamen = k;
                        originSquare.Kamen = null;
                        ChangeToDamaIfPossible(sq, k);
                        return 1;
                    }
                }
            }
            return 0;
        }

        void Dama_SebratKamen(int indexOrigin, int destinationIndex, Kaminek k)
        {
            // Myslenka je sebrat kameny tak, ze zkontroluji 4 diagonaly
            if (squares[indexOrigin].Souradnice.Y > squares[destinationIndex].Souradnice.Y)
            {
                if(squares[indexOrigin].Souradnice.X < squares[destinationIndex].Souradnice.X)
                {
                    for(int i = diagonal_topRight.Count - 1; i > 0 ; i--) // Sebrat po TOP_RIGHT diagonale
                    {
                        if(diagonal_topRight[i].Index > destinationIndex)
                        {
                            if (k.BelongToHrac == 1)
                                hrac2.kaminky.RemoveAll(r => r.Index == diagonal_topRight[i].Index);                          
                            else if (k.BelongToHrac == 2)
                                hrac1.kaminky.RemoveAll(r => r.Index == diagonal_topRight[i].Index);

                            squares[diagonal_topRight[i].Index].Kamen = null;
                        }                    
                    }                  
                }
                if (squares[indexOrigin].Souradnice.X > squares[destinationIndex].Souradnice.X)
                {
                    for (int i = diagonal_topLeft.Count - 1; i > 0; i--) // Sebrat po TOP_LEFT diagonale
                    {
                        if (diagonal_topLeft[i].Index > destinationIndex)
                        {
                            if (k.BelongToHrac == 1)
                                hrac2.kaminky.RemoveAll(r => r.Index == diagonal_topLeft[i].Index);
                            else if (k.BelongToHrac == 2)
                                hrac1.kaminky.RemoveAll(r => r.Index == diagonal_topLeft[i].Index);

                            squares[diagonal_topLeft[i].Index].Kamen = null;
                        }
                    }
                }
            }
            else if (squares[indexOrigin].Souradnice.Y < squares[destinationIndex].Souradnice.Y)
            {
                if (squares[indexOrigin].Souradnice.X < squares[destinationIndex].Souradnice.X)
                {
                    for (int i = 0; i < diagonal_botRight.Count; i++) // Sebrat po BOT_RIGHT diagonale
                    {
                        if (diagonal_botRight[i].Index < destinationIndex)
                        {
                            if (k.BelongToHrac == 1)
                                hrac2.kaminky.RemoveAll(r => r.Index == diagonal_botRight[i].Index);
                            else if (k.BelongToHrac == 2)
                                hrac1.kaminky.RemoveAll(r => r.Index == diagonal_botRight[i].Index);

                            squares[diagonal_botRight[i].Index].Kamen = null;
                        }
                    }
                }else if (squares[indexOrigin].Souradnice.X > squares[destinationIndex].Souradnice.X)
                {
                    for (int i = 0; i < diagonal_botLeft.Count; i++) // Sebrat po BOT_LEFT diagonale
                    {
                        if (diagonal_botLeft[i].Index < destinationIndex)
                        {
                            if (k.BelongToHrac == 1)
                                hrac2.kaminky.RemoveAll(r => r.Index == diagonal_botLeft[i].Index);
                            else if (k.BelongToHrac == 2)
                                hrac1.kaminky.RemoveAll(r => r.Index == diagonal_botLeft[i].Index);

                            squares[diagonal_botLeft[i].Index].Kamen = null;
                        }
                    }
                }
            }
            sumKaminky_after = hrac1.kaminky.Count + hrac2.kaminky.Count;

        }
        void SebratKamen(int indexOrigin, int destinationIndex)
        {
                //Pro hrac 1
                if (originSquare.Souradnice.Y % 2 == 1)
                {
                    if (indexOrigin - destinationIndex == -7)
                    {
                        hrac2.kaminky.RemoveAll(r => r.Index == indexOrigin + 3);
                        squares[indexOrigin + 3].Kamen = null;
                    }
                    if (indexOrigin - destinationIndex == -9)
                    {
                        hrac2.kaminky.RemoveAll(r => r.Index == indexOrigin + 4);
                        squares[indexOrigin + 4].Kamen = null;
                    }
                }
                else
                {
                    if (indexOrigin - destinationIndex == -7)
                    {
                        hrac2.kaminky.RemoveAll(r => r.Index == indexOrigin + 4);
                        squares[indexOrigin + 4].Kamen = null;
                    }
                    if (indexOrigin - destinationIndex == -9)
                    {
                        hrac2.kaminky.RemoveAll(r => r.Index == indexOrigin + 5);
                        squares[indexOrigin + 5].Kamen = null;
                    }
                }
                //Pro hrac 2
                if (originSquare.Souradnice.Y % 2 == 1)
                {
                    if (indexOrigin - destinationIndex == 7)
                    {
                        hrac1.kaminky.RemoveAll(r => r.Index == indexOrigin - 4);
                        squares[indexOrigin - 4].Kamen = null;
                    }
                    if (indexOrigin - destinationIndex == 9)
                    {
                        hrac1.kaminky.RemoveAll(r => r.Index == indexOrigin - 5);
                        squares[indexOrigin - 5].Kamen = null;
                    }
                }
                else
                {
                    if (indexOrigin - destinationIndex == 7)
                    {
                        hrac1.kaminky.RemoveAll(r => r.Index == indexOrigin - 3);
                        squares[indexOrigin - 3].Kamen = null;
                    }
                    if (indexOrigin - destinationIndex == 9)
                    {
                        hrac1.kaminky.RemoveAll(r => r.Index == indexOrigin - 4);
                        squares[indexOrigin - 4].Kamen = null;
                    }
            }       
        }

        // Zkontroluje jestli nemuze skocit dal (Pro normalni)
        public bool Skoc_Dal(Kaminek k)
        {

            clearDostupnePole();
            ukazatDostupnePole(k, k.BelongToHrac);
            if(!k.IsDama)
            {
                foreach (Square sq in dostupePolicka)
                {
                     if (!k.IsDama && Math.Abs(squares[k.Index].Souradnice.Y - sq.Souradnice.Y) == 2)
                    {
                        return true;
                    }
                }
            }
           
            return false;
        }

        public void clearDostupnePole()
        {
            foreach(Square sq in dostupePolicka)
            {
                sq.Color = Color.DarkSlateGray;
            }
            dostupePolicka = new List<Square>();
        }

        public void DisableOtherKamen(Kaminek k)
        {
           if(k.BelongToHrac == 1)
            {
                foreach(Kaminek kaminky in hrac1.kaminky)
                {
                    if (kaminky != k)
                        kaminky.Disable = true;
                }
            }
            if (k.BelongToHrac == 2)
            {
                foreach (Kaminek kaminky in hrac2.kaminky)
                {
                    if(kaminky != k)
                        kaminky.Disable = true;
                }
            }
        }

        public void EnableAllKaminek(Kaminek k)
        {
            if (k.BelongToHrac == 1)
            {
                foreach (Kaminek kaminky in hrac1.kaminky)
                {
                        kaminky.Disable = false;
                }
            }
            if (k.BelongToHrac == 2)
            {
                foreach (Kaminek kaminky in hrac2.kaminky)
                {
                        kaminky.Disable = false;
                }
            }
        }

        private void ChangeToDamaIfPossible(Square sq, Kaminek k)
        {
            if (sq.Souradnice.Y == 0 || sq.Souradnice.Y == 7)
            {
                k.IsDama = true;
            }
        }

        void naplnitDiagonali(Kaminek k) // Naplnit 4 směry(top-left/right, bot-left/right) do listu 
        {
            all_diagonals = new List<Square>();
            diagonal_botLeft = new List<Square>();
            diagonal_botRight = new List<Square>();
            diagonal_topLeft = new List<Square>();
            diagonal_topRight = new List<Square>();

            for (int i = 0; i < squares.Count; i++)
            {
                if (Math.Abs(squares[i].Souradnice.X - squares[k.Index].Souradnice.X) == Math.Abs(squares[i].Souradnice.Y - squares[k.Index].Souradnice.Y))
                {
                    all_diagonals.Add(squares[i]);
                }
            }

            for (int i = 0; i < all_diagonals.Count; i++)
            {
                if (all_diagonals[i].Index < k.Index)
                {
                    if (all_diagonals[i].Souradnice.X < squares[k.Index].Souradnice.X)
                    {
                        diagonal_topLeft.Add(all_diagonals[i]);
                    }
                    else
                    {
                        diagonal_topRight.Add(all_diagonals[i]);
                    }
                }
                if (all_diagonals[i].Index > k.Index)
                {
                    if (all_diagonals[i].Souradnice.X < squares[k.Index].Souradnice.X)
                    {
                        diagonal_botLeft.Add(all_diagonals[i]);
                    }
                    else
                    {
                        diagonal_botRight.Add(all_diagonals[i]);
                    }
                }
            }

            OznacDostupnePoleProDamu(k);
        }

        void OznacDostupnePoleProDamu(Kaminek k) // Označit dostupné políčka podle každé diagonále
        {
            for (int i = diagonal_topLeft.Count - 1; i >= 0; i--)
            {

                if (diagonal_topLeft[i].Kamen?.BelongToHrac == k.BelongToHrac)
                {
                    break;
                }
                else
                {
                    oznacitDostupnePole(diagonal_topLeft[i]);
                }
            }
            for (int i = diagonal_topRight.Count - 1; i >= 0; i--)
            {

                if (diagonal_topRight[i].Kamen?.BelongToHrac == k.BelongToHrac)
                {
                    break;
                }
                else
                {
                    oznacitDostupnePole(diagonal_topRight[i]);
                }
            }
            for (int i = 0; i < diagonal_botLeft.Count; i++)
            {

                if (diagonal_botLeft[i].Kamen?.BelongToHrac == k.BelongToHrac)
                {
                    break;
                }
                else
                {
                    oznacitDostupnePole(diagonal_botLeft[i]);
                }
            }
            for (int i = 0; i < diagonal_botRight.Count; i++)
            {

                if (diagonal_botRight[i].Kamen?.BelongToHrac == k.BelongToHrac)
                {
                    break;
                }
                else
                {
                    oznacitDostupnePole(diagonal_botRight[i]);
                }
            }
        }

        // Skontroluje jestli Dama může ještě někam skočit po snězení nějakého kamínku
        // Projede postupně 4 diagonály. Zkontroluje jestli za protiherním kamínkem je nějaké dostupné políčko. 
        public bool CanDamaJumpMore(Kaminek k)
        {
            naplnitDiagonali(k);
            int count = 0; // Pocita pocet dostupnych polich
            // Cac vong for ben duoi sau phai dc gop thanh ham
            if(k.BelongToHrac == 2)
            {
                for (int i = 0; i < diagonal_botLeft.Count; i++)
                {
                    if (diagonal_botLeft[i].Kamen?.BelongToHrac == 1)
                    {
                        int j = i + 1;
                        while (j < diagonal_botLeft.Count)
                        {
                            if (diagonal_botLeft[j].Kamen == null)
                            {
                                count++;
                                j++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
                for (int i = 0; i < diagonal_botRight.Count; i++)
                {
                    if (diagonal_botRight[i].Kamen?.BelongToHrac == 1)
                    {
                        int j = i + 1;
                        while (j < diagonal_botRight.Count)
                        {
                            if (diagonal_botRight[j].Kamen == null)
                            {
                                count++;
                                j++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
                for (int i = diagonal_topLeft.Count - 1; i > 0; i--)
                {
                    if (diagonal_topLeft[i].Kamen?.BelongToHrac == 1)
                    {
                        int j = i - 1;
                        while (j > 0)
                        {
                            if (diagonal_topLeft[j].Kamen == null)
                            {
                                count++;
                                j--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
                for (int i = diagonal_topRight.Count - 1; i > 0; i--)
                {
                    if (diagonal_topRight[i].Kamen?.BelongToHrac == 1)
                    {
                        int j = i - 1;
                        while (j > 0)
                        {
                            if (diagonal_topRight[j].Kamen == null)
                            {
                                count++;
                                j--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
            }else if (k.BelongToHrac == 1)
            {
                for (int i = 0; i < diagonal_topLeft.Count; i++)
                {
                    if (diagonal_topLeft[i].Kamen?.BelongToHrac == 2)
                    {
                        int j = i + 1;
                        while (j < diagonal_topLeft.Count)
                        {
                            if (diagonal_topLeft[j].Kamen == null)
                            {
                                count++;
                                j++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
                for (int i = 0; i < diagonal_botLeft.Count; i++)
                {
                    if (diagonal_botLeft[i].Kamen?.BelongToHrac == 2)
                    {
                        int j = i + 1;
                        while (j < diagonal_botLeft.Count)
                        {
                            if (diagonal_botLeft[j].Kamen == null)
                            {
                                count++;
                                j++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
                for (int i = diagonal_topRight.Count - 1; i > 0; i--)
                {
                    if (diagonal_topRight[i].Kamen?.BelongToHrac == 1)
                    {
                        int j = i - 1;
                        while (j > 0)
                        {
                            if (diagonal_topRight[j].Kamen == null)
                            {
                                count++;
                                j--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }
                }

                for (int i = diagonal_botRight.Count - 1; i > 0; i--)
                {
                    if (diagonal_botRight[i].Kamen?.BelongToHrac == 1)
                    {
                        int j = i - 1;
                        while (j > 0)
                        {
                            if (diagonal_botRight[j].Kamen == null)
                            {
                                count++;
                                j--;
                            }
                            else
                            {
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            
            //---------------------------------------------------------------------------------------
            if (count > 0 && nejakyKaminekBylOdstranen())
            {
                return true;
            }
            else
            {
               return false;
            }
        }

       public bool nejakyKaminekBylOdstranen()
        {
            Console.WriteLine("after: " + sumKaminky_after);
            Console.WriteLine("before: " + sumKaminky_before);
            if (sumKaminky_after < sumKaminky_before)
            {
                sumKaminky_before = sumKaminky_after;
                return true;
            }

               return false;
        
        }

        public bool IsGameOver()
        {
            if(hrac1.kaminky.Count == 0 || hrac2.kaminky.Count == 0)
            {
                return true;
            }

            return false;
        }
    }
}
