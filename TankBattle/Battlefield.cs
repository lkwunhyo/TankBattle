using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankBattle
{
    
    public class Battlefield
    {
        public const int WIDTH = 160;
        public const int HEIGHT = 120;

        private bool[,] battlefieldArray = new bool[WIDTH, HEIGHT];

        public Battlefield()
        {
            // Setting the entire array to true
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    battlefieldArray[x, y] = true;
                }
            }
            
            // Initialising random number generator
            Random rand = new Random();
            int yinitial = rand.Next(30, HEIGHT - 10);

            // Generating the random terrain
            for (int xcor = 0; xcor < WIDTH; xcor++)
            {
                int shuffle = rand.Next(-1, 2);

                if (shuffle == 0)
                {
                    yinitial = yinitial;
                    battlefieldArray[xcor, yinitial] = false;
                }
                else if (shuffle == -1)
                {
                    yinitial -= 1;

                    if (yinitial == 5)
                    {
                        yinitial += 1;
                    }

                    battlefieldArray[xcor, yinitial] = false;
                }
                else if (shuffle == 1)
                {
                    yinitial += 1;

                    if (yinitial == HEIGHT - 3)
                    {
                        yinitial -= 1;
                    }

                    battlefieldArray[xcor, yinitial] = false;
                }
            }

            for (int xFill = 0; xFill < WIDTH; xFill++)
            {
                for (int yFill = 0; yFill < HEIGHT; yFill++)
                {
                    if (TileAt(xFill, yFill))
                    {
                        battlefieldArray[xFill, yFill] = false;
                    }
                    else if (!TileAt(xFill, yFill))
                    {
                        break;
                    }
                }
            }

        }

        public bool TileAt(int x, int y)
        {
            // Returning true where position given to the battlefielArray
            // is true.
            
            if (battlefieldArray[x, y])
            {
                return true;
            } else
            {
                return false;
            }

        }

        public bool CheckTankCollide(int x, int y)
        {
            // Returning if there is room for a tank
            if (TileAt(x, y + 2) || TileAt(x + 1, y + 2) ||
                TileAt(x + 2, y + 2) || TileAt(x + 3, y + 2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int TankVerticalPosition(int x)
        {
            // Calculating the lowest Y coordinate at the given X coordinate
            
            for (int yPos = 0; yPos < HEIGHT; yPos++)
            {
                if (CheckTankCollide(x, yPos)) {
                    return yPos - 1;
                }
            }

            return 0;
            

        }

        public void DestroyGround(float destroyX, float destroyY, float radius)
        {
            // Destroying all terrain within a circle around destroyX and destroyY
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    // Equation for circle
                    if (((x - destroyX) * (x - destroyX) + (y - destroyY) * (y - destroyY)) <= radius * radius)
                    {
                        battlefieldArray[x, y] = false;
                    }
                }
            }

        }

        public bool CalculateGravity()
        {
            // Moving any loose terrain down a single tile
            bool value = false;

            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = HEIGHT - 1; y > 0; y--)
                {
                    if (!TileAt(x, y))
                    {
                        // Switch tile positions
                        if (TileAt(x, y - 1))
                        {
                            bool temp;
                            temp = battlefieldArray[x, y];
                            battlefieldArray[x, y] = battlefieldArray[x, y - 1];
                            battlefieldArray[x, y - 1] = temp;
                            value = true;
                        }
                    }
                }
            }

            return value;
        }
    }
}
