using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Priority_Queue;
using System;
using System.Collections.Generic;

namespace Endeavour
{
    enum TileContent
    { Grass, Road, Villge, Water, Lake, Forest, Farm, Paddock, Mine, Shop };

    class AI
    {
        // intrinsic
        Vector2 position;

        // specific
        float hunger = 0f;
        float thirst = 0f;
        float tiredness = 0f;

    }

    class WorldTile
    {
        public WorldTile(Color colour, bool isWalkable, float travelCost)
        {
            mColour = colour;
            mIsWalkable = isWalkable;
            mTravelCost = travelCost;
        }

        public Color mColour = Color.White;
        public bool mIsWalkable = true;
        public float mTravelCost = 0f;
        
        // static, pre-defined constants for world tiles
        static WorldTile()
        {
            DEFAULT     = new WorldTile(Color.White, false, 5f);
            Road        = new WorldTile(new Color(136, 0, 21), true, 0.2f);
            Grass       = new WorldTile(new Color(116, 228, 150), true, 2f);
            City        = new WorldTile(new Color(195, 195, 195), true, 2f);
            Forest      = new WorldTile(new Color(34, 177, 76), true, 4f);
            Sand        = new WorldTile(new Color(239, 228, 176), true, 8f);
            Water       = new WorldTile(new Color(0, 162, 232),  true, 20f);
            WorldBorder = new WorldTile(Color.Black, false, float.MaxValue);
        }

        public static WorldTile DEFAULT { get; private set; }
        public static WorldTile Road { get; private set; }
        public static WorldTile Grass { get; private set; }
        public static WorldTile City { get; private set; }
        public static WorldTile Forest { get; private set; }
        public static WorldTile Sand { get; private set; }
        public static WorldTile Water { get; private set; }
        public static WorldTile WorldBorder { get; private set; }

        
    }



    class World
    {
        public static List<WorldTile> GetNeighbours(WorldTile[,] grid, Point point, bool diagonals = true)
        {
            List<WorldTile> neighbours = new List<WorldTile>();

            neighbours.Add(grid[point.X - 1, point.Y]);
            neighbours.Add(grid[point.X + 1, point.Y]);
            neighbours.Add(grid[point.X, point.Y - 1]);
            neighbours.Add(grid[point.X, point.Y + 1]);

            if (diagonals)
            {
                neighbours.Add(grid[point.X - 1, point.Y - 1]);
                neighbours.Add(grid[point.X + 1, point.Y + 1]);
                neighbours.Add(grid[point.X + 1, point.Y - 1]);
                neighbours.Add(grid[point.X - 1, point.Y + 1]);
            }

            return neighbours;
        }

        public static List<Point> GetNeighbours(Point point, bool diagonals = true)
        {
            List<Point> neighbours = new List<Point>();

            neighbours.Add(new Point(point.X - 1, point.Y));
            neighbours.Add(new Point(point.X + 1, point.Y));
            neighbours.Add(new Point(point.X, point.Y - 1));
            neighbours.Add(new Point(point.X, point.Y + 1));

            if (diagonals)
            {
                neighbours.Add(new Point(point.X - 1, point.Y - 1));
                neighbours.Add(new Point(point.X + 1, point.Y + 1));
                neighbours.Add(new Point(point.X + 1, point.Y - 1));
                neighbours.Add(new Point(point.X - 1, point.Y + 1));
            }

            return neighbours;
        }

        //Dictionary<Color, TileContent> mMapDefinition; // maps colours to the meaning of that colour
        Dictionary<String, Texture2D> mTextureAtlas;
        Texture2D mPixel;
        //Texture2D mInputImage;
        RenderTarget2D mMap;
        GraphicsDevice mGraphicsDevice;

        WorldTile[,] mWorld; // = new WorldTile[64, 64];

        List<Point> mPath;

        int mWorldSizeX = 0;
        int mWorldSizeY = 0;
        int mGridSize = 8;
        int mWinWidth = 0;
        int mWinHeight = 0;


        public void ProcessInput(InputState inputState)
        {
            if (inputState.HasMouseMoved())
            {
                Point mp = inputState.mCurrentMouseState.Position;

                if (new Rectangle(0, 0, mWorldSizeX * mGridSize, mWorldSizeY * mGridSize).Contains(mp))
                {
                    mPath = GetPath_AStar(mWorld, new Point(2, 2), 
                        new Point(mp.X / mGridSize, mp.Y / mGridSize));

                }

            }

        }

        public void LoadContent(GraphicsDevice graphicsDevice, Dictionary<String, Texture2D> textureAtlas)
        {
            Dictionary<Color, WorldTile> TileLookup = new Dictionary<Color, WorldTile>
            {
                { Color.White, WorldTile.DEFAULT },
                { new Color(136, 0, 21), WorldTile.Road },
                { new Color(116, 228, 150), WorldTile.Grass },
                { new Color(195, 195, 195), WorldTile.City },
                { new Color(34, 177, 76), WorldTile.Forest },
                { new Color(239, 228, 176), WorldTile.Sand },
                { new Color(0, 162, 232), WorldTile.Water },
                { Color.Black, WorldTile.WorldBorder },
            };


            // set private members
            mGraphicsDevice = graphicsDevice;
            mTextureAtlas = textureAtlas;

            // create a 'pixel' texture
            mPixel = new Texture2D(mGraphicsDevice, mGridSize, mGridSize);

            // convert input texture into our useful 2d array
            Texture2D inputImage = mTextureAtlas["test_village"];

            mWorldSizeX = inputImage.Width;
            mWorldSizeY = inputImage.Height;
            mWorld = new WorldTile[mWorldSizeX, mWorldSizeY];

            Color[] colors1D = new Color[mWorldSizeX * mWorldSizeY];
            inputImage.GetData<Color>(colors1D);
            for (int y = 0; y < mWorldSizeY; y++)
            {
                for (int x = 0; x < mWorldSizeX; x++)
                {
                    Color c = colors1D[x + y * mWorldSizeX];

                    if (TileLookup.ContainsKey(c))
                    {
                        mWorld[x, y] = TileLookup[c];
                    }
                    else
                        mWorld[x, y] = WorldTile.DEFAULT;
                }
            }

            // populate the texture with white
            List<Color> colorList = new List<Color>();
            for (int i = 0; i < mGridSize * mGridSize; ++i)
                colorList.Add(Color.White);
            mPixel.SetData(colorList.ToArray());

            mWinWidth = mGraphicsDevice.PresentationParameters.BackBufferWidth;
            mWinHeight = mGraphicsDevice.PresentationParameters.BackBufferHeight;

            // map
            mMap = new RenderTarget2D(
                mGraphicsDevice,
                mWinWidth,
                mWinHeight);

            mPath = GetPath_AStar(mWorld, new Point(2, 2), new Point(57, 38));
        }

