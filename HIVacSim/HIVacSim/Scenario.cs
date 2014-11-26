// ----------------------------------------------------------------------------
// <copyright file="Scenario.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Schema;
    using HIVacSim.Utility;

    /// <summary>
    /// Defines the simulation scenario class, the manager of the
    /// simulation data and file I/O. 
    /// </summary>
    public class Scenario
    {
        #region Local variables
        //===========================================
        //Scenario data
        //===========================================

        /// <summary>
        /// The scenario population
        /// </summary>
        protected Population _pop;

        /// <summary>
        /// The scenario disease of interest
        /// </summary>
        protected Disease _info;

        /// <summary>
        /// The scenario preventive vaccine
        /// </summary>
        protected Vaccines _vaccine;

        /// <summary>
        /// The scenario intervention strategy
        /// </summary>
        protected Strategies _inteven;

        /// <summary>
        /// The scenario random number generator
        /// </summary>
        protected RandomDeviate _rnd;

        /// <summary>
        /// The scenario random number generator seed
        /// </summary>
        protected bool _seedAuto;

        /// <summary>
        /// The scenario random number generator seed
        /// </summary>
        protected int _seedUser;

        /// <summary>
        /// Uniform / Clustered initial distribution of infection
        /// </summary>
        protected bool _uinfect;

        //===========================================
        //Simulation configuration
        //===========================================

        /// <summary>
        /// Simulation's external clock
        /// </summary>
        protected ESimClock _simClock;

        /// <summary>
        /// Simulation's start date 
        /// </summary>
        protected DateTime _simDate;

        /// <summary>
        /// Duration of each simulation
        /// </summary>
        protected int _simDuration;

        /// <summary>
        /// Number of simulation runs
        /// </summary>
        protected int _simRuns;

        /// <summary>
        /// Simulation warm-up type
        /// </summary>
        protected EWarmup _simWarmType;

        /// <summary>
        /// Simulation warm-up duration
        /// </summary>
        protected int _simWarmupLen;

        /// <summary>
        /// Simulation warm-up minimum # of concurrent partners
        /// </summary>
        protected int _simWMaxConc;

        /// <summary>
        /// Simulation warm-up probability of concurrent partners
        /// </summary>
        protected double _simWPrConc;

        /// <summary>
        /// Simulation warm-up # of initial infected people
        /// </summary>
        protected Stochastic _simWInfected;

        /// <summary>
        /// Simulation's speed [1,100]
        /// </summary>
        protected int _simSpeed;

        /// <summary>
        /// Simulation's maximum delay for animation
        /// </summary>
        protected int _simMaxDelay;

        /// <summary>
        /// Current simulation sleep time
        /// </summary>
        protected int _simDelay;

        /// <summary>
        /// Simulation animation flag
        /// </summary>
        protected bool _simAnimate;

        //===========================================
        //File I/O Control variables
        //===========================================

        /// <summary>
        /// Indicates a that a file is open
        /// </summary>
        protected bool _fileOpen;

        /// <summary>
        /// Scenario file name
        /// </summary>
        protected string _fileName;

        /// <summary>
        /// New file, never saved before
        /// </summary>
        protected bool _fileNew;

        /// <summary>
        /// The current file has unsaved changes
        /// </summary>
        protected bool _fileChanged;

        /// <summary>
        /// Description of the scenario
        /// </summary>
        protected string _fileInfo;

        //===========================================
        //Path Length calculation
        //===========================================

        /// <summary>
        /// The maximum population size to calculate numerically
        /// </summary>
        protected int _plnumeric;

        /// <summary>
        /// The sample size for estimation
        /// </summary>
        protected int _plsample;

        /// <summary>
        /// The SWN p-value to switch algorithm
        /// </summary>
        protected double _plalgo;

        //===========================================
        //Control
        //===========================================

        /// <summary>
        /// Scenario validation error log 
        /// </summary>
        protected string _errLog;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Scenario()
        {
            //Random number generator setup
            this._rnd = new RandomDeviate();
            this._seedAuto = true;			//Initialise the generator with a automatic seed
            this._seedUser = 19650218;	    //Default custom seed
            this._simWMaxConc = 2;			//Default number of concurrent partners
            this._simWPrConc = 0.5;			//Default probability of concurrent partners
            this._simWInfected = new Stochastic(DeviateFunction.Average, 1.0);
            this._errLog = "No validation errors were reported.";
        }
        
        #endregion

        #region Public properties

        /// <summary>
        /// Returns the current population defined within the scenario.
        /// </summary>
        [CategoryAttribute("Configuration"),
        BrowsableAttribute(false),
        DescriptionAttribute("Returns the current population defined within the scenario.")]
        public Population Data
        {
            get { return this._pop; }
        }

        /// <summary>
        /// Gets the scenario's information to transmit (disease) definition.
        /// </summary>
        [CategoryAttribute("Configuration"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the scenario's information to transmit (disease) definition.")]
        public Disease Info
        {
            get { return this._info; }
        }

        /// <summary>
        /// Gets the scenario's preventive vaccine definition.
        /// </summary>
        [CategoryAttribute("Configuration"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the scenario's preventive vaccine definition.")]
        public Vaccines Vaccine
        {
            get { return this._vaccine; }
        }

        /// <summary>
        /// Gets the scenario's intervention strategies definition.
        /// </summary>
        [CategoryAttribute("Configuration"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the scenario's intervention strategies definition.")]
        public Strategies Strategy
        {
            get { return this._inteven; }
        }

        /// <summary>
        /// Gets the scenario's random number generator.
        /// </summary>
        [CategoryAttribute("Configuration"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the scenario's random number generator.")]
        public RandomDeviate Rnd
        {
            get { return this._rnd; }
        }

        /// <summary>
        /// Gets or sets the random number generator seeding behaviour.
        /// </summary>
        /// <remarks>
        /// The default option (<c>true</c>) initialise the random number generator once
        /// on start up with a internal generated seed using the system data/time, this 
        /// option will cause every simulation trial to generate a different results.
        /// <p></p>
        /// Alternatively, the user can disable the auto seeding option and use define
        /// a custom seed, which reset the random number generator to its initial status
        /// before start a new trial. This means that a second run of the same simulation
        /// trial will produce the same results.
        /// </remarks>
        [CategoryAttribute("Random Number"),
        DefaultValueAttribute(true),
        DescriptionAttribute("Gets or Sets the random number generator seeding behaviour.")]
        public bool AutoSeed
        {
            get 
            { 
                return this._seedAuto; 
            }

            set
            {
                this._seedAuto = value;
                if (this._seedAuto)
                {
                    this._rnd.ReSeed(Environment.TickCount);
                }
            }
        }

        /// <summary>
        /// Gets or sets the random number generator custom seed. 
        /// </summary>
        /// <remarks>
        /// This seed is used only when the scenario AutoSeed option is disabled
        /// </remarks>
        [CategoryAttribute("Random Number"),
        DefaultValueAttribute(19650218U),
        DescriptionAttribute("Gets or Sets the random number generator custom seed.")]
        public int CustomSeed
        {
            get { return this._seedUser; }
            set { this._seedUser = value; }
        }

        /// <summary>
        /// Gets or sets the uniform spread of the initial STD infection.
        /// </summary>
        [CategoryAttribute("WarmUp"),
        DefaultValueAttribute(false),
        DescriptionAttribute("Gets or sets the uniform spread of the initial STD infection.")]
        public bool UInfection
        {
            get { return this._uinfect; }
            set { this._uinfect = value; }
        }

        /// <summary>
        /// Gets or sets the external simulation clock.
        /// </summary>
        [CategoryAttribute("Configuration"),
        DefaultValueAttribute(ESimClock.Month),
        DescriptionAttribute("Gets or Sets the external simulation clock.")]
        public ESimClock UserClock
        {
            get { return this._simClock; }
            set { this._simClock = value; }
        }

        /// <summary>
        /// Gets or sets the simulation starting date.
        /// </summary>
        [CategoryAttribute("Configuration"),
        DescriptionAttribute("Gets or Sets the simulation starting date.")]
        public DateTime StartDate
        {
            get { return this._simDate; }
            set { this._simDate = value; }
        }

        /// <summary>
        /// Gets or sets the duration of each simulation run.
        /// </summary>
        [CategoryAttribute("Configuration"),
        DefaultValueAttribute(144),
        DescriptionAttribute("Gets or Sets the duration of each simulation run.")]
        public int Duration
        {
            get 
            { 
                return this._simDuration; 
            }

            set
            {
                if (value >= 1)
                {
                    this._simDuration = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of simulation runs
        /// </summary>
        [CategoryAttribute("Configuration"),
        DefaultValueAttribute(10),
        DescriptionAttribute("Gets or Sets the duration of each simulation run.")]
        public int Runs
        {
            get 
            { 
                return this._simRuns; 
            }

            set
            {
                if (value >= 1)
                {
                    this._simRuns = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the simulation warm-up configuration.
        /// </summary>
        [CategoryAttribute("WarmUp"),
        DefaultValueAttribute(EWarmup.Traditional),
        DescriptionAttribute("Gets or Sets the simulation warm-up configuration.")]
        public EWarmup WarmUpType
        {
            get { return this._simWarmType; }
            set { this._simWarmType = value; }
        }

        /// <summary>
        /// Gets or sets the duration of the warm-up for Traditional and Temporal
        /// warm-up types. For Conditional warm-p, it defines the maximum
        /// number of interactions without convergence.
        /// </summary>
        [CategoryAttribute("WarmUp"),
        DefaultValueAttribute(24),
        DescriptionAttribute("Gets or sets the duration of the warm-up for " +
            "Traditional and Temporal types. For Conditional warm-up, it " +
            "defines the maximum number of interactions without convergence.")]
        public int WarmUpTime
        {
            get 
            { 
                return this._simWarmupLen; 
            }

            set
            {
                if (value >= 0)
                {
                    this._simWarmupLen = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum number of concurrent partnerships
        /// allowed during the simulation warm-up.
        /// </summary>
        /// <remarks>
        /// After completing the warm-up this number switches back to the 
        /// original concurrency value defined within each group.
        /// </remarks>
        [CategoryAttribute("WarmUp"),
        DefaultValueAttribute(2),
        DescriptionAttribute("Gets or sets the minimum number of concurrent " +
                        "partnerships allowed during the simulation warm-up.")]
        public int WMaxConcurrent
        {
            get 
            { 
                return this._simWMaxConc; 
            }

            set
            {
                if (value >= 1)
                {
                    this._simWMaxConc = value;
                }
                else
                {
                    throw new SimulationException(
                        "Invalid maximum number of concurrent partners during warm-up [x < 1]");
                }
            }
        }

        /// <summary>
        /// Gets or sets the probability of concurrent partnerships
        /// during the simulation warm-up.
        /// </summary>
        /// <remarks>
        /// After completing the warm-up this number switches back to the 
        /// original concurrency value defined within each group.
        /// </remarks>
        [CategoryAttribute("WarmUp"),
        DefaultValueAttribute(0.5),
        DescriptionAttribute("Gets or sets the probability of concurrent " +
                                "partnerships during the simulation warm-up.")]
        public double WPrConcurrent
        {
            get { return this._simWPrConc; }
            set
            {
                if (Group.IsProbability(value) && value > 0.0)
                {
                    this._simWPrConc = value;
                }
                else
                {
                    throw new SimulationException(
                        "Invalid probability of concurrent partnerships during warm-up [x <= 0.0]");
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of initial infected individuals for 
        /// temporal and conditional warm-up.
        /// </summary>
        [CategoryAttribute("WarmUp"),
        DescriptionAttribute("Gets or sets the number of initial infected " +
                        "individuals for Temporal and Conditional warm-up."),
        Editor(typeof(DistributionValueEditor), typeof(UITypeEditor))]
        public Stochastic WarmUpInfected
        {
            get { return this._simWInfected; }
            set { this._simWInfected = value; }
        }

        /// <summary>
        /// Gets or sets the simulation speed.
        /// </summary>
        [CategoryAttribute("Configuration"),
        DefaultValueAttribute(100),
        DescriptionAttribute("Get or Set the simulation speed.")]
        public int Speed
        {
            get
            {
                return this._simSpeed;
            }

            set
            {
                if (value >= 0 && value <= 100)
                {
                    this._simSpeed = value;
                    this._simDelay = (int)(this._simMaxDelay - ((this._simMaxDelay * 0.5) * (value * 0.02)));
                }
            }
        }
        
        /// <summary>
        /// Get or set the maximum delay for animation.
        /// </summary>
        [CategoryAttribute("Configuration"),
        DefaultValueAttribute(2000),
        DescriptionAttribute("Get or Set the maximum delay for animation.")]
        public int MaximumDelay
        {
            get { return this._simMaxDelay; }
            set
            {
                if (value > 0)
                {
                    this._simMaxDelay = value;
                    this._simDelay = (int)(value - ((value * 0.5) * (this._simSpeed * 0.02)));
                }
            }
        }
        
        /// <summary>
        /// Gets the simulation current delay for animation.
        /// </summary>
        [CategoryAttribute("Configuration"),
        DefaultValueAttribute(0),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the simulation current delay for animation.")]
        public int Delay
        {
            get { return this._simDelay; }
        }
        
        /// <summary>
        /// Gets or sets the simulation animation status.
        /// </summary>
        [CategoryAttribute("Configuration"),
        DefaultValueAttribute(false),
        DescriptionAttribute("Gets or Sets the simulation animation status.")]
        public bool Animation
        {
            get { return this._simAnimate; }
            set { this._simAnimate = value; }
        }

        /// <summary>
        /// Gets the scenario validation erro log.
        /// </summary>
        [CategoryAttribute("Validation"),
        DefaultValueAttribute("No validation errors were reported."),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the scenario validation erro log.")]
        public string ErrorLog
        {
            get { return this._errLog; }
        }

        /// <summary>
        /// Gets or sets the maximum size for numerical calculation of the 
        /// network efficiency (path length)
        /// </summary>
        [CategoryAttribute("Path Length"),
        DefaultValueAttribute(500),
        DescriptionAttribute("Gets or sets the maximum size for numerical " +
                        "calculation of the network efficiency (path length)")]
        public int PLNumerical
        {
            get 
            { 
                return this._plnumeric; 
            }

            set
            {
                if (value >= 0)
                {
                    this._plnumeric = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the sample size for estimating the network 
        /// efficiency (path length)
        /// </summary>
        [CategoryAttribute("Path Length"),
        DefaultValueAttribute(400),
        DescriptionAttribute("Gets or sets the sample size for estimating " +
                             "the network efficiency (path length)")]
        public int PLEstimate
        {
            get 
            { 
                return this._plsample; 
            }

            set
            {
                if (value > 5)
                {
                    this._plsample = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the SWN p value (probability of casual) for 
        /// switching algorithm (Floyd 0--p--1 BFS)
        /// </summary>
        [CategoryAttribute("Path Length"),
        DefaultValueAttribute(0.13),
        DescriptionAttribute("Gets or sets the SWN p value (probability " +
                    "of casual) for switching algorithm (Floyd 0--p--1 BFS)")]
        public double PLAlgorithm
        {
            get 
            { 
                return this._plalgo; 
            }

            set
            {
                if (Group.IsProbability(value))
                {
                    this._plalgo = value;
                }
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new simulation scenario
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public bool NewScenario()
        {
            //Reset the scenario and std definition
            if (CloseScenario())
            {
                this._info = new Disease("HIV/AIDS");
                this._pop = new Population();
                this._vaccine = new Vaccines();
                this._inteven = new Strategies();
                this._fileOpen = true;
                this._fileNew = true;
                this._fileName = "Untitled.xml";
                this.SimDefault();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Close current simulation scenario
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public bool CloseScenario()
        {
            if (!this._fileOpen)
            {
                return true;
            }

            this._fileOpen = false;
            this._fileNew = false;
            this._fileName = "";
            this._fileChanged = false;
            this._fileInfo = "";
            this._info = null;
            this._pop.Clear();
            this._pop = null;
            this._vaccine.Clear();
            this._vaccine = null;
            this._inteven.Clear();
            this._inteven = null;
            this._errLog = "No validation errors were reported.";
            System.GC.Collect();
            return true;
        }

        /// <summary>
        /// Sets the default simulation parameters
        /// </summary>
        public void SimDefault()
        {
            //Default values
            this._simDate = DateTime.Now;
            this._simClock = ESimClock.Month;
            this._simDuration = 144;
            this._simRuns = 10;
            this._uinfect = false;
            this._simWarmType = EWarmup.Traditional;
            this._simWarmupLen = 24;
            this.MaximumDelay = 2000; //(2 Seconds)
            this._simSpeed = 100;
            this._seedAuto = true;
            this._seedUser = 19650218;
            this._simWMaxConc = 2;
            this._simWPrConc = 0.5;
            this._errLog = "No validation errors were reported.";
            this._plnumeric = 500;
            this._plsample = 400;
            this._plalgo = 0.13;
        }
        
        /// <summary>
        /// Validate the scenario data
        /// </summary>
        /// <remarks>
        /// The cause of the scenario invalidation can be found in the 
        /// <see cref="ErrorLog"/> property, immediately after the return.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if valid, otherwise <c>false</c>
        /// </returns>
        public bool ValidateScenario()
        {
            //Local variables
            int g, gCount, p;
            double rsum;
            Group grp;
            bool valid;
            string nl = Environment.NewLine;

            //Setup
            this._errLog = "";
            valid = true;
            gCount = this._pop.Count;

            //Checks for a valid file
            if (this._fileOpen == false)
            {
                this._errLog += "No file open!" + nl;
                valid = false;
            }

            //Validate the Population Size
            for (g = 0; g < gCount; g++)
            {
                grp = this._pop[g];
                if (grp.Size < 2)
                {
                    this._errLog += "Invalid population size for Group [" +
                                    grp.Name + "]." + nl;
                    valid = false;
                }
            }
            
            //Validate spherical search distance definition
            for (g = 0; g < gCount; g++)
            {
                grp = this._pop[g];
                if (grp.Topology == ETopology.Sphere)
                {
                    if (Math.Cos(grp.Distance / grp.Radius) > (Math.PI * grp.Radius))
                    {
                        this._errLog += "Invalid sphere searching distance " +
                            "for population group [" + grp.Name + "]" + nl;
                        valid = false;
                    }
                }
            }

            //Validate adjMatrix
            for (g = 0; g < gCount; g++)
            {
                rsum = 0.0;
                for (p = 0; p < gCount; p++)
                {
                    if (g == p) //Diagonal
                    {
                        if (this._pop.AdjMatrix[g, p] != 0.0)
                        {
                            this._errLog += "Adj. Matrix invalid diagonal at [" +
                                g.ToString() + "," + p.ToString() + "] = " +
                                this._pop.AdjMatrix[g, p] + nl;
                            valid = false;
                        }
                    }
                    else
                    {
                        if (this._pop.AdjMatrix[g, p] < 0.0 || this._pop.AdjMatrix[g, p] > 1.0)
                        {
                            this._errLog += "Adj. Matrix invalid value at [" +
                                g.ToString() + "," + p.ToString() + "] = " +
                                this._pop.AdjMatrix[g, p] + nl;
                            valid = false;
                        }

                        rsum += this._pop.AdjMatrix[g, p];
                    }
                }

                if (rsum != 0.0 && rsum != 1.0)
                {
                    this._errLog += "Adj. Matrix invalid sum at row [" +
                            g.ToString() + "] = " + rsum.ToString() + nl;
                    valid = false;
                }
            }

            //######### TODO ##########
            //Validate vaccines

            //Validate interventions
            //#########################

            //reset the error log
            if (valid)
            {
                this._errLog = "No validation errors were reported.";
            }

            //Returns the result
            return valid;
        }

        #endregion

        #region File I/O Methods

        /// <summary>
        /// Gets the current scenario file status
        /// </summary>
        [CategoryAttribute("File I/O"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the current scenario file status")]
        public bool IsFileOpen
        {
            get { return this._fileOpen; }
        }
        
        /// <summary>
        /// Gets the current scenario full file name
        /// </summary>
        [CategoryAttribute("File I/O"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the current scenario full file name")]
        public string FileName
        {
            get { return this._fileName; }
        }

        /// <summary>
        /// Gets the current scenario short file name
        /// </summary>
        [CategoryAttribute("File I/O"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the current scenario short file name")]
        public string ShortFileName
        {
            get
            {
                return this._fileName.Substring(this._fileName.LastIndexOf("\\") + 1);
            }
        }

        /// <summary>
        /// Gets the current scenario file creation status
        /// </summary>
        [CategoryAttribute("File I/O"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the current scenario file creation status")]
        public bool IsFileNew
        {
            get { return this._fileNew; }
        }
        
        /// <summary>
        /// Gets or sets the current scenario file saving status
        /// </summary>
        [CategoryAttribute("File I/O"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets or sets the current scenario file saving status")]
        public bool HasFileChanged
        {
            get { return this._fileChanged; }
            set { this._fileChanged = value; }
        }
        
        /// <summary>
        /// Gets or sets the scenario file description
        /// </summary>
        [CategoryAttribute("File I/O"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets or sets the scenario file description")]
        public string Description
        {
            get { return this._fileInfo; }
            set { this._fileInfo = value; }
        }
        
        /// <summary>
        /// Save the current scenarion data to a Xml file
        /// </summary>
        /// <param name="inFileName">
        /// The path and name of the file to save the scenario
        /// </param>
        /// <returns>True if successful, False otherwise</returns>
        public bool SaveScenario(string inFileName)
        {
            XmlTextWriter xmlWrite = new XmlTextWriter(
                                                    inFileName,
                                                    System.Text.Encoding.UTF8);
            xmlWrite.Formatting = Formatting.Indented;
            xmlWrite.Indentation = 4;

            //Start the XML Document
            xmlWrite.WriteStartDocument();

            //Write root element
            xmlWrite.WriteStartElement("HIVacSim");
            xmlWrite.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xmlWrite.WriteAttributeString("xmlns", "urn:HIVacSim");
            xmlWrite.WriteAttributeString("xsi:schemaLocation", "urn:HIVacSim HIVacSimData.xsd");
            xmlWrite.WriteAttributeString("Title",
                "Simulation Models for the Control of the Dynamics of HIV Infection Through Vaccination");
            xmlWrite.WriteAttributeString("by", "Israel Vieira");
            xmlWrite.WriteAttributeString("Version", "1.7");

            //Scenarion Settings
            xmlWrite.WriteComment("Scenario's simulation settings definition.");
            xmlWrite.WriteStartElement("Scenario");
            xmlWrite.WriteElementString("Description", this._fileInfo);
            xmlWrite.WriteElementString("SimClock", this._simClock.ToString());
            xmlWrite.WriteElementString("RunDuration", this._simDuration.ToString());
            xmlWrite.WriteElementString("NumberOfRuns", this._simRuns.ToString());
            xmlWrite.WriteElementString("WarmUpType", this._simWarmType.ToString());
            xmlWrite.WriteElementString("WarmUp", this._simWarmupLen.ToString());
            xmlWrite.WriteElementString("WarmUpMaxConcurrent", this._simWMaxConc.ToString());
            xmlWrite.WriteElementString("WarmUpPrConcurrent", this._simWPrConc.ToString("R"));
            this.Stochastic2Xml("WarmUpInfected", xmlWrite, this._simWInfected);
            xmlWrite.WriteElementString("Speed", this._simSpeed.ToString());
            xmlWrite.WriteElementString("MaxDelay", this._simMaxDelay.ToString());
            xmlWrite.WriteElementString("Animation", this._simAnimate.ToString().ToLower());
            xmlWrite.WriteElementString("AutoSeed", this._seedAuto.ToString().ToLower());
            xmlWrite.WriteElementString("CustomSeed", this._seedUser.ToString());
            xmlWrite.WriteElementString("PLNumerical", this._plnumeric.ToString());
            xmlWrite.WriteElementString("PLEstimate", this._plsample.ToString());
            xmlWrite.WriteElementString("PLAlgorithm", this._plalgo.ToString("R"));
            xmlWrite.WriteEndElement();//HIVacSim

            //STD Definition
            xmlWrite.WriteComment("Define the disease of interest to be simulated");
            xmlWrite.WriteStartElement("Disease");
            xmlWrite.WriteElementString("Name", this._info.Name);
            xmlWrite.WriteElementString("Male2Female", this._info.Male2Female.ToString("R"));
            xmlWrite.WriteElementString("Female2Male", this._info.Female2Male.ToString("R"));
            xmlWrite.WriteElementString("Male2Male", this._info.Male2Male.ToString("R"));
            xmlWrite.WriteElementString("LifeInfection", this._info.LifeInfection.ToString().ToLower());
            this.Stochastic2Xml("STDDuration", xmlWrite, this._info.STDDuration);
            xmlWrite.WriteElementString("AllowReinfection", this._info.AllowReinfection.ToString().ToLower());
            xmlWrite.WriteElementString("Mortality", this._info.Mortality.ToString("R"));
            this.Stochastic2Xml("LifeExpectancy", xmlWrite, this._info.LifeExpectancy);
            xmlWrite.WriteEndElement();//Disease

            //Save population and core Groups definitions
            xmlWrite.WriteComment("Population Core Groups Definition");
            xmlWrite.WriteStartElement("Population");
            xmlWrite.WriteAttributeString("Count", this._pop.Count.ToString());

            Group cGroup;
            int idx, i, j, grpCount;
            grpCount = this._pop.Count;
            for (idx = 0; idx < grpCount; idx++)
            {
                cGroup = this._pop[idx];
                xmlWrite.WriteStartElement("Group");
                xmlWrite.WriteAttributeString("Name", cGroup.Name);
                xmlWrite.WriteElementString("Id", cGroup.Id.ToString());
                xmlWrite.WriteElementString("Topology", cGroup.Topology.ToString());
                xmlWrite.WriteElementString("Alpha", cGroup.Alpha.ToString("R"));
                xmlWrite.WriteElementString("Beta", cGroup.Beta.ToString("R"));
                xmlWrite.WriteElementString("Radius", cGroup.Radius.ToString("R"));
                xmlWrite.WriteElementString("Distance", cGroup.Distance.ToString("R"));
                xmlWrite.WriteElementString("Degrees", cGroup.Degrees.ToString());
                xmlWrite.WriteElementString("Size", cGroup.Size.ToString());
                this.Stochastic2Xml("Age", xmlWrite, cGroup.Age);
                this.Stochastic2Xml("LifeExpectancy", xmlWrite, cGroup.LifeExpectancy);
                xmlWrite.WriteElementString("STDPrevalence", cGroup.STDPrevalence.ToString("R"));
                this.Stochastic2Xml("STDAge", xmlWrite, cGroup.STDAge);
                xmlWrite.WriteElementString("STDTest", cGroup.STDTest.ToString("R"));
                xmlWrite.WriteElementString("Female", cGroup.Female.ToString("R"));
                xmlWrite.WriteElementString("Male", cGroup.Male.ToString("R"));
                xmlWrite.WriteElementString("Homosexual", cGroup.Homosexual.ToString("R"));
                xmlWrite.WriteElementString("MaxConcurrent", cGroup.MaxConcurrent.ToString());
                xmlWrite.WriteElementString("PrConcurrent", cGroup.PrConcurrent.ToString("R"));
                xmlWrite.WriteElementString("PrNewPartner", cGroup.PrNewPartner.ToString("R"));
                xmlWrite.WriteElementString("PrCasual", cGroup.PrCasual.ToString("R"));
                xmlWrite.WriteElementString("PrInternal", cGroup.PrInternal.ToString("R"));
                this.Stochastic2Xml("StbDuration", xmlWrite, cGroup.StbDuration);
                this.Stochastic2Xml("StbTransitory", xmlWrite, cGroup.StbTransitory);
                this.Stochastic2Xml("StbContacts", xmlWrite, cGroup.StbContacts);
                xmlWrite.WriteElementString("StbSafeSex", cGroup.StbSafeSex.ToString("R"));
                this.Stochastic2Xml("CslDuration", xmlWrite, cGroup.CslDuration);
                this.Stochastic2Xml("CslContacts", xmlWrite, cGroup.CslContacts);
                xmlWrite.WriteElementString("CslSafeSex", cGroup.CslSafeSex.ToString("R"));
                xmlWrite.WriteEndElement();//Group
            }

            //Population
            xmlWrite.WriteEndElement();

            //Save population digraph adjacency matrix
            xmlWrite.WriteComment("Population interaction, digraph adjacency matrix");
            xmlWrite.WriteStartElement("AdjMatrix");
            xmlWrite.WriteAttributeString("Rows", grpCount.ToString());
            xmlWrite.WriteAttributeString("Columns", grpCount.ToString());

            for (i = 0; i < grpCount; i++)
            {
                for (j = 0; j < grpCount; j++)
                {
                    xmlWrite.WriteStartElement("Cell");
                    xmlWrite.WriteAttributeString("Row", i.ToString());
                    xmlWrite.WriteAttributeString("Column", j.ToString());
                    xmlWrite.WriteAttributeString("Data", this._pop.AdjMatrix[i, j].ToString("R"));
                    xmlWrite.WriteEndElement();
                }
            }

            //AdjMatrix
            xmlWrite.WriteEndElement();

            //Save vaccines
            xmlWrite.WriteComment("Preventive vaccine interventions");
            xmlWrite.WriteStartElement("Vaccines");
            xmlWrite.WriteAttributeString("Count", this._vaccine.Count.ToString());
            if (this._vaccine.Count > 0)
            {
                Vaccine cVac;
                grpCount = this._vaccine.Count;
                for (idx = 0; idx < grpCount; idx++)
                {
                    cVac = this._vaccine[idx];
                    xmlWrite.WriteStartElement("Vaccine");
                    xmlWrite.WriteAttributeString("Name", cVac.Name);
                    xmlWrite.WriteElementString("Id", cVac.Id.ToString());
                    xmlWrite.WriteElementString("Effectiveness", cVac.Effectiveness.ToString("R"));
                    xmlWrite.WriteElementString("Lifetime", cVac.Lifetime.ToString().ToLower());
                    xmlWrite.WriteElementString("Length", cVac.Length.ToString());
                    xmlWrite.WriteElementString("UsedBy", cVac.UsedBy.ToString());
                    xmlWrite.WriteEndElement();
                }
            }

            //Vaccines
            xmlWrite.WriteEndElement();

            //Save intervention strategies
            xmlWrite.WriteComment("Intervention strategies");
            xmlWrite.WriteStartElement("Interventions");
            xmlWrite.WriteAttributeString("Count", this._inteven.Count.ToString());
            if (this._inteven.Count > 0)
            {
                Strategy cStrat;
                grpCount = this._inteven.Count;
                for (idx = 0; idx < grpCount; idx++)
                {
                    cStrat = this._inteven[idx];
                    xmlWrite.WriteStartElement("Strategy");
                    xmlWrite.WriteAttributeString("Name", cStrat.Name);
                    xmlWrite.WriteElementString("Id", cStrat.Id.ToString());
                    xmlWrite.WriteElementString("Active", cStrat.Active.ToString().ToLower());
                    xmlWrite.WriteElementString("Strategy", cStrat.Intevention.ToString());
                    xmlWrite.WriteElementString("Groups", cStrat.Groups.ToString());
                    xmlWrite.WriteElementString("Clock", cStrat.Clock.ToString());
                    xmlWrite.WriteElementString("Population", cStrat.Population.ToString());
                    xmlWrite.WriteElementString("Vaccine", cStrat.UseVaccine.Id.ToString());
                    xmlWrite.WriteElementString("HIVTested", cStrat.HIVTested.ToString().ToLower());
                    xmlWrite.WriteElementString("HIVResult", cStrat.HIVResult.ToString());
                    xmlWrite.WriteEndElement();
                }
            }

            //Strategy
            xmlWrite.WriteEndElement();

            //End HIVacSim Element
            xmlWrite.WriteEndElement();

            //End the XML Document
            xmlWrite.WriteEndDocument();
            xmlWrite.Flush();
            xmlWrite.Close();

            //Reset Flags
            this._fileNew = false;
            this._fileName = inFileName;
            this._fileChanged = false;
            return true;
        }
        
        /// <summary>
        /// Writes a stochastic data structure to a Xml file
        /// </summary>
        /// <param name="label">The XML element header</param>
        /// <param name="file">The <see cref="XmlTextWriter"/>file handler to write the data</param>
        /// <param name="data">The <see cref="System.Xml"/> data to write to file</param>
        private void Stochastic2Xml(string label, XmlTextWriter file, Stochastic data)
        {
            file.WriteStartElement(label);
            file.WriteElementString("Distribution", data.Distribution.ToString());
            file.WriteElementString("ParamOne", data.Parameter1.ToString("R"));
            file.WriteElementString("ParamTwo", data.Parameter2.ToString("R"));
            file.WriteElementString("ParamThree", data.Parameter3.ToString("R"));
            file.WriteElementString("ParamFour", data.Parameter4.ToString("R"));
            file.WriteEndElement();

        }
        
        /// <summary>
        /// Open a scenario from a Xml file
        /// </summary>
        /// <param name="inFileName">
        /// The path and name of the scenario file to open
        /// </param>
        /// <param name="xsdPath">The path of the XML validation schema</param>
        /// <returns>True if successful, False otherwise</returns>
        public bool OpenScenario(string xsdPath, string inFileName)
        {
            //Validate the input file
            if (!XMLValidator(xsdPath, inFileName))
            {
                return false;
            }

            //Create a new scenario
            this.NewScenario();

            //Read xml file
            XmlTextReader xmlfile = new XmlTextReader(inFileName);
            XPathDocument xpnDoc = new XPathDocument(xmlfile);
            XPathNavigator xpnNav = xpnDoc.CreateNavigator();

            //Move to root and first children - HIVacSim
            xpnNav.MoveToRoot();		//Root node
            xpnNav.MoveToFirstChild();	//HIVacSim
            if (!xpnNav.MoveToFirstChild())
            {
                return false;
            }

            do
            {
                if (xpnNav.NodeType == XPathNodeType.Element)
                {
                    switch (xpnNav.Name)
                    {
                        case "Scenario":
                            {
                                xpnNav.MoveToFirstChild();
                                this._fileInfo = xpnNav.Value;
                                xpnNav.MoveToNext();
                                this._simClock = (ESimClock)Enum.Parse(typeof(ESimClock), xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._simDuration = int.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._simRuns = int.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._simWarmType = (EWarmup)Enum.Parse(typeof(EWarmup), xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._simWarmupLen = int.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._simWMaxConc = int.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._simWPrConc = double.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._simWInfected = Xml2Stochastic(xpnNav);
                                xpnNav.MoveToNext();
                                this._simSpeed = int.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this.MaximumDelay = int.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._simAnimate = bool.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._seedAuto = bool.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._seedUser = int.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._plnumeric = int.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._plsample = int.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._plalgo = double.Parse(xpnNav.Value);
                                xpnNav.MoveToParent();
                                break;
                            }

                        case "Disease":
                            {
                                xpnNav.MoveToFirstChild();
                                this._info.Name = xpnNav.Value;
                                xpnNav.MoveToNext();
                                this._info.Male2Female = double.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._info.Female2Male = double.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._info.Male2Male = double.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._info.LifeInfection = bool.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._info.STDDuration = Xml2Stochastic(xpnNav);
                                xpnNav.MoveToNext();
                                this._info.AllowReinfection = bool.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._info.Mortality = double.Parse(xpnNav.Value);
                                xpnNav.MoveToNext();
                                this._info.LifeExpectancy = Xml2Stochastic(xpnNav);
                                xpnNav.MoveToParent();
                                break;
                            }

                        case "Population":
                            {
                                if (xpnNav.HasChildren)
                                {
                                    xpnNav.MoveToFirstChild(); //Group 1
                                    do
                                    {
                                        Group core = new Group(this._rnd);
                                        core.Name = xpnNav.GetAttribute("Name", String.Empty);

                                        //Mode to the first element of the group
                                        xpnNav.MoveToFirstChild();
                                        core.Id = int.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.Topology = (ETopology)Enum.Parse(typeof(ETopology), xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.Alpha = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.Beta = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.Radius = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.Distance = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.Degrees = int.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.Size = int.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.Age = Xml2Stochastic(xpnNav);
                                        xpnNav.MoveToNext();
                                        core.LifeExpectancy = Xml2Stochastic(xpnNav);
                                        xpnNav.MoveToNext();
                                        core.STDPrevalence = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.STDAge = Xml2Stochastic(xpnNav);
                                        xpnNav.MoveToNext();
                                        core.STDTest = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.Female = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.Male = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.Homosexual = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.MaxConcurrent = int.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.PrConcurrent = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.PrNewPartner = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.PrCasual = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.PrInternal = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.StbDuration = Xml2Stochastic(xpnNav);
                                        xpnNav.MoveToNext();
                                        core.StbTransitory = Xml2Stochastic(xpnNav);
                                        xpnNav.MoveToNext();
                                        core.StbContacts = Xml2Stochastic(xpnNav);
                                        xpnNav.MoveToNext();
                                        core.StbSafeSex = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        core.CslDuration = Xml2Stochastic(xpnNav);
                                        xpnNav.MoveToNext();
                                        core.CslContacts = Xml2Stochastic(xpnNav);
                                        xpnNav.MoveToNext();
                                        core.CslSafeSex = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToParent();
                                        this._pop.AddGroup(core);
                                    }
                                    while (xpnNav.MoveToNext());

                                    xpnNav.MoveToParent();
                                }

                                break;
                            }

                        case "AdjMatrix":
                            {
                                double d;
                                int idx, i, j, count;
                                count = int.Parse(xpnNav.GetAttribute("Rows", String.Empty));
                                if (count > 0)
                                {
                                    count *= count;
                                    xpnNav.MoveToFirstChild(); //First cell
                                    for (idx = 0; idx < count; idx++)
                                    {
                                        i = int.Parse(xpnNav.GetAttribute("Row", String.Empty));
                                        j = int.Parse(xpnNav.GetAttribute("Column", String.Empty));
                                        d = double.Parse(xpnNav.GetAttribute("Data", String.Empty));
                                        this._pop.AdjMatrix[i, j] = d;
                                        xpnNav.MoveToNext();
                                    }

                                    xpnNav.MoveToParent();
                                }

                                break;
                            }

                        case "Vaccines":
                            {
                                if (xpnNav.HasChildren)
                                {
                                    xpnNav.MoveToFirstChild(); //Vaccine 1
                                    do
                                    {
                                        Vaccine cVac = new Vaccine();
                                        cVac.Name = xpnNav.GetAttribute("Name", String.Empty);
                                        //Mode to the first element of the vaccine
                                        xpnNav.MoveToFirstChild();
                                        cVac.Id = int.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        cVac.Effectiveness = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        cVac.Lifetime = bool.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        cVac.Length = int.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        if (xpnNav.Value.Length > 0)
                                        {
                                            cVac.UsedBy.Add(xpnNav.Value);
                                        }
                                        this._vaccine.Add(cVac);
                                        xpnNav.MoveToParent();
                                    } while (xpnNav.MoveToNext());

                                    xpnNav.MoveToParent();
                                }

                                break;
                            }

                        case "Interventions":
                            {
                                if (xpnNav.HasChildren)
                                {
                                    xpnNav.MoveToFirstChild(); //Strategy 1
                                    do
                                    {
                                        Strategy cStrat = new Strategy();
                                        cStrat.Name = xpnNav.GetAttribute("Name", String.Empty);
                                        //Mode to the first element of the strategy
                                        xpnNav.MoveToFirstChild();
                                        cStrat.Id = int.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        cStrat.Active = bool.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        cStrat.Intevention = (EStrategy)Enum.Parse(typeof(EStrategy), xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        if (cStrat.Intevention == EStrategy.Custom && xpnNav.Value.Length > 0)
                                        {
                                            cStrat.Groups.Add(xpnNav.Value);
                                        }
                                        xpnNav.MoveToNext();
                                        cStrat.Clock = int.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        cStrat.Population = double.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        cStrat.UseVaccine = this._vaccine[this._vaccine.IndexOf(int.Parse(xpnNav.Value))];
                                        xpnNav.MoveToNext();
                                        cStrat.HIVTested = bool.Parse(xpnNav.Value);
                                        xpnNav.MoveToNext();
                                        cStrat.HIVResult = (EHIVTest)Enum.Parse(typeof(EHIVTest), xpnNav.Value); ;
                                        this._inteven.Add(cStrat);
                                        xpnNav.MoveToParent();
                                    } while (xpnNav.MoveToNext());

                                    xpnNav.MoveToParent();
                                }

                                break;
                            }
                    }
                }
            } while (xpnNav.MoveToNext());

            //Update file I/O flags
            this._fileOpen = true;
            this._fileNew = false;
            this._fileName = inFileName;
            this._fileChanged = false;

            //Close the files
            xpnNav = null;
            xpnDoc = null; xmlfile.Close();
            GC.Collect();

            //Everything fine.
            return true;
        }
        
        /// <summary>
        /// Read a Xml file as a stochastic data structure
        /// </summary>
        /// <param name="xpnTNav">The Xml file <see cref="XPathNavigator" /></param>
        /// <returns>The <see cref="Stochastic"/> data read from file</returns>
        private Stochastic Xml2Stochastic(XPathNavigator xpnTNav)
        {
            Stochastic temp = new Stochastic();
            xpnTNav.MoveToFirstChild();
            temp.Distribution = (DeviateFunction)Enum.Parse(typeof(DeviateFunction), xpnTNav.Value);
            xpnTNav.MoveToNext();
            temp.Parameter1 = double.Parse(xpnTNav.Value);
            xpnTNav.MoveToNext();
            temp.Parameter2 = double.Parse(xpnTNav.Value);
            xpnTNav.MoveToNext();
            temp.Parameter3 = double.Parse(xpnTNav.Value);
            xpnTNav.MoveToNext();
            temp.Parameter4 = double.Parse(xpnTNav.Value);
            xpnTNav.MoveToParent();
            return temp;
        }
        
        /// <summary>
        /// HIVacSim data file validation
        /// </summary>
        /// <param name="xmlFile">The HIVacSim data file to be validated</param>
        /// <param name="xsdPath">The path of the XML validation schema</param>
        /// <returns>True for a valid data file, false otherwise.</returns>
        public bool XMLValidator(string xsdPath, string xmlFile)
        {
            try
            {
                //Clear the error log
                this._errLog = string.Empty;

                // Create the XmlSchemaSet class.
                XmlSchemaSet sc = new XmlSchemaSet();

                // Add the schema to the collection.
                sc.Add("urn:HIVacSim", xsdPath + "\\HIVacSimData.xsd");

                // Set the validation settings.
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.Schemas = sc;
                settings.ValidationEventHandler += new ValidationEventHandler(XMLValidationHandler);

                // Create the XmlReader object.
                XmlReader reader = XmlReader.Create(xmlFile, settings);

                // Parse the file. 
                while (reader.Read()) ;

                if (this._errLog.Length > 0)
                {
                    return false;
                }
                else
                {
                    this._errLog = "No validation errors were reported.";
                    return true;
                }
            }
            catch (Exception err)
            {
                this._errLog += err.Message + Environment.NewLine;
                return false;
            }
        }

        /// <summary>
        /// HIVacSim data file validator event handler, which creates 
        /// the validation error log
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="args">The event argument data</param>
        private void XMLValidationHandler(object sender, ValidationEventArgs args)
        {
            this._errLog += args.Message + Environment.NewLine;
        }

        #endregion
    }
}
