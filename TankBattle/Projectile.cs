using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TankBattle
{
    public class Projectile : WeaponEffect
    {
        // Private variables
        private float xPos;
        private float yPos;
        private float graV;
        private Shrapnel exP;
        private TankController tankPlayer;

        private float xVel;
        private float yVel;

        public Projectile(float x, float y, float angle, float power, float gravity, Shrapnel explosion, TankController player)
        {
            // Initialising the projectile properties
            xPos = x;
            yPos = y;
            graV = gravity;
            exP = explosion;
            tankPlayer = player;

            float angleRadians = (90 - angle) * (float)Math.PI / 180;
            float magnitude = power / 50;

            xVel = (float)Math.Cos(angleRadians) * magnitude;
            yVel = (float)Math.Sin(angleRadians) * -magnitude;

        }

        public override void Step()
        {
            // Moving the projectile
            for (int i = 0; i < 10; i++)
            {
                xPos += xVel;
                yPos += yVel;

                xPos += gcGame.WindSpeed() / 1000.0f;

                if (xPos < 0 || xPos > Battlefield.WIDTH || yPos > Battlefield.HEIGHT/* || yPos <= 0*/)
                {
                    gcGame.EndEffect(this);
                    return;
                } else if (yPos <= 0)
                {
                    yVel += graV;
                } else if (gcGame.CheckHitTank(xPos, yPos) && !(yPos <= 0))
                {
                    tankPlayer.ProjectileHit(xPos, yPos);
                    exP.Explode(xPos, yPos);
                    gcGame.AddEffect(exP);
                    gcGame.EndEffect(this);
                    return;
                }

                yVel += graV;

            }
        }

        public override void Draw(Graphics graphics, Size size)
        {
            // Drawing the projectile
            float x = (float)this.xPos * size.Width / Battlefield.WIDTH;
            float y = (float)this.yPos * size.Height / Battlefield.HEIGHT;
            float s = size.Width / Battlefield.WIDTH;

            RectangleF r = new RectangleF(x - s / 2.0f, y - s / 2.0f, s, s);
            Brush b = new SolidBrush(Color.WhiteSmoke);

            graphics.FillEllipse(b, r);

        }
    }
}
