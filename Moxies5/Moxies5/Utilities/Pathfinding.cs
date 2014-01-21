using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using Moxies5.Controllers;

namespace Moxies5.Utilities
{
    class Pathfinding
    {

        #region Fields
        private Tile[,] grid = PathfindingController.Tiles;
        public Tile startTile;
        public Tile endTile;
        public Tile currentTile;
        private Dictionary<Tile,Tile> came_from;
        private List<Tile> path;
        private List<PathTile> pathTiles;
        public bool useDiagonal = true;
        public List<Tile> ignoreList;
        private List<Tile> closedList = new List<Tile>();
        private List<Tile> openList = new List<Tile>();
        private Game game;
        public bool allowDiagonalMovement = true;
        #endregion

        #region Properties
        public List<Tile> Path
        {
            get
            {
                return path;
            }
        }
        #endregion

        public Pathfinding(Game game, List<Tile> tileIgnoreList)
        {
            this.game = game;
            came_from = new Dictionary<Tile, Tile>();
            path = new List<Tile>();
            pathTiles = new List<PathTile>();
            this.ignoreList = tileIgnoreList;
        }

        public void FindPath(Tile startTile, Tile endTile, Tile[,] grid)
        {

            #region Resetting and Such
            closedList.Clear();
            openList.Clear();
            came_from.Clear();
            path.Clear();
            pathTiles.Clear();

            foreach (Tile tile in grid)
            {
                tile.Reset();
            }

            this.startTile = startTile;
            this.endTile = endTile;
            this.grid = grid;
            #endregion

            openList.Add(startTile);


            while (openList.Count != 0)
            {
                currentTile = GetTileWithLowestCost(openList);

                //if current tile is the end tile then stop searching
                if (currentTile == endTile)
                {
                    pathTiles.Clear();
                    ConstructPath(currentTile);
                    return;
                }
                else
                {
                    //Add current tile to closed list and remove from open list
                    openList.Remove(currentTile);
                    closedList.Add(currentTile);

                    //Get adjacent tiles
                    List<Tile> adjacentTiles = GetAdjacentTiles(currentTile);

                    foreach (Tile aTile in adjacentTiles)
                    {
                        float distance = 10;
                        if(aTile.diagonal == true)
                            distance = 14;

                        float tempG = currentTile.cost + distance;
                        if (closedList.Contains(aTile) && tempG >= aTile.cost)
                        {
                            continue;
                        }

                        if (!openList.Contains(aTile) || tempG < aTile.cost)
                        {
                            came_from[aTile] = currentTile;
                            aTile.parent = currentTile;
                            aTile.cost = tempG;
                            if(!useDiagonal)
                                aTile.total = aTile.cost + ManhattanDistance(aTile) + TieBreaker(currentTile, endTile, startTile);
                            else
                                aTile.total = aTile.cost + DiagonalDistance(aTile) + TieBreaker(currentTile, endTile, startTile);

                            if (!openList.Contains(aTile))
                                openList.Add(aTile);
                        }
                    }
                }
            }
        }

        private void ConstructPath(Tile current)
        {
            path.Add(current);
            PathTile pTile = new PathTile(game, current.Location);
            pathTiles.Add(pTile);
            pTile.color = Color.Red;
            while (current != startTile)
            {
                Tile parent = GetParent(current);
                current = parent;
                PathTile nTile = new PathTile(game, current.Location);
                pathTiles.Add(nTile);
                path.Add(current);
            }
        }

        private Tile GetParent(Tile tile)
        {
            if (came_from[tile] != null)
            {
                return came_from[tile];
            }
            return null;
        }

        //Get the tile with the lowest cost
        public Tile GetTileWithLowestCost(List<Tile> openlist)
        {
            Tile tileWithLowestTotal = new Tile(game, 0,0);
            float lowestTotal = int.MaxValue;

            //search all open tiles and get the tile with the lowest total cost
            foreach (Tile oTile in openlist)
            {
                if (grid[(int)oTile.Location.X, (int)oTile.Location.Y].total <= lowestTotal)
                {
                    tileWithLowestTotal = grid[(int)oTile.Location.X, (int)oTile.Location.Y];
                    lowestTotal = tileWithLowestTotal.total;
                }
            }

            return tileWithLowestTotal;
        }

