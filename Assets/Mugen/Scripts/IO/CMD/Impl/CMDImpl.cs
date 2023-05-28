using MugenForever.Util;
using System.IO;

namespace Assets.Scripts.IO.CMD.Impl
{
    internal class CMDImpl : ReaderConfig
    {
        public CMDImpl(Stream data) : base(data)
        {

        }

        public CMDImpl(Stream data, bool isAddLastComment) : base(data, isAddLastComment)
        {

        }
    }
}
