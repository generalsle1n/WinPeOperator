using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinPeOperator
{
    internal class ioManager
    {
        public void createFile(String path, FileMode mode)
        {
            byte[] emtpy = new byte[0];
            using (FileStream fileStream = new FileStream(path, mode))
            {
                fileStream.Write(emtpy,0,emtpy.Length);
            }
        }

        public void writeToFile(string path, string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text.ToCharArray());
            using (FileStream fileStream = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                fileStream.Write(data, 0, data.Length);
            }
        }
    }
}
