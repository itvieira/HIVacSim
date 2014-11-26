// ----------------------------------------------------------------------------
// <copyright file="Disease.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using HIVacSim.Utility;

    /// <summary>
    /// Defines the characterise of the sexually transmitted disease
    /// to be transmitted over the network.
    /// </summary>
    [Serializable]
    public class Disease
    {
        #region Local variables
        private string		_name;
        private double		_male2female;
        private double		_female2male;
        private double		_male2male;
        private bool		_lifeInfection;
        private Stochastic	_stdDuration;
        private bool		_reinfection;
        private double		_mortality; 
        private	Stochastic	_lifeExpect;
        #endregion

        #region Constructor
    
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">The name of the disease of interest</param>
        public Disease(string name)
        {
            this._name = name;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Disease():this("Unknown")
        {}

        #endregion
        
        #region Public Properties
        /// <summary>
        /// Name of the disease of interest
        /// </summary>
        [CategoryAttribute("1 - Definition"),
        DefaultValueAttribute("HIV/AIDS"),
        DescriptionAttribute("The name of the disease of interest to simulated.")]
        public string Name
        {
            get {return this._name;}
            set	{this._name = value;}
        }

        /// <summary>
        /// The probability of transmission per single sexual intercourse 
        /// from Male to Female"
        /// </summary>
        [CategoryAttribute("2 - Transmissibility"),
        DefaultValueAttribute(0.003),
        DescriptionAttribute("The probability of transmission per single " + 
                             "sexual intercourse from Male to Female")]
        public double Male2Female
        {
            get {return this._male2female;}
            set	{this._male2female = value;}
        }

        /// <summary>
        /// The probability of transmission per single sexual intercourse 
        /// from Female to Male"
        /// </summary>
        [CategoryAttribute("2 - Transmissibility"),
        DefaultValueAttribute(0.002),
        DescriptionAttribute("The probability of transmission per single " + 
                             "sexual intercourse from Female to Male")]
        public double Female2Male
        {
            get {return this._female2male;}
            set	{this._female2male = value;}
        }
        
        /// <summary>
        /// The probability of transmission per single sexual intercourse 
        /// from Male to Male (Homosexual)"
        /// </summary>
        [CategoryAttribute("2 - Transmissibility"),
        DefaultValueAttribute(0.01),
        DescriptionAttribute("The probability of transmission per single " + 
                        "sexual intercourse from Male to Male, (homosexual)")]
        public double Male2Male
        {
            get {return this._male2male;}
            set	{this._male2male = value;}
        }

        /// <summary>
        /// Is the duration of STD infection lifelong?
        /// </summary>
        [CategoryAttribute("3 - Duration of Infection"),
        DefaultValueAttribute(true),
        DescriptionAttribute("Is the duration of disease infection lifelong?")]
        public bool LifeInfection
        {
            get {return this._lifeInfection;}
            set	{this._lifeInfection = value;}
        }
                
        /// <summary>
        /// The duration of the STD infection (if not lifelong)
        /// </summary>
        [CategoryAttribute("3 - Duration of Infection"),
        DescriptionAttribute("If not lifelong infection, defines the duration"+
            " of the STD infection according with the simulation clock."),
        Editor(typeof(DistributionValueEditor), typeof(UITypeEditor))]
        public Stochastic STDDuration
        {
            get {return this._stdDuration;}
            set	{this._stdDuration = value;}
        }

        /// <summary>
        /// If not life long infection, does the disease reinfect its victims? 
        /// </summary>
        [CategoryAttribute("3 - Duration of Infection"),
        DefaultValueAttribute(false),
        DescriptionAttribute("If not life long infection, does the disease " + 
                             "reinfect its victims? ")]
        public bool AllowReinfection
        {
            get {return this._reinfection;}
            set	{this._reinfection = value;}
        }
       
        /// <summary>
        /// Life expectancy after infection with the disease.
        /// </summary>
        [CategoryAttribute("1 - Definition"),
        DefaultValueAttribute("Fixed(0,0,0,0)"),
        DescriptionAttribute("Life expectancy after infection with the disease."),
        Editor(typeof(DistributionValueEditor), typeof(UITypeEditor))]
        public Stochastic LifeExpectancy
        {
            get {return this._lifeExpect;}
            set	{this._lifeExpect = value;}
        }

        /// <summary>
        /// Mortality rate from the disease infection.
        /// </summary>
        [CategoryAttribute("1 - Definition"),
        DefaultValueAttribute(1.0),
        DescriptionAttribute("Mortality rate from the disease infection.")]
        public double Mortality
        {
            get {return this._mortality;}
            set	{this._mortality = value;}
        }
        #endregion
    } 
}
