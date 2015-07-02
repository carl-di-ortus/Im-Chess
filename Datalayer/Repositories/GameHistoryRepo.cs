using System;
using System.Linq;
using Datalayer.Entities;

namespace Datalayer.Repositories
{
    public class GameHistoryRepo
    {
        private readonly ImChessModel _context;

        public GameHistoryRepo()
        {
            _context = ImChessModel.Instance;
        }

        public void AddGame(GameHistory game)
        {
            SwitchLastGame(game);
            _context.Games.Add(game);
            _context.SaveChanges();
        }

        public GameHistory GetGame(int id)
        {
            return _context.Games.First(g => g.Id == id);
        }

        public GameHistory GetLastGame()
        {
            return _context.Games.FirstOrDefault(g => g.LastGame);
        }

        public void UpdateGame(GameHistory game)
        {
            SwitchLastGame(game);
            var old = GetGame(game.Id);
            old.Notation = game.Notation;
            old.Pieces = game.Pieces;
            _context.SaveChanges();
        }

        private void SwitchLastGame(GameHistory game)
        {
            var last = GetLastGame();
            if (last != null)
            {
                last.LastGame = false;
            }
            game.LastGame = true;
        }
    }
}
