﻿//-----------------------------------------------------------------------
// <copyright file="GameWindow.xaml.cs" company="David Fedchuk, Gerson Eliu Sorto Flores, Lincoln Kaszynski, Sanaaia Okhlopkova, and Samuel Mace">
//     MIT License, 2022 (https://mit-license.org).
// </copyright>
//-----------------------------------------------------------------------
namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for GameWindow.xaml.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Encapsulation not taught.")]
    public partial class GameWindow : Window
    {
        /// <summary>
        /// The cell size.
        /// </summary>
        public double Cellsize = 25;

        /// <summary>
        /// The number of rows.
        /// </summary>
        public int RowRep;

        /// <summary>
        /// The list of player cell grids.
        /// </summary>
        public List<GridCell> PlayersCellRecords;

        /// <summary>
        /// The <see cref="AdvancedOptions"/> object for the GameWindow (assigned by reference in the constructor).
        /// </summary>
        private AdvancedOptions advancedOptions;

        /// <summary>
        /// Tracking changes event dictionary.
        /// </summary>
        private Dictionary<string, GridCell> gameStatus = new Dictionary<string, GridCell>();

        /// <summary>
        /// The game type.
        /// </summary>
        private StatusCodes.GameType gameType;

        /// <summary>
        /// The list of file contents.
        /// </summary>
        private List<string[]> fileContents = new List<string[]>();

        /// <summary>
        /// The player 1 screen.
        /// </summary>
        private bool screenPlayerOne = true;

        /// <summary>
        /// The player 1.
        /// </summary>
        private Player player1;

        /// <summary>
        /// The player 2.
        /// </summary>
        private Player player2;

        /// <summary>
        /// The player 1.
        /// </summary>
        private string player1Name;

        /// <summary>
        /// The player 2.
        /// </summary>
        private string player2Name;

        /// <summary>
        /// The computer player 1.
        /// </summary>
        private ComputerPlayer computerPlayer1;

        /// <summary>
        /// The computer player 2.
        /// </summary>
        private ComputerPlayer computerPlayer2;

        /// <summary>
        /// The first computer player's difficulty level.
        /// </summary>
        private StatusCodes.ComputerPlayerDifficulty computerPlayerDifficulty1;

        /// <summary>
        /// The second computer player's difficulty level.
        /// </summary>
        private StatusCodes.ComputerPlayerDifficulty computerPlayerDifficulty2;

        /// <summary>
        /// The player 1 window.
        /// </summary>
        private Canvas playerWindow1;

        /// <summary>
        /// The player 2 window.
        /// </summary>
        private Canvas playerWindow2;

        /// <summary>
        /// The dispatch timer.
        /// </summary>
        private DispatcherTimer dispatcherTimer;

        /// <summary>
        /// The object for saving and loading the game state.
        /// </summary>
        private SaveLoad savingAndLoading;

        /// <summary>
        /// The list of ship types.
        /// </summary>
        private List<int> shiptypelist;

        /// <summary>
        /// A temporary variable containing the bomb count (should be removed once <see cref="AdvancedOptionsWindow"/> is implemented).
        /// </summary>
        private int bombcounttest;

        /// <summary>
        /// Specifies whether or not the player can have multiple hits per turn.
        /// </summary>
        private bool optionPlayerTurnHits;

        /// <summary>
        /// Specifies whether or not the player can have as many hits as they have ships still afloat.
        /// </summary>
        private bool optionPlayerTurnShip;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameWindow" /> class.
        /// </summary>
        /// <param name="gameType"> This is the game type.</param>
        /// <param name="p_advancedOptions">The <see cref="AdvancedOptions"/> to be passed to the <see cref="GameWindow"/>.</param>
        public GameWindow(StatusCodes.GameType gameType, ref AdvancedOptions p_advancedOptions)
        {
            this.PlayersCellRecords = new List<GridCell>();

            this.InitializeComponent();

            this.gameType = gameType;

            switch (gameType)
            {
                case StatusCodes.GameType.PLAYER_TO_PLAYER:
                    this.Loaded += this.StartPlayerToPlayerGame;
                    break;
                case StatusCodes.GameType.PLAYER_TO_COMPUTER:
                    this.Loaded += this.StartPlayerToComputerGame;
                    break;
                case StatusCodes.GameType.COMPUTER_TO_COMPUTER:
                    this.Loaded += this.StartComputerToComputerGame;
                    break;
            }

            this.savingAndLoading = new SaveLoad();
            this.savingAndLoading.OnGameStatusUpdate += this.OnGameLoad;

            this.advancedOptions = p_advancedOptions;

            this.shiptypelist = new List<int>();

            foreach (StatusCodes.ShipType shipType in this.advancedOptions.ShipTypes)
            {
                switch (shipType)
                {
                    case StatusCodes.ShipType.DESTROYER:
                        this.shiptypelist.Add(1);
                        break;
                    case StatusCodes.ShipType.SUBMARINE:
                        this.shiptypelist.Add(2);
                        break;
                    case StatusCodes.ShipType.CRUISER:
                        this.shiptypelist.Add(3);
                        break;
                    case StatusCodes.ShipType.BATTLESHIP:
                        this.shiptypelist.Add(4);
                        break;
                    case StatusCodes.ShipType.CARRIER:
                        this.shiptypelist.Add(5);
                        break;
                }
            }

            this.bombcounttest = this.advancedOptions.BombCount;
            this.optionPlayerTurnHits = this.advancedOptions.PlayerCanAttackAgain;
            this.optionPlayerTurnShip = this.advancedOptions.EachShipGetsAShot;

            this.RowRep = this.advancedOptions.GridSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameWindow" /> class.
        /// </summary>
        /// <param name="gameType"> This is the game type.</param>
        public GameWindow(StatusCodes.GameType gameType)
        {
            this.PlayersCellRecords = new List<GridCell>();

            this.InitializeComponent();

            this.gameType = gameType;

            switch (gameType)
            {
                case StatusCodes.GameType.PLAYER_TO_PLAYER:
                    this.Loaded += this.StartPlayerToPlayerGame;
                    break;
                case StatusCodes.GameType.PLAYER_TO_COMPUTER:
                    this.Loaded += this.StartPlayerToComputerGame;
                    break;
                case StatusCodes.GameType.COMPUTER_TO_COMPUTER:
                    this.Loaded += this.StartComputerToComputerGame;
                    break;
            }

            this.shiptypelist = new List<int>() { 1, 2, 3, 4, 5 };
            this.bombcounttest = 0;
            this.optionPlayerTurnHits = false;
            this.optionPlayerTurnShip = false;
            this.RowRep = 10;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameWindow" /> class.
        /// </summary>
        /// <param name="pathFile"> This is the game file path.</param>
        public GameWindow(string pathFile)
        {
            this.InitializeComponent();
            this.RowRep = 10;
            string player1Name = string.Empty;
            string player2Name = string.Empty;

            string[,] player1Board = new string[this.RowRep, this.RowRep];
            string[,] player2Board = new string[this.RowRep, this.RowRep];

            if (File.Exists(pathFile))
            {
                using (StreamReader sr = File.OpenText(pathFile))
                {
                    sr.ReadLine();
                    player1Name = sr.ReadLine();
                    for (int i = 0; i < this.RowRep; i++)
                    {
                        string[] items = sr.ReadLine().Split(',');

                        for (int j = 0; j < this.RowRep; ++j)
                        {
                            player1Board[i, j] = items[j];
                        }
                    }

                    player2Name = sr.ReadLine();
                    for (int i = 0; i < this.RowRep; i++)
                    {
                        string[] items = sr.ReadLine().Split(',');

                        for (int j = 0; j < this.RowRep; ++j)
                        {
                            player2Board[i, j] = items[j];
                        }
                    }
                }
            }

            // start player one label visible
            this.PlayerOnelabel.Visibility = Visibility.Visible;
            this.PlayerTwolabel.Visibility = Visibility.Hidden;
            this.Confirm_Button.Visibility = Visibility.Hidden;

            // Create Players with their cells and their ships and grids colors
            // 1 = Black,2=dark blue,3=magenta,4=lightseagreen,5=purple,6=white,standard cadet blue
            this.player1 = new Player(1, player1Name, this.Cellsize, this.RowRep, 1, 3, player1Board, player2Board);
            this.player2 = new Player(2, player2Name, this.Cellsize, this.RowRep, 3, 1, player2Board, player1Board);

            // Create two Canvas to place the player elements on them
            this.playerWindow1 = new Canvas();
            this.playerWindow1.HorizontalAlignment = HorizontalAlignment.Center;
            this.playerWindow1.VerticalAlignment = VerticalAlignment.Center;
            this.playerWindow1.Uid = "Player1Canvas";
            this.playerWindow1.Width = (this.Cellsize * this.RowRep) * 2;
            this.playerWindow1.Visibility = Visibility.Visible;

            this.playerWindow2 = new Canvas();
            this.playerWindow2.HorizontalAlignment = HorizontalAlignment.Center;
            this.playerWindow2.VerticalAlignment = VerticalAlignment.Center;
            this.playerWindow2.Uid = "Player2Canvas";
            this.playerWindow2.Width = (this.Cellsize * this.RowRep) * 2;
            this.playerWindow2.Visibility = Visibility.Hidden;

            this.DeclarePlayerGridFromFile(this.player1, this.player2, this.PlayersCellRecords, this.playerWindow1, this.Cellsize);
            this.DeclarePlayerGridFromFile(this.player2, this.player1, this.PlayersCellRecords, this.playerWindow2, this.Cellsize);

            // load both canvas to this window grid
            this.Maingrid.Children.Add(this.playerWindow1);
            this.Maingrid.Children.Add(this.playerWindow2);

            this.Show();
        }

        /// <summary>
        ///  Gets a value indicating whether screen player1 is visible or not.
        /// </summary>
        public bool Switch
        {
            get { return this.screenPlayerOne; }
        }

        /// <summary>
        ///  Gets or sets a computer player 1 difficulty.
        /// </summary>
        public StatusCodes.ComputerPlayerDifficulty ComputerPlayerDifficulty1
        {
            get { return this.computerPlayerDifficulty1; }
            set { this.computerPlayerDifficulty1 = value; }
        }

        /// <summary>
        ///  Gets or sets a computer player 2 difficulty.
        /// </summary>
        public StatusCodes.ComputerPlayerDifficulty ComputerPlayerDifficulty2
        {
            get { return this.computerPlayerDifficulty2; }
            set { this.computerPlayerDifficulty2 = value; }
        }

        /// <summary>
        ///  Gets or sets a player 1 name.
        /// </summary>
        public string Player1Name
        {
            get { return this.player1Name; }
            set { this.player1Name = value; }
        }

        /// <summary>
        ///  Gets or sets a player 2 name.
        /// </summary>
        public string Player2Name
        {
            get { return this.player2Name; }
            set { this.player2Name = value; }
        }

        /// <summary>
        /// Start a player to player game.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments for the event.</param>
        private void StartPlayerToPlayerGame(object sender, EventArgs e)
        {
            // start player one label visible
            this.PlayerOnelabel.Visibility = Visibility.Visible;
            this.PlayerTwolabel.Visibility = Visibility.Hidden;
            this.AttackBtn.IsEnabled = false;
            if (!(!this.optionPlayerTurnHits && !this.optionPlayerTurnShip && this.bombcounttest == 0 && this.RowRep == 10 && this.shiptypelist.Distinct().Count() == 5))
            {
                this.SaveGameButton.Visibility = Visibility.Hidden;
            }

            // Create Players with their cells and their ships and grids colors
            // 1 = Black,2=dark blue,3=magenta,4=lightseagreen,5=purple,6=white,standard cadet blue
            this.player1 = new Player(1, this.player1Name, this.Cellsize, this.RowRep, 1, 3, this.shiptypelist, this.bombcounttest);
            this.player2 = new Player(2, this.player2Name, this.Cellsize, this.RowRep, 3, 1, this.shiptypelist, this.bombcounttest);
            this.player2.PlayerTurn = false;

            // Create two Canvas to place the player elements on them
            this.playerWindow1 = new Canvas();
            this.playerWindow1.HorizontalAlignment = HorizontalAlignment.Center;
            this.playerWindow1.VerticalAlignment = VerticalAlignment.Center;
            this.playerWindow1.Uid = "Player1Canvas";
            this.playerWindow1.Width = (this.Cellsize * this.RowRep) * 2;
            this.playerWindow1.Visibility = Visibility.Visible;

            this.playerWindow2 = new Canvas();
            this.playerWindow2.HorizontalAlignment = HorizontalAlignment.Center;
            this.playerWindow2.VerticalAlignment = VerticalAlignment.Center;
            this.playerWindow2.Uid = "Player2Canvas";
            this.playerWindow2.Width = (this.Cellsize * this.RowRep) * 2;
            this.playerWindow2.Visibility = Visibility.Hidden;

            this.DeclarePlayerGrid(this.player1, this.player2, this.PlayersCellRecords, this.playerWindow1, this.Cellsize);
            this.DeclarePlayerShips(this.player1, this.playerWindow1, this.Cellsize);

            this.DeclarePlayerGrid(this.player2, this.player1, this.PlayersCellRecords, this.playerWindow2, this.Cellsize);
            this.DeclarePlayerShips(this.player2, this.playerWindow2, this.Cellsize);

            // load both canvas to this window grid
            this.Maingrid.Children.Add(this.playerWindow1);
            this.Maingrid.Children.Add(this.playerWindow2);

            this.Show();
        }

        /// <summary>
        /// Start a player to computer game.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments for the event.</param>
        private void StartPlayerToComputerGame(object sender, EventArgs e)
        {
            // start player one label visible
            this.PlayerOnelabel.Visibility = Visibility.Visible;
            this.PlayerTwolabel.Visibility = Visibility.Hidden;
            this.AttackBtn.Visibility = Visibility.Hidden;
            this.SaveGameButton.Visibility = Visibility.Hidden;

            // Create Players with their cells and their ships and grids colors
            // 1 = Black,2=dark blue,3=magenta,4=lightseagreen,5=purple,6=white,standard cadet blue
            this.shiptypelist = new List<int> { 1, 2, 3, 4, 5 };
            this.player1 = new Player(1, this.player1Name, this.Cellsize, this.RowRep, 1, 3, this.shiptypelist, this.bombcounttest);
            this.player2 = new ComputerPlayer(2, "ComputerPlayerTwo", this.Cellsize, this.RowRep, 3, 1, this.computerPlayerDifficulty2, this.shiptypelist);
            this.player2.IsLocked = true;

            Logger.ConsoleInformation("------- Computer Grid ------");
            for (int i = 0; i < this.RowRep; i++)
            {
                for (int j = 0; j < this.RowRep; j++)
                {
                    Logger.ConsoleInformationForArray(this.player2.Board[i, j] + ", ");
                }

                Logger.ConsoleInformation("\n");
            }

            // Create two Canvas to place the player elements on them
            this.playerWindow1 = new Canvas();
            this.playerWindow1.HorizontalAlignment = HorizontalAlignment.Center;
            this.playerWindow1.VerticalAlignment = VerticalAlignment.Center;
            this.playerWindow1.Uid = "Player1Canvas";
            this.playerWindow1.Width = (this.Cellsize * this.RowRep) * 2;
            this.playerWindow1.Visibility = Visibility.Visible;

            this.DeclarePlayerGrid(this.player1, this.player2, this.PlayersCellRecords, this.playerWindow1, this.Cellsize);
            this.DeclarePlayerShips(this.player1, this.playerWindow1, this.Cellsize);

            // load both canvas to this window grid
            this.Maingrid.Children.Add(this.playerWindow1);

            this.Show();
        }

        /// <summary>
        /// Start a computer to computer game.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments for the event.</param>
        private void StartComputerToComputerGame(object sender, EventArgs e)
        {
            // start player one label visible
            this.PlayerOnelabel.Visibility = Visibility.Visible;
            this.PlayerTwolabel.Visibility = Visibility.Hidden;
            this.AttackBtn.Visibility = Visibility.Hidden;
            this.Confirm_Button.Visibility = Visibility.Hidden;
            this.SaveGameButton.Visibility = Visibility.Hidden;

            // Create Players with their cells and their ships and grids colors
            // 1 = Black,2=dark blue,3=magenta,4=lightseagreen,5=purple,6=white,standard cadet blue
            this.player1 = new ComputerPlayer(1, "ComputerPlayerOne", this.Cellsize, this.RowRep, 1, 3, this.computerPlayerDifficulty1, this.shiptypelist);
            this.player2 = new ComputerPlayer(2, "ComputerPlayerTwo", this.Cellsize, this.RowRep, 3, 1, this.computerPlayerDifficulty2, this.shiptypelist);

            Logger.ConsoleInformation("------- Computer Grid ------");
            for (int i = 0; i < this.RowRep; i++)
            {
                for (int j = 0; j < this.RowRep; j++)
                {
                    Logger.ConsoleInformationForArray(this.player1.Board[i, j] + ", ");
                }

                Logger.ConsoleInformation("\n");
            }

            Logger.ConsoleInformation("------- Computer Grid ------");
            for (int i = 0; i < this.RowRep; i++)
            {
                for (int j = 0; j < this.RowRep; j++)
                {
                    Logger.ConsoleInformationForArray(this.player2.Board[i, j] + ", ");
                }

                Logger.ConsoleInformation("\n");
            }

            // Create two Canvas to place the player elements on them
            this.playerWindow1 = new Canvas();
            this.playerWindow1.HorizontalAlignment = HorizontalAlignment.Center;
            this.playerWindow1.VerticalAlignment = VerticalAlignment.Center;
            this.playerWindow1.Uid = "Player1Canvas";
            this.playerWindow1.Width = (this.Cellsize * this.RowRep) * 2;
            this.playerWindow1.Visibility = Visibility.Visible;

            this.playerWindow2 = new Canvas();
            this.playerWindow2.HorizontalAlignment = HorizontalAlignment.Center;
            this.playerWindow2.VerticalAlignment = VerticalAlignment.Center;
            this.playerWindow2.Uid = "Player2Canvas";
            this.playerWindow2.Width = (this.Cellsize * this.RowRep) * 2;
            this.playerWindow2.Visibility = Visibility.Hidden;

            this.DeclareComputerPlayerGrid(this.player1, this.player2, this.PlayersCellRecords, this.playerWindow1, this.Cellsize);
            this.DeclareComputerPlayerShips(this.player1, this.playerWindow1, this.Cellsize);

            this.DeclareComputerPlayerGrid(this.player2, this.player1, this.PlayersCellRecords, this.playerWindow2, this.Cellsize);
            this.DeclareComputerPlayerShips(this.player2, this.playerWindow2, this.Cellsize);

            // load both canvas to this window grid
            this.Maingrid.Children.Add(this.playerWindow1);
            this.Maingrid.Children.Add(this.playerWindow2);

            this.dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            this.dispatcherTimer.Tick += this.DispatcherTimer_Tick;
            this.dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
            this.dispatcherTimer.Start();

            this.Show();
        }

        /// <summary>
        /// Changes the bomb button visibility based on the player in view.
        /// </summary>
        private void Bombbtnvis()
        {
            if (this.playerWindow1.Visibility == Visibility.Visible && this.player1.BombCount > 0 && this.player1.IsLocked == true)
            {
                this.BombLoader.Visibility = Visibility.Visible;
            }
            else if (this.playerWindow2.Visibility == Visibility.Visible && this.player2.BombCount > 0 && this.player2.IsLocked == true)
            {
                this.BombLoader.Visibility = Visibility.Visible;
            }
            else
            {
                this.BombLoader.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Make the windows visible with Attack button.
        /// </summary>
        private void SwitchPlayerWindows()
        {
            if (this.screenPlayerOne == true)
            {
                // change the visual to off for player 1 and turn on the vissible to player two canvas
                this.screenPlayerOne = false;
                this.PlayerOnelabel.Visibility = Visibility.Hidden;
                this.PlayerTwolabel.Visibility = Visibility.Visible;
                foreach (UIElement canvas in this.Maingrid.Children)
                {
                    if (canvas.Uid == "Player1Canvas")
                    {
                        canvas.Visibility = Visibility.Hidden;
                    }

                    if (canvas.Uid == "Player2Canvas")
                    {
                        canvas.Visibility = Visibility.Visible;
                    }
                }
                //// if the switch button has been clicked for the first time then allow player2 to move their ships
                this.SetConfirmButtonVisibility("Player2Canvas");
                if (this.player2.Name != "ComputerPlayerTwo")
                {
                    this.Bombbtnvis();
                }
            }
            else
            {
                // if the screen player one entert the method in false condition
                // change the visual to on for player 1 and turn off the vissible to player two canvas
                this.screenPlayerOne = true;
                this.PlayerOnelabel.Visibility = Visibility.Visible;
                this.PlayerTwolabel.Visibility = Visibility.Hidden;

                foreach (UIElement canvas in this.Maingrid.Children)
                {
                    if (canvas.Uid == "Player1Canvas")
                    {
                        canvas.Visibility = Visibility.Visible;
                    }

                    if (canvas.Uid == "Player2Canvas")
                    {
                        canvas.Visibility = Visibility.Hidden;
                    }
                }

                this.SetConfirmButtonVisibility("Player1Canvas");
                if (this.player2.Name != "ComputerPlayerTwo")
                {
                    this.Bombbtnvis();
                }
            }
        }

        /// <summary>
        /// The event for the dispatch timer.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments being passed to the event.</param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (this.player1.Winner || this.player2.Winner)
            {
                this.dispatcherTimer.Stop();
            }
            else
            {
                if (this.computerPlayerDifficulty1 ==
                    StatusCodes.ComputerPlayerDifficulty.COMPUTER_DIFFICULTY_HARD)
                {
                    ((ComputerPlayer)this.player1).AdvancedAttack(this.player2, this.RowRep);
                }
                else
                {
                    ((ComputerPlayer)this.player1).CompPlayerAttack(this.player2, this.RowRep);
                }

                this.SwitchPlayerWindows();

                if (this.computerPlayerDifficulty2 ==
                    StatusCodes.ComputerPlayerDifficulty.COMPUTER_DIFFICULTY_HARD)
                {
                    ((ComputerPlayer)this.player2).AdvancedAttack(this.player1, this.RowRep);
                }
                else
                {
                    ((ComputerPlayer)this.player2).CompPlayerAttack(this.player1, this.RowRep);
                }
            }
        }

        /// <summary>
        /// Method that declares the player grid.
        /// </summary>
        /// <param name="p_currentPlayer">The current player to be declared.</param>
        /// <param name="p_otherPlayer">The other player relative to the current player.</param>
        /// <param name="p_playersCellRecords">The current player's cell records.</param>
        /// <param name="p_currentPlayerWindow">The current player's window.</param>
        /// <param name="p_cellsize">The cell size (in pixels).</param>
        private void DeclarePlayerGrid(Player p_currentPlayer, Player p_otherPlayer, List<GridCell> p_playersCellRecords, Canvas p_currentPlayerWindow, double p_cellsize)
        {
            bool p_currentPlayerTurn = false;
            bool p_otherPlayerTurn = true;

            foreach (KeyValuePair<int, GridCell> mainPlayerPair in p_currentPlayer.Playergridsquarecollection)
            {
                // Create initial variables for interior iterations
                int gridMaincellnumber = mainPlayerPair.Key;
                GridCell mainPlayerCell = mainPlayerPair.Value;
                GridCell mainPlayerCell_New = mainPlayerPair.Value;
                int gridcellnumber = mainPlayerPair.Key;

                // Load in initial grids to game dictionary control for reports and outputs
                string gameStatusdictionarykey = p_currentPlayer.PlayerID.ToString() + "-" + mainPlayerPair.Value.TrackingID.ToString();
                this.gameStatus.Add(gameStatusdictionarykey, mainPlayerPair.Value);

                // add a click event for all cells in Player 1 grid only if the button is attack type
                if (mainPlayerCell.OffenseButton == true)
                {
                    // add click event
                    mainPlayerCell.Click += new RoutedEventHandler(delegate(object sender, RoutedEventArgs e)
                    {
                        // Double-check that both players ships are locked into place before allowing the user to attack grid spaces.
                        if (p_currentPlayer.IsLocked == true && p_otherPlayer.IsLocked == true)
                        {
                            if (p_currentPlayer.PlayerTurn)
                            {
                                // return list of targets to destroy
                                List<int> targets = this.SetAttack(mainPlayerCell.Buttonid, p_currentPlayer);

                                for (int i = targets.Count - 1; i >= 0; i--)
                                {
                                    if (targets[i] < 0 || targets[i] > (this.RowRep * this.RowRep))
                                    {
                                        targets.RemoveAt(i);
                                    }
                                }

                                int targetCount = 1;
                                bool bombHasHit = false;

                                foreach (int target in targets)
                                {
                                    bool checkIfCellHasHit = false;

                                    // go check the list of buttons for player two and change the status for them
                                    foreach (KeyValuePair<int, GridCell> otherPlayerPair in p_otherPlayer.Playergridsquarecollection)
                                    {
                                        int otherPlayercellnumber = otherPlayerPair.Key;
                                        GridCell otherPlayerPlayerCell = otherPlayerPair.Value;

                                        // turn off buttons on the enemy grid(player two left side)only if it is a defense button
                                        if (target == otherPlayerPlayerCell.Buttonid &&
                                            otherPlayerPlayerCell.OffenseButton == false)
                                        {
                                            if (otherPlayerPlayerCell.ShipContainedName != string.Empty)
                                            {
                                                Logger.ConsoleInformation("You have damaged my " + otherPlayerPlayerCell.ShipContainedName);
                                                otherPlayerPlayerCell.CellAttackStatus = StatusCodes.AttackStatus.ATTACKED_HIT;
                                            }
                                            else
                                            {
                                                Logger.ConsoleInformation("You will never succeed");
                                                otherPlayerPlayerCell.CellAttackStatus = StatusCodes.AttackStatus.ATTACKED_NOT_HIT;
                                            }

                                            gridcellnumber = (this.RowRep * this.RowRep) + otherPlayercellnumber;
                                            int rowNum;
                                            if ((gridcellnumber - (this.RowRep * this.RowRep)) % this.RowRep == 0)
                                            {
                                                rowNum = ((gridcellnumber - (this.RowRep * this.RowRep)) / this.RowRep) - 1;
                                            }
                                            else
                                            {
                                                rowNum = (gridcellnumber - (this.RowRep * this.RowRep)) / this.RowRep;
                                            }

                                            int colNum = ((gridcellnumber - (this.RowRep * this.RowRep)) - (((gridcellnumber - (this.RowRep * this.RowRep)) / this.RowRep) * this.RowRep)) - 1;
                                            if (colNum == -1)
                                            {
                                                colNum = this.RowRep - 1;
                                            }

                                            mainPlayerCell_New =
                                                p_currentPlayer.Playergridsquarecollection[gridcellnumber];
                                            string letterAttackGrid = p_otherPlayer.Board[rowNum, colNum];
                                            if (letterAttackGrid != "O" && letterAttackGrid != "H" && letterAttackGrid != "M")
                                            {
                                                otherPlayerPlayerCell.Background = Brushes.Red;
                                                otherPlayerPlayerCell.Content = letterAttackGrid;
                                                otherPlayerPlayerCell.Stricked = 1;

                                                p_otherPlayer.Board[rowNum, colNum] = "H";
                                                mainPlayerCell_New.Background = Brushes.Green;
                                                mainPlayerCell_New.Content = "H";
                                                mainPlayerCell_New.Stricked = 1;
                                                mainPlayerCell_New.AllowDrop = false;
                                                if (this.optionPlayerTurnHits)
                                                {
                                                    bombHasHit = true;
                                                }

                                                /*if (this.optionPlayerTurnHits && targetCount == targets.Count)
                                                {
                                                    p_currentPlayerTurn = true;
                                                    p_otherPlayerTurn = false;
                                                    Logger.Information("You can continue hitting!");
                                                }
                                                else*/

                                                if (this.optionPlayerTurnShip && p_currentPlayer.ShipCountTurn < p_currentPlayer.ShipCount && targetCount == targets.Count)
                                                {
                                                    p_currentPlayerTurn = true;
                                                    p_otherPlayerTurn = false;
                                                    Logger.Information("You can continue hitting! Attacks left: " + (p_currentPlayer.ShipCount - p_currentPlayer.ShipCountTurn));
                                                    p_currentPlayer.ShipCountTurn++;
                                                }
                                                else if (this.optionPlayerTurnShip && p_currentPlayer.ShipCountTurn >= p_currentPlayer.ShipCount && targetCount == targets.Count)
                                                {
                                                    p_currentPlayerTurn = false;
                                                    p_otherPlayerTurn = true;
                                                    p_currentPlayer.ShipCountTurn = 1;
                                                    if (p_otherPlayer.Name != "ComputerPlayerTwo")
                                                    {
                                                        Logger.Information("Switch Player. Next Player Turn.");
                                                    }
                                                }
                                            }
                                            else if (letterAttackGrid == "O")
                                            {
                                                otherPlayerPlayerCell.Background = Brushes.Red;
                                                otherPlayerPlayerCell.Content = "X";
                                                otherPlayerPlayerCell.Stricked = 1;

                                                p_otherPlayer.Board[rowNum, colNum] = "M";
                                                mainPlayerCell_New.Background = Brushes.Red;
                                                mainPlayerCell_New.Content = "X";
                                                mainPlayerCell_New.Stricked = 1;
                                                mainPlayerCell_New.AllowDrop = false;
                                                if (this.optionPlayerTurnShip && p_currentPlayer.ShipCountTurn < p_currentPlayer.ShipCount && targetCount == targets.Count)
                                                {
                                                    p_currentPlayerTurn = true;
                                                    p_otherPlayerTurn = false;
                                                    Logger.Information("You can continue hitting! Attacks left: " + (p_currentPlayer.ShipCount - p_currentPlayer.ShipCountTurn));
                                                    p_currentPlayer.ShipCountTurn++;
                                                }
                                                else if (this.optionPlayerTurnShip && p_currentPlayer.ShipCountTurn >= p_currentPlayer.ShipCount && targetCount == targets.Count)
                                                {
                                                    p_currentPlayerTurn = false;
                                                    p_otherPlayerTurn = true;
                                                    p_currentPlayer.ShipCountTurn = 1;
                                                    if (p_otherPlayer.Name != "ComputerPlayerTwo")
                                                    {
                                                        Logger.Information("Switch Player. Next Player Turn.");
                                                    }
                                                }
                                                else
                                                {
                                                    p_currentPlayerTurn = false;
                                                    p_otherPlayerTurn = true;
                                                    if (targets.Count == 1)
                                                    {
                                                        if (p_otherPlayer.Name != "ComputerPlayerTwo")
                                                        {
                                                            Logger.Information("Switch Player. Next Player Turn.");
                                                        }
                                                    }
                                                }
                                            }
                                            else if (letterAttackGrid == "H")
                                            {
                                                checkIfCellHasHit = true;
                                                if (this.optionPlayerTurnHits)
                                                {
                                                    bombHasHit = true;
                                                }

                                                /*if (this.optionPlayerTurnHits && targetCount == targets.Count)
                                                {
                                                    p_currentPlayerTurn = true;
                                                    p_otherPlayerTurn = false;
                                                    Logger.Information("You can continue hitting!");
                                                }
                                                else*/ if (this.optionPlayerTurnShip && p_currentPlayer.ShipCountTurn < p_currentPlayer.ShipCount && targetCount == targets.Count)
                                                {
                                                    p_currentPlayerTurn = true;
                                                    p_otherPlayerTurn = false;
                                                    Logger.Information("You can continue hitting! Attacks left: " + (p_currentPlayer.ShipCount - p_currentPlayer.ShipCountTurn));
                                                    p_currentPlayer.ShipCountTurn++;
                                                }
                                                else if (this.optionPlayerTurnShip && p_currentPlayer.ShipCountTurn >= p_currentPlayer.ShipCount && targetCount == targets.Count)
                                                {
                                                    p_currentPlayerTurn = false;
                                                    p_otherPlayerTurn = true;
                                                    p_currentPlayer.ShipCountTurn = 1;
                                                    if (p_otherPlayer.Name != "ComputerPlayerTwo")
                                                    {
                                                        Logger.Information("Switch Player. Next Player Turn.");
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (target >= 0 && target <= (this.RowRep * this.RowRep))
                                    {
                                        Coordinate attackedGridSpace = new Coordinate((short)mainPlayerCell_New.ColNum, (short)mainPlayerCell_New.RowNum);

                                        Logger.ConsoleInformation("Row Number: " + mainPlayerCell_New.RowNum);
                                        Logger.ConsoleInformation("Column Number: " + mainPlayerCell_New.ColNum);

                                        foreach (Ship testShip in p_otherPlayer.Playershipcollection)
                                        {
                                            // Logger.Information(testShip.ShipStartCoords.XCoordinate.ToString() + " "+ testShip.ShipStartCoords.YCoordinate.ToString());
                                            if (!checkIfCellHasHit)
                                            {
                                                AttackCoordinate tempCoordainte = testShip.AttackGridSpace(attackedGridSpace);
                                            }
                                        }
                                    }

                                    // Swicth windows between players
                                    if (p_otherPlayer.Name == "ComputerPlayerTwo")
                                    {
                                        if (this.computerPlayerDifficulty2 ==
                                            StatusCodes.ComputerPlayerDifficulty.COMPUTER_DIFFICULTY_HARD)
                                        {
                                            ((ComputerPlayer)p_otherPlayer).AdvancedAttack(p_currentPlayer, this.RowRep);
                                            p_currentPlayerTurn = true;
                                            p_otherPlayerTurn = false;
                                        }
                                        else
                                        {
                                            ((ComputerPlayer)p_otherPlayer).CompPlayerAttack(p_currentPlayer, this.RowRep);
                                            p_currentPlayerTurn = true;
                                            p_otherPlayerTurn = false;
                                        }
                                    }

                                    p_currentPlayer.PlayerTurn = p_currentPlayerTurn;
                                    p_otherPlayer.PlayerTurn = p_otherPlayerTurn;
                                    targetCount++;
                                }

                                if (bombHasHit)
                                {
                                    p_currentPlayerTurn = true;
                                    p_otherPlayerTurn = false;
                                    Logger.Information("You can continue hitting!");
                                    p_currentPlayer.PlayerTurn = p_currentPlayerTurn;
                                    p_otherPlayer.PlayerTurn = p_otherPlayerTurn;
                                }
                            }
                            else
                            {
                                if (p_otherPlayer.Name != "ComputerPlayerTwo")
                                {
                                    Logger.Information("Switch Player. Next Player Turn.");
                                }
                            }
                        }
                        else
                        {
                            // If the current player's ship placement is locked, but the other player's ship placement is not locked, let the player know.
                            if (p_currentPlayer.IsLocked == true && p_otherPlayer.IsLocked == false)
                            {
                                Logger.Error("Error: The other player has not yet confirmed their ship placement.");
                            }
                            else
                            {
                                Logger.Error("Error: Please confirm your ship placement before proceeding to attack.");
                            }
                        }
                    });
                }
                else
                {
                    // add the drag over event for when ships are dragged over the cells only if the cell is deffense type
                    // Create a method when an object is drag over this left button
                    mainPlayerCell.DragOver += new DragEventHandler(delegate(object sender, DragEventArgs e)
                    {
                        // find the sender uid extracting the date of the event
                        string myWarshipUid = e.Data.GetData(DataFormats.StringFormat).ToString();

                        // iterate thru the collection of ships to find the sender element with matching uid
                        foreach (Ship myShip in p_currentPlayer.Playershipcollection)
                        {
                            // if the sender element uid matches then this is my element, then move it with the mouse
                            if (myWarshipUid == myShip.Uid)
                            {
                                double shipMaxX = (p_currentPlayerWindow.Width / 2) - myShip.Width + p_cellsize;
                                double shipMaxY = (p_currentPlayerWindow.Width / 2) - myShip.Height + p_cellsize;

                                // check this method to see if its ok to move ship to requested cell
                                int overlappingCrew = this.AllowShipMove_CrewOverlapCheck(myShip, mainPlayerCell, p_currentPlayer, 0);

                                if (overlappingCrew == 0)
                                {
                                    Point grabPos = e.GetPosition(p_currentPlayerWindow);
                                    if (grabPos.X < shipMaxX && grabPos.Y < shipMaxY)
                                    {
                                        // update the fixed crew members
                                        myShip.Ship_Crewmembers = myShip.SetCrewmembers(mainPlayerCell.TrackingID, 0);
                                        Canvas.SetTop(myShip, mainPlayerCell.Top_Comp_ParentTop);
                                        myShip.Top_Comp_ParentTop = mainPlayerCell.Top_Comp_ParentTop;
                                        Canvas.SetLeft(myShip, mainPlayerCell.Left_Comp_ParentLeft);
                                        myShip.Left_Comp_ParentLeft = mainPlayerCell.Left_Comp_ParentLeft;
                                        myShip.Captain = mainPlayerCell.TrackingID;
                                        this.Reportchange(myShip);

                                        Coordinate shipStartCoords = this.ConvertCanvasCoordinatesToGridCoordinates(grabPos.X, grabPos.Y);
                                        Coordinate shipEndCoords = this.ConvertCanvasCoordinatesToGridCoordinates(grabPos.X, grabPos.Y);
                                        if (myShip.HDirection == true)
                                        {
                                            shipEndCoords.XCoordinate += (short)((myShip.Width / p_cellsize) - 1);
                                        }
                                        else if (myShip.HDirection == false)
                                        {
                                            shipEndCoords.YCoordinate += (short)((myShip.Height / p_cellsize) - 1);
                                        }

                                        this.UpdateShipCoords(myShip, shipStartCoords, shipEndCoords);
                                    }
                                }
                            }
                        }
                    });
                }

                //// Add player 1 cells to the window grid
                p_currentPlayerWindow.Children.Add(mainPlayerCell);
            }
        }

        /// <summary>
        /// Sets the attack mode.
        /// </summary>
        /// <param name="passTrackingID">The tracking ID for the custom attack.</param>
        /// <param name="currentplayer">The current player that is attacking.</param>
        /// <returns>The "bomb" attack coordinates (advanced option).</returns>
        private List<int> SetAttack(int passTrackingID, Player currentplayer)
        {
            List<int> threebythree = new List<int>();
            int gridID = passTrackingID;

            threebythree.Add(gridID);

            // Will load extra hits if bomb is active
            if (currentplayer.PlayerBombactivated == true && currentplayer.BombCount >= 0)
            {
                if (gridID % this.RowRep == 0)
                {
                    threebythree.Add(gridID + 1);
                    if (gridID == (this.RowRep * (this.RowRep - 1)))
                    {
                        threebythree.Add(gridID - (this.RowRep - 1));
                        threebythree.Add(gridID - this.RowRep);
                    }
                    else if (gridID == 0)
                    {
                        threebythree.Add(gridID + (this.RowRep + 1));
                        threebythree.Add(gridID + this.RowRep);
                    }
                    else
                    {
                        threebythree.Add(gridID - (this.RowRep - 1));
                        threebythree.Add(gridID + (this.RowRep + 1));
                        threebythree.Add(gridID + this.RowRep);
                        threebythree.Add(gridID - this.RowRep);
                    }
                }
                else if (gridID % this.RowRep == (this.RowRep - 1))
                {
                    threebythree.Add(gridID - 1);
                    if (gridID == (this.RowRep * (this.RowRep - 1)) + (this.RowRep - 1))
                    {
                        threebythree.Add(gridID - (this.RowRep + 1));
                        threebythree.Add(gridID - this.RowRep);
                    }
                    else if (gridID == (this.RowRep - 1))
                    {
                        threebythree.Add(gridID + (this.RowRep - 1));
                        threebythree.Add(gridID + this.RowRep);
                    }
                    else
                    {
                        threebythree.Add(gridID - (this.RowRep + 1));
                        threebythree.Add(gridID + (this.RowRep - 1));
                        threebythree.Add(gridID + this.RowRep);
                        threebythree.Add(gridID - this.RowRep);
                    }
                }
                else
                {
                    threebythree.Add(gridID + 1);
                    threebythree.Add(gridID - 1);
                    threebythree.Add(gridID + this.RowRep);
                    threebythree.Add(gridID - this.RowRep);
                    threebythree.Add(gridID + (this.RowRep - 1));
                    threebythree.Add(gridID - (this.RowRep - 1));
                    threebythree.Add(gridID + (this.RowRep + 1));
                    threebythree.Add(gridID - (this.RowRep + 1));
                }
            }

            // reset the bomb
            currentplayer.PlayerBombactivated = false;

            return threebythree;
        }

        /// <summary>
        /// Return a confirmation to do a move if there is no ships overlapping.
        /// </summary>
        /// <param name="myShip">The ship to be checked.</param>
        /// <param name="mainPlayerCell">The former grid space of the captain.</param>
        /// <param name="p_currentPlayer">The current player.</param>
        /// <param name="dragTurn">The current drag turn.</param>
        /// <returns>The new captain of the ship.</returns>
        private int AllowShipMove_CrewOverlapCheck(Ship myShip, GridCell mainPlayerCell, Player p_currentPlayer, int dragTurn)
        {
            int overlapingCrewMembers = 0;
            int newCaptain = mainPlayerCell.TrackingID;
            List<int> newCrewmembers = new List<int>();
            newCrewmembers = myShip.SetCrewmembers(newCaptain, dragTurn);

            foreach (int newMember in newCrewmembers)
            {
                foreach (Ship shipCheck in p_currentPlayer.Playershipcollection)
                {
                    if (shipCheck.Uid != myShip.Uid)
                    {
                        foreach (int oldCrewMember in shipCheck.Ship_Crewmembers)
                        {
                            if (newMember == oldCrewMember)
                            {
                                overlapingCrewMembers++;
                            }
                        }
                    }
                }
            }

            return overlapingCrewMembers;
        }

        /// <summary>
        /// Declare a computer player's grid.
        /// </summary>
        /// <param name="p_currentPlayer">The current player.</param>
        /// <param name="p_otherPlayer">The other player relative to the current player.</param>
        /// <param name="p_playersCellRecords">The current player's cell records.</param>
        /// <param name="p_currentPlayerWindow">The current player's window.</param>
        /// <param name="p_cellsize">The cell size (in pixels).</param>
        private void DeclareComputerPlayerGrid(Player p_currentPlayer, Player p_otherPlayer, List<GridCell> p_playersCellRecords, Canvas p_currentPlayerWindow, double p_cellsize)
        {
            foreach (KeyValuePair<int, GridCell> mainPlayerPair in p_currentPlayer.Playergridsquarecollection)
            {
                int gridcellnumber = mainPlayerPair.Key;
                GridCell mainPlayerCell = mainPlayerPair.Value;

                //// Add player cells to the window grid
                p_currentPlayerWindow.Children.Add(mainPlayerCell);
            }
        }

        /// <summary>
        /// Declares the player's ships.
        /// </summary>
        /// <param name="p_currentPlayer">The current player.</param>
        /// <param name="p_currentPlayerWindow">The other player relative to the current player.</param>
        /// <param name="p_cellsize">The cell size (in pixels).</param>
        private void DeclarePlayerShips(Player p_currentPlayer, Canvas p_currentPlayerWindow, double p_cellsize)
        {
            foreach (Ship navyShip in p_currentPlayer.Playershipcollection)
            {
                // Create initial variables list of initial crew members dictionary cell records with ship information
                List<int> initialshipcrew = navyShip.SetCrewmembers(navyShip.Captain, 0);

                // Load all ships information to main dictionary
                foreach (int iniMember in initialshipcrew)
                {
                    // Load initial ship information to gamestatus dictionary report
                    string initialship_cell_bind = p_currentPlayer.PlayerID.ToString() + "-" + iniMember.ToString();
                    this.gameStatus[initialship_cell_bind].ButtonOccupied = true;
                    this.gameStatus[initialship_cell_bind].ShipContainedID = navyShip.Uid;
                    this.gameStatus[initialship_cell_bind].ShipContainedName = navyShip.ShipName;
                    this.gameStatus[initialship_cell_bind].ShipContainedType = navyShip.ShipType;
                    this.gameStatus[initialship_cell_bind].Crewmembers = navyShip.Ship_Crewmembers;
                }

                // add a right click event to each ship
                navyShip.MouseRightButtonDown += new MouseButtonEventHandler(delegate(object sender, MouseButtonEventArgs e)
                {
                    if (p_currentPlayer.IsLocked == false)
                    {
                        // create a cell to pass a cell to this method and return the possible crew members for this turn
                        GridCell checkTempCell = new GridCell(p_currentPlayer.PlayerID, 0, string.Empty, p_currentPlayer);
                        checkTempCell.TrackingID = navyShip.Captain;
                        int overlappingCrew = this.AllowShipMove_CrewOverlapCheck(navyShip, checkTempCell, p_currentPlayer, 1);

                        if (overlappingCrew == 0)
                        {
                            double shipMaxX = (p_currentPlayerWindow.Width / 2) - navyShip.Height;
                            double shipMaxY = (p_currentPlayerWindow.Width / 2) - navyShip.Width;

                            if (navyShip.Top_Comp_ParentTop <= shipMaxY && navyShip.LeftToParentLeft <= shipMaxX)
                            {
                                // allow the right turn movement
                                navyShip.RotateShip(true);

                                // Update the stored Crewmembers to the new captain values
                                navyShip.Ship_Crewmembers = navyShip.SetCrewmembers(navyShip.Captain, 0);

                                // call report change and update for main dictionary
                                this.Reportchange(navyShip);
                            }
                        }
                    }
                });

                // create a move move event for player 1 ships to attacch the rectangle to the mouse
                navyShip.MouseMove += new MouseEventHandler(delegate(object sender, MouseEventArgs e)
                {
                    if (p_currentPlayer.IsLocked == false)
                    {
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            string objectUniqueID = navyShip.Uid;
                            DragDrop.DoDragDrop(navyShip, objectUniqueID, DragDropEffects.Move);
                        }
                    }
                });

                // Add player 1 Ships to the window grid
                p_currentPlayerWindow.Children.Add(navyShip);
            }
        }

        /// <summary>
        /// Method to declare the computer's ships.
        /// </summary>
        /// <param name="p_currentPlayer">The current (computer) player.</param>
        /// <param name="p_currentPlayerWindow">The other player relative to the current player.</param>
        /// <param name="p_cellsize">The current cell size (in pixels).</param>
        private void DeclareComputerPlayerShips(Player p_currentPlayer, Canvas p_currentPlayerWindow, double p_cellsize)
        {
            foreach (Ship ship_1 in p_currentPlayer.Playershipcollection)
            {
                // Add player 1 Ships to the window grid
                p_currentPlayerWindow.Children.Add(ship_1);
            }
        }

        /// <summary>
        /// Set a confirm button visibility.
        /// </summary>
        /// <param name="canvasUid">The id of the canvas.</param>
        private void SetConfirmButtonVisibility(string canvasUid)
        {
            if ((canvasUid == "Player1Canvas" && this.player1.IsLocked == true) || (canvasUid == "Player2Canvas" && this.player2.IsLocked == true))
            {
                this.Confirm_Button.IsEnabled = false;
                this.AttackBtn.IsEnabled = true;
            }
            else if ((canvasUid == "Player1Canvas" && this.player1.IsLocked == false) || (canvasUid == "Player2Canvas" && this.player2.IsLocked == false))
            {
                this.Confirm_Button.IsEnabled = true;
                this.AttackBtn.IsEnabled = false;
            }
        }

        /// <summary>
        /// Confirm Button Appearance.
        /// </summary>
        private void ConfirmBtnApp()
        {
            if (this.playerWindow1.Visibility == Visibility.Visible && this.player1.IsLocked == false)
            {
                this.Confirm_Button.Visibility = Visibility.Visible;
            }
            else if (this.playerWindow2.Visibility == Visibility.Hidden && this.player2.IsLocked == false)
            {
                this.Confirm_Button.Visibility = Visibility.Visible;
            }
            else
            {
                this.Confirm_Button.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Set a confirm ship placement button.
        /// </summary>
        /// <param name="canvasUid">The id of the canvas.</param>
        private void ConfirmShipPlacement(string canvasUid)
        {
            if (canvasUid == "Player1Canvas")
            {
                this.player1.IsLocked = true;
                this.player1.LockShipsIntoPlace();
                this.player1.SetShipsToBoard(canvasUid, this.RowRep);
            }
            else if (canvasUid == "Player2Canvas")
            {
                this.player2.IsLocked = true;
                this.player2.LockShipsIntoPlace();
                this.player2.SetShipsToBoard(canvasUid, this.RowRep);
            }

            this.SetConfirmButtonVisibility(canvasUid);
        }

        /// <summary>
        /// Converts canvas coordinate into grid coordinate.
        /// </summary>
        /// <param name="canvasX">The canvas x coordinate.</param>
        /// <param name="canvasY">The canvas y coordinate.</param>
        /// <returns>The converted coordinates.</returns>
        private Coordinate ConvertCanvasCoordinatesToGridCoordinates(double canvasX, double canvasY)
        {
            Coordinate gridCoordinate = new Coordinate();

            gridCoordinate.XCoordinate = (short)((canvasX / this.Cellsize) + 1);
            gridCoordinate.YCoordinate = (short)((canvasY / this.Cellsize) + 1);

            return gridCoordinate;
        }

        /// <summary>
        /// Method to update the logical coordinates of the passed in <paramref name="shipToUpdate"/> object.
        /// </summary>
        /// <param name="shipToUpdate">The ship whose coordinates are to be updated.</param>
        /// <param name="shipStartCoords">The starting (top-left) coordinates of the ship.</param>
        /// <param name="shipEndCoords">The ending (bottom-right) coordinates of the ship.</param>
        private void UpdateShipCoords(Ship shipToUpdate, Coordinate shipStartCoords, Coordinate shipEndCoords)
        {
            // Logger.ConsoleInformation("New Ship Start Coords: " + shipStartCoords.XCoordinate.ToString() + ", " + shipStartCoords.YCoordinate.ToString());
            // Logger.ConsoleInformation("New Ship End Coords: " + shipEndCoords.XCoordinate.ToString() + ", " + shipEndCoords.YCoordinate.ToString());
            shipToUpdate.UpdateShipCoords(shipStartCoords, shipEndCoords);
        }

        /// <summary>
        /// Fire missiles.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments for the event.</param>
        private void AttackBtn_Click_1(object sender, RoutedEventArgs e)
        {
            this.ConfirmBtnApp();

            // goto to method to change the screen view
            this.SwitchPlayerWindows();
        }

        /// <summary>
        /// Set a confirm button click.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments for the event.</param>
        private void Confirm_Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.playerWindow1.Visibility == Visibility.Visible)
            {
                this.ConfirmShipPlacement("Player1Canvas");
            }
            else if (this.playerWindow2.Visibility == Visibility.Visible)
            {
                this.ConfirmShipPlacement("Player2Canvas");
            }
        }

        /// <summary>
        /// The event that is invoked after the game is loaded.
        /// </summary>
        /// <param name="sender">The sender that invoked the event.</param>
        /// <param name="e">The arguments passed to the event (in this case, none).</param>
        private void OnGameLoad(object sender, EventArgs e)
        {
           // foreach(Ship navyShip)
        }

        /// <summary>
        /// The click event for the Save Game button.
        /// </summary>
        /// <param name="sender">The sender that invoked the event.</param>
        /// <param name="e">The arguments passed to the event.</param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Call Save method from SaveLoad.cs
            this.savingAndLoading.SaveGame(ref this.gameStatus);
        }

        /// <summary>
        /// Click event for the Load Game button.
        /// </summary>
        /// <param name="sender">The sender that invoked the event.</param>
        /// <param name="e">The arguments passed to the event.</param>
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            // Call Save method from SaveLoad.cs
            this.savingAndLoading.LoadGame(ref this.gameStatus);
        }

        /// <summary>
        /// report change to main dictionary.
        /// </summary>
        /// <param name="ship">this is my ship.</param>
        private void Reportchange(Ship ship)
        {
            foreach (KeyValuePair<string, GridCell> report in this.gameStatus)
            {
                string keytext = report.Key;
                GridCell gridCell = report.Value;

                if (ship.Uid == gridCell.ShipContainedID)
                {
                    gridCell.Crewmembers.Clear();
                    gridCell.ButtonOccupied = false;
                    gridCell.ShipContainedID = string.Empty;
                    gridCell.ShipContainedName = string.Empty;
                    gridCell.ShipContainedType = 0;
                }
            }

            // update main game dictionary
            this.Updatechange(ship);
        }

        /// <summary>
        /// update ship members in main dictionary.
        /// </summary>
        /// <param name="ship">this is my ship.</param>
        private void Updatechange(Ship ship)
        {
            foreach (int shipmember in ship.Ship_Crewmembers)
            {
                string reportcellID_ship = ship.PlayerID.ToString() + "-" + shipmember.ToString();

                foreach (KeyValuePair<string, GridCell> report in this.gameStatus)
                {
                    string reportcellID_m = report.Key;
                    GridCell gridCell = report.Value;

                    if (reportcellID_m == reportcellID_ship)
                    {
                        gridCell.Crewmembers = ship.Ship_Crewmembers;
                        gridCell.ButtonOccupied = true;
                        gridCell.ShipContainedID = ship.Uid;
                        gridCell.ShipContainedName = ship.ShipName;
                        gridCell.ShipContainedType = ship.ShipType;
                    }
                }
            }
        }

        /// <summary>
        /// Method that shows the report dictionary.
        /// </summary>
        /// <param name="sender">The sender that invoked the event.</param>
        /// <param name="e">The arguments passed to the event.</param>
        private void Reportgame_Click(object sender, RoutedEventArgs e)
        {
            for (int j = 1; j <= 2; j++)
            {
                for (int i = 1; i <= 5; i++)
                {
                    foreach (KeyValuePair<string, GridCell> report in this.gameStatus)
                    {
                        string keytext = report.Key;
                        GridCell gridCell = report.Value;

                        if (gridCell.Crewmembers.Count > 0 && gridCell.ShipContainedType == i && gridCell.PlayerID == j)
                        {
                            Logger.ConsoleInformation("key number (" + keytext + ")" + " contains " + gridCell.ShipContainedName + "_Player " + gridCell.PlayerID.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The button that loops though all of the GridCells and changes their contents to random values.
        /// </summary>
        /// <param name="sender">The sender that invoked the event.</param>
        /// <param name="e">The event arguments passed to the event.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (KeyValuePair<string, GridCell> pair in this.gameStatus)
            {
                pair.Value.Background = Brushes.Red;

                /*
                if (pair.Value.CellAttackStatus == StatusCodes.AttackStatus.ATTACKED_HIT)
                {

                    Logger.ConsoleInformation(pair.Key);
                }
                */
            }
        }

        /// <summary>
        /// The button that saves the game.
        /// </summary>
        /// <param name="sender">The sender that invoked the event.</param>
        /// <param name="e">The event arguments passed to the event.</param>
        private void BtnSaveGame_Click(object sender, RoutedEventArgs e)
        {
            if (this.player1.IsLocked == true && this.player2.IsLocked == true)
            {
                string path = this.player1.Name + "-" + this.player2.Name + "_" + DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss") + ".txt";
                string pathList = "SavedGamesList.txt";
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(DateTime.Now.ToString("G"));
                        sw.WriteLine(this.player1.Name);
                        for (int i = 0; i < this.RowRep; i++)
                        {
                            for (int j = 0; j < this.RowRep; j++)
                            {
                                sw.Write(this.player1.Board[i, j] + ',');
                            }

                            sw.Write("\n");
                        }

                        sw.WriteLine(this.player2.Name);
                        for (int i = 0; i < this.RowRep; i++)
                        {
                            for (int j = 0; j < this.RowRep; j++)
                            {
                                sw.Write(this.player2.Board[i, j] + ',');
                            }

                            sw.Write("\n");
                        }

                        using (StreamWriter sl = File.AppendText(pathList))
                        {
                            sl.WriteLine(path);
                        }

                        MainWindow mw = Application.Current.MainWindow as MainWindow;
                        mw.AddItem(path);

                        Logger.Information("The game was saved.");
                    }
                }
            }
            else
            {
                Logger.Error("Error: Both players must first confirm their ship placement before saving a game!");
            }
        }

        /// <summary>
        /// Method that declares the player grid.
        /// </summary>
        /// <param name="p_currentPlayer">The current player to be declared.</param>
        /// <param name="p_otherPlayer">The other player relative to the current player.</param>
        /// <param name="p_playersCellRecords">The current player's cell records.</param>
        /// <param name="p_currentPlayerWindow">The current player's window.</param>
        /// <param name="p_cellsize">The cell size (in pixels).</param>
        private void DeclarePlayerGridFromFile(Player p_currentPlayer, Player p_otherPlayer, List<GridCell> p_playersCellRecords, Canvas p_currentPlayerWindow, double p_cellsize)
        {
            bool p_currentPlayerTurn = false;
            bool p_otherPlayerTurn = true;

            foreach (KeyValuePair<int, GridCell> mainPlayerPair in p_currentPlayer.Playergridsquarecollection)
            {
                // Create initial variables for interior iterations
                int gridcellnumber = mainPlayerPair.Key;
                GridCell mainPlayerCell = mainPlayerPair.Value;

                // Load in initial grids to game dictionary control for reports and outputs
                string gameStatusdictionarykey = p_currentPlayer.PlayerID.ToString() + "-" + mainPlayerPair.Value.TrackingID.ToString();
                this.gameStatus.Add(gameStatusdictionarykey, mainPlayerPair.Value);

                // add a click event for all cells in Player 1 grid only if the button is attack type
                if (mainPlayerCell.OffenseButton == true)
                {
                    // add click event
                    mainPlayerCell.Click += new RoutedEventHandler(delegate(object sender, RoutedEventArgs e)
                    {
                        if (p_currentPlayer.PlayerTurn)
                        {
                            // go check the list of buttons for player two and change the status for them
                            foreach (KeyValuePair<int, GridCell> otherPlayerPair in p_otherPlayer.Playergridsquarecollection)
                            {
                                int otherPlayercellnumber = otherPlayerPair.Key;
                                GridCell otherPlayerPlayerCell = otherPlayerPair.Value;

                                // turn off buttons on the enemy grid(player two left side)only if it is a defense button
                                if (mainPlayerCell.Uid == otherPlayerPlayerCell.Uid &&
                                        otherPlayerPlayerCell.OffenseButton == false)
                                {
                                    // make changes to player two grid
                                    otherPlayerPlayerCell.Background = Brushes.Red;
                                    otherPlayerPlayerCell.Content = "X";
                                    otherPlayerPlayerCell.Stricked = 1;
                                    otherPlayerPlayerCell.AllowDrop = false;

                                    if (otherPlayerPlayerCell.ShipContainedName != string.Empty)
                                    {
                                        Logger.ConsoleInformation("You have damaged my " + otherPlayerPlayerCell.ShipContainedName);
                                        otherPlayerPlayerCell.CellAttackStatus = StatusCodes.AttackStatus.ATTACKED_HIT;
                                    }
                                    else
                                    {
                                        Logger.ConsoleInformation("You will never succeed");
                                        otherPlayerPlayerCell.CellAttackStatus = StatusCodes.AttackStatus.ATTACKED_NOT_HIT;
                                    }

                                    int rowNum;
                                    if ((gridcellnumber - (this.RowRep * this.RowRep)) % this.RowRep == 0)
                                    {
                                        rowNum = ((gridcellnumber - (this.RowRep * this.RowRep)) / this.RowRep) - 1;
                                    }
                                    else
                                    {
                                        rowNum = (gridcellnumber - (this.RowRep * this.RowRep)) / this.RowRep;
                                    }

                                    int colNum = ((gridcellnumber - (this.RowRep * this.RowRep)) - (((gridcellnumber - (this.RowRep * this.RowRep)) / this.RowRep) * this.RowRep)) - 1;
                                    if (colNum == -1)
                                    {
                                        colNum = this.RowRep - 1;
                                    }

                                    string letterAttackGrid = p_otherPlayer.Board[rowNum, colNum];
                                    if (letterAttackGrid != "O" && letterAttackGrid != "H" && letterAttackGrid != "M")
                                    {
                                        p_otherPlayer.Board[rowNum, colNum] = "H";
                                        mainPlayerCell.Background = Brushes.Green;
                                        mainPlayerCell.Content = "H";
                                        mainPlayerCell.IsEnabled = false;
                                        mainPlayerCell.Stricked = 1;
                                        mainPlayerCell.AllowDrop = false;
                                        if (this.optionPlayerTurnHits)
                                        {
                                            p_currentPlayerTurn = true;
                                            p_otherPlayerTurn = false;
                                        }
                                    }
                                    else
                                    {
                                        mainPlayerCell.Visibility = Visibility.Hidden;
                                        p_otherPlayer.Board[rowNum, colNum] = "M";
                                    }
                                }
                            }

                            Coordinate attackedGridSpace = new Coordinate((short)mainPlayerCell.ColNum, (short)mainPlayerCell.RowNum);

                            Logger.ConsoleInformation("Row Number: " + mainPlayerCell.RowNum);
                            Logger.ConsoleInformation("Column Number: " + mainPlayerCell.ColNum);

                            foreach (Ship testShip in p_otherPlayer.Playershipcollection)
                            {
                                // Logger.Information(testShip.ShipStartCoords.XCoordinate.ToString() + " "+ testShip.ShipStartCoords.YCoordinate.ToString());
                                AttackCoordinate tempCoordainte = testShip.AttackGridSpace(attackedGridSpace);
                            }

                            p_currentPlayer.PlayerTurn = p_currentPlayerTurn;
                            p_otherPlayer.PlayerTurn = p_otherPlayerTurn;
                        }
                        else
                        {
                            Logger.Information("Switch Player. Next Player Turn.");
                        }
                    });
                }
                //// Add player 1 cells to the window grid
                p_currentPlayerWindow.Children.Add(mainPlayerCell);
            }
        }

        /// <summary>
        /// Click event for <see cref="BombLoader"/>.
        /// </summary>
        /// <param name="sender">The sender that invoked the event.</param>
        /// <param name="e">The arguments passed to the event.</param>
        private void BombLoader_Click(object sender, RoutedEventArgs e)
        {
            if (this.playerWindow1.Visibility == Visibility.Visible)
            {
                this.player1.PlayerBombactivated = true;
                this.player1.BombCount -= 1;
                this.BombLoader.Visibility = Visibility.Hidden;
                Logger.Information("Please choose the spot to drop the bomb! Bombs Left: " + this.player1.BombCount);
            }
            else if (this.playerWindow2.Visibility == Visibility.Visible)
            {
                this.player2.PlayerBombactivated = true;
                this.player2.BombCount -= 1;
                this.BombLoader.Visibility = Visibility.Hidden;
                Logger.Information("Please choose the spot to drop the bomb! Bombs Left: " + this.player2.BombCount);
            }
        }

        /// <summary>
        /// The loaded event for the <see cref="GameWindow"/> class.
        /// </summary>
        /// <param name="sender">The sender that invoked the event.</param>
        /// <param name="e">The arguments passed to the event.</param>
        private void GameWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.BombLoader.Visibility = Visibility.Hidden;
        }
    }
}
