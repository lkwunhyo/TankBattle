using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankBattle
{
    public partial class GameplayForm : Form
    {
        private Color landscapeColour;
        private Random rng = new Random();
        private Image backgroundImage = null;
        private int levelWidth = 160;
        private int levelHeight = 120;
        private GameController currentGame;

        private BufferedGraphics backgroundGraphics;
        private BufferedGraphics gameplayGraphics;

        int debounce;

        public GameplayForm(GameController game)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.UserPaint, true);

            currentGame = game;

            string[] imageFilenames = { "Images\\background1.jpg",
                            "Images\\background2.jpg",
                            "Images\\background3.jpg",
                            "Images\\background4.jpg"};
            Color[] landscapeColours = { Color.FromArgb(255, 0, 0, 0),
                             Color.FromArgb(255, 73, 58, 47),
                             Color.FromArgb(255, 148, 116, 93),
                             Color.FromArgb(255, 133, 119, 109) };

            Random rand = new Random();
            int r = rand.Next(0, 3);
            backgroundImage = Image.FromFile(imageFilenames[r]);
            landscapeColour = landscapeColours[r];

            InitializeComponent();

            backgroundGraphics = InitBuffer();
            gameplayGraphics = InitBuffer();

            DrawBackground();

            DrawGameplay();

            NewTurn();
        }

        // From https://stackoverflow.com/questions/13999781/tearing-in-my-animation-on-winforms-c-sharp
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }

        public void EnableHumanControl()
        {
            //throw new NotImplementedException();
            controlPanel.Enabled = true;
        }

        public void AimTurret(float angle)
        {
            //throw new NotImplementedException();
            numericUpDown1.Value = (decimal)angle;
            
        }

        public void SetPower(int power)
        {
            //throw new NotImplementedException();
            trackBar1.Value = power;
        }
        public void SetWeaponIndex(int weapon)
        {
            //throw new NotImplementedException();
            comboBox1.SelectedIndex = weapon;
            
        }

        public void Fire()
        {
            //throw new NotImplementedException();
            currentGame.GetPlayerTank().Fire();
            controlPanel.Enabled = false;
            timer1.Enabled = true;
        }

        private void DrawGameplay()
        {
            //throw new NotImplementedException();
            backgroundGraphics.Render(gameplayGraphics.Graphics);
            currentGame.DisplayPlayerTanks(gameplayGraphics.Graphics, displayPanel.Size);
            currentGame.DrawWeaponEffects(gameplayGraphics.Graphics, displayPanel.Size);


        }

        private void NewTurn()
        {
            //throw new NotImplementedException();
            //currentGame.GetPlayerTank().GetPlayerById();
            PlayerTank currentTankPlayer = currentGame.GetPlayerTank();
            TankController currentId = currentTankPlayer.GetPlayerById();

            // Setting Form Title Caption to Current Round 
            this.Text = "Tank Battle - Round " + currentGame.CurrentRound() + " of " + currentGame.GetMaxRounds();

            // Set back color of control panel
            controlPanel.BackColor = currentId.GetTankColour();
            // Set player name to tank name
            label1.Text = currentId.Identifier();
            // Call AimTurret() to set current angle
            AimTurret(currentTankPlayer.GetPlayerAngle());
            // Call SetPower() to set current power
            SetPower(currentTankPlayer.GetTankPower());

            label7.Text = trackBar1.Value.ToString();

            // Updating wind label
            if (currentGame.WindSpeed() < 0)
            {
                label3.Text = currentGame.WindSpeed() + "W";
            } else
            {
                label3.Text = currentGame.WindSpeed() + "E";
            }

            // Clearing Combobox
            comboBox1.Items.Clear();

            // Adding to the Combobox
            foreach (string x in currentTankPlayer.CreateTank().WeaponList())
            {
                comboBox1.Items.Add(x);
            }

            // Setting the current weapon to the current player
            SetWeaponIndex(currentTankPlayer.GetCurrentWeapon());

            // Calling BeginTurn()
            currentId.BeginTurn(this, currentGame);

            /*
            button1.TabStop = false;
            comboBox1.TabStop = false;
            numericUpDown1.TabStop = false;
            trackBar1.TabStop = false;*/

            label7.Text = trackBar1.Value.ToString();

            debounce = 0;

            
        }
        
        private void DrawBackground()
        {
            Graphics graphics = backgroundGraphics.Graphics;
            Image background = backgroundImage;
            graphics.DrawImage(backgroundImage, new Rectangle(0, 0, displayPanel.Width, displayPanel.Height));

            Battlefield battlefield = currentGame.GetArena();
            Brush brush = new SolidBrush(landscapeColour);

            for (int y = 0; y < Battlefield.HEIGHT; y++)
            {
                for (int x = 0; x < Battlefield.WIDTH; x++)
                {
                    if (battlefield.TileAt(x, y))
                    {
                        int drawX1 = displayPanel.Width * x / levelWidth;
                        int drawY1 = displayPanel.Height * y / levelHeight;
                        int drawX2 = displayPanel.Width * (x + 1) / levelWidth;
                        int drawY2 = displayPanel.Height * (y + 1) / levelHeight;
                        graphics.FillRectangle(brush, drawX1, drawY1, drawX2 - drawX1, drawY2 - drawY1);
                    }
                }
            }
        }

        public BufferedGraphics InitBuffer()
        {
            BufferedGraphicsContext context = BufferedGraphicsManager.Current;
            Graphics graphics = displayPanel.CreateGraphics();
            Rectangle dimensions = new Rectangle(0, 0, displayPanel.Width, displayPanel.Height);
            BufferedGraphics bufferedGraphics = context.Allocate(graphics, dimensions);
            return bufferedGraphics;
        }

        private void displayPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = displayPanel.CreateGraphics();
            gameplayGraphics.Render(graphics);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            PlayerTank currentTankPlayer = currentGame.GetPlayerTank();
            currentTankPlayer.AimTurret((float)numericUpDown1.Value);

            DrawGameplay();
            
            displayPanel.Invalidate();
        } 

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            PlayerTank currentTankPlayer = currentGame.GetPlayerTank();
            currentTankPlayer.SetPower(trackBar1.Value);
            label7.Text = trackBar1.Value.ToString();

            displayPanel.Invalidate();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlayerTank currentTankPlayer = currentGame.GetPlayerTank();
            currentTankPlayer.SetWeaponIndex(currentTankPlayer.GetCurrentWeapon());

            displayPanel.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!currentGame.WeaponEffectTick())
            {
                currentGame.CalculateGravity();
                DrawBackground();
                DrawGameplay();
                displayPanel.Invalidate();
                if (currentGame.CalculateGravity())
                {
                    return;
                }
                else
                {
                    timer1.Enabled = false;
                    if (currentGame.TurnOver())
                    {
                        NewTurn();
                    }
                    else
                    {
                        Dispose();
                        currentGame.NextRound();
                        return;
                    }

                }
            }
            else
            {
                DrawGameplay();
                displayPanel.Invalidate();
                return;
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Fire();
            this.Focus();

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void GameplayForm_Load(object sender, EventArgs e)
        {
            button1.TabStop = false;
            comboBox1.TabStop = false;
            numericUpDown1.TabStop = false;
            trackBar1.TabStop = false;
        }

        private void GameplayForm_KeyDown(object sender, KeyEventArgs e)
        {
            PlayerTank currentTankPlayer = currentGame.GetPlayerTank();
            
            // Left Key
            if (e.KeyCode == Keys.Left)
            {
                if (numericUpDown1.Value > -90)
                {
                    numericUpDown1.Value--;
                }
                currentTankPlayer.AimTurret((float)numericUpDown1.Value);
                
            }
            // Right Key
            else if (e.KeyCode == Keys.Right)
            {
                if (numericUpDown1.Value < 90)
                {
                    numericUpDown1.Value++;
                }
                currentTankPlayer.AimTurret((float)numericUpDown1.Value);

            }
            // Up Key
            else if (e.KeyCode == Keys.Up)
            {
                if (trackBar1.Value < 100)
                {
                    trackBar1.Value++;
                    label7.Text = trackBar1.Value.ToString();
                }
                currentTankPlayer.SetPower(trackBar1.Value);

            }
            //Down Key
            else if (e.KeyCode == Keys.Down)
            {
                if (trackBar1.Value > 5)
                {
                    trackBar1.Value--;
                    label7.Text = trackBar1.Value.ToString();
                }
                currentTankPlayer.SetPower(trackBar1.Value);

            }

            DrawGameplay();
            displayPanel.Invalidate();
        }
        

        private void GameplayForm_KeyPress(object sender, KeyPressEventArgs e)
        {
           
            if (e.KeyChar == ' ')
            {
                debounce++;
                if (debounce == 1 && controlPanel.Enabled == true)
                {
                    Fire();
                    controlPanel.Enabled = false;
                }
            }
        }
    }
}
