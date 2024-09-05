using System;
using PhysicsCore;

namespace BaseDemoWindows
{
    public static class ProgramMain
    {
        [STAThread]
        static void Main()
        {
            using (var game = new BaseDemoGame())
            {
                game.Run();
            }
        }
    }
}