using System;

namespace 井字棋alpha_beta
{
    class Program
    {
        static void Main(string[] args)
        {
            // var board = new ChessBoard(new[,] {{0, 0, 0}, {0, 0, 0}, {0, 0, 0}});
            // board.Put(1, 2, 1);
            // board.UnPut(1, 2);
            // Console.WriteLine(board.Size);
            // Console.WriteLine(board.IsEmpty());
            // Console.WriteLine(board.IsFull());

            var game = new Game();
            game.Start();
        }
    }

    class ChessBoard
    {
        //棋盘状态
        public int[,] Board { get; private set; }

        //空闲状态标志
        public int EmptyFlag { get; private set; }

        //棋子数量
        public int Size { get; private set; } = 0;

        public ChessBoard(int[,] board, int emptyFlag = 0)
        {
            Board = board;
            EmptyFlag = emptyFlag;
        }

        //检测棋盘是否为空或指定位置是否为空
        public bool IsEmpty(int x = -1, int y = -1)
        {
            if (x == -1 && y == -1)
            {
                return Size == 0;
            }

            return Board[x, y] == 0;
        }

        //检测棋盘是否放满
        public bool IsFull() => Size == Board.GetLength(0) * Board.GetLength(1);

        //放置棋子
        public void Put(int x, int y, int flag)
        {
            Board[x, y] = flag;
            ++Size;
        }

        //取消放置棋子
        public void UnPut(int x, int y)
        {
            Board[x, y] = EmptyFlag;
            --Size;
        }
    }

    class Game
    {
        //棋盘
        private ChessBoard chessBoard;

        private const int Inf = 20;

        public Game()
        {
            chessBoard = new ChessBoard(new[,] {{0, 0, 0}, {0, 0, 0}, {0, 0, 0}});
        }

        //开始游戏
        public void Start()
        {
            Console.WriteLine("是否先下棋？");
            var command = int.Parse(Console.ReadLine());
            if (command == 1)
            {
                Console.WriteLine("请输入：");
                int a = int.Parse(Console.ReadLine());
                int b = int.Parse(Console.ReadLine());
                HumanPut(a, b);
            }

            while (true)
            {
                FindComputerMove(3, -Inf, Inf, out int x, out int y);
                Console.WriteLine($"Computer：{x},{y}");
                chessBoard.Put(x, y, (int) Player.Computer);
                if (IsWin(Player.Computer))
                {
                    Console.WriteLine("电脑获胜！");
                    break;
                }

                if (chessBoard.IsFull())
                {
                    Console.WriteLine("平手！");
                    break;
                }

                Console.WriteLine("请输入：");
                int a = int.Parse(Console.ReadLine());
                int b = int.Parse(Console.ReadLine());
                HumanPut(a, b);
                if (IsWin(Player.Human))
                {
                    Console.WriteLine("你获胜！！");
                    break;
                }

                if (chessBoard.IsFull())
                {
                    Console.WriteLine("平手！");
                    break;
                }
            }
        }

        //计算当前局面评估值，站在电脑的角度
        public int Evaluate()
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

        public void HumanPut(int x, int y)
        {
            chessBoard.Put(x, y, (int) Player.Human);
        }

        //alpha-beta剪枝搜索
        private int AlphaBetaSearch(int depth, int alpha, int beta, out int x, out int y)
        {
            x = -1;
            y = -1;
            if (chessBoard.IsFull())
            {
                return 0;
            }

            if (depth == 0)
            {
                return Evaluate();
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!chessBoard.IsEmpty(i, j)) continue;
                    chessBoard.Put(i, j, (int) Player.Computer);
                    int value = -AlphaBetaSearch(depth - 1, -beta, -alpha, out int x1, out int y1);
                    chessBoard.UnPut(i, j);
                    if (value >= beta)
                    {
                        x = i;
                        y = j;
                        return beta;
                    }

                    if (value > alpha)
                    {
                        alpha = value;
                    }
                }
            }

            return alpha;
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
            // if (chessBoard.IsFull())
            // {
            //     return Inf;
            // }

            if (maxDepth == 0 || chessBoard.Size == 8)
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
            // if (chessBoard.IsFull())
            // {
            //     return -Inf;
            // }

            if (maxDepth == 0 || chessBoard.Size == 8)
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

        public bool IsBoundToWin(Player player, out int x, out int y)
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

        public bool IsWin(Player player)
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


    enum Player
    {
        Computer = 1,
        Human = 2
    }
}