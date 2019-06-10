using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MazeGenerator
{
    public partial class MazeGenerator
    {
        Random rnd = new Random();

        private readonly int height;
        private readonly int width;

        private Point EntryPoint { get; set; }

        Stack<Point> mazeStack = new Stack<Point>();

        private Point[,] MazeMatrix { get; set; }

        public MazeGenerator(int height, int width)
        {
            this.height = height;
            this.width = width;

            this.InitializeMazeMatrix();
        }

        public void InitializeMazeMatrix()
        {
            int heightWithEdges = this.height + 2;
            int widthWithEdges = this.width + 2;

            this.MazeMatrix = new Point[heightWithEdges, widthWithEdges]; //space for edges

            var entryPoint = rnd.Next(1, widthWithEdges - 2);

            for (int i = 0; i < heightWithEdges; i++)
            {
                for (int j = 0; j < widthWithEdges; j++)
                {
                    this.MazeMatrix[i, j] = new Point();

                    if (i == 0 || j == 0 || i == heightWithEdges - 1 || j == widthWithEdges - 1) //edges
                    {
                        this.MazeMatrix[i, j].IsVisited = null;
                    }
                    else //space inside set to true
                    {
                        this.MazeMatrix[i, j].IsVisited = false;
                    }

                    if (entryPoint == j && i == 0)
                    {
                        this.MazeMatrix[i, j].IsVisited = true;
                        this.EntryPoint = this.MazeMatrix[i, j];
                        this.EntryPoint.IsFirstNode = true;
                    }

                    this.MazeMatrix[i, j].Y = i;
                    this.MazeMatrix[i, j].X = j;
                }
            }
        }

        public List<Point> GenerateMazePoints()
        {
            Point point = MazeMatrix[this.EntryPoint.Y + 1, this.EntryPoint.X]; //start beyond entry point

            this.EntryPoint.IsVisited = true;
            mazeStack.Push(this.EntryPoint);

            point.ParentPoints.Add(this.EntryPoint);

            mazeStack.Push(point);

            var processessing = true;

            while (processessing)
            {
                point.IsVisited = true;

                var possibleDirections = GetPossibleDirections(point);

                if (possibleDirections.Count > 1)
                {
                    point.IsNode = true;
                }
                else
                {
                    point.IsNode = false;
                }


                if (possibleDirections.Count == 0)
                {
                    var isNotEndpoint = new Func<Point, bool>(p => p.X != EntryPoint.X && p.Y != EntryPoint.Y);

                    if (mazeStack.Where(isNotEndpoint).All(p => !p.IsNode))
                    {
                        processessing = false;
                        break;
                    }

                    while (mazeStack.Any() && mazeStack.First().IsNode == false)
                    {
                        mazeStack.First().IsVisited = true;
                        mazeStack.Pop();
                    }

                    if (!mazeStack.Any())
                    {
                        processessing = false;
                        break;
                    }

                    point = mazeStack.First();

                    continue;
                }


                var directionToFollow = ChooseRandomPositions(possibleDirections);

                var nextPoint = GetNextPoint(point, directionToFollow);

                nextPoint.ParentPoints.Add(point);

                point = nextPoint;

                mazeStack.Push(nextPoint);
            }

            var mazeList = new List<Point>();

            for (int i = 0; i < this.height + 2; i++)
            {
                for (int j = 0; j < this.width + 2; j++)
                {
                    if (MazeMatrix[i, j].ParentPoints.Any())
                    {
                        mazeList.Add(MazeMatrix[i, j]);
                    }
                }
            }

            return mazeList;
        }

        public void CreateBitMapFromPoints(List<Point> points)
        {
            using (var directBitmap = new DirectBitmap(this.width + 2, this.height + 2))
            {
                var exitPointXCandidates = points.Where(p => p.Y == this.height).Select(p => p.X).ToList();

                var exitPointIndex = rnd.Next(0, exitPointXCandidates.Count + 1);

                var widthOfFrame = this.width + 2;
                var heightOfFrame = this.height + 2;

                for (int i = 0; i < heightOfFrame; i++)
                {
                    for (int j = 0; j < widthOfFrame; j++)
                    {
                        if (points.Any(p => p.Y == i && p.X == j) 
                            || (EntryPoint.X == j && EntryPoint.Y == i) 
                            || i == heightOfFrame - 1 && exitPointXCandidates[exitPointIndex] + 1 == j)
                        {
                            directBitmap.SetPixel(j, i, Color.White);
                        }
                        else
                        {
                            directBitmap.SetPixel(j, i, Color.Black);
                        }
                    }
                }

               directBitmap.Bitmap.Save("maze.png");
            }
        }

        private Point GetNextPoint(Point currentPoint, Direction direction)
        {
            int x = 0;
            int y = 0;

            switch (direction)
            {
                case Direction.Up:
                    x = currentPoint.X;
                    y = currentPoint.Y - 1;
                    break;
                case Direction.Down:
                    x = currentPoint.X;
                    y = currentPoint.Y + 1;
                    break;
                case Direction.Left:
                    x = currentPoint.X - 1;
                    y = currentPoint.Y;
                    break;
                case Direction.Right:
                    x = currentPoint.X + 1;
                    y = currentPoint.Y;
                    break;
                default:
                    break;
            }

            return this.MazeMatrix[y, x];
        }

        private List<Direction> GetPossibleDirections(Point point)
        {
            var possibleDirections = new List<Direction>();

            int x = point.X;
            int y = point.Y;


            if (IsAccessibleDirection(point, Direction.Up))
            {
                possibleDirections.Add(Direction.Up);
            }

            if (IsAccessibleDirection(point, Direction.Down))
            {
                possibleDirections.Add(Direction.Down);
            }

            if (IsAccessibleDirection(point, Direction.Left))
            {
                possibleDirections.Add(Direction.Left);
            }

            if (IsAccessibleDirection(point, Direction.Right))
            {
                possibleDirections.Add(Direction.Right);
            }

            return possibleDirections;
        }

        private bool IsAccessibleDirection(Point point, Direction direction)
        {
            int directionPointX = 0;
            int directionPointY = 0;

            switch (direction)
            {
                case Direction.Up:
                    directionPointX = point.X;
                    directionPointY = point.Y - 1;
                    break;
                case Direction.Down:
                    directionPointX = point.X;
                    directionPointY = point.Y + 1;
                    break;
                case Direction.Left:
                    directionPointX = point.X - 1;
                    directionPointY = point.Y;
                    break;
                case Direction.Right:
                    directionPointX = point.X + 1;
                    directionPointY = point.Y;
                    break;
                default:
                    break;
            }

            if (this.MazeMatrix[directionPointY, directionPointX].IsVisited == false)
            {
                var upVisited = MazeMatrix[directionPointY - 1, directionPointX].IsVisited;
                var downVisited = MazeMatrix[directionPointY + 1, directionPointX].IsVisited;
                var leftVisited = MazeMatrix[directionPointY, directionPointX - 1].IsVisited;
                var rightVisited = MazeMatrix[directionPointY, directionPointX + 1].IsVisited;

                List<bool?> visitedSides = new List<bool?>() { upVisited, downVisited, leftVisited, rightVisited };

                if (visitedSides.Where(visitedSide => visitedSide == true).Count() == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public Direction ChooseRandomPositions(List<Direction> possibleDirections)
        {
            if (possibleDirections.Count == 1)
            {
                return possibleDirections[0];
            }

            var randomNumber = rnd.Next(0, possibleDirections.Count);

            return possibleDirections[randomNumber];
        }
    }
}
