using System.Collections.Generic;

namespace MugenForever.CNS
{
    public class StateController
    {
        public string Type { get; set; } // Ex: "ChangeAnim", "VelSet"
        public List<string> Triggers { get; private set; } // Armazena triggers como strings por enquanto
        public List<StateControllerParameter> Parameters { get; private set; }

        public StateController()
        {
            Triggers = new List<string>();
            Parameters = new List<StateControllerParameter>();
        }
    }
}
