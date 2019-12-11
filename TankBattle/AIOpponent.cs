using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankBattle
{
    public class AIOpponent : TankController
    {
        

        public AIOpponent(string name, Chassis tank, Color colour) : base(name, tank, colour)
        {
            // AIOpponent constructor is handled in TankController
        }

        public override void BeginRound()
        {
            // Called every round
        }

        public override void BeginTurn(GameplayForm gameplayForm, GameController currentGame)
        {
            // Initiating random function
            Random rand = new Random();

            PlayerTank currentTankPlayer = currentGame.GetPlayerTank();
            TankController currentId = currentTankPlayer.GetPlayerById();

            int posX, posY;
            int[] playerPositions;
            int numX;

            posX = currentTankPlayer.GetX();
            posY = currentTankPlayer.GetYPos();

            // Initiating list for easier removal of players (!Exist())
            playerPositions = GameController.GetPlayerPositions(currentGame.TotalPlayers());
            List<int> playerPos = new List<int>(playerPositions);

            // Removing players that don't exist
            for (int i = 0; i < playerPos.Count; i++)
            {
                if (!currentGame.GetGameplayTank(i + 1).Exists())
                {
                    playerPos.Remove(currentGame.GetGameplayTank(i+1).GetX());
                }
            }


            // Calculating the closest tank to the current PlayerTank
            int distance = Math.Abs(playerPos[0] - posX);
            int idx = 0;

            for (int i = 1; i < playerPos.Count; i++)
            {
                int cdistance = Math.Abs(playerPos[i] - posX);
                if (cdistance < distance && playerPos[i] != posX)
                {
                    idx = i;
                    distance = cdistance;
                }
            }

            numX = playerPos[idx];


            /*
             // Testing to see the tank position stuff 
            Console.WriteLine("Current Tank:");
            Console.WriteLine(posX);
            Console.WriteLine("Closest Tank Position:");
            Console.WriteLine(numX);
            Console.WriteLine("Tank Positions:");
            for (int i = 0; i < playerPos.Count; i++)
            {
                Console.WriteLine(playerPos[i]);
            }*/
            
            // Aiming the turret in the direction of the closest tank
            if (numX < posX)
            {
                gameplayForm.AimTurret(rand.Next(-90, -10));
            } else
            {
                gameplayForm.AimTurret(rand.Next(10, 90));
            }


            // Setting tank power to a random number
            gameplayForm.SetPower(rand.Next(5, 100));
            

            // Making the tank to fire
            if (currentTankPlayer.Exists())
            {
                gameplayForm.Fire();

            }


        }

        public override void ProjectileHit(float x, float y)
        {
            // Called when a projectile lands
        }
    }
}
