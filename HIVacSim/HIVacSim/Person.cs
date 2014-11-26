// ----------------------------------------------------------------------------
// <copyright file="Person.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using HIVacSim.Utility;

    /// <summary>
    /// Defines the characteristics of a person, the basic entity of the model
    /// </summary>
    [Serializable]
    public class Person
    {
        #region Static Field
        /// <summary>
        /// Unique identification counter
        /// </summary>
        private static int PersonId = 0;    
        #endregion

        #region Local Variables
        private int _id;                // The person's unique identification
        private Group _myGroup;         // The parent group reference
        private int _age;               // Age in years
        private int _lifeExpect;        // Life expectancy in years
        private EGender _gender;        // Gender
        private bool _gay;              // Flag identifying sexual preference
        private ESTDStatus _std;        // Current STD state
        private int _stdAge;            // Age of the STD infection
        private int _stdDeath;          // Life expectancy after STD infection
        private bool _stdTest;          // STD testing state
        private int _stdDuration;       // STD infection duration
        private ERelation _partner;     // Partnership state
        private int _transition;        // Transition status duration
        private AdjList _edges;         // List of partnerships
        private BookList _friends;      // List of friends
        private Point3d _local;         // Geographical location

        // Flags for algorithms
        private int _visit;             // Current visited status
        private int _distance;          // Distance to someone else
        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static Person()
        {
            PersonId = 0;
        }

        /// <summary>
        /// Default constructor, creates a person from a given group
        /// </summary>
        /// <param name="toGroup">The group to which the person belongs</param>
        public Person(Group toGroup)
        {
            // Defines the person unique identification
            this._id = Interlocked.Increment(ref Person.PersonId);

            this._myGroup = toGroup;
            this._std = ESTDStatus.Susceptable;
            this._stdDeath = int.MaxValue;
            this._stdDuration = int.MaxValue;
            this._partner = ERelation.Available;
            this._edges = new AdjList();
            this._friends = new BookList();
            this._visit = -1;
            this._distance = 0;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the person' unique identification
        /// </summary>
        [Description("Gets the person' unique identification.")]
        public int Id
        {
            get { return this._id; }
        }

        /// <summary>
        /// Gets the person’s group
        /// </summary>
        [Description("Gets the person’s group")]
        public Group MyGroup
        {
            get { return this._myGroup; }
        }


        /// <summary>
        /// Gets or sets the person's age
        /// </summary>
        [DescriptionAttribute("Gets or sets the person's age")]
        public int Age
        {
            get { return this._age; }
            set { this._age = value; }
        }


        /// <summary>
        /// Gets or sets the person's life expectancy
        /// </summary>
        [DescriptionAttribute("Gets or sets the person's life expectancy")]
        public int LifeExpectancy
        {
            get { return this._lifeExpect; }
            set { this._lifeExpect = value; }
        }


        /// <summary>
        /// Gets or sets the person's gender
        /// </summary>
        [DescriptionAttribute("Gets or sets the person's gender")]
        public EGender Gender
        {
            get { return this._gender; }
            set { this._gender = value; }
        }


        /// <summary>
        /// Gets or sets the person's sexual preference (male only)
        /// </summary>
        [DescriptionAttribute("Gets or sets the person's sexual preference (male only)")]
        public bool Homosexual
        {
            get { return this._gay; }
            set { this._gay = value; }
        }


        /// <summary>
        /// Gets or sets the person's STD infection status
        /// </summary>
        [DescriptionAttribute("Gets or sets the person's STD infection status")]
        public ESTDStatus STDStatus
        {
            get { return this._std; }
            set { this._std = value; }
        }


        /// <summary>
        /// Gets or sets the age of the person's STD infection (STD positive only)
        /// </summary>
        [DescriptionAttribute("Gets or sets the age of the person's STD infection (STD positive only)")]
        public int STDAge
        {
            get { return this._stdAge; }
            set { this._stdAge = value; }
        }


        /// <summary>
        /// Gets or sets the life expectancy after STD infection (STD positive only)
        /// </summary>
        [DescriptionAttribute("Gets or sets the life expectancy after STD infection (STD positive only)")]
        public int STDDeath
        {
            get { return this._stdDeath; }
            set { this._stdDeath = value; }
        }


        /// <summary>
        /// Gets or sets the person's STD testing status
        /// </summary>
        [DescriptionAttribute("Gets or sets the person's STD testing status")]
        public bool STDTest
        {
            get { return this._stdTest; }
            set { this._stdTest = value; }
        }

        /// <summary>
        /// Gets or sets the duration of STD infection or protection
        /// </summary>
        [DescriptionAttribute("Gets or sets the duration of STD infection or protection")]
        public int STDDuration
        {
            get { return this._stdDuration; }
            set { this._stdDuration = value; }
        }

        /// <summary>
        /// Gets or sets the person's partnership status
        /// </summary>
        [DescriptionAttribute("Gets or sets the person's partnership status")]
        public ERelation Partnership
        {
            get { return this._partner; }
            set { this._partner = value; }
        }


        /// <summary>
        /// Gets or sets the person's transitory status duration
        /// </summary>
        [DescriptionAttribute("Gets or sets the person's transitory status duration")]
        public int Transitory
        {
            get { return this._transition; }
            set { this._transition = value; }
        }

        /// <summary>
        /// Gets or sets the geographical location of a person.
        /// </summary>
        [DescriptionAttribute("Gets or sets the geographical location of a person.")]
        public Point3d Location
        {
            get { return this._local; }
            set { this._local = value; }
        }

        /// <summary>
        /// Gets or sets the person's visited status.
        /// </summary>
        [DescriptionAttribute("Gets or sets the person's visited status.")]
        public int Visited
        {
            get { return this._visit; }
            set { this._visit = value; }
        }

        /// <summary>
        /// Gets or sets the person's distance to someone else.
        /// </summary>
        [DescriptionAttribute("Gets or sets the person's distance to someone else.")]
        public int Distance
        {
            get { return this._distance; }
            set { this._distance = value; }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the list of partnerships
        /// </summary>
        public AdjList Partners
        {
            get { return this._edges; }
        }


        /// <summary>
        /// Gets the list of friends
        /// </summary>
        public BookList Friends
        {
            get { return this._friends; }
        }


        /// <summary>
        /// Adds a new partner to partnership list
        /// </summary>
        /// <param name="partner">The partnership to add</param>
        /// <returns>True for successful addition, false otherwise</returns>
        public bool AddPartnership(Relation partner)
        {
            if (this._edges.Add(partner))
            {
                Relation match = new Relation(this, partner.Partnership, partner.Duration);
                partner.ToPerson._edges.Add(match);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Finish a partnership (remove from list)
        /// </summary>
        /// <param name="partner">The partner to be removed from list</param>
        /// <param name="addfriend">
        /// If true, adds the partners to each other's friends list
        /// </param>		
        /// <returns>
        /// True if the partnership has finished, false otherwise
        /// </returns>
        public bool EndPartnership(Person partner, bool addfriend)
        {
            if (this._edges.Remove(partner))
            {
                partner._edges.Remove(this);
                if (partner.MyGroup == this.MyGroup && addfriend)
                {
                    this.AddFriend(partner);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Finish a partnership (remove from list)
        /// </summary>
        /// <param name="partner">The partner to be removed from list</param>
        /// <returns>
        /// True if the partnership has finished, false otherwise
        /// </returns>
        public bool EndPartnership(Person partner)
        {
            return EndPartnership(partner, true);
        }


        /// <summary>
        /// Adds a new friend to friendship list
        /// </summary>
        /// <param name="friend">The friend to be added</param>
        /// <returns>
        /// True if the addition was successful, false otherwise
        /// </returns>
        public bool AddFriend(Person friend)
        {
            if (this._friends.Add(friend))
            {
                friend._friends.Add(this);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Finish a friendship (remove from list)
        /// </summary>
        /// <param name="friend">The friend to be removed</param>
        /// <returns>
        /// True if the friendship has finished, false otherwise
        /// </returns>
        public bool EndFriend(Person friend)
        {
            if (this._friends.Remove(friend))
            {
                friend._friends.Remove(this);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Finish partnerships and friendships before die.
        /// </summary>
        public void SayGoodBye()
        {
            int i, size;

            //Finish all partnerships before die
            size = this._edges.Count;
            for (i = 0; i < size; i++)
            {
                this.EndPartnership(this._edges[0].ToPerson, false);
            }

            //Finish all friendships
            size = this._friends.Count;
            for (i = 0; i < size; i++)
            {
                this.EndFriend(this._friends[0]);
            }
        }
        #endregion

        /// <summary>
        /// Resets the person id number
        /// </summary>
        public static void ResetId()
        {
            Person.PersonId = 0;
        }
    }
}