        //Check if inside boundry and walkable
        public List<Tile> GetAdjacentTiles(Tile currenttile)
        {
            List<Tile> adjacenttiles = new List<Tile>();
            Vector2 belowTilePos = new Vector2((int)currenttile.Location.X, (int)currenttile.Location.Y - 1);
            Vector2 topTilePos = new Vector2((int)currenttile.Location.X, (int)currenttile.Location.Y + 1);
            Vector2 rightTilePos = new Vector2((int)currenttile.Location.X + 1, (int)currenttile.Location.Y);
            Vector2 leftTilePos = new Vector2((int)currenttile.Location.X - 1, (int)currenttile.Location.Y);
            bool topBlocked = false;
            bool bottomBlocked = false;
            bool rightBlocked = false;
            bool leftBlocked = false;

            //Tile above
            if (topTilePos.Y < PathfindingController.TilesHigh)
            {
                Tile adjacenttile = grid[(int)topTilePos.X, (int)topTilePos.Y];
                if (!adjacenttile.Blocked || (adjacenttile.Location == endTile.Location && adjacenttile.CanBeUsedAsTarget))
                {
                    adjacenttiles.Add(adjacenttile);
                }
                else
                {
                    topBlocked = true;
                }
            }
            //Tile below
            if (belowTilePos.Y >= 0)
            {
                Tile adjacenttile = grid[(int)belowTilePos.X, (int)belowTilePos.Y];
                if (!adjacenttile.Blocked || (adjacenttile.Location == endTile.Location && adjacenttile.CanBeUsedAsTarget))
                {
                    adjacenttiles.Add(adjacenttile);
                }
                else
                {
                    bottomBlocked = true;
                }
            }
            //Tile to the right
            if (rightTilePos.X < PathfindingController.TilesWide)
            {
                Tile adjacenttile = grid[(int)rightTilePos.X, (int)rightTilePos.Y];
                if (!adjacenttile.Blocked || (adjacenttile.Location == endTile.Location && adjacenttile.CanBeUsedAsTarget))
                {
                    adjacenttiles.Add(adjacenttile);
                }
                else
                {
                    rightBlocked = true;
                }
            }
            //Tile to the left
            
            if (leftTilePos.X >= 0)
            {
                Tile adjacenttile = grid[(int)leftTilePos.X, (int)leftTilePos.Y];
                if (!adjacenttile.Blocked || (adjacenttile.Location == endTile.Location && adjacenttile.CanBeUsedAsTarget))
                {
                    adjacenttiles.Add(adjacenttile);
                }
                else
                {
                    leftBlocked = true;
                }
            }

            //Diagonals
            if (useDiagonal)
            {
                Vector2 aboveRightTilePos = new Vector2((int)currenttile.Location.X + 1, (int)currenttile.Location.Y + 1);
                Vector2 aboveLeftTilePos = new Vector2((int)currenttile.Location.X - 1, (int)currenttile.Location.Y + 1);
                Vector2 belowRightTilePos = new Vector2((int)currenttile.Location.X + 1, (int)currenttile.Location.Y - 1);
                Vector2 belowLeftTilePos = new Vector2((int)currenttile.Location.X - 1, (int)currenttile.Location.Y - 1);

                //Tile above right
                if (aboveRightTilePos.Y < PathfindingController.TilesHigh && aboveRightTilePos.X < PathfindingController.TilesWide)
                {
                    Tile adjacenttile = grid[(int)aboveRightTilePos.X, (int)aboveRightTilePos.Y];
                    adjacenttile.diagonal = true;
                    if (!adjacenttile.Blocked || (adjacenttile.Location == endTile.Location && adjacenttile.CanBeUsedAsTarget))
                    {
                        if (allowDiagonalMovement)
                            adjacenttiles.Add(adjacenttile);
                        else
                        {
                            if(!topBlocked && !rightBlocked)
                                adjacenttiles.Add(adjacenttile);
                        }
                    }
                }

                //Tile above left
                if (aboveLeftTilePos.Y < PathfindingController.TilesHigh && aboveLeftTilePos.X >= 0)
                {
                    Tile adjacenttile = grid[(int)aboveLeftTilePos.X, (int)aboveLeftTilePos.Y];
                    adjacenttile.diagonal = true;
                    if (!adjacenttile.Blocked || (adjacenttile.Location == endTile.Location && adjacenttile.CanBeUsedAsTarget))
                    {
                        if (allowDiagonalMovement)
                            adjacenttiles.Add(adjacenttile);
                        else
                        {
                            if (!topBlocked && !leftBlocked)
                                adjacenttiles.Add(adjacenttile);
                        }
                    }
                }

                //Tile below right
                
                if (belowRightTilePos.X < PathfindingController.TilesWide && belowRightTilePos.Y >= 0)
                {
                    Tile adjacenttile = grid[(int)belowRightTilePos.X, (int)belowRightTilePos.Y];
                    adjacenttile.diagonal = true;
                    if (!adjacenttile.Blocked || (adjacenttile.Location == endTile.Location && adjacenttile.CanBeUsedAsTarget))
                    {
                        if (allowDiagonalMovement)
                            adjacenttiles.Add(adjacenttile);
                        else
                        {
                            if (!bottomBlocked && !rightBlocked)
                                adjacenttiles.Add(adjacenttile);
                        }
                    }
                }
                //Tile below left
                
                if (belowLeftTilePos.X >= 0 && belowLeftTilePos.Y >= 0)
                {
                    Tile adjacenttile = grid[(int)belowLeftTilePos.X, (int)belowLeftTilePos.Y];
                    adjacenttile.diagonal = true;
                    if (!adjacenttile.Blocked || (adjacenttile.Location == endTile.Location && adjacenttile.CanBeUsedAsTarget))
                    {
                        if (allowDiagonalMovement)
                            adjacenttiles.Add(adjacenttile);
                        else
                        {
                            if (!bottomBlocked && !leftBlocked)
                                adjacenttiles.Add(adjacenttile);
                        }
                    }
                }
  
            }

            return adjacenttiles;
        }