        // http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
        // This is the diagonal distance
        public static float heuristic(Point end, Point next)
        {
            // When D = 1 and D2 = 1, this is called the Chebyshev distance.
            // When D = 1 and D2 = sqrt(2), this is called the octile distance.
            float D = 1;
            float D2 = 1;


            float dx = (float)Math.Abs(next.X - end.X);
            float dy = (float)Math.Abs(next.Y - end.Y);
            float h = D * (dx + dy) + (D2 - 2 * D) * Math.Min(dx, dy);

            return h;
        }

        // http://www.redblobgames.com/pathfinding/a-star/introduction.html
        public static List<Point> GetPath_AStar(WorldTile[,] grid, Point begin, Point end)
        {
            List<Point> path = new List<Point>();

            SimplePriorityQueue<Point> frontier = new SimplePriorityQueue<Point>();
            Dictionary<Point, float> costSoFar = new Dictionary<Point, float>();
            Dictionary<Point, Point> predecessors = new Dictionary<Point, Point>();

            frontier.Enqueue(begin, 0f);
            predecessors[begin] = begin;
            costSoFar[begin] = 0f;

            bool pathFound = false;

            while (frontier.Count != 0)
            {
                Point curr = frontier.Dequeue();
                if (curr == end)
                {
                    pathFound = true;
                    break;
                }

                foreach (Point next in GetNeighbours(curr))
                {
                    // tile is outside the world
                    if (next.X < 0 || next.X > grid.GetLength(0) || next.Y < 0 || next.Y > grid.GetLength(1))
                        continue;

                    if (!grid[next.X, next.Y].mIsWalkable)
                        continue;

                    float distance = (next.X - curr.X == 0 || next.Y - curr.Y == 0) ? 1f : (float)Math.Sqrt(2);

                    float newCost = costSoFar[curr] + distance + grid[next.X, next.Y].mTravelCost;
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;
                        float priority = newCost + heuristic(end, next);
                        frontier.Enqueue(next, priority);
                        predecessors[next] = curr;
                    }

                }

            }

            if (pathFound)
            {
                path.Add(end);
                Point temp = end;
                while (temp != begin)
                {

                    temp = predecessors[temp];
                    path.Add(temp);
                }

                path.Add(begin);
            }

            return path;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // world
            for (int y = 0; y < mWorldSizeY; y++)
            {
                for (int x = 0; x < mWorldSizeX; x++)
                {
                    spriteBatch.Draw(
                        mPixel,
                        new Vector2(x * mGridSize, y * mGridSize),
                        null,
                        null,
                        null,
                        0f,
                        Vector2.One,
                        mWorld[x, y].mColour,
                        SpriteEffects.None,
                        0f);
                }
            }

            // path
            float pathPixelScale = 2f;
            Vector2? previousCentreOfGrid = null;
            foreach (Point p in mPath)
            {
                Vector2 centreOfCurrentGrid = new Vector2(p.X * mGridSize, p.Y * mGridSize) + new Vector2(mGridSize / 2);

                if (previousCentreOfGrid != null)
                    DrawLine(spriteBatch, centreOfCurrentGrid, previousCentreOfGrid.Value);
                previousCentreOfGrid = centreOfCurrentGrid;

                spriteBatch.Draw(
                    mPixel,
                    new Vector2(p.X * mGridSize, p.Y * mGridSize) + new Vector2(mGridSize / 2),
                    null,
                    null,
                    new Vector2(mGridSize / pathPixelScale),
                    0f,
                    new Vector2(1 / pathPixelScale),
                    Color.Red,
                    SpriteEffects.None,
                    0f);

            }

            void DrawLine(SpriteBatch sb, Vector2 start, Vector2 end)
            {
                Vector2 edge = end - start;
                // calculate angle to rotate line
                float angle =
                    (float)Math.Atan2(edge.Y, edge.X);


                sb.Draw(mPixel,
                    new Rectangle(// rectangle defines shape of line and position of start of line
                        (int)start.X,
                        (int)start.Y,
                        (int)edge.Length(), //sb will strech the texture to fill this rectangle
                        1), //width of line, change this to make thicker line
                    null,
                    Color.Red, //colour of line
                    angle,     //angle of line (calulated above)
                    new Vector2(0, 0), // point in line about which to rotate
                    SpriteEffects.None,
                    0);

            }

            // scale 8 times
            //spriteBatch.Draw(mInputImage, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 8f, SpriteEffects.None, 0);
        }
    }
}
