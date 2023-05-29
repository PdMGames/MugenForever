using MugenForever.IO.AIR;
using System.Collections.Generic;

namespace Assets.Mugen.Scripts.IO.AIR
{
    internal interface IAIRState
    {
        HashSet<AIRState> States { get; }
    }
}
