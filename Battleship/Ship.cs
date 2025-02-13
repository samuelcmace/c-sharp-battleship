﻿//-----------------------------------------------------------------------
// <copyright file="Ship.cs" company="David Fedchuk, Gerson Eliu Sorto Flores, Lincoln Kaszynski, Sanaaia Okhlopkova, and Samuel Mace">
//     MIT License, 2022 (https://mit-license.org).
// </copyright>
//-----------------------------------------------------------------------
namespace Battleship
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for SHip.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Encapsulation not taught.")]
    public class Ship : UserControl
    {
        /// <summary>
        /// The left to parent left.
        /// </summary>
        public double LeftToParentLeft;

        /// <summary>
        /// The top to parent top.
        /// </summary>
        public double TopToParentTop;

        /// <summary>
        /// Is ship sunk or not.
        /// </summary>
        public bool ShipIsSunk;

        /// <summary>
        /// The ship's type.
        /// </summary>
        private int shipType;

        /// <summary>
        /// The ship's name.
        /// </summary>
        private string name;

        /// <summary>
        /// The ship's resistance.
        /// </summary>
        private int resistance;

        /// <summary>
        /// The ship's length.
        /// </summary>
        private int length;

        /// <summary>
        /// The ship's player ID.
        /// </summary>
        private int playerID;

        /// <summary>
        /// The ship's horizontal direction.
        /// </summary>
        private bool horDirection = true;

        /// <summary>
        /// The ship's start coordinate.
        /// </summary>
        private Coordinate shipStartCoords;

        /// <summary>
        /// The ship's end coordinate.
        /// </summary>
        private Coordinate shipEndCoords;

        /// <summary>
        /// Ship current crew.
        /// </summary>
        private List<int> actualCrewmembers;

        /// <summary>
        /// Ship driver cell.
        /// </summary>
        private int captain;

        /// <summary>
        /// Ship driver cell.
        /// </summary>
        private int newCaptain;

        /// <summary>
        /// The ship's length.
        /// </summary>
        private int grids;

        /// <summary>
        /// The ship's number of grid spaces.
        /// </summary>
        private int rowsgrid;

        /// <summary>
        /// Counter for the number of drags.
        /// </summary>
        private int drags = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ship" /> class.
        /// </summary>
        /// <param name="playerID"> This is the player ID passed from player class.</param>
        /// <param name="driver">The initial captain of the ship.</param>
        /// <param name="shipType"> this is the type of ship, Submarine,warship...</param>
        /// <param name="gridCellSize"> This is the size of the grid square passed from player class, determined in pixels.</param>
        /// <param name="rowTotal">The total number of rows.</param>
        /// <param name="startCoords"> This is the start coordinates of the ship.</param>
        public Ship(int playerID, int driver, int shipType, double gridCellSize, int rowTotal, Coordinate startCoords)
        {
            switch (shipType)
            {
                case 1:
                    this.name = "Destroyer";
                    this.length = 2;
                    this.resistance = 2;
                    this.grids = 2;
                    break;
                case 2:
                    this.name = "Submarine";
                    this.length = 3;
                    this.resistance = 3;
                    this.grids = 3;
                    break;
                case 3:
                    this.name = "Cruiser";
                    this.length = 3;
                    this.resistance = 3;
                    this.grids = 3;
                    break;
                case 4:
                    this.name = "Battleship";
                    this.length = 4;
                    this.resistance = 4;
                    this.grids = 4;
                    break;
                case 5:
                    this.name = "Carrier";
                    this.length = 5;
                    this.resistance = 5;
                    this.grids = 5;
                    break;
                default:
                    this.name = "Mistake";
                    this.length = 0;
                    this.resistance = 0;
                    this.grids = 0;
                    break;
            }

            this.actualCrewmembers = new List<int>();
            this.playerID = playerID;
            this.shipType = shipType;
            this.ShipIsSunk = false;
            this.shipStartCoords = startCoords;
            this.rowsgrid = rowTotal;
            this.actualCrewmembers = this.SetCrewmembers(driver, 0);

            Coordinate endCoords = new Coordinate((short)(this.shipStartCoords.XCoordinate + this.length - 1), this.ShipStartCoords.YCoordinate);
            this.shipEndCoords = endCoords;
            this.Width = this.length * gridCellSize;
            this.Height = gridCellSize;
            this.captain = driver;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ship"/> class.
        /// </summary>
        /// <param name="playerID">The ID of the player.</param>
        /// <param name="shipType">The type of ship to be created.</param>
        /// <param name="gridCellSize">The pixel-to-grid-space conversion factor.</param>
        /// <param name="horDirection">The direction of the ship (whether is is horizontal or not).</param>
        public Ship(int playerID, int shipType, double gridCellSize, bool horDirection)
        {
            this.playerID = playerID;
            this.shipType = shipType;
            this.ShipIsSunk = false;
            this.horDirection = horDirection;
            switch (shipType)
            {
                case 1:
                    this.name = "Destroyer";
                    this.length = 2;
                    this.resistance = 2;
                    this.grids = 2;
                    break;
                case 2:
                    this.name = "Submarine";
                    this.length = 3;
                    this.resistance = 3;
                    this.grids = 3;
                    break;
                case 3:
                    this.name = "Cruiser";
                    this.length = 3;
                    this.resistance = 3;
                    this.grids = 3;
                    break;
                case 4:
                    this.name = "Battleship";
                    this.length = 4;
                    this.resistance = 4;
                    this.grids = 4;
                    break;
                case 5:
                    this.name = "Carrier";
                    this.length = 5;
                    this.resistance = 5;
                    this.grids = 5;
                    break;
                default:
                    this.name = "Mistake";
                    this.length = 0;
                    this.resistance = 0;
                    this.grids = 0;
                    break;
            }

            this.Width = this.length * gridCellSize;
            this.Height = gridCellSize;
            if (horDirection)
            {
                this.Width = this.length * gridCellSize;
                this.Height = gridCellSize;
            }
            else
            {
                this.Width = gridCellSize;
                this.Height = this.length * gridCellSize;
            }
        }

        /// <summary>
        /// Event for the is ship sunk or not
        /// </summary>
        public event EventHandler OnShipIsSunk;

        /// <summary>
        /// Gets or sets the start coordinates of the ship.
        /// </summary>
        public Coordinate ShipStartCoords
        {
            get
            {
                return this.shipStartCoords;
            }

            set
            {
                this.shipStartCoords = value;
                Coordinate endCoords = new Coordinate();
                if (this.horDirection)
                {
                    endCoords = new Coordinate((short)(this.shipStartCoords.XCoordinate + this.length - 1), this.ShipStartCoords.YCoordinate);
                }
                else
                {
                    endCoords = new Coordinate(this.shipStartCoords.XCoordinate, (short)(this.ShipStartCoords.YCoordinate + this.length - 1));
                }

                this.shipEndCoords = endCoords;
            }
        }

        /// <summary>
        /// Gets the end coordinates of the ship.
        /// </summary>
        public Coordinate ShipEndCoords
        {
            get { return this.shipEndCoords; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether horizontal direction is loading for the ship, set true at loading.
        /// </summary>
        public bool HDirection
        {
            get { return this.horDirection; }
            set { this.horDirection = value; }
        }

        /// <summary>
        /// Gets or sets type of ship.
        /// </summary>
        public int ShipType
        {
            get { return this.shipType; }
            set { this.shipType = value; }
        }

        /// <summary>
        /// Gets or sets the number of hits this ship will resist.
        /// </summary>
        public int Resistance
        {
            get { return this.resistance; }
            set { this.resistance = value; }
        }

        /// <summary>
        /// Gets or sets the number of drags.
        /// </summary>
        public int DragsCounter
        {
            get { return this.drags; }
            set { this.drags = value; }
        }

        /// <summary>
        /// Gets or sets ship length.
        /// </summary>
        public int Length
        {
            get { return this.length; }
            set { this.length = value; }
        }

        /// <summary>
        /// Gets the player ID passed from player class.
        /// </summary>
        public int PlayerID
        {
            get { return this.playerID; }
        }

        /// <summary>
        /// Gets the name of the ship.
        /// </summary>
        public string ShipName
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets or sets the ship's left to parent left.
        /// </summary>
        public double Left_Comp_ParentLeft
        {
            get { return this.LeftToParentLeft; }
            set { this.LeftToParentLeft = value; }
        }

        /// <summary>
        /// Gets or sets the ship's top to parent top.
        /// </summary>
        public double Top_Comp_ParentTop
        {
            get { return this.TopToParentTop; }
            set { this.TopToParentTop = value; }
        }

        /// <summary>
        /// Gets or sets ship length.
        /// </summary>
        public int GridSpaces
        {
            get { return this.grids; }
            set { this.grids = value; }
        }

        /// <summary>
        /// Gets or sets the current crew members for each ship.
        /// </summary>
        public List<int> Ship_Crewmembers
        {
            get { return this.actualCrewmembers; }
            set { this.actualCrewmembers = value; }
        }

        /// <summary>
        /// Gets or sets the entry point for the ship when loaded.
        /// </summary>
        public int Captain
        {
            get { return this.captain; }
            set { this.captain = value; }
        }

        /// <summary>
        /// Gets or sets the new captain of the ship after drags.
        /// </summary>
        public int NewCaptain
        {
            get { return this.newCaptain; }
            set { this.newCaptain = value; }
        }

        /// <summary>
        /// This method will change the width or the Height of this ship and reverse it.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        public void RotateShip(bool trigger)
        {
            if (trigger == true)
            {
                // Instantiate new coordinate objects to store the ship's previous coordinates.
                Coordinate previousShipStartCoords;
                Coordinate previousShipEndCoords;

                if (this.HDirection == true)
                {
                    this.HDirection = false;
                    double switcher = this.Width; // base
                    this.Width = this.Height;
                    this.Height = switcher;
                    previousShipStartCoords = new Coordinate(this.shipStartCoords.XCoordinate, this.shipStartCoords.YCoordinate);
                    previousShipEndCoords = new Coordinate(this.shipStartCoords.XCoordinate, (short)(this.shipStartCoords.YCoordinate + this.length - 1));
                }
                else
                {
                    this.HDirection = true;
                    double switcher = this.Width; // base
                    this.Width = this.Height;
                    this.Height = switcher;
                    previousShipStartCoords = new Coordinate(this.shipStartCoords.XCoordinate, this.shipStartCoords.YCoordinate);
                    previousShipEndCoords = new Coordinate((short)(this.shipStartCoords.XCoordinate + this.length - 1), this.shipStartCoords.YCoordinate);
                }

                // Update the coordinate properties of the current ship.
                this.shipStartCoords = previousShipStartCoords;
                this.shipEndCoords = previousShipEndCoords;
            }
        }

        /// <summary>
        /// Set a method that attacks the coordinate.
        /// </summary>
        /// <param name="testCoordinate">The coordinate.</param>
        /// <returns>The attack coordinate.</returns>
        public AttackCoordinate AttackGridSpace(Coordinate testCoordinate)
        {
            AttackCoordinate attackCoordinate = new AttackCoordinate(testCoordinate.XCoordinate, testCoordinate.YCoordinate);

            // If the ship is horizontal, we only have to compare the X-Coordinate of the attacked grid space with the ship's grid spaces.
            if (this.HDirection == true)
            {
                if ((testCoordinate.YCoordinate == this.shipStartCoords.YCoordinate) && (testCoordinate.XCoordinate >= this.shipStartCoords.XCoordinate) && (testCoordinate.XCoordinate <= this.shipEndCoords.XCoordinate))
                {
                    attackCoordinate.CoordinateStatus = StatusCodes.AttackStatus.ATTACKED_HIT;

                    this.resistance--;

                    if (this.resistance <= 0)
                    {
                        this.OnShipIsSunk?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    attackCoordinate.CoordinateStatus = StatusCodes.AttackStatus.ATTACKED_NOT_HIT;
                }
            }
            else
            {
                if ((testCoordinate.XCoordinate == this.shipStartCoords.XCoordinate) && (testCoordinate.YCoordinate >= this.shipStartCoords.YCoordinate) && (testCoordinate.YCoordinate <= this.shipEndCoords.YCoordinate))
                {
                    attackCoordinate.CoordinateStatus = StatusCodes.AttackStatus.ATTACKED_HIT;

                    this.resistance--;

                    if (this.resistance <= 0)
                    {
                        this.OnShipIsSunk?.Invoke(this, EventArgs.Empty);
                    }
                }
                else
                {
                    attackCoordinate.CoordinateStatus = StatusCodes.AttackStatus.ATTACKED_NOT_HIT;
                }
            }

            return attackCoordinate;
        }

        /// <summary>
        /// Set a method that updates the ship's coordinates.
        /// </summary>
        /// <param name="shipStartCoords">The ship's start coordinate.</param>
        /// <param name="shipEndCoords">The ship's end coordinate.</param>
        public void UpdateShipCoords(Coordinate shipStartCoords, Coordinate shipEndCoords)
        {
            this.shipStartCoords = shipStartCoords;
            this.shipEndCoords = shipEndCoords;

            if (shipStartCoords.YCoordinate == shipEndCoords.YCoordinate)
            {
                this.HDirection = true;
            }
            else
            {
                this.HDirection = false;
            }
        }

        /// <summary>
        /// Return a list of new crew members of a ship if a drag occurs.
        /// </summary>
        /// <param name="p_capitan">see the cell for the ship has a crew.</param>
        /// <param name="dragTurn">The turn to be dragged.</param>
        /// <returns>Returns the new Crewmembers of the ship.</returns>
        public List<int> SetCrewmembers(int p_capitan, int dragTurn)
        {
            List<int> back = new List<int>();

            List<int> movingHorizontalCrewmembers = new List<int>();
            List<int> movingVerticalCrewmembers = new List<int>();

            int horizontalValue = p_capitan;
            int verticalValue = p_capitan;

            for (int i = 0; i < this.GridSpaces; i++)
            {
                movingHorizontalCrewmembers.Add(horizontalValue);
                horizontalValue++;
            }

            for (int i = 0; i < this.GridSpaces; i++)
            {
                movingVerticalCrewmembers.Add(verticalValue);
                verticalValue += this.rowsgrid;
            }

            // draging Crewmembers request for check
            if (this.HDirection == true && dragTurn == 0)
            {
                back = movingHorizontalCrewmembers;
            }

            // draging Crewmembers request for check
            if (this.HDirection == false && dragTurn == 0)
            {
                back = movingVerticalCrewmembers;
            }

            // turning Crewmembers request for check
            if (this.HDirection == true && dragTurn == 1)
            {
                back = movingVerticalCrewmembers;
            }

            // turning Crewmembers request for check
            if (this.HDirection == false && dragTurn == 1)
            {
                back = movingHorizontalCrewmembers;
            }

            return back;
        }
    }
}
