using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TankBattle
{
    public abstract class WeaponEffect
    {
        // Private variables
        protected GameController gcGame;

        public void RecordCurrentGame(GameController game)
        {
            // Setting the GameController field to game
            gcGame = game;
        }

        public abstract void Step();
        public abstract void Draw(Graphics graphics, Size displaySize);
    }
}
