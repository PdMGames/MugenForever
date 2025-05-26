using System.Collections.Generic;

namespace MugenForever.CNS
{
    public class StateControllerParameter
    {
        public string Name { get; set; }
        public string Value { get; set; } // Valor como string, parsing específico será feito depois

        public StateControllerParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
