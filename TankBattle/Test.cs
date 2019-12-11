using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using TankBattle;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace TankBattleTestSuite
{
    class RequirementException : Exception
    {
        public RequirementException()
        {
        }

        public RequirementException(string message) : base(message)
        {
        }

        public RequirementException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    class Test
    {
        #region Testing Code

        private delegate bool TestCase();

        private static string ErrorDescription = null;

        private static void SetErrorDescription(string desc)
        {
            ErrorDescription = desc;
        }

        private static bool FloatEquals(float a, float b)
        {
            if (Math.Abs(a - b) < 0.01) return true;
            return false;
        }

        private static Dictionary<string, string> unitTestResults = new Dictionary<string, string>();

        private static void Passed(string name, string comment)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[passed] ");
            Console.ResetColor();
            Console.Write("{0}", name);
            if (comment != "")
            {
                Console.Write(": {0}", comment);
            }
            if (ErrorDescription != null)
            {
                throw new Exception("ErrorDescription found for passing test case");
            }
            Console.WriteLine();
        }
        private static void Failed(string name, string comment)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[failed] ");
            Console.ResetColor();
            Console.Write("{0}", name);
            if (comment != "")
            {
                Console.Write(": {0}", comment);
            }
            if (ErrorDescription != null)
            {
                Console.Write("\n{0}", ErrorDescription);
                ErrorDescription = null;
            }
            Console.WriteLine();
        }
        private static void FailedToMeetRequirement(string name, string comment)
        {
            Console.Write("[      ] ");
            Console.Write("{0}", name);
            if (comment != "")
            {
                Console.Write(": ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("{0}", comment);
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        private static void DoTest(TestCase test)
        {
            // Have we already completed this test?
            if (unitTestResults.ContainsKey(test.Method.ToString()))
            {
                return;
            }

            bool passed = false;
            bool metRequirement = true;
            string exception = "";
            try
            {
                passed = test();
            }
            catch (RequirementException e)
            {
                metRequirement = false;
                exception = e.Message;
            }
            catch (Exception e)
            {
                exception = e.GetType().ToString();
            }

            string className = test.Method.ToString().Replace("Boolean Test", "").Split('0')[0];
            string fnName = test.Method.ToString().Split('0')[1];

            if (metRequirement)
            {
                if (passed)
                {
                    unitTestResults[test.Method.ToString()] = "Passed";
                    Passed(string.Format("{0}.{1}", className, fnName), exception);
                }
                else
                {
                    unitTestResults[test.Method.ToString()] = "Failed";
                    Failed(string.Format("{0}.{1}", className, fnName), exception);
                }
            }
            else
            {
                unitTestResults[test.Method.ToString()] = "Failed";
                FailedToMeetRequirement(string.Format("{0}.{1}", className, fnName), exception);
            }
            Cleanup();
        }

        private static Stack<string> errorDescriptionStack = new Stack<string>();


        private static void Requires(TestCase test)
        {
            string result;
            bool wasTested = unitTestResults.TryGetValue(test.Method.ToString(), out result);

            if (!wasTested)
            {
                // Push the error description onto the stack (only thing that can change, not that it should)
                errorDescriptionStack.Push(ErrorDescription);

                // Do the test
                DoTest(test);

                // Pop the description off
                ErrorDescription = errorDescriptionStack.Pop();

                // Get the proper result for out
                wasTested = unitTestResults.TryGetValue(test.Method.ToString(), out result);

                if (!wasTested)
                {
                    throw new Exception("This should never happen");
                }
            }

            if (result == "Failed")
            {
                string className = test.Method.ToString().Replace("Boolean Test", "").Split('0')[0];
                string fnName = test.Method.ToString().Split('0')[1];

                throw new RequirementException(string.Format("-> {0}.{1}", className, fnName));
            }
            else if (result == "Passed")
            {
                return;
            }
            else
            {
                throw new Exception("This should never happen");
            }

        }

        #endregion

        #region Test Cases
        private static GameController InitialiseGame()
        {
            Requires(TestGameController0GameController);
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0PlayerController);
            Requires(TestGameController0SetPlayer);

            GameController game = new GameController(2, 1);
            Chassis tank = Chassis.CreateTank(1);
            TankController player1 = new PlayerController("player1", tank, Color.Orange);
            TankController player2 = new PlayerController("player2", tank, Color.Purple);
            game.SetPlayer(1, player1);
            game.SetPlayer(2, player2);
            return game;
        }
        private static void Cleanup()
        {
            while (Application.OpenForms.Count > 0)
            {
                Application.OpenForms[0].Dispose();
            }
        }
        private static bool TestGameController0GameController()
        {
            GameController game = new GameController(2, 1);
            return true;
        }
        private static bool TestGameController0TotalPlayers()
        {
            Requires(TestGameController0GameController);

            GameController game = new GameController(2, 1);
            return game.TotalPlayers() == 2;
        }
        private static bool TestGameController0GetMaxRounds()
        {
            Requires(TestGameController0GameController);

            GameController game = new GameController(3, 5);
            return game.GetMaxRounds() == 5;
        }
        private static bool TestGameController0SetPlayer()
        {
            Requires(TestGameController0GameController);
            Requires(TestChassis0CreateTank);

            GameController game = new GameController(2, 1);
            Chassis tank = Chassis.CreateTank(1);
            TankController player = new PlayerController("playerName", tank, Color.Orange);
            game.SetPlayer(1, player);
            return true;
        }
        private static bool TestGameController0GetPlayerById()
        {
            Requires(TestGameController0GameController);
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0PlayerController);

            GameController game = new GameController(2, 1);
            Chassis tank = Chassis.CreateTank(1);
            TankController player = new PlayerController("playerName", tank, Color.Orange);
            game.SetPlayer(1, player);
            return game.GetPlayerById(1) == player;
        }
        private static bool TestGameController0GetTankColour()
        {
            Color[] arrayOfColours = new Color[8];
            for (int i = 0; i < 8; i++)
            {
                arrayOfColours[i] = GameController.GetTankColour(i + 1);
                for (int j = 0; j < i; j++)
                {
                    if (arrayOfColours[j] == arrayOfColours[i]) return false;
                }
            }
            return true;
        }
        private static bool TestGameController0GetPlayerPositions()
        {
            int[] positions = GameController.GetPlayerPositions(8);
            for (int i = 0; i < 8; i++)
            {
                if (positions[i] < 0) return false;
                if (positions[i] > 160) return false;
                for (int j = 0; j < i; j++)
                {
                    if (positions[j] == positions[i]) return false;
                }
            }
            return true;
        }
        private static bool TestGameController0Shuffle()
        {
            int[] ar = new int[100];
            for (int i = 0; i < 100; i++)
            {
                ar[i] = i;
            }
            GameController.Shuffle(ar);
            for (int i = 0; i < 100; i++)
            {
                if (ar[i] != i)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool TestGameController0BeginGame()
        {
            GameController game = InitialiseGame();
            game.BeginGame();

            foreach (Form f in Application.OpenForms)
            {
                if (f is GameplayForm)
                {
                    return true;
                }
            }
            return false;
        }
        private static bool TestGameController0GetArena()
        {
            Requires(TestBattlefield0Battlefield);
            GameController game = InitialiseGame();
            game.BeginGame();
            Battlefield battlefield = game.GetArena();
            if (battlefield != null) return true;

            return false;
        }
        private static bool TestGameController0GetPlayerTank()
        {
            Requires(TestGameController0GameController);
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0PlayerController);
            Requires(TestGameController0SetPlayer);
            Requires(TestPlayerTank0GetPlayerById);

            GameController game = new GameController(2, 1);
            Chassis tank = Chassis.CreateTank(1);
            TankController player1 = new PlayerController("player1", tank, Color.Orange);
            TankController player2 = new PlayerController("player2", tank, Color.Purple);
            game.SetPlayer(1, player1);
            game.SetPlayer(2, player2);

            game.BeginGame();
            PlayerTank ptank = game.GetPlayerTank();
            if (ptank.GetPlayerById() != player1 && ptank.GetPlayerById() != player2)
            {
                return false;
            }
            if (ptank.CreateTank() != tank)
            {
                return false;
            }

            return true;
        }

        private static bool TestChassis0CreateTank()
        {
            Chassis tank = Chassis.CreateTank(1);
            if (tank != null) return true;
            else return false;
        }
        private static bool TestChassis0DrawTankSprite()
        {
            Requires(TestChassis0CreateTank);
            Chassis tank = Chassis.CreateTank(1);

            int[,] tankGraphic = tank.DrawTankSprite(45);
            if (tankGraphic.GetLength(0) != 12) return false;
            if (tankGraphic.GetLength(1) != 16) return false;
            // We don't really care what the tank looks like, but the 45 degree tank
            // should at least look different to the -45 degree tank
            int[,] tankGraphic2 = tank.DrawTankSprite(-45);
            for (int y = 0; y < 12; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    if (tankGraphic2[y, x] != tankGraphic[y, x])
                    {
                        return true;
                    }
                }
            }

            SetErrorDescription("Tank with turret at -45 degrees looks the same as tank with turret at 45 degrees");

            return false;
        }
        private static void DisplayLine(int[,] array)
        {
            string report = "";
            report += "A line drawn from 3,0 to 0,3 on a 4x4 array should look like this:\n";
            report += "0001\n";
            report += "0010\n";
            report += "0100\n";
            report += "1000\n";
            report += "The one produced by Chassis.CreateLine() looks like this:\n";
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    report += array[y, x] == 1 ? "1" : "0";
                }
                report += "\n";
            }
            SetErrorDescription(report);
        }
        private static bool TestChassis0CreateLine()
        {
            int[,] ar = new int[,] { { 0, 0, 0, 0 },
                                     { 0, 0, 0, 0 },
                                     { 0, 0, 0, 0 },
                                     { 0, 0, 0, 0 } };
            Chassis.CreateLine(ar, 3, 0, 0, 3);

            // Ideally, the line we want to see here is:
            // 0001
            // 0010
            // 0100
            // 1000

            // However, as we aren't that picky, as long as they have a 1 in every row and column
            // and nothing in the top-left and bottom-right corners

            int[] rows = new int[4];
            int[] cols = new int[4];
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (ar[y, x] == 1)
                    {
                        rows[y] = 1;
                        cols[x] = 1;
                    }
                    else if (ar[y, x] > 1 || ar[y, x] < 0)
                    {
                        // Only values 0 and 1 are permitted
                        SetErrorDescription(string.Format("Somehow the number {0} got into the array.", ar[y, x]));
                        return false;
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (rows[i] == 0)
                {
                    DisplayLine(ar);
                    return false;
                }
                if (cols[i] == 0)
                {
                    DisplayLine(ar);
                    return false;
                }
            }
            if (ar[0, 0] == 1)
            {
                DisplayLine(ar);
                return false;
            }
            if (ar[3, 3] == 1)
            {
                DisplayLine(ar);
                return false;
            }

            return true;
        }
        private static bool TestChassis0GetHealth()
        {
            Requires(TestChassis0CreateTank);
            // As long as it's > 0 we're happy
            Chassis tank = Chassis.CreateTank(1);
            if (tank.GetHealth() > 0) return true;
            return false;
        }
        private static bool TestChassis0WeaponList()
        {
            Requires(TestChassis0CreateTank);
            // As long as there's at least one result and it's not null / a blank string, we're happy
            Chassis tank = Chassis.CreateTank(1);
            if (tank.WeaponList().Length == 0) return false;
            if (tank.WeaponList()[0] == null) return false;
            if (tank.WeaponList()[0] == "") return false;
            return true;
        }

        private static TankController CreateTestingPlayer()
        {
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0PlayerController);

            Chassis tank = Chassis.CreateTank(1);
            TankController player = new PlayerController("player1", tank, Color.Aquamarine);
            return player;
        }

        private static bool TestTankController0PlayerController()
        {
            Requires(TestChassis0CreateTank);

            Chassis tank = Chassis.CreateTank(1);
            TankController player = new PlayerController("player1", tank, Color.Aquamarine);
            if (player != null) return true;
            return false;
        }
        private static bool TestTankController0CreateTank()
        {
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0PlayerController);

            Chassis tank = Chassis.CreateTank(1);
            TankController p = new PlayerController("player1", tank, Color.Aquamarine);
            if (p.CreateTank() == tank) return true;
            return false;
        }
        private static bool TestTankController0Identifier()
        {
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0PlayerController);

            const string PLAYER_NAME = "kfdsahskfdajh";
            Chassis tank = Chassis.CreateTank(1);
            TankController p = new PlayerController(PLAYER_NAME, tank, Color.Aquamarine);
            if (p.Identifier() == PLAYER_NAME) return true;
            return false;
        }
        private static bool TestTankController0GetTankColour()
        {
            Requires(TestChassis0CreateTank);
            Requires(TestTankController0PlayerController);

            Color playerColour = Color.Chartreuse;
            Chassis tank = Chassis.CreateTank(1);
            TankController p = new PlayerController("player1", tank, playerColour);
            if (p.GetTankColour() == playerColour) return true;
            return false;
        }
        private static bool TestTankController0AddScore()
        {
            TankController p = CreateTestingPlayer();
            p.AddScore();
            return true;
        }
        private static bool TestTankController0GetVictories()
        {
            Requires(TestTankController0AddScore);

            TankController p = CreateTestingPlayer();
            int wins = p.GetVictories();
            p.AddScore();
            if (p.GetVictories() == wins + 1) return true;
            return false;
        }
        private static bool TestPlayerController0BeginRound()
        {
            TankController p = CreateTestingPlayer();
            p.BeginRound();
            return true;
        }
        private static bool TestPlayerController0BeginTurn()
        {
            Requires(TestGameController0BeginGame);
            Requires(TestGameController0GetPlayerById);
            GameController game = InitialiseGame();

            game.BeginGame();

            // Find the gameplay form
            GameplayForm gameplayForm = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f is GameplayForm)
                {
                    gameplayForm = f as GameplayForm;
                }
            }
            if (gameplayForm == null)
            {
                SetErrorDescription("Gameplay form was not created by GameController.BeginGame()");
                return false;
            }

            // Find the control panel
            Panel controlPanel = null;
            foreach (Control c in gameplayForm.Controls)
            {
                if (c is Panel)
                {
                    foreach (Control cc in c.Controls)
                    {
                        if (cc is NumericUpDown || cc is Label || cc is TrackBar)
                        {
                            controlPanel = c as Panel;
                        }
                    }
                }
            }

            if (controlPanel == null)
            {
                SetErrorDescription("Control panel was not found in GameplayForm");
                return false;
            }

            // Disable the control panel to check that NewTurn enables it
            controlPanel.Enabled = false;

            game.GetPlayerById(1).BeginTurn(gameplayForm, game);

            if (!controlPanel.Enabled)
            {
                SetErrorDescription("Control panel is still disabled after HumanPlayer.NewTurn()");
                return false;
            }
            return true;

        }
        private static bool TestPlayerController0ProjectileHit()
        {
            TankController p = CreateTestingPlayer();
            p.ProjectileHit(0, 0);
            return true;
        }

        private static bool TestPlayerTank0PlayerTank()
        {
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);
            return true;
        }
        private static bool TestPlayerTank0GetPlayerById()
        {
            Requires(TestPlayerTank0PlayerTank);
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);
            if (playerTank.GetPlayerById() == p) return true;
            return false;
        }
        private static bool TestPlayerTank0CreateTank()
        {
            Requires(TestPlayerTank0PlayerTank);
            Requires(TestTankController0CreateTank);
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);
            if (playerTank.CreateTank() == playerTank.GetPlayerById().CreateTank()) return true;
            return false;
        }
        private static bool TestPlayerTank0GetPlayerAngle()
        {
            Requires(TestPlayerTank0PlayerTank);
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);
            float angle = playerTank.GetPlayerAngle();
            if (angle >= -90 && angle <= 90) return true;
            return false;
        }
        private static bool TestPlayerTank0AimTurret()
        {
            Requires(TestPlayerTank0PlayerTank);
            Requires(TestPlayerTank0GetPlayerAngle);
            float angle = 75;
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);
            playerTank.AimTurret(angle);
            if (FloatEquals(playerTank.GetPlayerAngle(), angle)) return true;
            return false;
        }
        private static bool TestPlayerTank0GetTankPower()
        {
            Requires(TestPlayerTank0PlayerTank);
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);

            playerTank.GetTankPower();
            return true;
        }
        private static bool TestPlayerTank0SetPower()
        {
            Requires(TestPlayerTank0PlayerTank);
            Requires(TestPlayerTank0GetTankPower);
            int power = 65;
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);
            playerTank.SetPower(power);
            if (playerTank.GetTankPower() == power) return true;
            return false;
        }
        private static bool TestPlayerTank0GetCurrentWeapon()
        {
            Requires(TestPlayerTank0PlayerTank);

            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);

            playerTank.GetCurrentWeapon();
            return true;
        }
        private static bool TestPlayerTank0SetWeaponIndex()
        {
            Requires(TestPlayerTank0PlayerTank);
            Requires(TestPlayerTank0GetCurrentWeapon);
            int weapon = 3;
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);
            playerTank.SetWeaponIndex(weapon);
            if (playerTank.GetCurrentWeapon() == weapon) return true;
            return false;
        }
        private static bool TestPlayerTank0Draw()
        {
            Requires(TestPlayerTank0PlayerTank);
            Size bitmapSize = new Size(640, 480);
            Bitmap image = new Bitmap(bitmapSize.Width, bitmapSize.Height);
            Graphics graphics = Graphics.FromImage(image);
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);
            playerTank.Draw(graphics, bitmapSize);
            graphics.Dispose();

            for (int y = 0; y < bitmapSize.Height; y++)
            {
                for (int x = 0; x < bitmapSize.Width; x++)
                {
                    if (image.GetPixel(x, y) != image.GetPixel(0, 0))
                    {
                        // Something changed in the image, and that's good enough for me
                        return true;
                    }
                }
            }
            SetErrorDescription("Nothing was drawn.");
            return false;
        }
        private static bool TestPlayerTank0GetX()
        {
            Requires(TestPlayerTank0PlayerTank);

            TankController p = CreateTestingPlayer();
            int x = 73;
            int y = 28;
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, x, y, game);
            if (playerTank.GetX() == x) return true;
            return false;
        }
        private static bool TestPlayerTank0GetYPos()
        {
            Requires(TestPlayerTank0PlayerTank);

            TankController p = CreateTestingPlayer();
            int x = 73;
            int y = 28;
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, x, y, game);
            if (playerTank.GetYPos() == y) return true;
            return false;
        }
        private static bool TestPlayerTank0Fire()
        {
            Requires(TestPlayerTank0PlayerTank);

            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);
            playerTank.Fire();
            return true;
        }
        private static bool TestPlayerTank0DamagePlayer()
        {
            Requires(TestPlayerTank0PlayerTank);
            TankController p = CreateTestingPlayer();

            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);
            playerTank.DamagePlayer(10);
            return true;
        }
        private static bool TestPlayerTank0Exists()
        {
            Requires(TestPlayerTank0PlayerTank);
            Requires(TestPlayerTank0DamagePlayer);

            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            PlayerTank playerTank = new PlayerTank(p, 32, 32, game);
            if (!playerTank.Exists()) return false;
            playerTank.DamagePlayer(playerTank.CreateTank().GetHealth());
            if (playerTank.Exists()) return false;
            return true;
        }
        private static bool TestPlayerTank0CalculateGravity()
        {
            Requires(TestGameController0GetArena);
            Requires(TestBattlefield0DestroyGround);
            Requires(TestPlayerTank0PlayerTank);
            Requires(TestPlayerTank0DamagePlayer);
            Requires(TestPlayerTank0Exists);
            Requires(TestPlayerTank0CreateTank);
            Requires(TestChassis0GetHealth);

            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            game.BeginGame();
            // Unfortunately we need to rely on DestroyTerrain() to get rid of any terrain that may be in the way
            game.GetArena().DestroyGround(Battlefield.WIDTH / 2.0f, Battlefield.HEIGHT / 2.0f, 20);
            PlayerTank playerTank = new PlayerTank(p, Battlefield.WIDTH / 2, Battlefield.HEIGHT / 2, game);
            int oldX = playerTank.GetX();
            int oldY = playerTank.GetYPos();

            playerTank.CalculateGravity();

            if (playerTank.GetX() != oldX)
            {
                SetErrorDescription("Caused X coordinate to change.");
                return false;
            }
            if (playerTank.GetYPos() != oldY + 1)
            {
                SetErrorDescription("Did not cause Y coordinate to increase by 1.");
                return false;
            }

            int initialArmour = playerTank.CreateTank().GetHealth();
            // The tank should have lost 1 armour from falling 1 tile already, so do
            // (initialArmour - 2) damage to the tank then drop it again. That should kill it.

            if (!playerTank.Exists())
            {
                SetErrorDescription("Tank died before we could check that fall damage worked properly");
                return false;
            }
            playerTank.DamagePlayer(initialArmour - 2);
            if (!playerTank.Exists())
            {
                SetErrorDescription("Tank died before we could check that fall damage worked properly");
                return false;
            }
            playerTank.CalculateGravity();
            if (playerTank.Exists())
            {
                SetErrorDescription("Tank survived despite taking enough falling damage to destroy it");
                return false;
            }

            return true;
        }
        private static bool TestBattlefield0Battlefield()
        {
            Battlefield battlefield = new Battlefield();
            return true;
        }
        private static bool TestBattlefield0TileAt()
        {
            Requires(TestBattlefield0Battlefield);

            bool foundTrue = false;
            bool foundFalse = false;
            Battlefield battlefield = new Battlefield();
            for (int y = 0; y < Battlefield.HEIGHT; y++)
            {
                for (int x = 0; x < Battlefield.WIDTH; x++)
                {
                    if (battlefield.TileAt(x, y))
                    {
                        foundTrue = true;
                    }
                    else
                    {
                        foundFalse = true;
                    }
                }
            }

            if (!foundTrue)
            {
                SetErrorDescription("IsTileAt() did not return true for any tile.");
                return false;
            }

            if (!foundFalse)
            {
                SetErrorDescription("IsTileAt() did not return false for any tile.");
                return false;
            }

            return true;
        }
        private static bool TestBattlefield0CheckTankCollide()
        {
            Requires(TestBattlefield0Battlefield);
            Requires(TestBattlefield0TileAt);

            Battlefield battlefield = new Battlefield();
            for (int y = 0; y <= Battlefield.HEIGHT - Chassis.HEIGHT; y++)
            {
                for (int x = 0; x <= Battlefield.WIDTH - Chassis.WIDTH; x++)
                {
                    int colTiles = 0;
                    for (int iy = 0; iy < Chassis.HEIGHT; iy++)
                    {
                        for (int ix = 0; ix < Chassis.WIDTH; ix++)
                        {

                            if (battlefield.TileAt(x + ix, y + iy))
                            {
                                colTiles++;
                            }
                        }
                    }
                    if (colTiles == 0)
                    {
                        if (battlefield.CheckTankCollide(x, y))
                        {
                            SetErrorDescription("Found collision where there shouldn't be one");
                            return false;
                        }
                    }
                    else
                    {
                        if (!battlefield.CheckTankCollide(x, y))
                        {
                            SetErrorDescription("Didn't find collision where there should be one");
                            return false;
                        }
                    }
                }
            }

            return true;
        }
        private static bool TestBattlefield0TankVerticalPosition()
        {
            Requires(TestBattlefield0Battlefield);
            Requires(TestBattlefield0TileAt);

            Battlefield battlefield = new Battlefield();
            for (int x = 0; x <= Battlefield.WIDTH - Chassis.WIDTH; x++)
            {
                int lowestValid = 0;
                for (int y = 0; y <= Battlefield.HEIGHT - Chassis.HEIGHT; y++)
                {
                    int colTiles = 0;
                    for (int iy = 0; iy < Chassis.HEIGHT; iy++)
                    {
                        for (int ix = 0; ix < Chassis.WIDTH; ix++)
                        {

                            if (battlefield.TileAt(x + ix, y + iy))
                            {
                                colTiles++;
                            }
                        }
                    }
                    if (colTiles == 0)
                    {
                        lowestValid = y;
                    }
                }

                int placedY = battlefield.TankVerticalPosition(x);
                if (placedY != lowestValid)
                {
                    SetErrorDescription(string.Format("Tank was placed at {0},{1} when it should have been placed at {0},{2}", x, placedY, lowestValid));
                    return false;
                }
            }
            return true;
        }
        private static bool TestBattlefield0DestroyGround()
        {
            Requires(TestBattlefield0Battlefield);
            Requires(TestBattlefield0TileAt);

            Battlefield battlefield = new Battlefield();
            for (int y = 0; y < Battlefield.HEIGHT; y++)
            {
                for (int x = 0; x < Battlefield.WIDTH; x++)
                {
                    if (battlefield.TileAt(x, y))
                    {
                        battlefield.DestroyGround(x, y, 0.5f);
                        if (battlefield.TileAt(x, y))
                        {
                            SetErrorDescription("Attempted to destroy terrain but it still exists");
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            SetErrorDescription("Did not find any terrain to destroy");
            return false;
        }
        private static bool TestBattlefield0CalculateGravity()
        {
            Requires(TestBattlefield0Battlefield);
            Requires(TestBattlefield0TileAt);
            Requires(TestBattlefield0DestroyGround);

            Battlefield battlefield = new Battlefield();
            for (int x = 0; x < Battlefield.WIDTH; x++)
            {
                if (battlefield.TileAt(x, Battlefield.HEIGHT - 1))
                {
                    if (battlefield.TileAt(x, Battlefield.HEIGHT - 2))
                    {
                        // Seek up and find the first non-set tile
                        for (int y = Battlefield.HEIGHT - 2; y >= 0; y--)
                        {
                            if (!battlefield.TileAt(x, y))
                            {
                                // Do a gravity step and make sure it doesn't slip down
                                battlefield.CalculateGravity();
                                if (!battlefield.TileAt(x, y + 1))
                                {
                                    SetErrorDescription("Moved down terrain even though there was no room");
                                    return false;
                                }

                                // Destroy the bottom-most tile
                                battlefield.DestroyGround(x, Battlefield.HEIGHT - 1, 0.5f);

                                // Do a gravity step and make sure it does slip down
                                battlefield.CalculateGravity();

                                if (battlefield.TileAt(x, y + 1))
                                {
                                    SetErrorDescription("Terrain didn't fall");
                                    return false;
                                }

                                // Otherwise this seems to have worked
                                return true;
                            }
                        }


                    }
                }
            }
            SetErrorDescription("Did not find any appropriate terrain to test");
            return false;
        }
        private static bool TestWeaponEffect0RecordCurrentGame()
        {
            Requires(TestShrapnel0Shrapnel);
            Requires(TestGameController0GameController);

            WeaponEffect weaponEffect = new Shrapnel(1, 1, 1);
            GameController game = new GameController(2, 1);
            weaponEffect.RecordCurrentGame(game);
            return true;
        }
        private static bool TestProjectile0Projectile()
        {
            Requires(TestShrapnel0Shrapnel);
            TankController player = CreateTestingPlayer();
            Shrapnel explosion = new Shrapnel(1, 1, 1);
            Projectile projectile = new Projectile(25, 25, 45, 30, 0.02f, explosion, player);
            return true;
        }
        private static bool TestProjectile0Step()
        {
            Requires(TestGameController0BeginGame);
            Requires(TestShrapnel0Shrapnel);
            Requires(TestProjectile0Projectile);
            Requires(TestWeaponEffect0RecordCurrentGame);
            GameController game = InitialiseGame();
            game.BeginGame();
            TankController player = game.GetPlayerById(1);
            Shrapnel explosion = new Shrapnel(1, 1, 1);

            Projectile projectile = new Projectile(25, 25, 45, 100, 0.01f, explosion, player);
            projectile.RecordCurrentGame(game);
            projectile.Step();

            // We can't really test this one without a substantial framework,
            // so we just call it and hope that everything works out

            return true;
        }
        private static bool TestProjectile0Draw()
        {
            Requires(TestGameController0BeginGame);
            Requires(TestGameController0GetPlayerById);
            Requires(TestShrapnel0Shrapnel);
            Requires(TestProjectile0Projectile);
            Requires(TestWeaponEffect0RecordCurrentGame);

            Size bitmapSize = new Size(640, 480);
            Bitmap image = new Bitmap(bitmapSize.Width, bitmapSize.Height);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.Black); // Blacken out the image so we can see the projectile
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            game.BeginGame();
            TankController player = game.GetPlayerById(1);
            Shrapnel explosion = new Shrapnel(1, 1, 1);

            Projectile projectile = new Projectile(25, 25, 45, 100, 0.01f, explosion, player);
            projectile.RecordCurrentGame(game);
            projectile.Draw(graphics, bitmapSize);
            graphics.Dispose();

            for (int y = 0; y < bitmapSize.Height; y++)
            {
                for (int x = 0; x < bitmapSize.Width; x++)
                {
                    if (image.GetPixel(x, y) != image.GetPixel(0, 0))
                    {
                        // Something changed in the image, and that's good enough for me
                        return true;
                    }
                }
            }
            SetErrorDescription("Nothing was drawn.");
            return false;
        }
        private static bool TestShrapnel0Shrapnel()
        {
            TankController player = CreateTestingPlayer();
            Shrapnel explosion = new Shrapnel(1, 1, 1);

            return true;
        }
        private static bool TestShrapnel0Explode()
        {
            Requires(TestShrapnel0Shrapnel);
            Requires(TestWeaponEffect0RecordCurrentGame);
            Requires(TestGameController0GetPlayerById);
            Requires(TestGameController0BeginGame);

            GameController game = InitialiseGame();
            game.BeginGame();
            TankController player = game.GetPlayerById(1);
            Shrapnel explosion = new Shrapnel(1, 1, 1);
            explosion.RecordCurrentGame(game);
            explosion.Explode(25, 25);

            return true;
        }
        private static bool TestShrapnel0Step()
        {
            Requires(TestShrapnel0Shrapnel);
            Requires(TestWeaponEffect0RecordCurrentGame);
            Requires(TestGameController0GetPlayerById);
            Requires(TestGameController0BeginGame);
            Requires(TestShrapnel0Explode);

            GameController game = InitialiseGame();
            game.BeginGame();
            TankController player = game.GetPlayerById(1);
            Shrapnel explosion = new Shrapnel(1, 1, 1);
            explosion.RecordCurrentGame(game);
            explosion.Explode(25, 25);
            explosion.Step();

            // Again, we can't really test this one without a full framework

            return true;
        }
        private static bool TestShrapnel0Draw()
        {
            Requires(TestShrapnel0Shrapnel);
            Requires(TestWeaponEffect0RecordCurrentGame);
            Requires(TestGameController0GetPlayerById);
            Requires(TestGameController0BeginGame);
            Requires(TestShrapnel0Explode);
            Requires(TestShrapnel0Step);

            Size bitmapSize = new Size(640, 480);
            Bitmap image = new Bitmap(bitmapSize.Width, bitmapSize.Height);
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(Color.Black); // Blacken out the image so we can see the explosion
            TankController p = CreateTestingPlayer();
            GameController game = InitialiseGame();
            game.BeginGame();
            TankController player = game.GetPlayerById(1);
            Shrapnel explosion = new Shrapnel(10, 10, 10);
            explosion.RecordCurrentGame(game);
            explosion.Explode(25, 25);
            // Step it for a bit so we can be sure the explosion is visible
            for (int i = 0; i < 10; i++)
            {
                explosion.Step();
            }
            explosion.Draw(graphics, bitmapSize);

            for (int y = 0; y < bitmapSize.Height; y++)
            {
                for (int x = 0; x < bitmapSize.Width; x++)
                {
                    if (image.GetPixel(x, y) != image.GetPixel(0, 0))
                    {
                        // Something changed in the image, and that's good enough for me
                        return true;
                    }
                }
            }
            SetErrorDescription("Nothing was drawn.");
            return false;
        }

        private static GameplayForm InitialiseGameplayForm(out NumericUpDown angleCtrl, out TrackBar powerCtrl, out Button fireCtrl, out Panel controlPanel, out ListBox weaponSelect)
        {
            Requires(TestGameController0BeginGame);

            GameController game = InitialiseGame();

            angleCtrl = null;
            powerCtrl = null;
            fireCtrl = null;
            controlPanel = null;
            weaponSelect = null;

            game.BeginGame();
            GameplayForm gameplayForm = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f is GameplayForm)
                {
                    gameplayForm = f as GameplayForm;
                }
            }
            if (gameplayForm == null)
            {
                SetErrorDescription("GameController.BeginGame() did not create a GameplayForm and that is the only way GameplayForm can be tested");
                return null;
            }

            bool foundDisplayPanel = false;
            bool foundControlPanel = false;

            foreach (Control c in gameplayForm.Controls)
            {
                // The only controls should be 2 panels
                if (c is Panel)
                {
                    // Is this the control panel or the display panel?
                    Panel p = c as Panel;

                    // The display panel will have 0 controls.
                    // The control panel will have separate, of which only a few are mandatory
                    int controlsFound = 0;
                    bool foundFire = false;
                    bool foundAngle = false;
                    bool foundAngleLabel = false;
                    bool foundPower = false;
                    bool foundPowerLabel = false;


                    foreach (Control pc in p.Controls)
                    {
                        controlsFound++;

                        // Mandatory controls for the control panel are:
                        // A 'Fire!' button
                        // A NumericUpDown for controlling the angle
                        // A TrackBar for controlling the power
                        // "Power:" and "Angle:" labels

                        if (pc is Label)
                        {
                            Label lbl = pc as Label;
                            if (lbl.Text.ToLower().Contains("angle"))
                            {
                                foundAngleLabel = true;
                            }
                            else
                            if (lbl.Text.ToLower().Contains("power"))
                            {
                                foundPowerLabel = true;
                            }
                        }
                        else
                        if (pc is Button)
                        {
                            Button btn = pc as Button;
                            if (btn.Text.ToLower().Contains("fire"))
                            {
                                foundFire = true;
                                fireCtrl = btn;
                            }
                        }
                        else
                        if (pc is TrackBar)
                        {
                            foundPower = true;
                            powerCtrl = pc as TrackBar;
                        }
                        else
                        if (pc is NumericUpDown)
                        {
                            foundAngle = true;
                            angleCtrl = pc as NumericUpDown;
                        }
                        else
                        if (pc is ListBox)
                        {
                            weaponSelect = pc as ListBox;
                        }
                    }

                    if (controlsFound == 0)
                    {
                        foundDisplayPanel = true;
                    }
                    else
                    {
                        if (!foundFire)
                        {
                            SetErrorDescription("Control panel lacks a \"Fire!\" button OR the display panel incorrectly contains controls");
                            return null;
                        }
                        else
                        if (!foundAngle)
                        {
                            SetErrorDescription("Control panel lacks an angle NumericUpDown OR the display panel incorrectly contains controls");
                            return null;
                        }
                        else
                        if (!foundPower)
                        {
                            SetErrorDescription("Control panel lacks a power TrackBar OR the display panel incorrectly contains controls");
                            return null;
                        }
                        else
                        if (!foundAngleLabel)
                        {
                            SetErrorDescription("Control panel lacks an \"Angle:\" label OR the display panel incorrectly contains controls");
                            return null;
                        }
                        else
                        if (!foundPowerLabel)
                        {
                            SetErrorDescription("Control panel lacks a \"Power:\" label OR the display panel incorrectly contains controls");
                            return null;
                        }

                        foundControlPanel = true;
                        controlPanel = p;
                    }

                }
                else
                {
                    SetErrorDescription(string.Format("Unexpected control ({0}) named \"{1}\" found in GameplayForm", c.GetType().FullName, c.Name));
                    return null;
                }
            }

            if (!foundDisplayPanel)
            {
                SetErrorDescription("No display panel found");
                return null;
            }
            if (!foundControlPanel)
            {
                SetErrorDescription("No control panel found");
                return null;
            }
            return gameplayForm;
        }

        private static bool TestGameplayForm0GameplayForm()
        {
            NumericUpDown angle;
            TrackBar power;
            Button fire;
            Panel controlPanel;
            ListBox weaponSelect;
            GameplayForm gameplayForm = InitialiseGameplayForm(out angle, out power, out fire, out controlPanel, out weaponSelect);

            if (gameplayForm == null) return false;

            return true;
        }
        private static bool TestGameplayForm0EnableHumanControl()
        {
            Requires(TestGameplayForm0GameplayForm);
            GameController game = InitialiseGame();
            game.BeginGame();

            // Find the gameplay form
            GameplayForm gameplayForm = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f is GameplayForm)
                {
                    gameplayForm = f as GameplayForm;
                }
            }
            if (gameplayForm == null)
            {
                SetErrorDescription("Gameplay form was not created by GameController.BeginGame()");
                return false;
            }

            // Find the control panel
            Panel controlPanel = null;
            foreach (Control c in gameplayForm.Controls)
            {
                if (c is Panel)
                {
                    foreach (Control cc in c.Controls)
                    {
                        if (cc is NumericUpDown || cc is Label || cc is TrackBar)
                        {
                            controlPanel = c as Panel;
                        }
                    }
                }
            }

            if (controlPanel == null)
            {
                SetErrorDescription("Control panel was not found in GameplayForm");
                return false;
            }

            // Disable the control panel to check that EnableControlPanel enables it
            controlPanel.Enabled = false;

            gameplayForm.EnableHumanControl();

            if (!controlPanel.Enabled)
            {
                SetErrorDescription("Control panel is still disabled after GameplayForm.EnableHumanControl()");
                return false;
            }
            return true;

        }
        private static bool TestGameplayForm0AimTurret()
        {
            Requires(TestGameplayForm0GameplayForm);
            NumericUpDown angle;
            TrackBar power;
            Button fire;
            Panel controlPanel;
            ListBox weaponSelect;
            GameplayForm gameplayForm = InitialiseGameplayForm(out angle, out power, out fire, out controlPanel, out weaponSelect);

            if (gameplayForm == null) return false;

            float testAngle = 27;

            gameplayForm.AimTurret(testAngle);
            if (FloatEquals((float)angle.Value, testAngle)) return true;

            else
            {
                SetErrorDescription(string.Format("Attempted to set angle to {0} but angle is {1}", testAngle, (float)angle.Value));
                return false;
            }
        }
        private static bool TestGameplayForm0SetPower()
        {
            Requires(TestGameplayForm0GameplayForm);
            NumericUpDown angle;
            TrackBar power;
            Button fire;
            Panel controlPanel;
            ListBox weaponSelect;
            GameplayForm gameplayForm = InitialiseGameplayForm(out angle, out power, out fire, out controlPanel, out weaponSelect);

            if (gameplayForm == null) return false;

            int testPower = 71;

            gameplayForm.SetPower(testPower);
            if (power.Value == testPower) return true;

            else
            {
                SetErrorDescription(string.Format("Attempted to set power to {0} but power is {1}", testPower, power.Value));
                return false;
            }
        }
        private static bool TestGameplayForm0SetWeaponIndex()
        {
            Requires(TestGameplayForm0GameplayForm);
            NumericUpDown angle;
            TrackBar power;
            Button fire;
            Panel controlPanel;
            ListBox weaponSelect;
            GameplayForm gameplayForm = InitialiseGameplayForm(out angle, out power, out fire, out controlPanel, out weaponSelect);

            if (gameplayForm == null) return false;

            gameplayForm.SetWeaponIndex(0);

            // WeaponSelect is optional behaviour, so it's okay if it's not implemented here, as long as the method works.
            return true;
        }
        private static bool TestGameplayForm0Fire()
        {
            Requires(TestGameplayForm0GameplayForm);
            // This is something we can't really test properly without a proper framework, so for now we'll just click
            // the button and make sure it disables the control panel
            NumericUpDown angle;
            TrackBar power;
            Button fire;
            Panel controlPanel;
            ListBox weaponSelect;
            GameplayForm gameplayForm = InitialiseGameplayForm(out angle, out power, out fire, out controlPanel, out weaponSelect);

            controlPanel.Enabled = true;
            fire.PerformClick();
            if (controlPanel.Enabled)
            {
                SetErrorDescription("Control panel still enabled immediately after clicking fire button");
                return false;
            }

            return true;
        }
        private static void UnitTests()
        {
            DoTest(TestGameController0GameController);
            DoTest(TestGameController0TotalPlayers);
            DoTest(TestGameController0GetMaxRounds);
            DoTest(TestGameController0SetPlayer);
            DoTest(TestGameController0GetPlayerById);
            DoTest(TestGameController0GetTankColour);
            DoTest(TestGameController0GetPlayerPositions);
            DoTest(TestGameController0Shuffle);
            DoTest(TestGameController0BeginGame);
            DoTest(TestGameController0GetArena);
            DoTest(TestGameController0GetPlayerTank);
            DoTest(TestChassis0CreateTank);
            DoTest(TestChassis0DrawTankSprite);
            DoTest(TestChassis0CreateLine);
            DoTest(TestChassis0GetHealth);
            DoTest(TestChassis0WeaponList);
            DoTest(TestTankController0PlayerController);
            DoTest(TestTankController0CreateTank);
            DoTest(TestTankController0Identifier);
            DoTest(TestTankController0GetTankColour);
            DoTest(TestTankController0AddScore);
            DoTest(TestTankController0GetVictories);
            DoTest(TestPlayerController0BeginRound);
            DoTest(TestPlayerController0BeginTurn);
            DoTest(TestPlayerController0ProjectileHit);
            DoTest(TestPlayerTank0PlayerTank);
            DoTest(TestPlayerTank0GetPlayerById);
            DoTest(TestPlayerTank0CreateTank);
            DoTest(TestPlayerTank0GetPlayerAngle);
            DoTest(TestPlayerTank0AimTurret);
            DoTest(TestPlayerTank0GetTankPower);
            DoTest(TestPlayerTank0SetPower);
            DoTest(TestPlayerTank0GetCurrentWeapon);
            DoTest(TestPlayerTank0SetWeaponIndex);
            DoTest(TestPlayerTank0Draw);
            DoTest(TestPlayerTank0GetX);
            DoTest(TestPlayerTank0GetYPos);
            DoTest(TestPlayerTank0Fire);
            DoTest(TestPlayerTank0DamagePlayer);
            DoTest(TestPlayerTank0Exists);
            DoTest(TestPlayerTank0CalculateGravity);
            DoTest(TestBattlefield0Battlefield);
            DoTest(TestBattlefield0TileAt);
            DoTest(TestBattlefield0CheckTankCollide);
            DoTest(TestBattlefield0TankVerticalPosition);
            DoTest(TestBattlefield0DestroyGround);
            DoTest(TestBattlefield0CalculateGravity);
            DoTest(TestWeaponEffect0RecordCurrentGame);
            DoTest(TestProjectile0Projectile);
            DoTest(TestProjectile0Step);
            DoTest(TestProjectile0Draw);
            DoTest(TestShrapnel0Shrapnel);
            DoTest(TestShrapnel0Explode);
            DoTest(TestShrapnel0Step);
            DoTest(TestShrapnel0Draw);
            DoTest(TestGameplayForm0GameplayForm);
            DoTest(TestGameplayForm0EnableHumanControl);
            DoTest(TestGameplayForm0AimTurret);
            DoTest(TestGameplayForm0SetPower);
            DoTest(TestGameplayForm0SetWeaponIndex);
            DoTest(TestGameplayForm0Fire);
        }
        
        #endregion
        
        #region CheckClasses

        private static bool CheckClasses()
        {
            string[] classNames = new string[] { "Program", "AIOpponent", "Battlefield", "Shrapnel", "GameplayForm", "GameController", "PlayerController", "Projectile", "TankController", "PlayerTank", "Chassis", "WeaponEffect" };
            string[][] classFields = new string[][] {
                new string[] { "Main" }, // Program
                new string[] { }, // AIOpponent
                new string[] { "TileAt","CheckTankCollide","TankVerticalPosition","DestroyGround","CalculateGravity","WIDTH","HEIGHT"}, // Battlefield
                new string[] { "Explode" }, // Shrapnel
                new string[] { "EnableHumanControl","AimTurret","SetPower","SetWeaponIndex","Fire","InitBuffer"}, // GameplayForm
                new string[] { "TotalPlayers","CurrentRound","GetMaxRounds","SetPlayer","GetPlayerById","GetGameplayTank","GetTankColour","GetPlayerPositions","Shuffle","BeginGame","CommenceRound","GetArena","DisplayPlayerTanks","GetPlayerTank","AddEffect","WeaponEffectTick","DrawWeaponEffects","EndEffect","CheckHitTank","DamagePlayer","CalculateGravity","TurnOver","ScoreWinner","NextRound","WindSpeed"}, // GameController
                new string[] { }, // PlayerController
                new string[] { }, // Projectile
                new string[] { "CreateTank","Identifier","GetTankColour","AddScore","GetVictories","BeginRound","BeginTurn","ProjectileHit"}, // TankController
                new string[] { "GetPlayerById","CreateTank","GetPlayerAngle","AimTurret","GetTankPower","SetPower","GetCurrentWeapon","SetWeaponIndex","Draw","GetX","GetYPos","Fire","DamagePlayer","Exists","CalculateGravity"}, // PlayerTank
                new string[] { "DrawTankSprite","CreateLine","CreateTankBMP","GetHealth","WeaponList","FireWeapon","CreateTank","WIDTH","HEIGHT","NUM_TANKS"}, // Chassis
                new string[] { "RecordCurrentGame","Step","Draw"} // WeaponEffect
            };

            Assembly assembly = Assembly.GetExecutingAssembly();

            Console.WriteLine("Checking classes for public methods...");
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsPublic)
                {
                    if (type.Namespace != "TankBattle")
                    {
                        Console.WriteLine("Public type {0} is not in the TankBattle namespace.", type.FullName);
                        return false;
                    }
                    else
                    {
                        int typeIdx = -1;
                        for (int i = 0; i < classNames.Length; i++)
                        {
                            if (type.Name == classNames[i])
                            {
                                typeIdx = i;
                                classNames[typeIdx] = null;
                                break;
                            }
                        }
                        foreach (MemberInfo memberInfo in type.GetMembers())
                        {
                            string memberName = memberInfo.Name;
                            bool isInherited = false;
                            foreach (MemberInfo parentMemberInfo in type.BaseType.GetMembers())
                            {
                                if (memberInfo.Name == parentMemberInfo.Name)
                                {
                                    isInherited = true;
                                    break;
                                }
                            }
                            if (!isInherited)
                            {
                                if (typeIdx != -1)
                                {
                                    bool fieldFound = false;
                                    if (memberName[0] != '.')
                                    {
                                        foreach (string allowedFields in classFields[typeIdx])
                                        {
                                            if (memberName == allowedFields)
                                            {
                                                fieldFound = true;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        fieldFound = true;
                                    }
                                    if (!fieldFound)
                                    {
                                        Console.WriteLine("The public field \"{0}\" is not one of the authorised fields for the {1} class.\n", memberName, type.Name);
                                        Console.WriteLine("Remove it or change its access level.");
                                        return false;
                                    }
                                }
                            }
                        }
                    }

                    //Console.WriteLine("{0} passed.", type.FullName);
                }
            }
            for (int i = 0; i < classNames.Length; i++)
            {
                if (classNames[i] != null)
                {
                    Console.WriteLine("The class \"{0}\" is missing.", classNames[i]);
                    return false;
                }
            }
            Console.WriteLine("All public methods okay.");
            return true;
        }
        
        #endregion

        public static void Main()
        {
            if (CheckClasses())
            {
                UnitTests();

                int passed = 0;
                int failed = 0;
                foreach (string key in unitTestResults.Keys)
                {
                    if (unitTestResults[key] == "Passed")
                    {
                        passed++;
                    }
                    else
                    {
                        failed++;
                    }
                }

                Console.WriteLine("\n{0}/{1} unit tests passed", passed, passed + failed);
                if (failed == 0)
                {
                    Console.WriteLine("Starting up TankBattle...");
                    Program.Main();
                    return;
                }
            }

            Console.WriteLine("\nPress enter to exit.");
            Console.ReadLine();
        }
    }
}
