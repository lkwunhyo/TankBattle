using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankBattle
{
    

    public class StandardTank : Chassis
    {

        public override int[,] DrawTankSprite(float angle)
        {
            // Drawing the player tank in a 2D array
            int[,] graphic = { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                               { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                               { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                               { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                               { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                               { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                               { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                               { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
                               { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 },
                               { 0, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 0, 0, 0 },
                               { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0 },
                               { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
            };

            // Using trigonometry to calculate the angle positions
            double radians, distX, distY;

            if (angle < 0)
            {
                radians = (90 - angle) * (Math.PI / 180);
                distX = Math.Cos(radians);
                distY = Math.Sin(radians);

                CreateLine(graphic, 6, 7, 6 - (int)(distY * 5), 7 + (int)(distX * 7));

            } else if (angle > 0)
            {
                radians = (90 - angle) * (Math.PI / 180);
                distX = Math.Cos(radians);
                distY = Math.Sin(radians);

                CreateLine(graphic, 6, 7, 6 - (int)(distY * 5), 7 + (int)(distX * 7));

            } else
            {
                CreateLine(graphic, 6, 7, 1, 7);
            }

            // Leaving the array edges blank
            for (int y = 0; y < 12; y++)
            {
                graphic[y, 0] = 0;
                graphic[y, 15] = 0;
            }

            return graphic;
        }

        public override void FireWeapon(int weapon, PlayerTank playerTank, GameController currentGame)
        {
            // Firing the specified weapon from playerTank
            float xPos = playerTank.GetX();
            float yPos = playerTank.GetYPos();

            xPos += Chassis.WIDTH / 2;
            yPos += Chassis.HEIGHT / 2;

            TankController tankContr = playerTank.GetPlayerById();

            Shrapnel newShrapnel = new Shrapnel(100, 4, 4);

            Projectile newProj = new Projectile(xPos, yPos, playerTank.GetPlayerAngle(), playerTank.GetTankPower(), 0.01f, newShrapnel, tankContr);

            currentGame.AddEffect(newProj);

        }

        public override int GetHealth()
        {
            // Initialising the health of the standard tank.
            return 100;

        }

        public override string[] WeaponList()
        {
            // Returning a string name of the type of ammo.
            return new string[] { "Standard shell" };

        }
    }

    public abstract class Chassis 
    {
        public const int WIDTH = 4;
        public const int HEIGHT = 3;
        public const int NUM_TANKS = 1;

        public abstract int[,] DrawTankSprite(float angle);

        public static void CreateLine(int[,] graphic, int X1, int Y1, int X2, int Y2)
        {
            // Implementation of Bresenham's Line Algorithm.
            int w = X2 - X1;
            int h = Y2 - Y1;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                graphic[X1, Y1] = 1;
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    X1 += dx1;
                    Y1 += dy1;
                }
                else
                {
                    X1 += dx2;
                    Y1 += dy2;
                }
            }

        }

        public Bitmap CreateTankBMP(Color tankColour, float angle)
        {
            int[,] tankGraphic = DrawTankSprite(angle);
            int height = tankGraphic.GetLength(0);
            int width = tankGraphic.GetLength(1);

            Bitmap bmp = new Bitmap(width, height);
            Color transparent = Color.FromArgb(0, 0, 0, 0);
            Color tankOutline = Color.FromArgb(255, 0, 0, 0);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (tankGraphic[y, x] == 0)
                    {
                        bmp.SetPixel(x, y, transparent);
                    }
                    else
                    {
                        bmp.SetPixel(x, y, tankColour);
                    }
                }
            }

            // Outline each pixel
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    if (tankGraphic[y, x] != 0)
                    {
                        if (tankGraphic[y - 1, x] == 0)
                            bmp.SetPixel(x, y - 1, tankOutline);
                        if (tankGraphic[y + 1, x] == 0)
                            bmp.SetPixel(x, y + 1, tankOutline);
                        if (tankGraphic[y, x - 1] == 0)
                            bmp.SetPixel(x - 1, y, tankOutline);
                        if (tankGraphic[y, x + 1] == 0)
                            bmp.SetPixel(x + 1, y, tankOutline);
                    }
                }
            }

            return bmp;
        }

        public abstract int GetHealth();

        public abstract string[] WeaponList();

        public abstract void FireWeapon(int weapon, PlayerTank playerTank, GameController currentGame);

        public static Chassis CreateTank(int tankNumber)
        {
            // Returning the method created above.
            // (Has no given stats yet)
            return new StandardTank();


        }
    }
}
