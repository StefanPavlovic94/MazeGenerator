using System.Collections.Generic;

namespace MazeGenerator
{
    public partial class MazeGenerator
    {
        public class Point
        {

            public int X;
            public int Y;

            public bool? IsVisited { get; set; }
            public bool IsNode { get; set; }
            public bool IsFirstNode { get; set; }
            public List<Point> ParentPoints { get; set; }

            public Point()
            {
                this.ParentPoints = new List<Point>();
            }
        }
    }
}
