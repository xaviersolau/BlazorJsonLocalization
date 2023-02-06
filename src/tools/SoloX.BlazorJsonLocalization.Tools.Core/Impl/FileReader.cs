using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SoloX.BlazorJsonLocalization.Tools.Core.Impl
{
    public class FileReader : IReader
    {
        private readonly string fileSufix;

        public FileReader(string fileSufix)
        {
            this.fileSufix = fileSufix;
        }

        public void Read(string location, string name, Action<Stream> reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader", "The argument reader was null.");
            }

            string path = name + fileSufix;

            string text = Path.Combine(location, path);

            if (File.Exists(text))
            {
                using (FileStream stream = File.Open(text, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    reader(stream);    
                }
            }
        }
    }
}
