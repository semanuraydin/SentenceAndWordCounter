using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SentenceAndWordCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            SentenceAndWordCounter cls = new SentenceAndWordCounter();
            Thread thread = new Thread(cls.MainThread);
            thread.Start();
        }
    }
}
