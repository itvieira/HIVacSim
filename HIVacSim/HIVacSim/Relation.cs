// ----------------------------------------------------------------------------
// <copyright file="Relation.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;

    /// <summary>
    /// Define the characteristics of the partnerships, the edges of the graph.
    /// </summary>
    [Serializable]
    public class Relation
    {
        #region Local variables
        /// <summary>
        /// Identify the partner
        /// </summary>
        private Person _person;

        /// <summary>
        /// Defines the type of partnership
        /// </summary>
        private EPartners _partner;

        /// <summary>
        /// Defines the duration of the partnership
        /// </summary>
        private int _duration;

        /// <summary>
        /// Identify if this relation has already been visited
        /// </summary>
        private int _vitited;
        #endregion

        #region Constructors
        /// <summary>
        /// Defines an internal partnership
        /// </summary>
        /// <param name="prs">identify the partner</param>
        /// <param name="type">partnership type</param>
        /// <param name="duration">partnership duration</param>
        public Relation(Person prs, EPartners type, int duration)
        {
            this._person = prs;
            this._partner = type;
            this._duration = duration;
            this._vitited = -1;
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Get or set the partner
        /// </summary>
        public Person ToPerson
        {
            get { return this._person; }
            set { this._person = value; }
        }

        /// <summary>
        /// Get or set the type of partnership
        /// </summary>
        public EPartners Partnership
        {
            get { return this._partner; }
            set { this._partner = value; }
        }

        /// <summary>
        /// Get or set the duration of the partnership
        /// </summary>
        public int Duration
        {
            get { return this._duration; }
            set { this._duration = value; }
        }

        /// <summary>
        /// Gets or sets the last visit to this relation.
        /// </summary>
        public int Visited
        {
            get { return this._vitited; }
            set { this._vitited = value; }
        }
        #endregion
    }
}
