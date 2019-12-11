using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankBattle
{
    abstract public class TankController
    {
        // Private variables
        private string tankName;
        private Chassis chassisTank;
        private Color tankColour;
        private int roundsWon;



        public TankController(string name, Chassis tank, Color colour)
        {
            // Initialising the TankController name, chassis, and colour
            tankName = name;
            chassisTank = tank;
            tankColour = colour;
            roundsWon = 0;
    }
        public Chassis CreateTank()
        {
            // Returning the Chassis associated with this TankController
            return chassisTank;
        }
        public string Identifier()
        {
            // Returning the TankController's name
            return tankName;
        }
        public Color GetTankColour()
        {
            // Returning the TankController's colour
            return tankColour;
        }
        public void AddScore()
        {
            // Incrementing the rounds won of the TankController
            roundsWon++;
        }
        public int GetVictories()
        {
            // Returning the number of rounds won by the TankController
            return roundsWon;
        }

        public abstract void BeginRound();

        public abstract void BeginTurn(GameplayForm gameplayForm, GameController currentGame);

        public abstract void ProjectileHit(float x, float y);
    }
}
