using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankBattle
{
    public class Shrapnel : WeaponEffect
    {
        // Private variables
        private int expDmg;
        private int expRad;
        private int destrRad;

        private float shrapX;
        private float shrapY;
        private float shrapLife;

        public Shrapnel(int explosionDamage, int explosionRadius, int earthDestructionRadius)
        {
            // Initialising the explosion damage, radius, and the destruction radius fields
            expDmg = explosionDamage;
            expRad = explosionRadius;
            destrRad = earthDestructionRadius;

        }

        public void Explode(float x, float y)
        {
            // Detonating the shrapnel at the given coordinates
            shrapX = x;
            shrapY = y;

            shrapLife = 1.0f;
        }

        public override void Step()
        {

            shrapLife -= 0.05f;

            if (shrapLife <= 0)
            {
                gcGame.DamagePlayer(shrapX, shrapY, expDmg, expRad);
                gcGame.GetArena().DestroyGround(shrapX, shrapY, destrRad);
                gcGame.EndEffect(this);
            }
        }

        public override void Draw(Graphics graphics, Size displaySize)
        {
            // Drawing a frame of the shrapnel
            float x = (float)this.shrapX * displaySize.Width / Battlefield.WIDTH;
            float y = (float)this.shrapY * displaySize.Height / Battlefield.HEIGHT;
            float radius = displaySize.Width * (float)((1.0 - shrapLife) * expRad * 3.0 / 2.0) / Battlefield.WIDTH;

            int alpha = 0, red = 0, green = 0, blue = 0;

            if (shrapLife < 1.0 / 3.0)
            {
                red = 255;
                alpha = (int)(shrapLife * 3.0 * 255);
            }
            else if (shrapLife < 2.0 / 3.0)
            {
                red = 255;
                alpha = 255;
                green = (int)((shrapLife * 3.0 - 1.0) * 255);
            }
            else
            {
                red = 255;
                alpha = 255;
                green = 255;
                blue = (int)((shrapLife * 3.0 - 2.0) * 255);
            }

            RectangleF rect = new RectangleF(x - radius, y - radius, radius * 2, radius * 2);
            Brush b = new SolidBrush(Color.FromArgb(alpha, red, green, blue));

            graphics.FillEllipse(b, rect);

        }
    }
}
