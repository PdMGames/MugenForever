using System.Collections.Generic;

namespace MugenForever.Sff
{
    public class SffInfo
    {
        public List<SffSprite> sprites;
        public string verHi;
        public string verLo1;
        public string verLo2;
        public string verLo3;
        // Outros campos do SFF que podem ser úteis podem ser adicionados aqui
        // como totalImage, totalGroups, comments, etc.

        public SffInfo()
        {
            sprites = new List<SffSprite>();
        }
    }
}
