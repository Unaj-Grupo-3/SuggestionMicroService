namespace Application.Helpers
{
    public class CalculateDistance
    {
        public CalculateDistance() { }

        public static int Calculate(double longitude1, double longitude2, double latitude1, double latitude2)
        {
            double theta = longitude1 - longitude2;

            double distance = 60 * 1.1515 * (180 / Math.PI) * Math.Acos(
                 Math.Sin(latitude1 * (Math.PI / 180)) * Math.Sin(latitude2 * (Math.PI / 180)) +
                 Math.Cos(latitude1 * (Math.PI / 180)) * Math.Cos(latitude2 * (Math.PI / 180)) * Math.Cos(theta * (Math.PI / 180))
                );

            return Convert.ToInt32(Math.Round(distance * 1.609344, 2));
        }
    }
}
