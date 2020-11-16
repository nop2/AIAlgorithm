using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace 井字棋
{
    public class Game : ViewModelBase
    {
        //棋盘
        private ChessBoard chessBoard;

        private const int Inf = 20;

        #region Binding

        public RelayCommand<string> HumanPutCommand { get; set; }

        private string pos00;

        public string Pos00
        {
            get => pos00;
            set
            {
                pos00 = value;
                RaisePropertyChanged();
            }
        }

        private string pos01;

        public string Pos01
        {
            get => pos01;
            set
            {
                pos01 = value;
                RaisePropertyChanged();
            }
        }

        private string pos02;

        public string Pos02
        {
            get => pos02;
            set
            {
                pos02 = value;
                RaisePropertyChanged();
            }
        }

        private string pos10;

        public string Pos10
        {
            get => pos10;
            set
            {
                pos10 = value;
                RaisePropertyChanged();
            }
        }

        private string pos11;

        public string Pos11
        {
            get => pos11;
            set
            {
                pos11 = value;
                RaisePropertyChanged();
            }
        }

        private string pos12;

        public string Pos12
        {
            get => pos12;
            set
            {
                pos12 = value;
                RaisePropertyChanged();
            }
        }

        private string pos20;

        public string Pos20
        {
            get => pos20;
            set
            {
                pos20 = value;
                RaisePropertyChanged();
            }
        }

        private string pos21;

        public string Pos21
        {
            get => pos21;
            set
            {
                pos21 = value;
                RaisePropertyChanged();
            }
        }

        private string pos22;

        public string Pos22
        {
            get => pos22;
            set
            {
                pos22 = value;
                RaisePropertyChanged();
            }
        }

        private void UpdateUi(int x, int y)
        {
            string flag = "";
            if (chessBoard.Board[x, y] == (int) Player.Computer) flag = "X";
            if (chessBoard.Board[x, y] == (int) Player.Human) flag = "〇";
            this.GetType().GetProperty($"Pos{x}{y}")?.SetValue(this, flag);
        }

        #endregion

        public Game()
        {
            chessBoard = new ChessBoard(new[,] {{0, 0, 0}, {0, 0, 0}, {0, 0, 0}});
            HumanPutCommand = new RelayCommand<string>(pos =>
            {
                var nums = pos.Split(",");
                HumanPut(int.Parse(nums[0]), int.Parse(nums[1]));
            });

            NewGame();
        }

        private void NewGame()
        {
            chessBoard = new ChessBoard(new[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } });
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    UpdateUi(i,j);
                }
            }
            var select = MessageBox.Show("是否要走第一步？", "电脑为X，用户标记〇", MessageBoxButton.YesNo);
            if (select == MessageBoxResult.No)
            {
                ComputerPut();
            }
        }

        //计算当前局面评估值，站在电脑的角度
        private int Evaluate()
        {
            int humanScore = 0, computerScore = 0;
            for (int i = 0; i < 3; i++)
            {
                bool humanExistRow = false, computerExistRow = false;
                bool humanExistCol = false, computerExistCol = false;

                for (int j = 0; j < 3; j++)
                {
                    if (chessBoard.Board[i, j] == (int) Player.Human)
                    {
                        humanExistRow = true;
                    }

                    if (chessBoard.Board[i, j] == (int) Player.Computer)
                    {
                        computerExistRow = true;
                    }

                    if (chessBoard.Board[j, i] == (int) Player.Human)
                    {
                        humanExistCol = true;
                    }

                    if (chessBoard.Board[j, i] == (int) Player.Computer)
                    {
                        computerExistCol = true;
                    }
                }

                if (!humanExistRow)
                {
                    ++computerScore;
                }

                if (!humanExistCol)
                {
                    ++computerScore;
                }

                if (!computerExistRow)
                {
                    ++humanScore;
                }

                if (!computerExistCol)
                {
                    ++humanScore;
                }
            }

            bool humanExistDiagonal1 = false;
            bool humanExistDiagonal2 = false;
            bool computerExistDiagonal1 = false;
            bool computerExistDiagonal2 = false;
            if (chessBoard.Board[0, 0] == (int) Player.Human || chessBoard.Board[1, 1] == (int) Player.Human ||
                chessBoard.Board[2, 2] == (int) Player.Human)
            {
                humanExistDiagonal1 = true;
            }

            if (chessBoard.Board[0, 0] == (int) Player.Computer || chessBoard.Board[1, 1] == (int) Player.Computer ||
                chessBoard.Board[2, 2] == (int) Player.Computer)
            {
                computerExistDiagonal1 = true;
            }

            if (chessBoard.Board[2, 0] == (int) Player.Human || chessBoard.Board[1, 1] == (int) Player.Human ||
                chessBoard.Board[0, 2] == (int) Player.Human)
            {
                humanExistDiagonal2 = true;
            }

            if (chessBoard.Board[2, 0] == (int) Player.Computer || chessBoard.Board[1, 1] == (int) Player.Computer ||
                chessBoard.Board[0, 2] == (int) Player.Computer)
            {
                computerExistDiagonal2 = true;
            }

            if (!humanExistDiagonal1)
            {
                ++computerScore;
            }

            if (!humanExistDiagonal2)
            {
                ++computerScore;
            }

            if (!computerExistDiagonal1)
            {
                ++humanScore;
            }

            if (!computerExistDiagonal2)
            {
                ++humanScore;
            }

            //Console.WriteLine($"computer:{computerScore} human:{humanScore}");

            return computerScore - humanScore;
        }

        private void HumanPut(int x, int y)
        {
            if (!chessBoard.IsEmpty(x, y)) return;
            chessBoard.Put(x, y, (int) Player.Human);
            UpdateUi(x, y);
            if (IsWin(Player.Human))
            {
                MessageBox.Show("你赢了", "你赢了");
                NewGame();
                return;
            }

            if (chessBoard.IsFull())
            {
                MessageBox.Show("平局", "平局");
                NewGame();
                return;
            }
            ComputerPut();
            if (chessBoard.IsFull())
            {
                MessageBox.Show("平局", "平局");
                NewGame();
                return;
            }
        }

        private void ComputerPut()
        {
            FindComputerMove(3, -Inf, Inf, out int x, out int y);
            chessBoard.Put(x, y, (int) Player.Computer);
            UpdateUi(x, y);
            if (IsWin(Player.Computer))
            {
                MessageBox.Show("电脑赢了", "电脑赢了");
                NewGame();
                return;
            }
        }

        /// <summary>
        /// 找到合适的放置棋子位置，使本层alpha值最大，站在电脑的角度
        /// </summary>
        /// <param name="maxDepth">搜素最大深度</param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int FindComputerMove(int maxDepth, int alpha, int beta, out int x, out int y)
        {
            x = 0;
            y = 0;
            if (chessBoard.IsFull())
            {
                return 0;
            }

            if (maxDepth == 0)
            {
                return Evaluate();
            }

            if (IsBoundToWin(Player.Computer, out int x1, out int y1))
            {
                x = x1;
                y = y1;
                return Inf;
            }

            int value = alpha;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //当前在MAX节点，进行beta剪枝，beta为上层MIN节点当前最小值
                    if (value >= beta)
                    {
                        return value;
                    }

                    if (chessBoard.IsEmpty(i, j))
                    {
                        chessBoard.Put(i, j, (int) Player.Computer);
                        int temp = FindHumanMove(maxDepth - 1, value, beta, out _, out _);
                        chessBoard.UnPut(i, j);
                        if (temp > value)
                        {
                            value = temp;
                            x = i;
                            y = j;
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// MIN节点,
        /// </summary>
        /// <param name="maxDepth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int FindHumanMove(int maxDepth, int alpha, int beta, out int x, out int y)
        {
            x = 0;
            y = 0;
            if (chessBoard.IsFull())
            {
                return 0;
            }

            if (maxDepth == 0)
            {
                return Evaluate();
            }

            if (IsBoundToWin(Player.Human, out int x1, out int y1))
            {
                x = x1;
                y = y1;
                return -Inf;
            }

            int value = beta;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //当前在MIN节点，进行alpha剪枝，用到alpha值
                    if (value <= alpha)
                    {
                        return value;
                    }

                    if (chessBoard.IsEmpty(i, j))
                    {
                        chessBoard.Put(i, j, (int) Player.Human);
                        int temp = FindComputerMove(maxDepth - 1, alpha, value, out _, out _);
                        chessBoard.UnPut(i, j);
                        if (temp < value)
                        {
                            value = temp;
                            x = i;
                            y = j;
                        }
                    }
                }
            }

            return value;
        }

        private bool IsBoundToWin(Player player, out int x, out int y)
        {
            x = 0;
            y = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (chessBoard.IsEmpty(i, j))
                    {
                        chessBoard.Put(i, j, (int) player);
                        if (IsWin(player))
                        {
                            x = i;
                            y = j;
                            chessBoard.UnPut(i, j);
                            return true;
                        }

                        chessBoard.UnPut(i, j);
                    }
                }
            }

            return false;
        }

        private bool IsWin(Player player)
        {
            for (int i = 0; i < 3; i++)
            {
                bool allRow = true;
                bool allCol = true;
                for (int j = 0; j < 3; j++)
                {
                    if (chessBoard.Board[i, j] != (int) player)
                    {
                        allRow = false;
                    }

                    if (chessBoard.Board[j, i] != (int) player)
                    {
                        allCol = false;
                    }
                }

                if (allCol || allRow)
                {
                    return true;
                }
            }

            if ((chessBoard.Board[0, 0] == (int) player && chessBoard.Board[1, 1] == (int) player &&
                 chessBoard.Board[2, 2] == (int) player)
                || (chessBoard.Board[0, 2] == (int) player && chessBoard.Board[1, 1] == (int) player &&
                    chessBoard.Board[2, 0] == (int) player))
            {
                return true;
            }

            return false;
        }
    }


    public enum Player
    {
        Computer = 1,
        Human = 2
    }
}