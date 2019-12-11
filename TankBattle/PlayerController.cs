using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TankBattle
{
    public class PlayerController : TankController
    {
        public PlayerController(string name, Chassis tank, Color colour) : base(name, tank, colour)
        {
            // PlayerController constructor is handled in TankController
        }

        public override void BeginRound()
        {
            // Called each round
        }

        public override void BeginTurn(GameplayForm gameplayForm, GameController currentGame)
        {
            // Enabling the Control Panel
            gameplayForm.EnableHumanControl();
        }

        public override void ProjectileHit(float x, float y)
        {
            // Called when a projectile lands
        }
    }
}
