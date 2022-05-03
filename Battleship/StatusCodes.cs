﻿//-----------------------------------------------------------------------
// <copyright file="StatusCodes.cs" company="Battleship Coding Group">
//     Battleship Coding Group, 2022
// </copyright>
//-----------------------------------------------------------------------
namespace Battleship
{
    /// <summary>
    /// Implementation of project-wide status codes.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Encapsulation not yet taught.")]
    public class StatusCodes
    {
        /// <summary>
        /// Types of attack statuses.
        /// </summary>
        public enum AttackStatus
        {
            /// <summary>
            /// Represents a not attacked status.
            /// </summary>
            NOT_ATTACKED,

            /// <summary>
            /// Represents an attacked but not hit status.
            /// </summary>
            ATTACKED_NOT_HIT,

            /// <summary>
            /// Represents an attacked and hit status.
            /// </summary>
            ATTACKED_HIT,
        }

        /// <summary>
        /// Types of applications statuses.
        /// </summary>
        public enum ApplicationStatus
        {
            /// <summary>
            /// Represents a game not yet started.
            /// </summary>
            GAME_NOT_STARTED,

            /// <summary>
            /// Represents a start of a game.
            /// </summary>
            GAME_STARTED,

            /// <summary>
            /// Represents a player1.
            /// </summary>
            PLAYER_1_TURN,

            /// <summary>
            /// Represents a player2.
            /// </summary>
            PLAYER_2_TURN,

            /// <summary>
            /// Represents an end of the game.
            /// </summary>
            GAME_ENDED,
        }

        /// <summary>
        /// Types of game types.
        /// </summary>
        public enum GameType
        {
            /// <summary>
            /// Represents a player to player game.
            /// </summary>
            PLAYER_TO_PLAYER,

            /// <summary>
            /// Represents a player to computer game.
            /// </summary>
            PLAYER_TO_COMPUTER,

            /// <summary>
            /// Represents a computer to computer game.
            /// </summary>
            COMPUTER_TO_COMPUTER,
        }

        public enum GridSpaceStatus
        {
            GRID_SPACE_OCCUPIED,
            GRID_SPACE_NOT_OCCUPIED,
        }

        public enum AttackStrategy
        {
            ATTACK_DIFFICULTY_EASY,
            ATTACK_DIFFICULTY_HARD,
        }

        public enum DefenseStrategy
        {
            DEFENSE_DIFFICULTY_EASY,
            DEFENSE_DIFFICULTY_CORNER,
            DEFENSE_DIFFICULTY_CENTER,
        }
    }
}