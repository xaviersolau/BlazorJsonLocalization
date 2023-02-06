using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Tools.Core
{
    public interface IReader
    {
        void Read(string location, string name, Action<Stream> reader);
    }
}
