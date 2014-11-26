// ----------------------------------------------------------------------------
// <copyright file="Strategy.cs" company="HIVacSim">
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
    /// Defines a preventive intervention strategy.
    /// </summary>
    [Serializable]
    public class Strategy
    {
        #region Static Field
        /// <summary>
        /// Unique identification counter
        /// </summary>
        private static int IdCounter;
        #endregion

        #region Local variables
        private int _id;		            //Intervention unique id number
        private string _name;		        //Short name describing the intervention
        private bool _active;	            //Active the strategy
        private EStrategy _strategy;	    //Defines the intervention strategy
        private ListOfIds _groups;	        //List of groups to intervene
        private int _clock;		            //Time for intervention
        private double _popvac;	            //Percentage of population to be vaccinated
        private Vaccine _usevac;	        //Vaccine to be used
        private bool _hivtest;	            //Vaccine only tested individuals
        private EHIVTest _hivresult;	    //HIV testing results
        #endregion //Local variables

        #region Constructor

        /// <summary>
        /// Static constructor
        /// </summary>
        static Strategy()
        {
            IdCounter = 0;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Strategy()
        {
            this._id = Interlocked.Increment(ref Strategy.IdCounter);
            this._name = "Intervention " + this._id.ToString();
            this._strategy = EStrategy.AllGroups;
            this._groups = new ListOfIds();
            this._hivtest = false;
            this._hivresult = EHIVTest.NotImportant;
            this._clock = 1;
            this.Population = 1.0;
        }
        #endregion

        #region Public properties

        /// <summary>
        /// Intervention strategy unique identification
        /// </summary>
        [CategoryAttribute("Definition"),
        DefaultValueAttribute(1),
        DescriptionAttribute("Intervention strategy unique identification.")]
        public int Id
        {
            get 
            { 
                return this._id;
            }

            set
            {
                this._id = value;
                if (Strategy.IdCounter <= this._id)
                {
                    Strategy.IdCounter = this._id + 1;
                }
            }
        }


        /// <summary>
        /// Intervention strategy identification name
        /// </summary>
        [CategoryAttribute("Definition"),
        DefaultValueAttribute("Intervention 1"),
        DescriptionAttribute("Intervention strategy identification name.")]
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// Is this intervention strategy active?  
        /// </summary>
        [CategoryAttribute("Definition"),
        DefaultValueAttribute("Intervention 1"),
        DescriptionAttribute("Is this intervention strategy active?")]
        public bool Active
        {
            get { return this._active; }
            set { this._active = value; }
        }

        /// <summary>
        /// Defines the intervention strategy
        /// </summary>
        [CategoryAttribute("Definition"),
        DescriptionAttribute("Defines the intervention strategy")]
        public EStrategy Intevention
        {
            get { return this._strategy; }
            set { this._strategy = value; }
        }

        /// <summary>
        /// Gets the list of groups to be intervened
        /// </summary>
        [CategoryAttribute("Definition"),
        DescriptionAttribute("Gets the list of groups to be intervened")]
        public ListOfIds Groups
        {
            get { return this._groups; }
        }


        /// <summary>
        /// Time for intervention as a function of the clock t
        /// </summary>
        [CategoryAttribute("Definition"),
        DescriptionAttribute("Time for intervention as a function of the clock t")]
        public int Clock
        {
            get { return this._clock; }
            set { this._clock = value; }
        }

        /// <summary>
        /// Percentage of population to be vaccinated [1 – 100%]
        /// </summary>
        [CategoryAttribute("Definition"),
        DescriptionAttribute("Percentage of population to be vaccinated [1 – 100%]")]
        public double Population
        {
            get 
            { 
                return this._popvac;
            }

            set
            {
                if (value > 0.0 && value <= 1.0)
                {
                    this._popvac = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(
                        "Population < 0 or population > 100%",
                        "Invalid population argumet.");
                }
            }
        }


        /// <summary>
        /// Vaccine to be used
        /// </summary>
        [CategoryAttribute("Definition"),
        DescriptionAttribute("Vaccine to be used")]
        public Vaccine UseVaccine
        {
            get { return this._usevac; }
            set { this._usevac = value; }
        }


        /// <summary>
        /// Vaccine only HIV tested individuals?
        /// </summary>
        [CategoryAttribute("Definition"),
        DescriptionAttribute("Vaccine only HIV tested individuals?")]
        public bool HIVTested
        {
            get { return this._hivtest; }
            set { this._hivtest = value; }
        }

        /// <summary>
        /// HIV test result
        /// </summary>
        [CategoryAttribute("Definition"),
        DescriptionAttribute("HIV test result")]
        public EHIVTest HIVResult
        {
            get { return this._hivresult; }
            set { this._hivresult = value; }
        }
        #endregion //Properties

        #region Static Method
        /// <summary>
        /// Resets the strategy unique id counter to 1.
        /// </summary>
        public static void Reset()
        {
            Strategy.IdCounter = 0;
        }
        #endregion
    }
}
