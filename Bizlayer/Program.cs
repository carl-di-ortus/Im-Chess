using System;
using System.Diagnostics;
using System.Linq;
using Datalayer.Entities;
using Datalayer.Repositories;

namespace Bizlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            var engine = new EngineProcess(@"C:\Users\karolis.martinkus\Documents\Visual Studio 2013\Projects\Im-Chess\stockfish-6-64.exe");
        }
    }
}
