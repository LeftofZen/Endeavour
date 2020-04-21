using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	static class Fluid
	{
		public static float getSumOfNeighbours(float[,] world, int x, int y)
		{
			var sum = world[x, y];
			sum += world[x - 1, y];
			sum += world[x + 1, y];
			sum += world[x, y - 1];
			sum += world[x, y + 1];

			return sum;
		}

		public static void FluidPrintGrid(float[,] grid)
		{
			var gridSizeX = grid.GetLength(0);
			var gridSizeY = grid.GetLength(1);

			for (var y = 0; y < gridSizeY; y++)
			{
				for (var x = 0; x < gridSizeX; x++)
				{
					var intVal = (int)(grid[x, y] / 10);
					var c = ' ';
					if (intVal > 0)
					{
						c = (char)(intVal + '1');
					}
					else if (grid[x, y] > 0)
					{
						c = '.';
					}

					if (x == 0 || y == 0 || x == gridSizeX - 1 || y == gridSizeY - 1)
					{
						c = '\u2588';
					}

					Console.Write(c);

				}
				Console.Write('\n');
			}
		}

		static void FluidMain(string[] args)
		{
			const int worldSize = 16;
			var world = new float[worldSize, worldSize];
			var worldBB = new float[worldSize, worldSize];
			float[,] worldTemp;

			world[7, 8] = 100f;
			world[8, 8] = 100f;

			var count = 0;
			const int iterations = 5;

			while (count++ < iterations)
			{
				Common.ClearGrid(worldBB);

				var sumWater = 0f;

				// set back buffer outer ring to 0
				// we're cheating here by not clearing the whole thing
				for (var i = 0; i < worldSize; i++)
				{
					worldBB[0, i] = 0f;           // left side
					worldBB[worldSize - 1, i] = 0f; // right side
					worldBB[i, 0] = 0f;           // top side
					worldBB[i, worldSize - 1] = 0f; // bottom side
				}

				// iterate
				for (var y = 1; y < worldSize - 1; y++)
				{
					for (var x = 1; x < worldSize - 1; x++)
					{
						var s = getSumOfNeighbours(world, x, y);
						worldBB[x, y] = s / 5f;
						sumWater += s / 5f;
					}
				}

				// flip
				worldTemp = world;
				world = worldBB;
				worldBB = worldTemp;

				// print
				FluidPrintGrid(world);

				Console.WriteLine(sumWater);

			}

			//Console.WriteLine("hello world");
			_ = Console.ReadLine();
		}
	}
}
