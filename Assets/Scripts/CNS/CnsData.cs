using System.Collections.Generic;

namespace MugenForever.CNS
{
    public class CnsData
    {
        // Statedef ID -> StateDefinition
        public Dictionary<int, StateDefinition> StateDefinitions { get; private set; }
        // Poderíamos adicionar constantes [Data] aqui depois
        // public Dictionary<string, string> Constants { get; private set; }

        public CnsData()
        {
            StateDefinitions = new Dictionary<int, StateDefinition>();
            // Constants = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
        }
    }
}
