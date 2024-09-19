using System;
using PhysicsCore;

namespace IntermediateAxisWindows
{
    public static class ProgramMain
    {
        [STAThread]
        static void Main()
        {
            using (var game = new IntermediateAxisGame())
            {
                game.Run();
            }
        }
    }
}