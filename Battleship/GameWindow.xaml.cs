﻿//-----------------------------------------------------------------------
// <copyright file="GameWindow.xaml.cs" company="Team">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------
namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for GameWindow.xaml
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
        public int RowRep = 10;

        /// <summary>
        /// The list of player cell grids.
        /// </summary>
        public List<GridCell> PlayersCellRecords;

        /// <summary>
        /// The game type.
        /// </summary>
        private StatusCodes.GameType gameType;

        /// <summary>
        /// The player 1 screen.
        /// </summary>
        private bool screenPlayerOne = true;

        /// <summary>
        /// Is screen for player 1 locked or not.
        /// </summary>
        private bool isLocked = false;

        /// <summary>
        /// Is screen for player 2 locked or not.
        /// </summary>
        private bool isLocked2 = false;

        /// <summary>
        /// The player 1.
        /// </summary>
        private Player player1;

        /// <summary>
        /// The player 2.
        /// </summary>
        private Player player2;

        /// <summary>
        /// The computer player 1.
        /// </summary>
        private ComputerPlayer computerPlayer1;

        /// <summary>
        /// The computer player 1.
        /// </summary>
        private ComputerPlayer computerPlayer2;

        /// <summary>
        /// The player 1 window.
        /// </summary>
        private Canvas playerWindow1;

        /// <summary>
        /// The player 2 window.
        /// </summary>
        private Canvas playerWindow2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameWindow" /> class.
        /// </summary>
        /// <param name="gameType"> This is the game type.</param>
        public GameWindow(StatusCodes.GameType gameType)
        {
            this.PlayersCellRecords = new List<GridCell>();

            this.InitializeComponent();

            this.gameType = gameType;

            this.isLocked = false;
            this.isLocked2 = false;

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
                default:
                    this.Loaded += this.StartGame;
                    break;
            }
        }

        /// <summary>
        ///  Gets a value indicating whether screen player1 is visible or not
        /// </summary>
        public bool Switch
        {
            get { return this.screenPlayerOne; }
        }

        /// <summary>
        /// Start a player to player game.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments for the event.</param>
        public void StartPlayerToPlayerGame(object sender, EventArgs e)
        {
            this.StartGame(sender, e);
        }

        /// <summary>
        /// Start a player to computer game.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments for the event.</param>
        public void StartPlayerToComputerGame(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Start a computer to computer game.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments for the event.</param>
        public void StartComputerToComputerGame(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Start the game.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments for the event.</param>
        public void StartGame(object sender, EventArgs e)
        {
            // start player one label visible
            PlayerOnelabel.Visibility = Visibility.Visible;
            PlayerTwolabel.Visibility = Visibility.Hidden;

            // Create Players with their cells and their ships and grids colors
            // 1 = Black,2=dark blue,3=magenta,4=lightseagreen,5=purple,6=white,standard cadet blue
            this.player1 = new Player(1, "PlayerOne", this.Cellsize, this.RowRep, 1, 3);
            this.player2 = new Player(2, "PlayerOne", this.Cellsize, this.RowRep, 3, 6);

            // Create two Canvas to place the player elements on them 
            this.playerWindow1 = new Canvas();
            this.playerWindow1.HorizontalAlignment = HorizontalAlignment.Center;
            this.playerWindow1.VerticalAlignment = VerticalAlignment.Center;
            this.playerWindow1.Uid = "Player1Canvas";
            this.playerWindow1.Width = (this.Cellsize * this.RowRep) * 2;

            this.playerWindow2 = new Canvas();
            this.playerWindow2.HorizontalAlignment = HorizontalAlignment.Center;
            this.playerWindow2.VerticalAlignment = VerticalAlignment.Center;
            this.playerWindow2.Uid = "Player2Canvas";
            this.playerWindow2.Width = (this.Cellsize * this.RowRep) * 2;
            this.playerWindow2.Visibility = Visibility.Hidden; // load this canvas hidden for player 2

            //////Offense buttons for player one actions
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Load all cells from player one to the player one canvas
            foreach (GridCell player_1_Offense_button in this.player1.Playergridsquarecollection)
            {
                this.PlayersCellRecords.Add(player_1_Offense_button);

                // add a click event for all cells in Player 1 grid only if the button is attack type
                if (player_1_Offense_button.OffenseButton == true)
                {
                    // add click event
                    player_1_Offense_button.Click += new RoutedEventHandler(button_Click); // base
                    void button_Click(object sender, System.EventArgs e)
                    {
                        // go check the list of buttons for player two and change the status for them
                        foreach (GridCell player_2_deffense_button in this.player2.Playergridsquarecollection)
                        {
                            // turn off buttons on the enemy grid(player two left side)only if it is a defense button
                            if (player_1_Offense_button.Uid == player_2_deffense_button.Uid && player_2_deffense_button.OffenseButton == false)
                            {
                                // make changes to player two grid
                                player_2_deffense_button.Background = Brushes.Red;
                                player_2_deffense_button.Content = "X";
                                player_2_deffense_button.Stricked = 1;
                                player_2_deffense_button.AllowDrop = false;

                                // make changes to player one grid
                                player_1_Offense_button.Visibility = Visibility.Hidden;
                            }
                        }

                        Coordinate attackedGridSpace = new Coordinate((short)player_1_Offense_button.ColNum, (short)player_1_Offense_button.RowNum);

                        Logger.ConsoleInformation("Row Number: " + player_1_Offense_button.RowNum);
                        Logger.ConsoleInformation("Column Number: " + player_1_Offense_button.ColNum);

                        foreach (Ship testShip in this.player1.Playershipcollection)
                        {
                            AttackCoordinate tempCoordainte = testShip.AttackGridSpace(attackedGridSpace);
                        }

                        // Swicth windows between players 
                        this.SwitchPlayerWindows();
                    }
                }
                else 
                {
                    // add the drag over event for when ships are dragged over the cells only if the cell is deffense type
                    // Create a method when an object is drag over this left button
                    player_1_Offense_button.DragOver += new DragEventHandler(Onbuttondragover);
                    void Onbuttondragover(object sender, DragEventArgs e)
                    {
                        // find the sender uid extracting the date of the event
                        string myWarshipUid = e.Data.GetData(DataFormats.StringFormat).ToString();

                        // iterate thru the collection of ships to find the sender element with matching uid
                        foreach (Ship ship in this.player1.Playershipcollection)
                        {
                            // if the sender element uid matches then this is my element, then move it with the mouse
                            if (myWarshipUid == ship.Uid)
                            {
                                Point grabPos = e.GetPosition(this.playerWindow1); // find the position of the mouse compared to the canvas for player one
                                double shipMaxX = (this.playerWindow1.Width / 2) - ship.Width + this.Cellsize;
                                double shipMaxY = (this.playerWindow1.Width / 2) - ship.Height + this.Cellsize;
                                 if (grabPos.X < shipMaxX && grabPos.Y < shipMaxY)
                                 {
                                   Canvas.SetTop(ship, player_1_Offense_button.Top_Comp_ParentTop);
                                    ship.Top_Comp_ParentTop = player_1_Offense_button.Top_Comp_ParentTop;
                                   Canvas.SetLeft(ship, player_1_Offense_button.Left_Comp_ParentLeft);
                                    ship.Left_Comp_ParentLeft = player_1_Offense_button.Left_Comp_ParentLeft;

                                    Coordinate shipStartCoords = this.ConvertCanvasCoordinatesToGridCoordinates(grabPos.X, grabPos.Y);
                                    Coordinate shipEndCoords = this.ConvertCanvasCoordinatesToGridCoordinates(grabPos.X, grabPos.Y);

                                    if (ship.HDirection == true)
                                    {
                                        shipEndCoords.XCoordinate += (short)((ship.Width / this.Cellsize) - 1);
                                    }
                                    else if (ship.HDirection == false)
                                    {
                                        shipEndCoords.YCoordinate += (short)((ship.Height / this.Cellsize) - 1);
                                    }

                                    this.UpdateShipCoords(ship, shipStartCoords, shipEndCoords);
                                }
                            }
                        }
                    }
                }
                //// Add player 1 cells to the window grid
                this.playerWindow1.Children.Add(player_1_Offense_button);
            }

            //// Ships for player one actions
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //// Load all ships from player one to the player one canvas
            
                foreach (Ship ship_1 in this.player1.Playershipcollection)
                {
                    // create a move move event for player 1 ships to attacch the rectangle to the mouse
                    ship_1.MouseMove += new MouseEventHandler(Warship_MouseMove);
                    ship_1.MouseRightButtonDown += new MouseButtonEventHandler(Warship_MouseRightButtonDown);

                    void Warship_MouseMove(object sender, MouseEventArgs e)
                    {
                        if (this.isLocked == false)
                        {
                            if (e.LeftButton == MouseButtonState.Pressed)
                            {
                            string objectUniqueID = ship_1.Uid;
                            Point grabPos = e.GetPosition(ship_1);
                            Canvas.SetTop(ship_1, grabPos.Y);
                            Canvas.SetLeft(ship_1, grabPos.X);
                            DragDrop.DoDragDrop(ship_1, objectUniqueID, DragDropEffects.Move);
                            }
                        }
                    }

                    // Rotate ships for player one with rigth click(refer to ship class constructor)
                    void Warship_MouseRightButtonDown(object sender, System.EventArgs e)
                    {
                        if (this.isLocked == false)
                        {
                           if (ship_1.HDirection == true)
                           {
                              double shipMaxY = (this.playerWindow1.Width / 2) - (this.Cellsize * 2);
                              double shipMaxX = this.playerWindow1.Width / 2;
                              if (ship_1.Top_Comp_ParentTop < shipMaxY && ship_1.Left_Comp_ParentLeft < shipMaxX)
                              {
                                ship_1.Rotateship(true);
                              }
                           }
                           else 
                           {
                              double shipMaxY = this.playerWindow1.Width / 2;
                              double shipMaxX = (this.playerWindow1.Width / 2) - (this.Cellsize * 2);
                              if (ship_1.Top_Comp_ParentTop < shipMaxY && ship_1.Left_Comp_ParentLeft < shipMaxX)
                              {
                                ship_1.Rotateship(true);
                              }
                           }
                        }
                    }

                    // Add player 1 Ships to the window grid
                    this.playerWindow1.Children.Add(ship_1);
                }

            // offense buttons for player two actions 
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Load all cells from player two to the player two canvas
            foreach (GridCell player_2_Offense_button in this.player2.Playergridsquarecollection)
            {
                this.PlayersCellRecords.Add(player_2_Offense_button);

                // add a click event for all cells in Player 1 grid only if the button is attack type
                if (player_2_Offense_button.OffenseButton == true)
                {
                    // add click event
                    player_2_Offense_button.Click += new RoutedEventHandler(button_Click); // base
                    void button_Click(object sender, System.EventArgs e)
                    {
                        // go check the list of buttons for player one and change the status for them
                        foreach (GridCell player_1_deffense_button in this.player1.Playergridsquarecollection)
                        {
                            // turn off buttons on the enemy grid(player two left side)only if it is a defense button
                            if (player_2_Offense_button.Uid == player_1_deffense_button.Uid && player_1_deffense_button.OffenseButton == false)
                            {
                                // make changes to player two grid
                                player_1_deffense_button.Background = Brushes.Red;
                                player_1_deffense_button.Content = "X";
                                player_1_deffense_button.Stricked = 1;
                                player_1_deffense_button.AllowDrop = false;

                                // make changes to player one grid
                                player_2_Offense_button.Visibility = Visibility.Hidden;
                            }
                        }

                        Coordinate attackedGridSpace = new Coordinate((short)player_2_Offense_button.ColNum, (short)player_2_Offense_button.RowNum);

                        Logger.ConsoleInformation("Row Number: " + player_2_Offense_button.RowNum);
                        Logger.ConsoleInformation("Column Number: " + player_2_Offense_button.ColNum);

                        foreach (Ship testShip in this.player2.Playershipcollection)
                        {
                            AttackCoordinate tempCoordainte = testShip.AttackGridSpace(attackedGridSpace);
                        }

                        // Swicth windows between players
                        this.SwitchPlayerWindows();
                    }
                }
                else 
                {
                    // add the drag over event for when ships are dragged over the cells only if the cell is deffense type
                    // Create a method when an object is drag over this left button
                    player_2_Offense_button.DragOver += new DragEventHandler(Onbuttondragover);

                        void Onbuttondragover(object sender, DragEventArgs e)
                        {
                            // find the sender uid extracting the date of the event
                            string myWarshipUid = e.Data.GetData(DataFormats.StringFormat).ToString();

                            // iterate thru the collection of ships to find the sender element with matching uid
                            foreach (Ship ship in this.player2.Playershipcollection)
                            {
                                if (this.isLocked2 == false)
                                {
                                    // if the sender element uid matches then this is my element, then move it with the mouse
                                    if (myWarshipUid == ship.Uid)
                                    {
                                        Point grabPos = e.GetPosition(this.playerWindow2); // find the position of the mouse compared to the canvas for player two
                                        double shipMaxX = (this.playerWindow2.Width / 2) - ship.Width + this.Cellsize;
                                        double shipMaxY = (this.playerWindow2.Width / 2) - ship.Height + this.Cellsize;
                                       if (grabPos.X < shipMaxX && grabPos.Y < shipMaxY)
                                       {
                                        Canvas.SetTop(ship, player_2_Offense_button.Top_Comp_ParentTop);
                                        ship.Top_Comp_ParentTop = player_2_Offense_button.Top_Comp_ParentTop;
                                        Canvas.SetLeft(ship, player_2_Offense_button.Left_Comp_ParentLeft);
                                        ship.Left_Comp_ParentLeft = player_2_Offense_button.Left_Comp_ParentLeft;

                                        Coordinate shipStartCoords = this.ConvertCanvasCoordinatesToGridCoordinates(grabPos.X, grabPos.Y);
                                        Coordinate shipEndCoords = this.ConvertCanvasCoordinatesToGridCoordinates(grabPos.X, grabPos.Y);

                                        if (ship.HDirection == true)
                                        {
                                            shipEndCoords.XCoordinate += (short)((ship.Width / this.Cellsize) - 1);
                                        }
                                        else if (ship.HDirection == false)
                                        {
                                            shipEndCoords.YCoordinate += (short)((ship.Height / this.Cellsize) - 1);
                                        }

                                        this.UpdateShipCoords(ship, shipStartCoords, shipEndCoords);
                                    }
                                    }
                                }
                            }
                        }
                }
                //// Add player 2 cells to the window grid
                this.playerWindow2.Children.Add(player_2_Offense_button);
            }

            // player two ships actions
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Load all ships from player two to the player two canvas
            foreach (Ship ship_2 in this.player2.Playershipcollection)
            {
                // create a move move event for player 2 ships to attacch the rectangle to the mouse
                ship_2.MouseMove += new MouseEventHandler(Warship_MouseMove);
                ship_2.MouseRightButtonDown += new MouseButtonEventHandler(Warship_MouseRightButtonDown);

                void Warship_MouseMove(object sender, MouseEventArgs e)
                {
                    if (this.isLocked2 == false)
                    {
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            string objectUniqueID = ship_2.Uid;
                            Point grabPos = e.GetPosition(ship_2);
                            Canvas.SetTop(ship_2, grabPos.Y);
                            Canvas.SetLeft(ship_2, grabPos.X);
                            DragDrop.DoDragDrop(ship_2, objectUniqueID, DragDropEffects.Move);
                        }
                    }
                }

                // Rotate ships for player two with rigth click(refer to ship class constructor)
                void Warship_MouseRightButtonDown(object sender, System.EventArgs e)
                {
                    if (this.isLocked2 == false)
                    {
                        if (ship_2.HDirection == true)
                        {
                            double shipMaxY = (this.playerWindow2.Width / 2) - (this.Cellsize * 2);
                            double shipMaxX = this.playerWindow2.Width / 2;
                            if (ship_2.Top_Comp_ParentTop < shipMaxY && ship_2.Left_Comp_ParentLeft < shipMaxX)
                            {
                                ship_2.Rotateship(true);
                            }
                        }
                        else
                        {
                            double shipMaxY = this.playerWindow2.Width / 2;
                            double shipMaxX = (this.playerWindow2.Width / 2) - (this.Cellsize * 2);
                            if (ship_2.Top_Comp_ParentTop < shipMaxY && ship_2.Left_Comp_ParentLeft < shipMaxX)
                            {
                                ship_2.Rotateship(true);
                            }
                        }
                    }
                }

                // Add player 2 Ships to the window grid
                this.playerWindow2.Children.Add(ship_2);
            }

            // load both canvas to this window grid
            this.Maingrid.Children.Add(this.playerWindow1);
            this.Maingrid.Children.Add(this.playerWindow2);

            this.Show();
        }

        /// <summary>
        /// Make the windows visible with Attack button
        /// </summary>
        public void SwitchPlayerWindows()
        {
            if (this.screenPlayerOne == true)
            {
                // change the visual to off for player 1 and turn on the vissible to player two canvas
                this.screenPlayerOne = false;
                PlayerOnelabel.Visibility = Visibility.Hidden;
                PlayerTwolabel.Visibility = Visibility.Visible;

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
            }
            else
            {
                // if the screen player one entert the method in false condition
                // change the visual to on for player 1 and turn off the vissible to player two canvas
                this.screenPlayerOne = true;
                PlayerOnelabel.Visibility = Visibility.Visible;
                PlayerTwolabel.Visibility = Visibility.Hidden;

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
            }
        }

        /// <summary>
        /// Set a confirm button visibility.
        /// </summary>
        /// <param name="canvasUid">The id of the canvas.</param>
        private void SetConfirmButtonVisibility(string canvasUid)
        {
            if ((canvasUid == "Player1Canvas" && this.isLocked == true) || (canvasUid == "Player2Canvas" && this.isLocked2 == true))
            {
                this.Confirm_Button.IsEnabled = false;
            }
            else if ((canvasUid == "Player1Canvas" && this.isLocked) == false || (canvasUid == "Player2Canvas" && this.isLocked2 == false))
            { 
                this.Confirm_Button.IsEnabled = true;
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
                this.isLocked = true;
            }
            else if (canvasUid == "Player2Canvas")
            {
                this.isLocked2 = true;
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
            Logger.ConsoleInformation("New Ship Start Coords: " + shipStartCoords.XCoordinate.ToString() + ", " + shipStartCoords.YCoordinate.ToString());
            Logger.ConsoleInformation("New Ship End Coords: " + shipEndCoords.XCoordinate.ToString() + ", " + shipEndCoords.YCoordinate.ToString());
            shipToUpdate.UpdateShipCoords(shipStartCoords, shipEndCoords);
        }

        /// <summary>
        /// Fire missiles
        /// </summary>
        /// <param name="sender">The object that initiated the event</param>
        /// <param name="e">The event arguments for the event.</param>
        private void AttackBtn_Click_1(object sender, RoutedEventArgs e)
        {
            // goto to method to change the screen view
            this.SwitchPlayerWindows();
        }

        /// <summary>
        /// Set a report button click.
        /// </summary>
        /// <param name="sender">The object that initiated the event</param>
        /// <param name="e">The event arguments for the event.</param>
        private void Reportbtn_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Set a confirm button click.
        /// </summary>
        /// <param name="sender">The object that initiated the event</param>
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
    }
}
