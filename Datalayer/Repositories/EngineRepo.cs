using System.Collections.Generic;
using System.Linq;
using Datalayer.Entities;

namespace Datalayer.Repositories
{
    public class EngineRepo
    {
        private readonly ImChessModel _context;

        public EngineRepo()
        {
            _context = ImChessModel.Instance;
        }

        public void AddOption(string engineName, EngineOption option)
        {
            _context.Engines.First(e => e.Name == engineName).Options.Add(option);
            _context.SaveChanges();
        }

        public void Create(Engine engine)
        {
            _context.Engines.Add(engine);
            _context.SaveChanges();
        }

        public bool Exists(string name)
        {
            return _context.Engines.Any(e => e.Name == name);
        }

        public Engine Get(string name)
        {
            return _context.Engines.First(e => e.Name == name);
        }

        public IList<EngineOption> GetAllOptions(string name)
        {
            return _context.Engines.First(e => e.Name == name).Options;
        }

        public EngineOption GetOption(string engine, string option)
        {
            return _context.Engines.First(e => e.Name == engine).Options.First(o => o.Name == option);
        }

        public void Replace(Engine engine)
        {
            _context.Engines.Remove(_context.Engines.First(e => e.Name == engine.Name));
            _context.Engines.Add(engine);
            _context.SaveChanges();
        }

        public void SetOption(string engine, string option, dynamic value)
        {
            _context.Engines.First(e => e.Name == engine).Options.First(o => o.Name == option).Value = value;
            _context.SaveChanges();
        }
    }
}
