using System;
using System.Collections.Generic;

namespace Algorithms.Array
{

	public class CountGroup
	{
		enum Neighbor
		{
			UPPER,
			LOWER,
			LEFT,
			RIGHT
		}

		class Node
		{
			internal int x;
			internal int y;
			internal int value;
			internal int group;
			internal Matrix matrix;

            public Node(int x, int y, int value, int group, Matrix matrix)
            {
 				this.x = x;
				this.y = y;
				this.value = value;
				this.group = group;
				this.matrix = matrix;
			}
			public int X
			{
				get
                {
                    return x;
                }
			}
			public int Y
			{
				get
                {
                    return y;
                }
			}
			public bool NodeValued
			{
				get
				{
					return (value > 0);
				}
			}
			public bool Grouped
			{
				get
				{
					return (group > 0);
				}
			}
			public int Group
			{
				get
				{
					return group;
				}
				set
				{
					this.group = value;
				}
			}
			public Node getValuedAndUngroupedNeighbor(Neighbor n)
			{
				Node node = null;
				switch (n)
				{
				case Neighbor.UPPER:
					if (x > 0)
					{
						node = matrix.getNode(x - 1, y);
					}
					break;
				case Neighbor.LOWER:
					if (x < (matrix.Size-1))
					{
						node = matrix.getNode(x + 1, y);
					}
					break;
				case Neighbor.LEFT:
					if (y > 0)
					{
						node = matrix.getNode(x, y - 1);
					}
					break;
				case Neighbor.RIGHT:
					if (y < (matrix.Size-1))
					{
						node = matrix.getNode(x, y + 1);
					}
					break;
				}
				if (node != null && node.NodeValued && !node.Grouped)
				{
					return node;
				}
				else
				{
					return null;
				}
			}
		}
		class NodeQueue
		{
			List<Node> queue;
 
            public NodeQueue()
            {
				queue = new List<Node>();
			}
			public bool Empty
			{
				get
				{
					return queue.Count == 0;
				}
			}
			public void add(Node node)
			{
				if (node == null)
				{
					return;
				}
				for (int i = queue.Count - 1; i >= 0; i--)
				{
					Node n = queue[i];
					if (node.X == n.X && node.Y == n.Y)
					{
						return;
					}
				}
				queue.Add(node);
			}
			public Node remove()
			{
				if (queue.Count > 0)
				{
                    Node node = queue[0];
					queue.RemoveAt(0);
                    return node;
				}
				else
				{
					return null;
				}
			}
		}
		class Matrix
		{
			internal Node[][] matrix;

            public Matrix(int[][] m)
            {
				if (m == null || m.Length <= 1 || m.Length != m[0].Length)
				{
					throw new System.ArgumentException("Invalid input matrix");
				}
				matrix = new Node[m.Length][];
				for (int i = 0; i < m.Length; i++)
				{
					matrix[i] = new Node[m.Length];
					for (int j = 0; j < m.Length; j++)
					{
                        matrix[i][j] = new Node(i, j, m[i][j], 0, this);
                    }
                }
			}

			public int Size
			{
				get
				{
					return matrix.Length;
				}
			}

			public Node getNode(int x, int y)
			{
				if (0 <= x && x < Size && 0 <= y && y < Size)
				{
					return matrix[x][y];
				}
				else
				{
					return null;
				}
			}

			public void printMatrixGroups()
			{
				Console.WriteLine("Group Matrix:");
				for (int x = 0; x < Size; x++)
				{
					for (int y = 0; y < Size; y++)
					{
						Console.Write(" " + matrix[x][y].Group);
					}
					Console.Write("\r\n");
				}
			}
		}

		Matrix matrix;
		IDictionary<int, IList<int>> groupMap = new Dictionary<int, IList<int>>();
		NodeQueue groupNodeQueue = new NodeQueue();

		public CountGroup(int[][] m)
		{
            matrix = new Matrix(m);
        }


        public IDictionary<int, int> countgroups()
		{
			IDictionary<int, int> result = new Dictionary<int, int>();

			int currentGroup = 0;
			int currentGroupSize = 0;

			// mark groups to matrix nodes using BFS or Breath First Search
			// not to use PFS or Path First Search which needs a recursive method and may cause stack overflow
			for (int i = 0; i < matrix.Size; i++)
			{
				for (int j = 0; j < matrix.Size; j++)
				{
					Node node = matrix.getNode(i, j);
					// skip un-valued or grouped node
					if (!node.NodeValued || node.Grouped)
					{
						continue;
					}

					// new group
					currentGroup++;
					currentGroupSize = 0;
					groupNodeQueue.add(node);

					// process nodes in queue
					while (!groupNodeQueue.Empty)
					{
						Node nodeInQueue = groupNodeQueue.remove();
						nodeInQueue.Group = currentGroup;
						currentGroupSize++;

						// add valued neighbor nodes to queue
						groupNodeQueue.add(nodeInQueue.getValuedAndUngroupedNeighbor(Neighbor.UPPER));
						groupNodeQueue.add(nodeInQueue.getValuedAndUngroupedNeighbor(Neighbor.LOWER));
						groupNodeQueue.add(nodeInQueue.getValuedAndUngroupedNeighbor(Neighbor.LEFT));
						groupNodeQueue.add(nodeInQueue.getValuedAndUngroupedNeighbor(Neighbor.RIGHT));

					}
					// add current group to groupMap using group size as key
					if (groupMap.ContainsKey(currentGroupSize))
					{
						groupMap[currentGroupSize].Add(currentGroup);
					}
					else
					{
						IList<int> entry = new List<int>();
						entry.Add(currentGroup);
						groupMap[currentGroupSize] = entry;
					}

					matrix.printMatrixGroups();
				}
			}
			// for each group size, group count is size of value list
			foreach (int key in groupMap.Keys)
			{
				result[key] = groupMap[key].Count;
			}
			return result;
		}

		private static void printResult(IDictionary<int, int> result)
		{
			Console.WriteLine("Size\t#Groups");
			foreach (int key in new SortedSet<int>(result.Keys))
			{
				Console.Write("  " + key + "\t  " + result[key] + "\r\n");
			}
		}

		public static void Main(string[] args)
		{
			int[][] m1 = new int[][]
			{
				new int[] {1, 0, 1, 1, 0},
				new int[] {0, 1, 0, 0, 1},
				new int[] {1, 0, 1, 1, 0},
				new int[] {1, 0, 1, 1, 0},
				new int[] {0, 1, 0, 0, 1}
			};
			CountGroup cg1 = new CountGroup(m1);
			Console.WriteLine("Matrix 1:");
			IDictionary<int, int> result1 = cg1.countgroups();
			printResult(result1);

			int[][] m2 = new int[][]
			{
				new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
				new int[] {1, 1, 1, 1, 0, 0, 0, 0, 0, 0},
				new int[] {1, 1, 1, 0, 0, 0, 0, 1, 1, 1},
				new int[] {1, 1, 0, 0, 1, 0, 0, 1, 1, 1},
				new int[] {1, 0, 1, 0, 0, 1, 1, 0, 0, 0},
				new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
				new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
				new int[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
			};
			CountGroup cg2 = new CountGroup(m2);
			Console.WriteLine("\r\nMatrix 2:");
			IDictionary<int, int> result2 = cg2.countgroups();
			printResult(result2);
            Console.ReadLine();
		}
	}

}