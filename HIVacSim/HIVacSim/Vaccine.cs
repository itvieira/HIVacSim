// ----------------------------------------------------------------------------
// <copyright file="Vaccine.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.ComponentModel;
    using System.Threading;

    /// <summary>
    /// Defines the characteristics of a preventive vaccine.
    /// </summary>
    [Serializable]
    public class Vaccine
    {
        #region Static Field
        /// <summary>
        /// Unique identification counter
        /// </summary>
        private static int IdCounter;
        #endregion

        #region Local variables
        private int _id;		    //Vaccine unique id number
        private string _name;		//Short name describing the vaccine
        private double _effective;  //The effectiveness of the vaccine;
        private bool _lifetime;	    //Does the vaccine provides a lifetime protection;
        private int _length;	    //Length of the vaccine protection (if not lifetime)
        private ListOfIds _usedby;	//Number of interventions using this vaccine
        #endregion //Local variables

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Vaccine()
        {
            this._id = Interlocked.Increment(ref Vaccine.IdCounter);
            this._name = "Vaccine " + this._id.ToString();
            this._lifetime = false;
            this._usedby = new ListOfIds();
        }
        #endregion

        #region Public properties

        /// <summary>
        /// Vaccine unique identification
        /// </summary>
        [Category("1 - Definition"),
        DefaultValue(1),
        Description("Vaccine unique identification.")]
        public int Id
        {
            get 
            { 
                return this._id; 
            }

            set
            {
                this._id = value;
                if (Vaccine.IdCounter <= this._id)
                {
                    Vaccine.IdCounter = this._id + 1;
                }
            }
        }

        /// <summary>
        /// Vaccine identification name
        /// </summary>
        [Category("1 - Definition"),
        DefaultValue("Vaccine 1"),
        Description("Vaccine identification name.")]
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// The effectiveness of the vaccine in stopping HIV transmission.
        /// </summary>
        [Category("1 - Definition"),
        Description("The effectiveness of the vaccine in stopping HIV transmission.")]
        public double Effectiveness
        {
            get 
            { 
                return this._effective; 
            }

            set
            {
                if (value > 0.0)
                {
                    this._effective = value;
                }
            }
        }

        /// <summary>
        /// Does the vaccine provides a lifetime protection against HIV transmission?
        /// </summary>
        [Category("1 - Definition"),
        DefaultValue(false),
        Description("Does the vaccine provides a lifetime protection against HIV transmission?")]
        public bool Lifetime
        {
            get { return this._lifetime; }
            set { this._lifetime = value; }
        }

        /// <summary>
        /// Length of the vaccine protection (if not lifetime).
        /// </summary>
        [Category("1 - Definition"),
        Description("Length of the vaccine protection (if not lifetime).")]
        public int Length
        {
            get 
            { 
                return this._length; 
            }

            set
            {
                if (value > 0)
                {
                    this._length = value;
                }
            }
        }

        /// <summary>
        /// Gets the list of intervention strategies using this vaccine
        /// </summary>
        [Category("2 - Control"),
        Description("List of intervention strategies using this vaccine")]
        public ListOfIds UsedBy
        {
            get { return this._usedby; }
        }
        #endregion //Properties

        #region Static Method
        /// <summary>
        /// Resets the vaccine unique id counter to 1.
        /// </summary>
        public static void Reset()
        {
            Vaccine.IdCounter = 0;
        }
        #endregion
    }
}
