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
//
//constexpr int SIZE = 3;
map<string, bool> boardMap; //记录已出现过的棋盘

class Node
{
public:
	static vector<vector<int>> target; //目标状态
	vector<vector<int>> board; //棋盘状态
	map<int, int> posMap; //棋盘数字位置映射
	Node* parent = nullptr; //父节点,用于构造路径
	int size; //棋盘长度
	int step = 0; //当前移动次数
	int op = 0; //上一步操作

	Node(vector<vector<int>> board, int step = 0, int op = 0, Node* parent = nullptr, int mode = 1)
	{
		this->board = board;
		this->step = step;
		this->op = op;
		this->mode = mode;
		this->parent = parent;
		this->size = board.size();

		int counter = 0;
		for (int i = 0; i < size; ++i)
		{
			for (int j = 0; j < size; ++j)
			{
				posMap[board[i][j]] = counter++;
			}
		}
	}

	bool operator<(const Node& n1) const
	{
		return f() > n1.f();
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

private:
	//启发信息
	int mode; //估价函数选择

	int f() const
	{
		return g() + (mode == 1 ? h1() : h2());
	}

	//耗费函数
	int g() const
	{
		return step;
	}

	//估价函数1, 错位的个数
	int h1() const
	{
		int sum = 0;
		int len = board.size();
		for (int i = 0; i < len; ++i)
		{
			for (int j = 0; j < len; ++j)
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
	int h2() const
	{
		return 0;
	}
};

auto targetBoard = vector<vector<int>>{{1, 2, 3}, {4, 0, 5}, {6, 7, 8}};
vector<vector<int>> Node::target = targetBoard;

void GenerateNode(priority_queue<Node>& queue, Node& frontNode, int x, int y, int nx, int ny, int op, int mode);

int main()
{
	auto start = vector<vector<int>>{
		{0, 2, 3},
		{1, 4, 7},
		{6, 8, 5}
	};

	int mode = 1; //估价函数

	shared_ptr<Node> startNode(new Node(start));
	boardMap[(*startNode).ToString()] = true;
	priority_queue<Node> queue;

	auto startClock = clock();

	queue.push(*startNode);

	while (!queue.empty())
	{
		auto frontNode = queue.top();
		queue.pop();
		//判断是否搜索到目标状态
		if (frontNode.isTarget())
		{
			cout << "Solved in " << (double)((long long)clock() - startClock) / CLOCKS_PER_SEC << "s." << endl;
			cout << frontNode.step << endl;
			frontNode.Print();
			cout << frontNode.parent->step << endl;
			frontNode.parent->Print();
			break;
		}
		//扩展所有可能的节点
		int pos0 = frontNode.posMap[0];
		int x = pos0 / frontNode.size;
		int y = pos0 % frontNode.size;

		if (x - 1 >= 0)
		{
			GenerateNode(queue, frontNode, x, y, x - 1, y, 0, mode);
		}
		if (x + 1 < frontNode.size)
		{
			GenerateNode(queue, frontNode, x, y, x + 1, y, 1, mode);
		}
		if (y - 1 >= 0)
		{
			GenerateNode(queue, frontNode, x, y, x, y - 1, 2, mode);
		}
		if (y + 1 < frontNode.size)
		{
			GenerateNode(queue, frontNode, x, y, x, y + 1, 3, mode);
		}
	}

	/*
	auto b = vector<vector<int>>{{3, 2, 1}, {4, 0, 5}, {6, 7, 8}}; //错位2
	auto c = vector<vector<int>>{{1, 2, 3}, {5, 0, 4}, {7, 6, 8}}; //错误4

	node* n = new node(b);
	
	node n1(c, 1); //5
	node n2(a, 6); //6
	node n3(b); //2

	priority_queue<node> queue;
	queue.push(n1);
	queue.push(n2);
	queue.push(n3);
	queue.push(*n);

	while (!queue.empty())
	{
		node d = queue.top();
		d.Print();
		queue.pop();
		cout << endl;
	}*/
}

void GenerateNode(priority_queue<Node>& queue, Node& frontNode, int x, int y, int nx, int ny, int op, int mode)
{
	//auto vectorTmp = frontNode.board;
	shared_ptr<Node> newNode(new Node(frontNode.board, frontNode.step + 1, op, &frontNode, mode));
	swap((*newNode).board[nx][ny], (*newNode).board[x][y]);
	string flag = (*newNode).ToString();
	if (boardMap[flag] == false)
	{
		boardMap[flag] = true;
		queue.push(*newNode);
	}
}
