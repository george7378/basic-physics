using System;
using PhysicsCore;

namespace WeightlessWindows
{
    public static class ProgramMain
    {
        [STAThread]
        static void Main()
        {
            using (var game = new WeightlessGame())
            {
                game.Run();
            }
        }
    }
}