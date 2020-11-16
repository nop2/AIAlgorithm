#include <iostream>
#include <algorithm>
#include <utility>
#include <vector>
#include <string>
#include <map>
#include <stack>
#include <ctime>
#include <queue>
#include <memory>
using namespace std;

//节点，保存棋盘信息
class Node
{
public:
	static vector<vector<int>> target; //目标状态
	vector<vector<int>> board; //棋盘状态
	map<int, int> posMap; //棋盘数字位置映射
	shared_ptr<Node> parent; //父节点,用于构造路径
	int size; //棋盘长度
	int step = 0; //当前移动次数
	int op = 0; //上一步操作

	Node(vector<vector<int>> board, int step = 0, int op = 0, shared_ptr<Node> parent = nullptr, int mode = 1)
	{
		this->board = board;
		this->step = step;
		this->op = op;
		this->mode = mode;
		this->size = board.size();
		this->parent = parent;

		int counter = 0;
		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				posMap[board[i][j]] = counter++;
			}
		}
	}

	string ToString()
	{
		string s;
		for (auto& row : board)
		{
			for (int num : row)
			{
				s.push_back('a' + num);
			}
		}
		return s;
	}

	void Print()
	{
		for (auto& row : board)
		{
			for (int num : row)
			{
				cout << num << " ";
			}
			cout << endl;
		}
	}

	bool isTarget()
	{
		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				if (board[i][j] != target[i][j])
				{
					return false;
				}
			}
		}
		return true;
	}

	//启发信息函数,值越小越好
	int f()
	{
		if (mode == 0)
		{
			//return 0;
			return g();
		}
		if (mode == 1)
		{
			return g() + h1();
		}
		return g() + h2();
	}

private:
	//启发信息
	int mode; //估价函数选择

	//耗费函数
	int g()
	{
		return step;
	}

	//估价函数1, 错位的个数
	int h1()
	{
		int sum = 0;

		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				if (board[i][j] != target[i][j])
				{
					++sum;
				}
			}
		}
		return sum;
	}

	//估价函数2，错误位置到正确位置的距离和
	int h2()
	{
		int sum = 0;
		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				int pos = posMap[target[i][j]];
				int x = pos / size;
				int y = pos % size;
				sum += abs(x - i) + abs(y - j);
			}
		}
		return sum;
	}
};

//棋盘类，封装了节点
class ChessBoard
{
public:
	shared_ptr<Node> pNode; //节点指针
	ChessBoard(shared_ptr<Node> pNode)
	{
		this->pNode = pNode;
	}

	bool operator<(const ChessBoard chessBoard) const
	{
		return pNode->f() > chessBoard.pNode->f();
	}
};

//估价函数
enum MODE
{
	BFS = 0,
	WRONG_POSITION = 1,
	DISTANCE_BETWEEN_WRONG_POSITION = 2
};

//生成扩展节点的所有子节点并加入open表
void GenerateNode(priority_queue<ChessBoard>& queue, ChessBoard& frontChessBoard, int x, int y, int nx, int ny, int op,
                  int mode);
//搜索解
void Solve(vector<vector<int>> start, vector<vector<int>> target, MODE mode);


vector<vector<int>> Node::target = vector<vector<int>>();
map<string, bool> boardMap; //记录已出现过的棋盘
string rows[4] = {"↑", "↓", "←", "→"};
int expandingNodeCounter = 0; //扩展节点数
int generatingNodeCounter = 0; //生成节点数


int main()
{
	//初始节点状态
	auto start2 = vector<vector<int>>{
		{4, 5, 6},
		{0, 1, 2},
		{3, 7, 8}
	};
	auto start1 = vector<vector<int>>{
		{1, 3, 4},
		{8, 0, 6},
		{2, 7, 5}
	};
	//目标状态
	auto targetBoard = vector<vector<int>>{
		{1, 2, 3},
		{8, 0, 4},
		{7, 6, 5}
	};

	
	
	while (true)
	{
		cout << "估价函数\n 0:BFS\n 1:错位数\n 2:错位距离和\n";
		cout << "输入估价函数：";
		int op;
		cin >> op;
		if (op == 0)
		{
			//搜索解
			Solve(start2, targetBoard, MODE::BFS);
		}
		else if (op == 1)
		{
			Solve(start2, targetBoard, MODE::WRONG_POSITION);
		}
		else if (op == 2)
		{
			Solve(start2, targetBoard, MODE::DISTANCE_BETWEEN_WRONG_POSITION);
		}
		boardMap.clear();
		generatingNodeCounter = 0;
		expandingNodeCounter = 0;
	}
}

