using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace TankBattle
{
    public class GameController
    {
        // Private variables
        private TankController[] playersArray;
        private int roundsPlayed;
        private int playersCount;
        private int currentRound;

        private int tankStart;

        private Battlefield tankBattlefield;
        private PlayerTank[] playerTankArray;


        private int currentPlayer;

        private int windSpeed;

        List<WeaponEffect> weaponEffects;

        GameplayForm newForm;


        public GameController(int numPlayers, int numRounds)
        {
            // Assigning the number of players to a variable.
            playersCount = numPlayers;

            // Creating an array containing the number of player from 2 - 8.
            playersArray = new TankController[numPlayers];

            // Creating a variable storing the number of rounds which will be
            // played.
            roundsPlayed = numRounds;

            // Creating a list to contain weapon effects.
            weaponEffects = new List<WeaponEffect>();
            

        }

        public int TotalPlayers()
        {
            // Return the number of players in the game.
            return playersCount;

        }

        public int CurrentRound()
        {
            // Return the current round variable initialised in BeginGame().
            return currentRound;

            //throw new NotImplementedException();
        }

        public int GetMaxRounds()
        {
            // Return the number of rounds which will be played in the game.
            return roundsPlayed;
        }

        public void SetPlayer(int playerNum, TankController player)
        {
            // Setting appropriate field in TankController array to player
            playersArray[playerNum - 1] = player;
            
        }

        public TankController GetPlayerById(int playerNum)
        {
            // Returning the player ID
            return playersArray[playerNum - 1];
        }

        public PlayerTank GetGameplayTank(int playerNum)
        {
            // Returning the Tank associated with the playerNum
            return playerTankArray[playerNum - 1];
        }

        public static Color GetTankColour(int playerNum)
        {
            // Distributing a color to each potential playerNum.
            
            if (playerNum == 1)
            {
                return Color.Red;
            } else if (playerNum == 2)
            {
                return Color.LightGreen;
            } else if (playerNum == 3)
            {
                return Color.Cyan;
            } else if (playerNum == 4)
            {
                return Color.Orange;
            } else if (playerNum == 5)
            {
                return Color.MediumPurple;
            } else if (playerNum == 6)
            {
                return Color.Yellow;
            } else if (playerNum == 7)
            {
                return Color.White;
            } else
            {
                return Color.DeepPink;
            }
            
            
        }

        public static int[] GetPlayerPositions(int numPlayers)
        {
            // Initiating tank positions and the array
            int[] tankPosition = new int[numPlayers];
            int tankA;
            int tankB;
            int tankC;
            int tankD;
            int tankE;
            int tankF;
            int tankG;
            int tankH;
            
            if (numPlayers == 2)
            {
                tankA = Battlefield.WIDTH / 4;
                tankB = tankA + Battlefield.WIDTH / 2;
                tankPosition = new int[] { tankA, tankB };
            } else if (numPlayers == 3)
            {
                tankA = Battlefield.WIDTH / 6;
                tankB = tankA + Battlefield.WIDTH / 3;
                tankC = tankB + Battlefield.WIDTH / 3;
                tankPosition = new int[] { tankA, tankB, tankC };
            } else if (numPlayers == 4)
            {
                tankA = Battlefield.WIDTH / 8;
                tankB = tankA + Battlefield.WIDTH / 4;
                tankC = tankB + Battlefield.WIDTH / 4;
                tankD = tankC + Battlefield.WIDTH / 4;
                tankPosition = new int[] { tankA, tankB, tankC, tankD };
            } else if (numPlayers == 5)
            {
                tankA = Battlefield.WIDTH / 10;
                tankB = tankA + Battlefield.WIDTH / 5;
                tankC = tankB + Battlefield.WIDTH / 5;
                tankD = tankC + Battlefield.WIDTH / 5;
                tankE = tankD + Battlefield.WIDTH / 5;
                tankPosition = new int[] { tankA, tankB, tankC, tankD, tankE };
            } else if (numPlayers == 6)
            {
                tankA = Battlefield.WIDTH / 12;
                tankB = tankA + Battlefield.WIDTH / 6;
                tankC = tankB + Battlefield.WIDTH / 6;
                tankD = tankC + Battlefield.WIDTH / 6;
                tankE = tankD + Battlefield.WIDTH / 6;
                tankF = tankE + Battlefield.WIDTH / 6;
                tankPosition = new int[] { tankA, tankB, tankC, tankD, tankE, tankF };
            } else if (numPlayers == 7)
            {
                tankA = Battlefield.WIDTH / 14;
                tankB = tankA + Battlefield.WIDTH / 7;
                tankC = tankB + Battlefield.WIDTH / 7;
                tankD = tankC + Battlefield.WIDTH / 7;
                tankE = tankD + Battlefield.WIDTH / 7;
                tankF = tankE + Battlefield.WIDTH / 7;
                tankG = tankF + Battlefield.WIDTH / 7;
                tankPosition = new int[] { tankA, tankB, tankC, tankD, tankE, tankF, tankG };
            } else if (numPlayers == 8)
            {
                tankA = Battlefield.WIDTH / 16;
                tankB = tankA + Battlefield.WIDTH / 8;
                tankC = tankB + Battlefield.WIDTH / 8;
                tankD = tankC + Battlefield.WIDTH / 8;
                tankE = tankD + Battlefield.WIDTH / 8;
                tankF = tankE + Battlefield.WIDTH / 8;
                tankG = tankF + Battlefield.WIDTH / 8;
                tankH = tankG + Battlefield.WIDTH / 8;
                tankPosition = new int[] { tankA, tankB, tankC, tankD, tankE, tankF, tankG, tankH };
            }

            // Returning the tank positions in an array
            return tankPosition;
        }

        public static void Shuffle(int[] array)
        {
            // Implementing the Random method.
            Random rand = new Random();

            // Standard shuffling algorithm
            for (int i = 0; i < array.Length; i++)
            {
                int temp = array[i];
                int r = rand.Next(i, array.Length);
                array[i] = array[r];
                array[r] = temp;
            }
            
        }

        public void BeginGame()
        {
            // Initialising variables to begin the game
            currentRound = 1;
            tankStart = 0;
            CommenceRound();

        }

        public void CommenceRound()
        {
            // Initialising variables to begin a round
            currentPlayer = tankStart;
            tankBattlefield = new Battlefield();

            int[] tankConstPositions = GetPlayerPositions(playersCount);

            foreach (TankController x in playersArray)
            {
                x.BeginRound();
            }

            // Shuffling the tank position array to randomise spawn
            Shuffle(tankConstPositions);

            playerTankArray = new PlayerTank[playersCount];

            
            for (int i = 0; i < playersCount; i++)
            {
                playerTankArray[i] = new PlayerTank(playersArray[i], tankConstPositions[i], tankBattlefield.TankVerticalPosition(tankConstPositions[i]), this);
            }

            // Initiating wind speed
            Random rand = new Random();
            windSpeed = rand.Next(-100, 100);

            //GameplayForm newForm = new GameplayForm(this);
            newForm = new GameplayForm(this);


            newForm.Show();

        }

        public Battlefield GetArena()
        {
            // Returning the battlefield
            return tankBattlefield;
        }

        public void DisplayPlayerTanks(Graphics graphics, Size displaySize)
        {
            // Displaying tanks that exist
            foreach(PlayerTank x in playerTankArray)
            {
                if(x.Exists())
                {
                    x.Draw(graphics, displaySize);
                }
            }
        }

        public PlayerTank GetPlayerTank()
        {
            // Returning the current PlayerTank
            return playerTankArray[currentPlayer];
        }

        public void AddEffect(WeaponEffect weaponEffect)
        {
            // Adding weapon effects
            weaponEffects.Add(weaponEffect);
            weaponEffect.RecordCurrentGame(this);

        }

        public bool WeaponEffectTick()
        {
            // Checking if weapon effect animations are still ongoing
            foreach(WeaponEffect x in weaponEffects)
            {
                x.Step();
                return true;
            }
            return false;
        }

        public void DrawWeaponEffects(Graphics graphics, Size displaySize)
        {
            // Drawing weapon effects
            foreach(WeaponEffect x in weaponEffects)
            {
                x.Draw(graphics, displaySize);
            }
        }

        public void EndEffect(WeaponEffect weaponEffect)
        {
            // Removing the referenced WeaponEffect
            weaponEffects.Remove(weaponEffect);
        }

        public bool CheckHitTank(float projectileX, float projectileY)
        {
            // Checking for projectile collision
            if (projectileX < 0 || projectileX > Battlefield.WIDTH || projectileY > Battlefield.HEIGHT)
            {
                return false;
            }
            if (tankBattlefield.TileAt((int)projectileX, (int)projectileY))
            {
                return true;
            }
            
            foreach(PlayerTank x in playerTankArray)
            {
                if (x != playerTankArray[currentPlayer])
                {
                    int xPos = x.GetX();
                    int yPos = x.GetYPos();

                    if (projectileX > xPos && projectileX < xPos + Chassis.WIDTH &&
                        projectileY > yPos && projectileY < yPos + Chassis.HEIGHT)
                    {
                        if (x.Exists())
                        {
                            return true;
                        }
                    }
                }

            }

            return false;
        }

        public void DamagePlayer(float damageX, float damageY, float explosionDamage, float radius)
        {
            // Inflicting damage to any tanks within the explosion radius            
            foreach(PlayerTank x in playerTankArray)
            {
                if (x.Exists())
                {
                    float xPos = x.GetX() + Chassis.WIDTH / 2;
                    float yPos = x.GetYPos() + Chassis.HEIGHT / 2;

                    float dist = (float)Math.Sqrt(Math.Pow(xPos - damageX, 2) + Math.Pow(yPos - damageY, 2));
                    float damage;

                    if (dist > radius)
                    {
                        damage = 0;
                    } else if (dist < radius && dist > radius / 2)
                    {
                        damage = (explosionDamage * (radius - dist)) / radius;
                    } else if (dist < radius / 2)
                    {
                        damage = explosionDamage;
                    } else
                    {
                        damage = 0;
                    }

                    x.DamagePlayer((int)damage);
                }
            }
        }

        public bool CalculateGravity()
        {
            // Checks for hanging battlefield pixels or tanks
            bool temp = false;

            if (tankBattlefield.CalculateGravity())
            {
                temp = true;
            }

            foreach(PlayerTank x in playerTankArray)
            {
                if (x.CalculateGravity())
                {
                    temp = true;
                }
            }

            return temp;
        }

        public bool TurnOver()
        {
            
            // Checking how many players still exist
            int count = 0;
            for (int i = 0; i < playerTankArray.Length; i++)
            {
                if (playerTankArray[i].Exists())
                {
                    count++;
                }
            }

            // When there are 2 or more players in the battlefield
            if (count >= 2)
            {
                
                currentPlayer++;

                if (currentPlayer >= TotalPlayers())
                {
                    currentPlayer = 0;
                }

                // If the player is dead, skip to the next player
                while (!playerTankArray[currentPlayer].Exists())
                {
                    currentPlayer++;

                    if (currentPlayer >= TotalPlayers())
                    {
                        currentPlayer = 0;
                    }
                }

                // Setting random wind speed
                Random rand = new Random();
                windSpeed += rand.Next(-10, 10);

                if (windSpeed < -100)
                {
                    windSpeed = -100;
                }
                else if (windSpeed > 100)
                {
                    windSpeed = 100;
                }
                return true;

            } else
            {
                // Ending the game
                ScoreWinner();
                return false;
            }
            

        }

        public void ScoreWinner()
        {
            // Increasing the player's rounds won
            foreach(PlayerTank x in playerTankArray)
            {
                if(x.Exists())
                {
                    x.GetPlayerById().AddScore();
                }
            }
        }

        public void NextRound()
        {
            // Deciding if the round is over
            currentRound++;
            if (currentRound <= GetMaxRounds())
            {
                tankStart++;
            }
            if (tankStart == playersCount)
            {
                tankStart = 0;
            }
            CommenceRound();

            if (currentRound > GetMaxRounds())
            {
                // Showing another titleform after the max rounds
                newForm.Close();
                TitleForm titleForm = new TitleForm();
                titleForm.Show();

            }
        }
        
        public int WindSpeed()
        {
            // Returning the wind speed
            return windSpeed;
        }
    }
}
