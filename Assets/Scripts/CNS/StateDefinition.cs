using System.Collections.Generic;

namespace MugenForever.CNS
{
    public class StateDefinition
    {
        public int Id { get; set; } // O número do Statedef
        public string Type { get; set; } // S, C, A, L (Stand, Crouch, Air, Land)
        public string MoveType { get; set; } // A, I, H (Attack, Idle, Hit)
        public string Physics { get; set; } // S, C, A, N (Stand, Crouch, Air, None)
        // Outros parâmetros do Statedef como anim, velset, ctrl, poweradd, etc.
        public Dictionary<string, string> Parameters { get; private set; }
        public List<StateController> Controllers { get; private set; }

        public StateDefinition(int id)
        {
            Id = id;
            Parameters = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
            Controllers = new List<StateController>();
        }
    }
}