void Solve(vector<vector<int>> start, vector<vector<int>> target, MODE mode)
{
	//int mode = MODE::WRONG_POSITION; //估价函数
	
	//初始化起始节点
	shared_ptr<Node> startNode(new Node(std::move(start)));
	startNode->target = std::move(target);
	
	
	boardMap[(*startNode).ToString()] = true;

	//节点open表，使用优先队列实现
	priority_queue<ChessBoard> openedList;

	//记录启动时间
	auto startClock = clock();

	openedList.push(ChessBoard(startNode));
	//扩展节点直到找到解或open表为空
	while (!openedList.empty())
	{
		auto frontChessBoard = openedList.top();
		auto totalStep = frontChessBoard.pNode->step;
		openedList.pop();
		//判断是否搜索到目标状态
		if (frontChessBoard.pNode->isTarget())
		{
			//搜索完成时间
			auto clockTime = clock() - startClock;
			
			//将节点压栈倒推路径
			stack<shared_ptr<Node>> nodeStack;
			do
			{
				nodeStack.push(frontChessBoard.pNode);
				frontChessBoard = frontChessBoard.pNode->parent;
			}
			while (frontChessBoard.pNode != nullptr);

			while (!nodeStack.empty())
			{
				auto node = nodeStack.top();
				nodeStack.pop();
				if (node->step > 0)
				{
					cout << "Step " << node->step << ". " << rows[node->op] << endl;
				}
				node->Print();
				cout << endl;
			}
			
			//输出信息
			cout << "Solved in " << static_cast<double>(clockTime) / CLOCKS_PER_SEC << "s." << endl;
			cout << "Total step：" << totalStep << endl;
			cout << "Number of extension nodes: " << expandingNodeCounter << endl;
			cout << "Number of generated nodes: " << generatingNodeCounter << endl;
			cout << endl;
			
			return;
		}

		//扩展节点数+1
		++expandingNodeCounter;
		//计算0所在位置
		int pos0 = frontChessBoard.pNode->posMap[0];
		int x = pos0 / frontChessBoard.pNode->size;
		int y = pos0 % frontChessBoard.pNode->size;

		//扩展所可能节点
		if (x - 1 >= 0)
		{
			GenerateNode(openedList, frontChessBoard, x, y, x - 1, y, 0, mode);
		}
		if (x + 1 < frontChessBoard.pNode->size)
		{
			GenerateNode(openedList, frontChessBoard, x, y, x + 1, y, 1, mode);
		}
		if (y - 1 >= 0)
		{
			GenerateNode(openedList, frontChessBoard, x, y, x, y - 1, 2, mode);
		}
		if (y + 1 < frontChessBoard.pNode->size)
		{
			GenerateNode(openedList, frontChessBoard, x, y, x, y + 1, 3, mode);
		}
	}

	cout << "未找到解" << endl;
}


void GenerateNode(priority_queue<ChessBoard>& queue, ChessBoard& frontChessBoard, int x, int y, int nx, int ny, int op,
                  int mode)
{
	auto vectorTmp = frontChessBoard.pNode->board;
	swap(vectorTmp[nx][ny], vectorTmp[x][y]);
	string s;
	for (auto& row : vectorTmp)
	{
		for (int num : row)
		{
			s.push_back('a' + num);
		}
	}
	//将符合条件的子节点加入open表
	if (boardMap[s] == false)
	{
		boardMap[s] = true;
		shared_ptr<Node> newNode(new Node(vectorTmp, frontChessBoard.pNode->step + 1, op, frontChessBoard.pNode, mode));
		queue.push(ChessBoard(newNode));
		//生成节点数+1
		++generatingNodeCounter;
	}
}

/*
 *不在位距离和的计算消耗资源较多或该估价函数根本不合适，性能要差于BFS
 *
 */
