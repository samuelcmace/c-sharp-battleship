﻿//-----------------------------------------------------------------------
// <copyright file="Player.cs" company="David Fedchuk, Gerson Eliu Sorto Flores, Lincoln Kaszynski, Sanaaia Okhlopkova, and Samuel Mace">
//     MIT License, 2022 (https://mit-license.org).
// </copyright>
//-----------------------------------------------------------------------
namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Interaction logic for Player. This class is a canvas that will load with a list of grid squares and a list of ships attached as a property.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Encapsulation not taught.")]
    public class Player
    {
        /// <summary>
        /// The list of player's ships.
        /// </summary>
        protected List<Ship> playerShips = new List<Ship>();

        /// <summary>
        /// Tracking changes event dictionary.
        /// </summary>
        protected Dictionary<int, GridCell> playerGridCells = new Dictionary<int, GridCell>();

        /// <summary>
        /// Used to contain the board information.
        /// </summary>
        protected string[,] board;

        /// <summary>
        /// Whether or not the current player's ships are locked.
        /// </summary>
        private bool isLocked;

        /// <summary>
        /// Whether or not the current player's ships are locked.
        /// </summary>
        private bool playerTurn;

        /// <summary>
        /// The player's ID.
        /// </summary>
        private int playerID;

        /// <summary>
        /// The player's bomb count.
        /// </summary>
        private int bombCount;

        /// <summary>
        /// The player's bomb trigger.
        /// </summary>
        private bool playerBombactivated;

        /// <summary>
        /// The player's name.
        /// </summary>
        private string name;

        /// <summary>
        /// Whether or not the current player is a winner.
        /// </summary>
        private bool winner = false;

        /// <summary>
        /// The count of player's ships.
        /// </summary>
        private int shipCount;

        /// <summary>
        /// The count of player's ships per turn.
        /// </summary>
        private int shipCountTurn;

        /// <summary>
        /// Initializes a new instance of the <see cref="Player" /> class.
        /// </summary>
        /// <param name="player_ID"> This is the ID of the player. </param>
        /// <param name="player_Name"> this is the name of the Player.</param>
        /// <param name="gridcellSize"> This is the size in pixels for the grid square dimension.</param>
        /// <param name="maxCol"> This is the max number of columns requested at the moment pf loading.</param>
        /// <param name="buttoncolorForDeffense"> this is the color for the button created, refer to Custom button class(switch case in constructor).</param>
        /// <param name="buttoncolorForOffense"> This is the side of the screen to load the canvas, if left then reversed count, if right then incremental from one.</param>
        /// <param name="shiptypes">The types of ships to be instantiated (advanced option).</param>
        /// <param name="p_bombcount">How many "bomb" moves a player has (advanced option).</param>
        public Player(int player_ID, string player_Name, double gridcellSize, int maxCol, int buttoncolorForDeffense, int buttoncolorForOffense, List<int> shiptypes, int p_bombcount)
        {
            this.bombCount = p_bombcount;
            this.isLocked = false;
            this.playerTurn = true;
            this.shipCount = shiptypes.Count;
            this.shipCountTurn = 1;

            // set the fixed properties
            this.name = player_Name;
            this.playerID = player_ID;

            // List of Alphabet letters to give names to gridcells
            string[] capital_letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            this.board = new string[maxCol, maxCol];

            for (int k = 0; k < maxCol; k++)
            {
                for (int j = 0; j < maxCol; j++)
                {
                    this.board[k, j] = "O";
                }
            }

            // set the player cstom button(grid) collection list if it is placed on the right side of screen
            // Make two grids of buttons for each player one for defense and one for offense(Defense buttons wnot have click events)
            for (int grids = 0; grids < 2; grids++)
            {
                // determine the grid type and assign a flag to the buttons if they are defense or offense.
                if (grids == 0)
                {
                    // will iterate the number of columns requested
                    for (int col = 0; col < maxCol; col++)
                    {
                        // will iterate the number of rows(same as the columns) requested
                        for (int row = 0; row < maxCol; row++)
                        {
                            // create a name for this button will result in a string(10.0001, this is elevated to the ten thounsans to allow a high number of buttons without the repeating of the name)
                            double rowthousand = col + 1 + ((row + 1) * 0.0001);

                            GridCell myButton = new GridCell(player_ID, buttoncolorForDeffense, rowthousand.ToString(), this);

                            // myButton.Content = capital_letters[col] + (row + 1);
                            myButton.TrackingID = (col + 1) + (row * maxCol);
                            myButton.Content = (col + 1) + (row * maxCol);
                            myButton.Width = gridcellSize;
                            myButton.Height = gridcellSize;
                            myButton.RowNum = row + 1;
                            myButton.ColNum = col + 1;
                            myButton.OffenseButton = false;
                            myButton.AllowDrop = true;
                            myButton.Buttonid = (col * maxCol) + row;
                            myButton.Uid = capital_letters[col] + (row + 1); // will result in an id(A1) string
                            Canvas.SetTop(myButton, row * gridcellSize); // assign a value where it will be loaded if plased on a canvas
                            Canvas.SetLeft(myButton, col * gridcellSize); // assign a value where it will be loaded if plased on a canvas
                            myButton.Left_Comp_ParentLeft = col * gridcellSize;
                            myButton.Top_Comp_ParentTop = row * gridcellSize;
                            this.playerGridCells.Add(col + 1 + (row * maxCol), myButton);
                        }
                    }
                }
                else
                {
                    // second iteration for creating the attack grid
                    // offset this buttons from the left to become the attack grid to compaensate the space occupied by the defense grid
                    double gridOffsetWhenVisual = gridcellSize * maxCol;
                    int offenseIDcountOffset = maxCol * maxCol;

                    // will iterate the number of columns requested
                    for (int col = 0; col < maxCol; col++)
                    {
                        // will iterate the number of rows(same as the columns) requested
                        for (int row = 0; row < maxCol; row++)
                        {
                            // create a name for this button will result in a string(10.0001, this is elevated to the ten thounsans to allow a high number of buttons without the repeating of the name)
                            double rowthousand = col + 1 + ((row + 1) * 0.0001);
                            int dictionaryOffset = maxCol * maxCol;
                            GridCell myButton = new GridCell(player_ID, buttoncolorForOffense, rowthousand.ToString(), this);

                            // myButton.Content = capital_letters[col] + (row + 1);
                            myButton.TrackingID = offenseIDcountOffset + col + 1 + (row * maxCol);
                            myButton.Content = col + 1 + (row * maxCol);
                            myButton.Width = gridcellSize;
                            myButton.Height = gridcellSize;
                            myButton.RowNum = row + 1;
                            myButton.ColNum = col + 1;
                            myButton.OffenseButton = true;
                            myButton.AllowDrop = false;
                            myButton.Buttonid = (col * maxCol) + row;
                            myButton.Uid = capital_letters[col] + (row + 1); // will result in an id(A1) string
                            Canvas.SetTop(myButton, row * gridcellSize); // assign a value where it will be loaded if plased on a canvas
                            Canvas.SetLeft(myButton, (col * gridcellSize) + gridOffsetWhenVisual); // assign a value where it will be loaded if plased on a canvas
                            this.playerGridCells.Add(dictionaryOffset + col + 1 + (row * maxCol), myButton);
                        }
                    }
                }
            }

            // Create a collection of ships for any player
            int driver = maxCol + 1;
            int i = 0;
            foreach (int type in shiptypes)
            {
                i++;

                // get an image retainer for the iteration
                BitmapImage shipPic = new BitmapImage();
                shipPic.BeginInit();
                shipPic.UriSource = new Uri(@"./Images/Warship.jpg", UriKind.Relative);
                shipPic.EndInit();
                Coordinate startCoords = new Coordinate(1, (short)(i + 1));

                // Construct the ships with the image above
                Ship warship = new Ship(player_ID, driver, type, gridcellSize, maxCol, startCoords);
                warship.Background = new ImageBrush(shipPic);
                warship.Uid = player_ID.ToString() + "_" + i.ToString();

                // Find the loading cell requested for the ship horizontally
                foreach (KeyValuePair<int, GridCell> pair in this.playerGridCells)
                {
                    int gridcellnumber = pair.Key;
                    GridCell gridCellbutton = pair.Value;
                    if (gridcellnumber == driver && gridCellbutton.OffenseButton == false)
                    {
                        // Canvas.SetTop(warship, gridCellbutton.TopToParentTop);
                        // Canvas.SetLeft(warship, gridCellbutton.LeftToParentLeft);
                        Canvas.SetTop(warship, i * gridcellSize);
                        Canvas.SetLeft(warship, 0);
                        warship.Captain = gridCellbutton.TrackingID;
                        warship.LeftToParentLeft = gridCellbutton.LeftToParentLeft;
                        warship.Top_Comp_ParentTop = gridCellbutton.Top_Comp_ParentTop;
                        gridCellbutton.ButtonOccupied = true; // occupies the driver_pilot_cell inside the dictionary
                    }
                }

                warship.OnShipIsSunk += this.PlayerShipSunk;
                driver += maxCol;

                // load the ship to the list of ships
                this.playerShips.Add(warship);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player" /> class.
        /// </summary>
        /// <param name="player_ID"> This is the ID of the player. </param>
        /// <param name="player_Name"> this is the name of the Player.</param>
        /// <param name="gridcellSize"> This is the size in pixels for the grid square dimension.</param>
        /// <param name="maxCol"> This is the max number of columns requested at the moment pf loading.</param>
        /// <param name="buttoncolorForDeffense"> this is the color for the button created, refer to Custom button class(switch case in constructor).</param>
        /// <param name="buttoncolorForOffense"> This is the side of the screen to load the canvas, if left then reversed count, if right then incremental from one.</param>
        /// <param name="p_currentPlayerBoard"> The current player's board.</param>
        /// <param name="p_otherPlayerBoard"> The other player's board.</param>
        public Player(int player_ID, string player_Name, double gridcellSize, int maxCol, int buttoncolorForDeffense, int buttoncolorForOffense, string[,] p_currentPlayerBoard, string[,] p_otherPlayerBoard)
        {
            this.isLocked = true;
            this.playerTurn = true;

            // set the fixed properties
            this.name = player_Name;
            this.playerID = player_ID;

            // List of Alphabet letters to give names to gridcells
            string[] capital_letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

            this.board = p_currentPlayerBoard;

            // set the player cstom button(grid) collection list if it is placed on the right side of screen
            // Make two grids of buttons for each player one for defense and one for offense(Defense buttons wnot have click events)
            for (int grids = 0; grids < 2; grids++)
            {
                // determine the grid type and assign a flag to the buttons if they are defense or offense.
                if (grids == 0)
                {
                    // will iterate the number of columns requested
                    for (int col = 0; col < maxCol; col++)
                    {
                        // will iterate the number of rows(same as the columns) requested
                        for (int row = 0; row < maxCol; row++)
                        {
                            // create a name for this button will result in a string(10.0001, this is elevated to the ten thounsans to allow a high number of buttons without the repeating of the name)
                            double rowthousand = col + 1 + ((row + 1) * 0.0001);

                            GridCell myButton = new GridCell(player_ID, buttoncolorForDeffense, rowthousand.ToString(), this);

                            // myButton.Content = capital_letters[col] + (row + 1);
                            myButton.TrackingID = (col + 1) + (row * maxCol);
                            myButton.Content = (col + 1) + (row * maxCol);
                            myButton.Width = gridcellSize;
                            myButton.Height = gridcellSize;
                            myButton.RowNum = row + 1;
                            myButton.ColNum = col + 1;
                            myButton.OffenseButton = false;
                            myButton.AllowDrop = true;
                            myButton.Buttonid = (col * maxCol) + row;
                            myButton.Uid = capital_letters[col] + (row + 1); // will result in an id(A1) string
                            Canvas.SetTop(myButton, row * gridcellSize); // assign a value where it will be loaded if plased on a canvas
                            Canvas.SetLeft(myButton, col * gridcellSize); // assign a value where it will be loaded if plased on a canvas
                            myButton.Left_Comp_ParentLeft = col * gridcellSize;
                            myButton.Top_Comp_ParentTop = row * gridcellSize;
                            if (this.board[row, col] == "H" || this.board[row, col] == "M")
                            {
                                myButton.Background = Brushes.Red;
                                myButton.Content = "X";
                                myButton.Stricked = 1;
                                myButton.AllowDrop = false;
                            }
                            else if (this.board[row, col] != "H" && this.board[row, col] != "M" &&
                                     this.board[row, col] != "O")
                            {
                                myButton.Background = Brushes.Azure;
                                myButton.Content = this.board[row, col];
                                myButton.Stricked = 1;
                            }

                            this.playerGridCells.Add(col + 1 + (row * maxCol), myButton);
                        }
                    }
                }
                else
                {
                    // second iteration for creating the attack grid
                    // offset this buttons from the left to become the attack grid to compaensate the space occupied by the defense grid
                    double gridOffsetWhenVisual = gridcellSize * maxCol;
                    int offenseIDcountOffset = maxCol * maxCol;

                    // will iterate the number of columns requested
                    for (int col = 0; col < maxCol; col++)
                    {
                        // will iterate the number of rows(same as the columns) requested
                        for (int row = 0; row < maxCol; row++)
                        {
                            // create a name for this button will result in a string(10.0001, this is elevated to the ten thounsans to allow a high number of buttons without the repeating of the name)
                            double rowthousand = col + 1 + ((row + 1) * 0.0001);
                            int dictionaryOffset = maxCol * maxCol;
                            GridCell myButton = new GridCell(player_ID, buttoncolorForOffense, rowthousand.ToString(), this);

                            // myButton.Content = capital_letters[col] + (row + 1);
                            myButton.TrackingID = offenseIDcountOffset + col + 1 + (row * maxCol);
                            myButton.Content = col + 1 + (row * maxCol);
                            myButton.Width = gridcellSize;
                            myButton.Height = gridcellSize;
                            myButton.RowNum = row + 1;
                            myButton.ColNum = col + 1;
                            myButton.OffenseButton = true;
                            myButton.AllowDrop = false;
                            myButton.Buttonid = (col * maxCol) + row;
                            myButton.Uid = capital_letters[col] + (row + 1); // will result in an id(A1) string
                            Canvas.SetTop(myButton, row * gridcellSize); // assign a value where it will be loaded if plased on a canvas
                            Canvas.SetLeft(myButton, (col * gridcellSize) + gridOffsetWhenVisual); // assign a value where it will be loaded if plased on a canvas
                            if (p_otherPlayerBoard[row, col] == "H")
                            {
                                myButton.Background = Brushes.Green;
                                myButton.Content = "H";
                                myButton.IsEnabled = false;
                                myButton.Stricked = 1;
                                myButton.AllowDrop = false;
                            }
                            else if (p_otherPlayerBoard[row, col] == "M")
                            {
                                myButton.Visibility = Visibility.Hidden;
                                myButton.Stricked = 1;
                            }

                            this.playerGridCells.Add(dictionaryOffset + col + 1 + (row * maxCol), myButton);
                        }
                    }
                }
            }

            int driver = maxCol + 1;
            for (int i = 1; i <= 5; i++)
            {
                int check = 0;
                bool start = false;
                Coordinate startCoords = new Coordinate(1, (short)(i + 1));
                Ship warship = new Ship(player_ID, driver, i, gridcellSize, maxCol, startCoords);

                for (int row = 0; row < maxCol; row++)
                {
                    for (int col = 0; col < maxCol; col++)
                    {
                        if (this.board[row, col] == warship.ShipName.Substring(0, 2) && !start)
                        {
                            check++;
                            start = true;
                            startCoords = new Coordinate((short)(col + 1), (short)(row + 1));
                        }
                        else if (this.board[row, col] == warship.ShipName.Substring(0, 2))
                        {
                            check++;
                            if (startCoords.XCoordinate == col)
                            {
                                warship.HDirection = true;
                                warship.ShipStartCoords = startCoords;
                            }
                            else if (startCoords.YCoordinate == row)
                            {
                                warship.HDirection = false;
                                warship.ShipStartCoords = startCoords;
                            }
                        }
                    }
                }

                warship.Resistance = check;
                if (warship.Resistance == 0)
                {
                    warship.ShipIsSunk = true;
                }

                warship.OnShipIsSunk += this.PlayerShipSunk;

                // load the ship to the list of ships
                this.playerShips.Add(warship);
                driver += maxCol;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the ship placement is locked.
        /// </summary>
        public bool IsLocked
        {
            get => this.isLocked;
            set => this.isLocked = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is player's turn or not.
        /// </summary>
        public bool PlayerTurn
        {
            get { return this.playerTurn; }
            set { this.playerTurn = value; }
        }

        /// <summary>
        /// Gets player name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets player ID.
        /// </summary>
        public int PlayerID
        {
            get { return this.playerID; }
        }

        /// <summary>
        /// Gets or sets player ID.
        /// </summary>
        public int BombCount
        {
            get { return this.bombCount; }
            set { this.bombCount = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player's bomb is activated.
        /// </summary>
        public bool PlayerBombactivated
        {
            get { return this.playerBombactivated; }
            set { this.playerBombactivated = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the player has won.
        /// </summary>
        public bool Winner
        {
            get { return this.winner; }
            set { this.winner = value; }
        }

        /// <summary>
        /// Gets or sets a count of player's ships.
        /// </summary>
        public int ShipCount
        {
            get { return this.shipCount; }
            set { this.shipCount = value; }
        }

        /// <summary>
        /// Gets or sets a count of player's ships per turn.
        /// </summary>
        public int ShipCountTurn
        {
            get { return this.shipCountTurn; }
            set { this.shipCountTurn = value; }
        }

        /// <summary>
        /// Gets player collection of ships.
        /// </summary>
        public List<Ship> Playershipcollection
        {
            get { return this.playerShips; }
        }

        /// <summary>
        /// Gets player Button collections for its personal grid.
        /// </summary>
        public Dictionary<int, GridCell> Playergridsquarecollection
        {
            get { return this.playerGridCells; }
        }

        /// <summary>
        /// Gets or sets the board information.
        /// </summary>
        public string[,] Board
        {
            get { return this.board; }
            set { this.board = value; }
        }

        /// <summary>
        /// Set ships to player's board.
        /// </summary>
        /// <param name="canvasUid">The name of canvas.</param>
        /// <param name="rowRep">The number of columns and rows.</param>
        public void SetShipsToBoard(string canvasUid, int rowRep)
        {
            foreach (Ship ship in this.Playershipcollection)
            {
                int startColumn = (int)ship.ShipStartCoords.XCoordinate - 1;
                int startRow = (int)ship.ShipStartCoords.YCoordinate - 1;
                int endColumn = (int)ship.ShipEndCoords.XCoordinate - 1;
                int endRow = (int)ship.ShipEndCoords.YCoordinate - 1;
                string letter = ship.ShipName.Substring(0, 2);
                if (ship.HDirection == true)
                {
                    for (int i = startColumn; i <= endColumn; i++)
                    {
                        this.Board[startRow, i] = letter;
                    }
                }
                else if (ship.HDirection == false)
                {
                    for (int i = startRow; i <= endRow; i++)
                    {
                        this.Board[i, startColumn] = letter;
                    }
                }
            }

            Logger.ConsoleInformation("------- " + canvasUid + " ------");
            for (int i = 0; i < rowRep; i++)
            {
                for (int j = 0; j < rowRep; j++)
                {
                    Logger.ConsoleInformationForArray(this.Board[i, j] + ", ");
                }

                Logger.ConsoleInformation("\n");
            }
        }

        /// <summary>
        /// Locks the ships into place.
        /// </summary>
        public void LockShipsIntoPlace()
        {
            foreach (Ship ship in this.playerShips)
            {
                string shipName = ship.ShipName.Substring(0, 2);

                // Loop through each of the GridCells that the ship is currently placed on.
                foreach (int crewMember in ship.Ship_Crewmembers)
                {
                    // Set the GridCell's color to Azure.
                    this.playerGridCells[crewMember].Background = Brushes.Azure;
                    this.playerGridCells[crewMember].Content = shipName;
                }

                // Hide the ships visibility.
                ship.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Checks the crew members of the ship.
        /// </summary>
        /// <param name="p_crew">The list of crew members to be checked.</param>
        /// <returns>Returns whether the potential crew members' grid spaces are currently occupied.</returns>
        public StatusCodes.GridSpaceStatus Checkshipcrewmembers(List<int> p_crew)
        {
            StatusCodes.GridSpaceStatus shipCrewMemberStatus = StatusCodes.GridSpaceStatus.GRID_SPACE_NOT_OCCUPIED;

            foreach (int crewMember in p_crew)
            {
                foreach (Ship testShip in this.playerShips)
                {
                    foreach (int testShipCrewMember in testShip.Ship_Crewmembers)
                    {
                        if (testShipCrewMember == crewMember)
                        {
                            shipCrewMemberStatus = StatusCodes.GridSpaceStatus.GRID_SPACE_OCCUPIED;
                            break;
                        }
                    }
                }
            }

            return shipCrewMemberStatus;
        }

        /// <summary>
        /// Set a player ship sunk.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments for the event.</param>
        protected void PlayerShipSunk(object sender, EventArgs e)
        {
            Ship ship = sender as Ship;
            ship.ShipIsSunk = true;
            this.shipCount--;

            if (ship.Resistance == 0)
            {
                Logger.Information(ship.ShipName + " has been sunk!");
            }

            this.CheckIfPlayerHasWon();
        }

        /// <summary>
        /// Set a method that checks if player has won.
        /// </summary>
        private void CheckIfPlayerHasWon()
        {
            int numberOfShipsThatHaveBeenSunk = 0;

            foreach (Ship ship in this.Playershipcollection)
            {
                if (ship.ShipIsSunk == true)
                {
                    numberOfShipsThatHaveBeenSunk++;
                }
            }

            if (numberOfShipsThatHaveBeenSunk == this.Playershipcollection.Count)
            {
                this.winner = true;
                Logger.Information(this.Name + " Lost");
                Environment.Exit(0);
            }
        }
    }
}
