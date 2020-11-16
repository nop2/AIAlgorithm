using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 井字棋
{
    public class ChessBoard
    {
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
}