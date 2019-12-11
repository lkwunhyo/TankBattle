using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankBattle
{
    public partial class TitleForm : Form
    {
        public TitleForm()
        {
            InitializeComponent();
        }

        private void newGameButton_Click(object sender, EventArgs e)
        {
            GameController game = new GameController(2, 1);
            TankController player1 = new PlayerController("Player 1", Chassis.CreateTank(1), GameController.GetTankColour(1));
            TankController player2 = new AIOpponent("Player 2", Chassis.CreateTank(1), GameController.GetTankColour(2));
            TankController player3 = new AIOpponent("Player 3", Chassis.CreateTank(1), GameController.GetTankColour(3));
            TankController player4 = new AIOpponent("Player 4", Chassis.CreateTank(1), GameController.GetTankColour(4));
            TankController player5 = new AIOpponent("Player 5", Chassis.CreateTank(1), GameController.GetTankColour(5));
            TankController player6 = new AIOpponent("Player 6", Chassis.CreateTank(1), GameController.GetTankColour(6));
            TankController player7 = new AIOpponent("Player 7", Chassis.CreateTank(1), GameController.GetTankColour(7));
            TankController player8 = new AIOpponent("Player 8", Chassis.CreateTank(1), GameController.GetTankColour(8));

            game.SetPlayer(1, player1);
            game.SetPlayer(2, player2);
            //game.SetPlayer(3, player3);
            //game.SetPlayer(4, player4);
            //game.SetPlayer(5, player5);
            //game.SetPlayer(6, player6);
            //game.SetPlayer(7, player7);
            //game.SetPlayer(8, player8);
            game.BeginGame();
        }
    }
}
