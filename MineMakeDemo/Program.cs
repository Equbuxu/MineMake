using System;

namespace MineMakeDemo
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Type 1 to generate a demo world\nType 2 to generate a SkyGrid world");
            int worldType = Convert.ToInt32(Console.ReadLine());
            switch (worldType)
            {
                case 1:
                    Demo demo = new Demo();
                    break;
                case 2:
                    SkyGrid skyGrid = new SkyGrid();
                    break;
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
