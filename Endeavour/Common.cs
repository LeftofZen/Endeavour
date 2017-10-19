using System;
using System.Collections;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Common
    {
        public class Tile
        {
            public Tile(Point p)
            {
                point = p;
            }

            public Tile(Tile t)
            {
                this.isSolid = t.isSolid;
                this.isOnPath = t.isOnPath;
                this.point = new Point(t.point.x, t.point.y);

            }

            public List<Point> GetNeighbours()
            {
                return new List<Point>
                {
                    new Point(point.x-1, point.y),
                    new Point(point.x-1, point.y-1),
                    new Point(point.x,   point.y-1),
                    new Point(point.x+1, point.y-1),
                    new Point(point.x+1, point.y),
                    new Point(point.x+1, point.y+1),
                    new Point(point.x,   point.y+1),
                    new Point(point.x-1, point.y+1),
                };
            }

            //public int pathVal = 0;
            public bool isSolid = false;
            // public bool hasBeenVisited = false;
            public bool isOnPath = false;
            public Point point;
            public bool wasVisited = false;

        }

        public static List<Point> GetNeighbours(Tile[,] grid, Point curr)
        {
            return grid[curr.x, curr.y].GetNeighbours();
        }


        public class Point
        {
            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int x;
            public int y;

            public override string ToString()
            {
                return String.Format("[{0},{1}]", x, y);
            }

            public static bool operator ==(Point p1, Point p2)
            {
                return p1?.x == p2?.x && p1?.y == p2?.y;
            }

            public static bool operator !=(Point p1, Point p2)
            {
                return !(p1 == p2);
            }

            public override bool Equals(object obj)
            {
                var item = obj as Point;

                if (item == null)
                {
                    return false;
                }
                else
                {
                    return this == item;
                }
            }

            public override int GetHashCode()
            {
                return x ^ (y << 16);
            }
        }

        public static void PrintGridTile(Tile[,] grid)
        {
            int gridSizeX = grid.GetLength(0);
            int gridSizeY = grid.GetLength(1);

            for (int y = 0; y < gridSizeY; y++)
            {
                for (int x = 0; x < gridSizeX; x++)
                {
                    Tile t = grid[x, y];

                    char c = ' ';

                    if (t.isSolid)
                        c = '\u2588';
                    //c = 'X';
                    //if (t.cost > 0)
                    //c = (char)((int)t.cost + '0');
                    else if (t.isOnPath)
                        c = '*';
                    else if (t.wasVisited)
                        c = '.';


                    Console.Write(c);

                }
                Console.Write('\n');
            }
        }

		public static void ClearGrid(float[,] grid)
		{
			int gridSizeX = grid.GetLength(0);
			int gridSizeY = grid.GetLength(1);

			for (int y = 0; y < gridSizeY; y++)
			{
				for (int x = 0; x < gridSizeX; x++)
				{
					grid[x, y] = 0f;
				}
			}
		}

		public static double Distance(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.x - p1.x, 2) + Math.Pow(p2.y - p1.y, 2));
        }

    }
}
