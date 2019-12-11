using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankBattle
{
    public class PlayerTank
    {
        // Private variables
        private TankController tankPlayer;
        private int tankXPos;
        private int tankYPos;
        private GameController tankGame;
        private Chassis tank;
        private Bitmap tankBMP;


        private float tankAngle;
        private int tankPower;
        private int currentWeap;

        private int currentDurability;

        public PlayerTank(TankController player, int tankX, int tankY, GameController game)
        {
            // Initialising the player, tank coordinates, and tankGame fields
            tankPlayer = player;
            tankXPos = tankX;
            tankYPos = tankY;
            tankGame = game;

            tank = player.CreateTank();
            currentDurability = tank.GetHealth();

            tankAngle = 0;
            tankPower = 25;
            currentWeap = 0;

            tankBMP = tank.CreateTankBMP(tankPlayer.GetTankColour(), tankAngle);

        }

        public TankController GetPlayerById()
        {
            // Returning the TankController associated with the PlayerTank
            return tankPlayer;
        }
        public Chassis CreateTank()
        {
            // Returning ther Chassis associated with the PlayerTank
            return tank;
        }

        public float GetPlayerAngle()
        {
            // Returning the PlayerTank's current aiming angle
            return tankAngle;
        }

        public void AimTurret(float angle)
        {
            // Setting up the PlayerTank's current aiming angle
            tankAngle = angle;
            tankBMP = tank.CreateTankBMP(tankPlayer.GetTankColour(), tankAngle);
        }

        public int GetTankPower()
        {
            // Returning the current PlayerTank's power
            return tankPower;
        }

        public void SetPower(int power)
        {
            // Setting the current PlayerTank's power
            tankPower = power;
        }

        public int GetCurrentWeapon()
        {
            // Returning the current PlayerTank's weapon
            return currentWeap;
        }
        public void SetWeaponIndex(int newWeapon)
        {
            // Setting the current PlayerTank's weapon
            currentWeap = newWeapon;
        }

        public void Draw(Graphics graphics, Size displaySize)
        {
            // Drawing the PlayerTank to graphics
            int drawX1 = displaySize.Width * tankXPos / Battlefield.WIDTH;
            int drawY1 = displaySize.Height * tankYPos / Battlefield.HEIGHT;
            int drawX2 = displaySize.Width * (tankXPos + Chassis.WIDTH) / Battlefield.WIDTH;
            int drawY2 = displaySize.Height * (tankYPos + Chassis.HEIGHT) / Battlefield.HEIGHT;
            graphics.DrawImage(tankBMP, new Rectangle(drawX1, drawY1, drawX2 - drawX1, drawY2 - drawY1));

            int drawY3 = displaySize.Height * (tankYPos - Chassis.HEIGHT) / Battlefield.HEIGHT;
            Font font = new Font("Arial", 8);
            Brush brush = new SolidBrush(Color.White);

            int pct = currentDurability * 100 / 100;
            if (pct < 100)
            {
                graphics.DrawString(pct + "%", font, brush, new Point(drawX1, drawY3));
            }

        }

        public int GetX()
        {
            // Returning the X coordinate of the PlayerTank
            return tankXPos;
        }
        public int GetYPos()
        {
            // Returning the Y coordinate of the PlayerTank
            return tankYPos;
        }

        public void Fire()
        {
            // Setting the PlayerTank to fire
            CreateTank().FireWeapon(currentWeap, this, tankGame);
        }

        public void DamagePlayer(int damageAmount)
        {
            // Damaging the PlayerTank
            currentDurability -= damageAmount;
        }

        public bool Exists()
        {
            // Checking if the PlayerTank exists/alive
            if (currentDurability > 0)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public bool CalculateGravity()
        {
            // Setting the PlayerTank to take damage and fall if possible
            if (!this.Exists())
            {
                return false;
            }

            Battlefield tankCollField = tankGame.GetArena();
            if (tankCollField.CheckTankCollide(tankXPos, tankYPos + 1))
            {
                return false;
            } else
            {
                tankYPos++;
                currentDurability--;
            }

            if (tankYPos == Battlefield.HEIGHT - Chassis.HEIGHT)
            {
                currentDurability = 0;
            }
            return true;
        }
    }
}
