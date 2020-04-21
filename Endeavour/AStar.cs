using System;
using System.Collections.Generic;
using static ConsoleApp1.Common;
using Priority_Queue;

namespace ConsoleApp1
{
	static class AStar
	{
		public static double heuristic(Point curr, Point end)
		{
			// Diagonal distance
			const int D = 1;
			const int D2 = 1;

			var dx = Math.Abs(curr.x - end.x);
			var dy = Math.Abs(curr.y - end.y);
			double h = D * (dx + dy) + (D2 - 2 * D) * Math.Min(dx, dy);


			// Manhattan distance
			//double h = Math.Abs(curr.x - end.x) + Math.Abs(curr.y - end.y);

			// Euclidean distance
			//double h = Distance(curr, end);

			h *= 200;
			return h;
		}

		public static void PrintList(List<Point> list)
		{
			foreach (var p in list)
			{
				Console.Write("[{0}, {1}]", p.x, p.y);
			}
		}

		public static List<Point> GetPath(Tile[,] grid, Point begin, Point end)
		{
			Console.WriteLine("[GetPath]");

			var path = new List<Point>();

			var frontier = new SimplePriorityQueue<Point>();

			frontier.Enqueue(begin, 0);

			var came_from = new Dictionary<Point, Point>();
			var cost_so_far = new Dictionary<Point, float>();

			came_from[begin] = null;
			cost_so_far[begin] = 0;

			var pathFound = false;

			while (frontier.Count != 0)
			{
				// Get first point
				var current = frontier.Dequeue();

				grid[current.x, current.y].wasVisited = true;

				// if this is the end point we're done
				if (current == end)
				{
					pathFound = true;
					break;
				}

				// grab neighbours
				var neighbours = GetNeighbours(grid, current);

				foreach (var next in neighbours)
				{
					// don't even consider solid tiles
					if (grid[next.x, next.y].isSolid)
					{
						continue;
					}

					// get the distance between current node and the neighbor
					var nDistance = ((current.x - next.x == 0 || current.y - next.y == 0) ? 1f : (float)Math.Sqrt(2));

					// calculate the new cost of this tile
					var new_cost = cost_so_far[current] + nDistance;

					// if we haven't given this tile a cost, or the cost is less than
					// the existing cost, update it
					if (!cost_so_far.ContainsKey(next) || new_cost < cost_so_far[next])
					{
						cost_so_far[next] = new_cost;
						var priority = new_cost + (float)heuristic(end, next);
						frontier.Enqueue(next, priority);
						came_from[next] = current;
					}
				}
			}

			if (pathFound)
			{
				// construct the path (will be in reverse
				path.Add(end);
				var curr = came_from[end];
				while (curr != begin)
				{
					path.Add(curr);
					curr = came_from[curr];
				}
				path.Add(begin);

				foreach (var p in path)
				{
					Console.WriteLine(p);
				}

				// Reverse the path so it's beginning-to-end
				path.Reverse();
			}

			Console.WriteLine("{0}", pathFound);

			return path;
		}

		static void Main2(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			const int gridSize = 20;
			var grid = new Tile[gridSize, gridSize];
			var begin = new Point(2, 2);
			var end = new Point(16, 15);

			for (var y = 0; y < gridSize; y++)
			{
				for (var x = 0; x < gridSize; x++)
				{
					var temp = new Point(x, y);
					grid[x, y] = new Tile(temp);

					//grid[x, y].distance = (int)Distance(end, temp);

					// borders are solid
					if (x == 0 || y == 0 || x == gridSize - 1 || y == gridSize - 1)
					{
						grid[x, y].isSolid = true;
					}
				}
			}

			// add some walls
			for (var i = 7; i < 20; i++)
			{
				grid[i, 13].isSolid = true;
			}

			for (var i = 14; i < 20; i++)
			{
				grid[7, i].isSolid = true;
			}

			grid[7, 17].isSolid = false;

			// get a path
			var path = GetPath(grid, begin, end);

			// just for printing
			foreach (var p in path)
			{
				grid[p.x, p.y].isOnPath = true;
			}

			Common.PrintGridTile(grid);
			_ = Console.ReadLine();

		}


	}
}

