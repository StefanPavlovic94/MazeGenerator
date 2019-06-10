using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var mazeGenerator = new MazeGenerator(100, 100);
            var points = mazeGenerator.GenerateMazePoints();
            mazeGenerator.CreateBitMapFromPoints(points);
        }
    }
}
