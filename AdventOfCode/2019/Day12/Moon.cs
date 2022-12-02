using System.Linq;

namespace AdventOfCode2019.Day12
{
    public class Moon
    {
        public Point3D Position { get; set; } = new Point3D();

        public Point3D Velocity { get; set; } = new Point3D();

        public int Id { get; set; }

        public Moon(string input, int id)
        {
            Id = id;

            var trimmed = input.Replace("<", "").Replace(">", "");

            foreach(var coordinate in trimmed.Split(","))
            {
                var assignment = coordinate.Trim().Split("=").ToArray();
                var value = int.Parse(assignment[1]);
                switch(assignment[0].ToLower())
                {
                    case "x": Position.X = value; break;
                    case "y": Position.Y = value; break;
                    case "z": Position.Z = value; break;
                }
            }
        }

        public void ApplyVelocity()
        {
            Position.Add(Velocity);
        }

        public int PotentialEnergy() => Position.Energy();
        public int KineticEnergy() => Velocity.Energy();
        public int TotalEnergy => PotentialEnergy() * KineticEnergy();
    }
}
