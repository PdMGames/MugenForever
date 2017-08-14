using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MugenForever.SFF
{
    interface SFFInterface
    {
        SFFInfo read(FileStream fileSream);
    }
}