        //Calculate manhattan distance
        public int ManhattanDistance(Tile adjacenttile)
        {
            int manhattan = Math.Abs((int)(endTile.Location.X - adjacenttile.Location.X)) * Math.Abs((int)(endTile.Location.X - adjacenttile.Location.X)) +
                Math.Abs((int)(endTile.Location.Y - adjacenttile.Location.Y)) * Math.Abs((int)(endTile.Location.Y - adjacenttile.Location.Y));

            return manhattan;
        }

        //Calculate euclidean distance
        public int EuclideanDistance(Tile adjacenttile)
        {

            // H = Math.sqrt(Math.pow((start.x - destination.x), 2) + Math.pow((start.y - destination.y), 2));

            int euclidean = (int)Math.Sqrt(Math.Pow((adjacenttile.Location.X - endTile.Location.X), 2) + Math.Pow((adjacenttile.Location.Y - endTile.Location.Y), 2));

            return euclidean;
        }

        //calculate diagonal distance
        public int DiagonalDistance(Tile adjacenttile)
        {
            //math.max(math.abs(node.x-end.x),math.abs(node.y-end.y))
            int dd = Math.Max(Math.Abs((int)adjacenttile.Location.X - (int)endTile.Location.X), Math.Abs((int)adjacenttile.Location.Y - (int)endTile.Location.Y));
            return dd;
        }

        //calculate the tie breaker
        public float TieBreaker(Tile currenttile, Tile endtile, Tile starttile)
        {
            int dx1 = (int)(currentTile.Location.X - endtile.Location.X);
            int dy1 = (int)(currentTile.Location.Y - endtile.Location.Y);
            int dx2 = (int)(starttile.Location.X - endtile.Location.X);
            int dy2 = (int)(starttile.Location.Y - endtile.Location.Y);
            int cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
            return cross * 0.001f;

        }

        public void Draw()
        {
            if (path != null)
            {
                foreach (PathTile tile in pathTiles)
                {
                    tile.Draw();
                }
            }
        } 

        public Tile GetTileFromMouse()
        {
            Tile tile = null;
            Vector2 mousePos = MainController.Camera.get_mouse_pos(MainController.GraphicsDevice);
            try
            {
                Vector2 tilePos = new Vector2((float)(mousePos.X / Tile.tileWidth), (float)(mousePos.Y / Tile.tileHeight));
                tile = PathfindingController.Tiles[(int)tilePos.X, (int)tilePos.Y];
            }
            catch
            {
                return null;
            }
            return tile;
        }

        public Tile GetTileFromVector(Vector2 vector)
        {
            Tile tile = null;
            try
            {
                Vector2 tilePos = new Vector2((float)(vector.X / Tile.tileWidth), (float)(vector.Y / Tile.tileHeight));
                tile = PathfindingController.Tiles[(int)tilePos.X, (int)tilePos.Y];
            }
            catch
            {
                return null;
            }
            return tile;
        }

        public Tile GetNextTileInPath(Tile current)
        {
            if (came_from.Count > 0)
            {
                foreach (Tile tile in path)
                {
                    if (came_from[tile] != null)
                    {
                        if (came_from[tile] == current)
                        {
                            return tile;
                        }
                    }
                }
            }
            return null;
        }

   }


    class PathTile
    {
        Texture2D texture;
        Vector2 location;
        Game game;
        public Color color;

        public PathTile(Game game, Vector2 location)
        {
            this.location = location;
            this.game = game;
            texture = game.Content.Load<Texture2D>("barpixel");
            color = Color.Orange;
        }

        public void Draw()
        {
            MainController.CameraSpriteBatch.Draw(texture, new Vector2(location.X * Tile.tileWidth, location.Y * Tile.tileHeight), new Rectangle(0, 0, 64, 64), color * 0.5f, 0, Vector2.Zero, 1, SpriteEffects.None,0);

        }

        public override string ToString()
        {

            return location.ToString();
        }
    }
}
