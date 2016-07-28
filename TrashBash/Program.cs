using System;

namespace TrashBash
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TrashBash game = new TrashBash())
            {
                game.Run();
            }
        }
    }
}

