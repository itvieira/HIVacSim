// ----------------------------------------------------------------------------
// <copyright file="Group.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.IO;
    using System.Threading;
    using HIVacSim.Utility;

    /// <summary>
    /// Defines the structure of a population group as a simple,
    /// unweighed and undirected graph.
    /// </summary>
    [Serializable]
    public class Group
    {
        #region Static Field
        /// <summary>
        /// Unique identification counter
        /// </summary>
        private static int IdCounter;
        #endregion

        #region Local Variables
        private int _id;			        // Group id
        private string _name;			    // Group name
        private ETopology _topology;		// The group representation topology
        private int _size;			        // Population size
        private Stochastic _age;			// Population age distribution
        private Stochastic _lifeExpect;	    // Life expectancy
        private double _female;		        // Proportion of females
        private double _male;			    // Proportion of males
        private double _gay;			    // Proportion of homosexual males
        private double _stdPrev;		    // STD Prevalence
        private Stochastic _stdAge;		    // Age of STD infection
        private double _stdTest;		    // STD testing rate
        private double _prSniper;		    // Probability of looking for a partnership
        private int _maxPartner;	        // Maximum number of concurrent partners
        private double _prConcurrent;	    // Probability of concurrent partnerships
        private double _prCasual;		    // Probability of casual partnership
        private double _prInternal;	        // Probability of search first internal for casual partnership
        private Stochastic _stbDuration;	// Duration of stable partnership
        private Stochastic _stbTransit;	    // Time between stable partnerships
        private Stochastic _stbSex;		    // Rare of sexual contacts for stable
        private double _stbPrSafe;		    // Probability of safe sex practice for stable
        private Stochastic _cslDuration;	// Duration of casual partnership
        private Stochastic _cslSex;		    // Rare of sexual contacts for casual
        private double _cslPrSafe;		    // Probability of safe sex practice for casual

        //Undirected graph definition
        private Person[] _vertices;		    // Collection of vertices (people)
        private RandomDeviate _rnd;			// Random number generator
        private bool _init;			        // Is the group population initialised?

        //Population interaction and control variables
        private double _alpha;			    // List of friends precondition function
        private double _beta;			    // Random search precondition function
        private double _radius;		        // Radius of the sphere
        private double _distance;		    // Search distance on the sphere surface
        private int _degrees;		        // Search list of acquaintance up to n degrees
        private bool _warmed;		        // Flag to identify if this group has finished its warm-up
        private double _initPrev;		    // Holds the initial value for HIV prevalence
        private int _initMPartner;	        // Holds the initial value for Maximum concurrent partners.
        private double _initPrConcur;	    // Holds the initial value for Probability of concurrent partners.

        //Properties Control
        private int _visit;			    //Temporary visit index
        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor
        /// </summary>
        static Group()
        {
            IdCounter = 0;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Group(RandomDeviate rnd)
        {
            // Defines the group unique identification
            this._id = Interlocked.Increment(ref Group.IdCounter);

            this._name = "Group " + this._id.ToString();
            this._rnd = rnd;
            this._alpha = 1.0;
            this._beta = 1.0;
            this._degrees = 3;
            this._maxPartner = 5;	//Default number of partners
            this._prConcurrent = 0.0;
            this._topology = ETopology.Sphere;
            this._radius = 6378;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Group unique identification
        /// </summary>
        [CategoryAttribute("1 - Definition"),
        DefaultValueAttribute(1),
        DescriptionAttribute("Group unique identification.")]
        public int Id
        {
            get 
            { 
                return this._id;
            }

            set
            {
                this._id = value;
                if (Group.IdCounter <= this._id)
                {
                    Group.IdCounter = this._id + 1;
                }
            }
        }


        /// <summary>
        /// Group identification name
        /// </summary>
        [CategoryAttribute("1 - Definition"),
        DefaultValueAttribute("Group 1"),
        DescriptionAttribute("Group identification name.")]
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }


        /// <summary>
        /// Group population representation topology
        /// </summary>
        [CategoryAttribute("6 - Structure"),
        DefaultValueAttribute(ETopology.Sphere),
        DescriptionAttribute("Group population representation topology.")]
        public ETopology Topology
        {
            get { return this._topology; }
            set { this._topology = value; }
        }


        /// <summary>
        /// Size of the group population
        /// </summary>
        [CategoryAttribute("1 - Definition"),
        DescriptionAttribute("Size of the group population.")]
        public int Size
        {
            get 
            { 
                return this._size; 
            }

            set
            {
                if (value >= 2)
                {
                    this._size = value;
                }
            }
        }


        /// <summary>
        /// Age distribution of the group population
        /// </summary>
        [CategoryAttribute("1 - Definition"),
        DescriptionAttribute("Age distribution of the group population."),
        Editor(typeof(DistributionValueEditor), typeof(UITypeEditor))]
        public Stochastic Age
        {
            get { return this._age; }
            set { this._age = value; }
        }


        /// <summary>
        /// Life expectancy of the group population
        /// </summary>
        [CategoryAttribute("1 - Definition"),
        DescriptionAttribute("Life expectancy of the group population."),
        Editor(typeof(DistributionValueEditor), typeof(UITypeEditor))]
        public Stochastic LifeExpectancy
        {
            get { return this._lifeExpect; }
            set { this._lifeExpect = value; }
        }


        /// <summary>
        /// Proportion of female within the group population
        /// </summary>
        [CategoryAttribute("1 - Definition"),
        DefaultValueAttribute(0.50),
        DescriptionAttribute("Proportion of female within the group population.")]
        public double Female
        {
            get 
            { 
                return this._female; 
            }

            set
            {
                if (IsProbability(value))
                {
                    this._female = value;
                    this._male = 1 - this._female;
                }
            }
        }


        /// <summary>
        /// Proportion of males within the group population
        /// </summary>
        [CategoryAttribute("1 - Definition"),
        DefaultValueAttribute(0.50),
        DescriptionAttribute("Proportion of males within the group population.")]
        public double Male
        {
            get 
            { 
                return this._male; 
            }
            
            set
            {
                if (IsProbability(value))
                {
                    this._male = value;
                    this._female = 1 - this._male;
                }
            }
        }


        /// <summary>
        /// Proportion of males homosexual within the group population
        /// </summary>
        [CategoryAttribute("1 - Definition"),
        DefaultValueAttribute(0.0),
        DescriptionAttribute("Proportion of males homosexual within the group population.")]
        public double Homosexual
        {
            get
            {
                return this._gay;
            }

            set
            {
                if (IsProbability(value))
                    this._gay = value;
            }
        }


        /// <summary>
        /// STD prevalence within the group population
        /// </summary>
        [CategoryAttribute("2 - STD"),
        DescriptionAttribute("STD prevalence within the group population.")]
        public double STDPrevalence
        {
            get 
            {
                return this._stdPrev;
            }

            set
            {
                if (IsProbability(value))
                    this._stdPrev = value;
            }
        }


        /// <summary>
        /// Age distribution of STD infection within the group population
        /// </summary>
        [CategoryAttribute("2 - STD"),
        DescriptionAttribute("Age distribution of STD infection within the group population."),
        Editor(typeof(DistributionValueEditor), typeof(UITypeEditor))]
        public Stochastic STDAge
        {
            get { return this._stdAge; }
            set { this._stdAge = value; }
        }


        /// <summary>
        /// Rate of STD testing within the group population
        /// </summary>
        [CategoryAttribute("2 - STD"),
        DescriptionAttribute("Rate of STD testing within the group population.")]
        public double STDTest
        {
            get 
            {
                return this._stdTest; 
            }

            set
            {
                if (IsProbability(value))
                {
                    this._stdTest = value;
                }
            }
        }


        /// <summary>
        /// Probability of be looking for a new partnership 
        /// </summary>
        [CategoryAttribute("3 - Behaviour"),
        DescriptionAttribute("Probability of be looking for a new partnership " +
            "at any unit of time.")]
        public double PrNewPartner
        {
            get 
            { 
                return this._prSniper; 
            }

            set
            {
                if (IsProbability(value))
                {
                    this._prSniper = value;
                }
            }
        }


        /// <summary>
        /// Maximum number of concurrent partnerships
        /// </summary>
        [CategoryAttribute("3 - Behaviour"),
        DefaultValueAttribute(5),
        DescriptionAttribute("Maximum number of concurrent partnerships.")]
        public int MaxConcurrent
        {
            get 
            { 
                return this._maxPartner; 
            }
            
            set
            {
                if (value >= 1)
                {
                    this._maxPartner = value;
                }
            }
        }

        /// <summary>
        /// Probability of concurrent partnership
        /// </summary>
        [CategoryAttribute("3 - Behaviour"),
        DefaultValueAttribute(0.0),
        DescriptionAttribute("Probability of concurrent partnership.")]
        public double PrConcurrent
        {
            get 
            { 
                return this._prConcurrent;
            }

            set
            {
                if (IsProbability(value))
                {
                    this._prConcurrent = value;
                }
            }
        }

        /// <summary>
        /// Probability of a casual relationship 
        /// </summary>
        [CategoryAttribute("3 - Behaviour"),
        DescriptionAttribute("Probability of a casual relationship.")]
        public double PrCasual
        {
            get 
            { 
                return this._prCasual; 
            }

            set
            {
                if (IsProbability(value))
                {
                    this._prCasual = value;
                }
            }
        }


        /// <summary>
        /// Probability of searching for a casual partner first internal
        /// and then external.
        /// </summary>
        [CategoryAttribute("3 - Behaviour"),
        DescriptionAttribute("Probability of searching for a casual partner " +
                             "first internal and then external.")]
        public double PrInternal
        {
            get 
            { 
                return this._prInternal; 
            }

            set
            {
                if (IsProbability(value))
                {
                    this._prInternal = value;
                }
            }
        }


        /// <summary>
        /// Duration of stable partnerships
        /// </summary>
        [CategoryAttribute("4 - Stable"),
        DescriptionAttribute("Duration of stable partnerships."),
        Editor(typeof(DistributionValueEditor), typeof(UITypeEditor))]
        public Stochastic StbDuration
        {
            get { return this._stbDuration; }
            set { this._stbDuration = value; }
        }


        /// <summary>
        /// Time between stable partnerships
        /// </summary>
        [CategoryAttribute("4 - Stable"),
        DescriptionAttribute("Time between stable partnerships."),
        Editor(typeof(DistributionValueEditor), typeof(UITypeEditor))]
        public Stochastic StbTransitory
        {
            get { return this._stbTransit; }
            set { this._stbTransit = value; }
        }


        /// <summary>
        /// Rate of sexual intercourse for stable partnership per unit of time
        /// </summary>
        [CategoryAttribute("4 - Stable"),
        DescriptionAttribute("Rate of sexual intercourse for stable " +
                             "partnership per unit of time."),
        Editor(typeof(DistributionValueEditor), typeof(UITypeEditor))]
        public Stochastic StbContacts
        {
            get { return this._stbSex; }
            set { this._stbSex = value; }
        }


        /// <summary>
        /// Probability of safe sex practice during sexual intercourse for 
        /// stable partnership
        /// </summary>
        [CategoryAttribute("4 - Stable"),
        DescriptionAttribute("Probability of safe sex practice during sexual " +
            "intercourse for stable partnership.")]
        public double StbSafeSex
        {
            get 
            { 
                return this._stbPrSafe; 
            }

            set
            {
                if (IsProbability(value))
                {
                    this._stbPrSafe = value;
                }
            }
        }


        /// <summary>
        /// Duration of casual relationship 
        /// </summary>
        [CategoryAttribute("5 - Casual"),
        DescriptionAttribute("Duration of casual relationship."),
        Editor(typeof(DistributionValueEditor), typeof(UITypeEditor))]
        public Stochastic CslDuration
        {
            get { return this._cslDuration; }
            set { this._cslDuration = value; }
        }


        /// <summary>
        /// Rate of sexual intercourse for a casual relationship per unit of time
        /// </summary>
        [CategoryAttribute("5 - Casual"),
        DescriptionAttribute("Rate of sexual intercourse for a casual " +
                             "relationship per unit of time."),
        Editor(typeof(DistributionValueEditor), typeof(UITypeEditor))]
        public Stochastic CslContacts
        {
            get { return this._cslSex; }
            set { this._cslSex = value; }
        }


        /// <summary>
        /// Probability of safe sex practice during sexual intercourse for 
        /// casual relationship
        /// </summary>
        [CategoryAttribute("5 - Casual"),
        DescriptionAttribute("Probability of safe sex practice during sexual " +
            "intercourse for casual relationship.")]
        public double CslSafeSex
        {
            get 
            { 
                return this._cslPrSafe; 
            }

            set
            {
                if (IsProbability(value))
                {
                    this._cslPrSafe = value;
                }
            }
        }


        /// <summary>
        /// Is the group's population (graph vertices) initialised.
        /// </summary>
        [CategoryAttribute("7 - Control"),
        DescriptionAttribute("Is the group's population (graph vertices) initialised.")]
        public bool Initialised
        {
            get { return this._init; }
        }


        /// <summary>
        ///Individual list of friends function's constant.
        /// </summary>
        [CategoryAttribute("6 - Structure"),
        DefaultValueAttribute(1.0),
        DescriptionAttribute("Individual list of friends function's constant.")]
        public double Alpha
        {
            get { return this._alpha; }
            set { this._alpha = value; }
        }


        /// <summary>
        ///Population searching function's constant.
        /// </summary>
        [CategoryAttribute("6 - Structure"),
        DefaultValueAttribute(1.0),
        DescriptionAttribute("Population searching function's constant.")]
        public double Beta
        {
            get { return this._beta; }
            set { this._beta = value; }
        }


        /// <summary>
        /// Radius of the real world sphere (population spherical structure).
        /// </summary>
        [CategoryAttribute("6 - Structure"),
        DefaultValueAttribute(6378.0),
        DescriptionAttribute("Radius of the real world sphere " +
                             "(population spherical structure).")]
        public double Radius
        {
            get { return this._radius; }
            set { this._radius = value; }
        }


        /// <summary>
        /// Searching distance on the surface of the sphere 
        /// (population spherical topology).
        /// </summary>
        [CategoryAttribute("6 - Structure"),
        DefaultValueAttribute(2500.0),
        DescriptionAttribute("Searching distance on the surface of the " +
                             "sphere (population spherical topology).")]
        public double Distance
        {
            get { return this._distance; }
            set { this._distance = value; }
        }

        /// <summary>
        /// Defines the maximum number of trials to perform when searching 
        /// for a partner within this group.
        /// </summary>
        [CategoryAttribute("6 - Structure"),
        DescriptionAttribute("Defines the maximum number of trials to perform" +
            " when searching for a partner within this group.")]
        public int MaxTrials
        {
            get { return (int)(this._beta * Math.Log(this._size)); }
        }

        /// <summary>
        /// Maximum degree of separation to search for a partner or friend 
        /// in the list of acquaintances.
        /// </summary>
        [CategoryAttribute("6 - Structure"),
        DefaultValueAttribute(3),
        DescriptionAttribute("Maximum degree of separation to search for a " +
            "partner or friend in the list of acquaintances.")]
        public int Degrees
        {
            get
            { 
                return this._degrees; 
            }

            set
            {
                if (value >= 1 && value <= 3)
                {
                    this._degrees = value;
                }
            }
        }

        /// <summary>
        /// Initial value for HIV prevalence
        /// </summary>
        [CategoryAttribute("7 - Control"),
        DefaultValueAttribute(0.0),
        DescriptionAttribute("Initial value for HIV prevalence.")]
        public double InitPrevalence
        {
            get { return this._initPrev; }
            set { this._initPrev = value; }
        }

        /// <summary>
        /// Original value for maximum number of concurrent partnerships.
        /// </summary>
        [CategoryAttribute("7 - Control"),
        DefaultValueAttribute(1),
        DescriptionAttribute("Original value for maximum number of concurrent partnerships.")]
        public int InitMaxConcurrent
        {
            get 
            { 
                return this._initMPartner; 
            }

            set
            {
                if (value > 0)
                {
                    this._initMPartner = value;
                }
            }
        }


        /// <summary>
        /// Original value for probability of concurrent partnerships.
        /// </summary>
        [CategoryAttribute("7 - Control"),
        DefaultValueAttribute(1.0),
        DescriptionAttribute("Original value for probability of concurrent partnerships.")]
        public double InitPrConcurrent
        {
            get 
            { 
                return this._initPrConcur; 
            }

            set
            {
                if (Group.IsProbability(value))
                {
                    this._initPrConcur = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the group's warm-up status during conditional warm-up.
        /// </summary>
        [CategoryAttribute("7 - Control"),
        DefaultValueAttribute(false),
        DescriptionAttribute("Gets or sets the group's warm-up status during conditional warm-up.")]
        public bool WarmedUp
        {
            get { return this._warmed; }
            set { this._warmed = value; }
        }

        #endregion

        #region Graph Properties and Methods
        /// <summary>
        /// Get or set the <see cref="Person"/> properties of a person within the population.
        /// </summary>
        public Person this[int index, string who]
        {
            get
            {
                if (index < 0 || index > this._size || !this._init)
                {
                    throw new ArgumentOutOfRangeException(
                        "index < 0 || index > Group.Size = " + index.ToString(),
                        "Invalid index, value out side the population's " +
                        "boundaries or not initialised. " + who);
                }
                else
                {
                    return this._vertices[index];
                }
            }

            set
            {
                if (index < 0 || index > this._size || !this._init)
                {
                    throw new ArgumentOutOfRangeException(
                        "index < 0 || index > Group.Size = " + index.ToString(),
                        "Invalid index, value out side the population's " +
                        "boundaries or not initialised." + who);
                }
                else
                {
                    this._vertices[index] = value;
                }
            }
        }

        /// <summary>
        /// Creates the group's population
        /// </summary>
        /// <param name="std">Definition of the STD infection definition</param>
        public void CreatePopulation(Disease std)
        {
            CreatePopulation(std, true);
        }

        /// <summary>
        /// Creates the group's population
        /// </summary>
        /// <param name="std">Definition of the STD infection definition</param>
        /// <param name="setstd">Sets the population's STD infection status</param>
        public void CreatePopulation(Disease std, bool setstd)
        {
            if (this._size < 1)
            {
                throw new ArgumentException(
                    "Invalid population size on method create population.",
                    "Group.Size < 1");
            }

            //Clear previous data before create new population
            if (this._init)
            {
                this.ClearPopulation();
            }

            //Resize the number of graph vertices
            this._vertices = new Person[this._size];

            //Defines the properties for each vertex
            for (int i = 0; i < this._vertices.Length; i++)
            {
                this._vertices[i] = this.CreatePerson(std, setstd);
            }

            //Routine completed, sets the initialisation status
            this._init = true;
        }

        /// <summary>
        /// Creates a new person for this core group
        /// </summary>
        /// <param name="std">Definition of the STD infection</param>
        /// <returns>The new person</returns>
        public Person CreatePerson(Disease std)
        {
            return CreatePerson(std, true);
        }

        /// <summary>
        /// Creates a new person for this core group
        /// </summary>
        /// <param name="std">Definition of the STD infection</param>
        /// <param name="setstd">Sets the population's STD infection status</param>
        /// <returns>The new person</returns>
        public Person CreatePerson(Disease std, bool setstd)
        {
            //Create a new person;
            Person prs = new Person(this);

            //Define the properties for each person

            prs.Age = (int)this._rnd.Sample(this._age);
            prs.LifeExpectancy = (int)this._rnd.Sample(this._lifeExpect);

            //Defines gender and sexual preference
            if (this._rnd.Bernoulli(this._female))
            {
                prs.Gender = EGender.Female;
            }
            else
            {
                prs.Gender = EGender.Male;
                prs.Homosexual = this._rnd.Bernoulli(this._gay);
            }

            //Defines STD infection properties
            if (setstd)
            {
                if (this._rnd.Bernoulli(this._stdPrev))
                {
                    prs.STDStatus = ESTDStatus.Infected;
                    prs.STDAge = (int)this._rnd.Sample(this._stdAge);

                    //Checks if STD can cause death
                    if (this._rnd.Bernoulli(std.Mortality))
                    {
                        prs.STDDeath = (int)this._rnd.Sample(std.LifeExpectancy);
                        if (prs.STDAge >= prs.STDDeath)
                        {
                            prs.STDAge = 0;
                        }
                    }//STD deaths

                    //Sets the duration of infection for non lifelong STDs.
                    if (!std.LifeInfection)
                    {
                        prs.STDDuration = (int)this._rnd.Sample(std.STDDuration);
                        if (prs.STDAge >= prs.STDDuration)
                        {
                            prs.STDDuration += prs.STDAge;
                        }
                    }//STD duration
                }//STD infection
            }//Set STD status

            prs.STDTest = this._rnd.Bernoulli(this._stdTest);

            //Define the geographical location
            if (prs.MyGroup.Topology == ETopology.Sphere)
            {
                prs.Location = this._rnd.Sphere3d();
            }

            return prs;
        }

        /// <summary>
        /// Clear the group's population
        /// </summary>
        /// <remarks>
        /// You should call GC.Collect() after you've cleared one or more
        /// group's data to avoid wasting computer memory.
        /// </remarks>
        public void ClearPopulation()
        {
            if (this._init)
            {
                for (int i = 0; i < this._vertices.Length; i++)
                {
                    this._vertices[i].SayGoodBye();
                }

                this._vertices = null;

                //Resets the initialisation status
                this._init = false;
            }
        }

        #endregion

        #region Graph Characteristics

        /// <summary>
        /// Calculates the small-world network characteristic path length using
        /// Floyd-Warshall's all-pair shortest path algorithm in a NxN matrix.
        /// </summary>
        /// <returns>The network's characteristic path length (median).</returns>
        public double SWNPLFloydFull()
        {
            int count, row, col, k, inf;
            double[] dmean;
            int[,] adjmat;
            Person prs, target;

            //setup
            this._visit = int.MaxValue;
            count = this._size;
            dmean = new double[count];
            adjmat = new int[count, count];
            inf = count;

            //Create the adjacency matrix
            for (row = 0; row < count; row++)
            {
                prs = this._vertices[row];
                for (col = row + 1; col < count; col++)
                {
                    target = this._vertices[col];
                    if (prs.Friends.Contains(target))
                    {
                        adjmat[row, col] = 1;
                        adjmat[col, row] = 1;
                    }
                    else
                    {
                        adjmat[row, col] = inf;
                        adjmat[col, row] = inf;
                    }
                }
            }

            //Floyd's all-pairs algorithm
            for (k = 0; k < count; k++)
            {
                for (row = 0; row < count; row++)
                {
                    if (adjmat[row, k] < inf)
                    {
                        for (col = 0; col < count; col++)
                        {
                            if (adjmat[k, col] < inf)
                            {
                                adjmat[row, col] = Math.Min(adjmat[row, col], adjmat[row, k] + adjmat[k, col]);
                            }
                        }
                    }
                }
            }

            //Calculate the mean shortest path
            for (row = 0; row < count; row++)
            {
                for (col = 0; col < count; col++)
                {
                    dmean[row] += adjmat[row, col];
                }
                dmean[row] = dmean[row] / count;
            }

            //Calculate the median
            DataFunction.QSort(dmean, 0, dmean.Length - 1, this._rnd);
            if (count % 2 != 0) //Count is Odd
            {
                return dmean[(count - 1) / 2];
            }
            else
            {
                count = count / 2;
                return (dmean[count - 1] + dmean[count]) / 2.0;
            }
        }

        /// <summary>
        /// Calculates the small-world network characteristic path length using
        /// Floyd-Warshall's all-pair shortest path algorithm in a triangular matrix.
        /// </summary>
        /// <remarks>
        /// Small-World original algorithm, valid only for a full connected network.
        /// </remarks>
        /// <returns>The network's characteristic path length (median).</returns>
        public double SWNPLFloyd()
        {
            int count, row, col, k, inf;
            double[] dmean;
            TriangArray adjmat;
            Person prs, target;

            //setup
            this._visit = int.MaxValue;
            count = this._size;
            dmean = new double[count];
            adjmat = new TriangArray(count);
            inf = count;

            //Create the adjacency matrix
            for (row = 0; row < count; row++)
            {
                prs = this._vertices[row];
                for (col = row + 1; col < count; col++)
                {
                    target = this._vertices[col];
                    if (prs.Friends.Contains(target))
                    {
                        adjmat[row, col] = 1;
                    }
                    else
                    {
                        adjmat[row, col] = inf;
                    }
                }
            }

            //Floyd's all-pairs algorithm
            for (k = 0; k < count; k++)
            {
                for (row = 0; row < count; row++)
                {
                    if (adjmat[row, k] < inf)
                    {
                        for (col = 0; col < count; col++)
                        {
                            if (adjmat[k, col] < inf)
                            {
                                adjmat[row, col] = Math.Min(adjmat[row, col], adjmat[row, k] + adjmat[k, col]);
                            }
                        }
                    }
                }
            }

            //Calculate the mean shortest path
            for (row = 0; row < count; row++)
            {
                for (col = 0; col < count; col++)
                {
                    dmean[row] += adjmat[row, col];
                }
                dmean[row] = dmean[row] / count;
            }

            //Calculate the median
            DataFunction.QSort(dmean, 0, dmean.Length - 1, this._rnd);
            if (count % 2 != 0) //Count is Odd
            {
                return dmean[(count - 1) / 2];
            }
            else
            {
                count = count / 2;
                return (dmean[count - 1] + dmean[count]) / 2.0;
            }
        }

        /// <summary>
        /// Calculates the small-world network characteristic path length using
        /// Breadth-First Search (BFS) algorithm.
        /// </summary>
        /// <remarks>
        /// Small-World original algorithm, valid only for a full connected network.
        /// </remarks>
        /// <returns>The network characteristic path length (median).</returns>
        public double SWNPLBfs()
        {
            int count, i, j, inf;
            double[] dmean;
            Person prs, target;

            //setup
            this._visit = int.MaxValue;
            count = this._size;
            dmean = new double[count];
            inf = count;

            //Reset visited status
            for (i = 0; i < count; i++)
            {
                this._vertices[i].Visited = 0;
            }

            //Start main loop
            for (i = 0; i < count; i++)
            {
                prs = this._vertices[i];
                for (j = i + 1; j < count; j++)
                {
                    target = this._vertices[j];
                    target.Distance = inf;

                    //Update distance
                    BFS(prs, target);
                    dmean[i] += target.Distance;
                    dmean[j] += target.Distance;
                }
                dmean[i] = dmean[i] / count;
            }

            //Calculate the median
            DataFunction.QSort(dmean, 0, dmean.Length - 1, this._rnd);
            if (count % 2 != 0) //Count is Odd
            {
                return dmean[(count - 1) / 2];
            }
            else
            {
                count = count / 2;
                return (dmean[count - 1] + dmean[count]) / 2.0;
            }
        }

        /// <summary>
        /// Breadth-First Search algorithm to find the shortest path between 
        /// a pairs of vertices in the network.
        /// </summary>
        /// <param name="s">The source person</param>
        /// <param name="v">The target person</param>
        private void BFS(Person s, Person v)
        {
            Person prs;
            BookList adj;

            PQueue q = new PQueue();
            this._visit = (this._visit - 1 % int.MaxValue);

            s.Distance = 0;
            s.Visited = this._visit;
            q.Enqueue(s);

            while (!q.IsEmpty())
            {
                prs = q.Dequeue();
                adj = prs.Friends;
                adj.Reset();
                while (adj.MoveNext())
                {
                    if (adj.Current.Visited != this._visit)
                    {
                        adj.Current.Visited = this._visit;
                        adj.Current.Distance = prs.Distance + 1;
                        q.Enqueue(adj.Current);
                        if (v.Equals(adj.Current))
                        {
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the small-world network clustering coefficient.
        /// </summary>
        /// <returns>The network's clustering coefficient</returns>
        public double SWNClustering()
        {
            int p, i, j, count, edges;
            double smean;
            Person prs;
            Person[] ngb;

            smean = 0.0;
            for (p = 0; p < this._size; p++)
            {
                prs = this._vertices[p];
                count = prs.Friends.Count;
                if (count > 1)
                {
                    //Get the list of neighbours
                    ngb = prs.Friends.ToArray();

                    //Calculate the clustering
                    edges = 0;
                    for (i = 0; i < count; i++)
                    {
                        for (j = i + 1; j < count; j++)
                        {
                            if (ngb[i].Friends.Contains(ngb[j]))
                            {
                                edges++;
                            }
                        }
                    }
                    smean += edges / (count * (count - 1.0) / 2.0);
                }
            }//For person

            //Calculate the group's average clustering
            return smean / this._size;

        }

        /// <summary>
        /// Calculates the small-world network characteristics
        /// </summary>
        /// <param name="algo">The network efficiency algorithm switch</param>
        /// <returns>The set of network properties</returns>
        public SWNInfo SWNProperties(double algo)
        {
            return SWNProperties(algo, true, 0);
        }

        /// <summary>
        /// Calculates the small-world network characteristics
        /// </summary>
        /// <param name="algo">The network efficiency algorithm switch</param>
        /// <param name="exact">
        /// If <c>true</c> the exact path length will be calculated, 
        /// otherwise an approximation will be used.
        /// </param>
        /// <param name="ssize">
        /// The size of the sample to estimate the true path length
        /// </param>
        /// <returns>The set of network properties</returns>
        public SWNInfo SWNProperties(double algo, bool exact, int ssize)
        {
            #region Local variables and setup
            int count, row, col, k, maxn, inf, diameter, idx;
            double sumd, sumdi, sdegree, edges;
            double smean, smeand, smeandi;
            double[] xdata;
            TriangArray adjmat;
            RPairArray sample;
            RPair pair;
            Person prs, target;
            Person[] ngb;
            SWNInfo info;

            //Setup
            this._visit = int.MaxValue;
            count = this._size;
            maxn = count * (count - 1) / 2;	//Exact value
            xdata = new double[maxn];
            inf = this._size;	            //Infinity
            sumd = 0.0;
            sumdi = 0.0;
            sdegree = 0.0;
            diameter = 0;
            info = new SWNInfo();
            info.Connected = true;
            info.Vertices = count;
            #endregion

            //Shortest path calculation
            if (exact) //Calculate a exact value for path length
            {
                idx = 0;
                if (this.PrCasual >= algo)//Use BFS
                {
                    #region BFS shortest distance
                    //Reset visited status and sum the degree
                    for (row = 0; row < count; row++)
                    {
                        this._vertices[row].Visited = 0;
                        sdegree += this._vertices[row].Friends.Count;
                    }

                    //Start main loop
                    for (row = 0; row < count; row++)
                    {
                        prs = this._vertices[row];
                        for (col = row + 1; col < count; col++)
                        {
                            target = this._vertices[col];
                            target.Distance = inf;

                            //Find the shortest path
                            BFS(prs, target);

                            //Checks for full connectivity
                            if (target.Distance < inf)
                            {
                                sumd += target.Distance;
                                sumdi += (1.0 / target.Distance);
                                xdata[idx] = (1.0 / target.Distance);
                            }
                            else
                            {
                                info.Connected = false;
                            }

                            //Updates the diameter
                            if (target.Distance > diameter)
                            {
                                diameter = target.Distance;
                            }

                            idx++;
                        }//For col
                    }//For row
                    #endregion //BFS algorithm
                }
                else //Use Floyd-Warshall algorithm
                {
                    #region Floyd-Warshall all-pair shortest path
                    adjmat = new TriangArray(count);

                    //Create the adjacency matrix
                    for (row = 0; row < count; row++)
                    {
                        //Gets the next person and sum the degree
                        prs = this._vertices[row];
                        sdegree += prs.Friends.Count;
                        for (col = row + 1; col < count; col++)
                        {
                            target = this._vertices[col];
                            if (prs.Friends.Contains(target))
                            {
                                adjmat[row, col] = 1;
                            }
                            else
                            {
                                adjmat[row, col] = inf;
                            }
                        }//For col
                    }//For row

                    //Floyd's all-pairs algorithm
                    for (k = 0; k < count; k++)
                    {
                        for (row = 0; row < count; row++)
                        {
                            if (adjmat[row, k] < inf)
                            {
                                for (col = 0; col < count; col++)
                                {
                                    if (adjmat[k, col] < inf)
                                    {
                                        adjmat[row, col] = Math.Min(adjmat[row, col], adjmat[row, k] + adjmat[k, col]);
                                    }
                                }//For col
                            }//If infinity
                        }//For row
                    }//For k

                    //Sum the shortest path, multiply by 2 because we're 
                    //using only half of the adj. matrix
                    for (row = 0; row < count; row++)
                    {
                        for (col = row + 1; col < count; col++)
                        {
                            //Checks for full connectivity
                            if (adjmat[row, col] < inf)
                            {
                                sumd += adjmat[row, col];
                                sumdi += (1.0 / adjmat[row, col]);
                                xdata[idx] = (1.0 / adjmat[row, col]);
                            }
                            else
                            {
                                info.Connected = false;
                            }

                            //Updates the diameter
                            if (adjmat[row, col] >= diameter)
                            {
                                diameter = adjmat[row, col];
                            }

                            idx++;
                        }//For col
                    }//For row

                    #endregion //Floyd-Warshall
                }//End path length
            }
            else //Estimate the path length with BFS
            {
                #region Estimate path length

                //Validates the sample size and creates the sampling array
                sample = new RPairArray(count, ssize, this._rnd);

                //Reset visited status and sum the degree
                for (row = 0; row < count; row++)
                {
                    this._vertices[row].Visited = 0;
                    sdegree += this._vertices[row].Friends.Count;
                }

                //Starts sample main loop
                for (row = 0; row < ssize; row++)
                {
                    //Gets two random individuals
                    pair = sample.Next();
                    prs = this._vertices[pair.Source];
                    target = this._vertices[pair.Target];
                    target.Distance = inf;

                    //Finds the shortest path between prs and target
                    BFS(prs, target);
                    if (target.Distance < inf)
                    {
                        sumd += target.Distance;
                        sumdi += 1.0 / target.Distance;
                    }
                    else
                    {
                        info.Connected = false;
                    }

                    //Updates the diameter
                    if (target.Distance > diameter)
                    {
                        diameter = target.Distance;
                    }
                }//For sample row

                #endregion //Samples shortest path
            }//If exact path length

            #region Global properties

            //Calculate the path length
            if (info.Connected)
            {
                if (exact) //Checks for exact calculation
                {
                    info.PathLength = (1.0 / maxn) * sumd;
                }
                else
                {
                    info.PathLength = (1.0 / ssize) * sumd;
                }//If exact

                info.Diameter = diameter;
            }
            else
            {
                info.PathLength = double.PositiveInfinity;
                info.Diameter = double.PositiveInfinity;
            }//If connected

            if (exact) //Calculate the exact values
            {
                info.GConnectivity = maxn / sumdi;
                info.GEfficiency = (1.0 / maxn) * sumdi;

                //Calculate the variance and standard error
                info.GVEfficiency = DataFunction.Variance(xdata, false);
                info.GSEfficiency = Math.Sqrt(info.GVEfficiency) / Math.Sqrt(xdata.Length);
            }
            else	//Estimate the values
            {
                info.GConnectivity = ssize / sumdi;
                info.GEfficiency = (1.0 / ssize) * sumdi;
            }//If exact

            //Average degree of the network
            info.Degree = sdegree / count;

            //Cost for an unweighted and undirected network (K/(N(N-1)/2)
            edges = sdegree / 2.0;
            info.Cost = edges / maxn;
            info.Edges = edges;

            //Expected values for regular and random networks
            info.LRegular = this._size / (2 * edges);
            info.CRegular = (3.0 * (edges - 2.0)) / (4.0 * (edges - 1.0));
            info.LRandom = Math.Log(this.Size) / Math.Log(edges);
            info.CRandom = edges / maxn;

            #endregion

            #region Local properties
            smean = 0.0;
            smeand = 0.0;
            smeandi = 0.0;
            for (k = 0; k < this._size; k++)
            {
                prs = this._vertices[k];
                count = prs.Friends.Count;
                if (count > 1)
                {
                    //Get the list of neighbours
                    ngb = prs.Friends.ToArray();

                    //Calculate the clustering
                    sumdi = 0.0;
                    for (row = 0; row < count; row++)
                    {
                        for (col = row + 1; col < count; col++)
                        {
                            if (ngb[row].Friends.Contains(ngb[col]))
                            {
                                sumdi++;
                            }
                        }//For col
                    }//For row

                    count = count * (count - 1) / 2; // Maximum # of edges
                    smean += sumdi / count;
                    if (sumdi > 0.0)
                    {
                        smeand += count / sumdi;
                    }

                    smeandi += (1.0 / count) * sumdi;
                }//If count > 1
            }//Main for k

            info.Clustering = smean / this._size;
            info.LConnectivity = smeand / this._size;
            info.LEfficiency = smeandi / this._size;
            #endregion //Local properties

            return info;
        }//End of NetworkProperties


        /// <summary>
        /// Takes a sample of the network shortest paths (characteristic path length)
        /// using Breadth-First Search (BFS) algorithm.
        /// </summary>
        /// <param name="data">The matrix to save the sample data</param>
        /// <param name="colidx">The matrix column to save the data</param>
        /// <param name="size">The size of the sample</param>
        public void SWNPLSample(ref double[,] data, int colidx, int size)
        {
            int count, inf, row;
            RPairArray sample;
            RPair pair;
            Person prs, target;

            //setup
            this._visit = int.MaxValue;
            count = this._size;
            inf = count;

            //Validate the sample size and creates array
            sample = new RPairArray(count, size, this._rnd);

            //Reset visited status
            for (row = 0; row < count; row++)
            {
                this._vertices[row].Visited = 0;
            }

            //Main row loop
            for (row = 0; row < size; row++)
            {
                //Gets two random individuals
                pair = sample.Next();
                prs = this._vertices[pair.Source];
                target = this._vertices[pair.Target];
                target.Distance = inf;

                //Finds the shortest path between prs and target
                BFS(prs, target);
                if (target.Distance < inf)
                {
                    data[row, colidx] = 1.0 / target.Distance;
                }
            }
        }//End NetSPSample


        /// <summary>
        /// Writes the network adjacency matrix representation to a comma 
        /// separated values (csv) file. 
        /// </summary>
        /// <param name="sw">
        /// The output <see cref="StreamWriter"/> file.
        /// </param>
        public void ToAdjMatrix(StreamWriter sw)
        {
            //Local variables
            int count, row, col;
            TriangArray adjmat;
            Person prs, target;

            //Setup
            count = this._size;
            adjmat = new TriangArray(count);

            //Create the adjacency matrix
            for (row = 0; row < count; row++)
            {
                //Gets the next person and sum the degree
                prs = this._vertices[row];
                for (col = row + 1; col < count; col++)
                {
                    target = this._vertices[col];
                    if (prs.Friends.Contains(target))
                    {
                        adjmat[row, col] = 1;
                    }
                    else
                    {
                        adjmat[row, col] = 0;
                    }
                }//For col
            }//For row

            //Writes to file
            adjmat.ToCSV(sw);

        }//And of ToAdjMatrix

        #endregion

        #region Static Method
        /// <summary>
        /// Verify if a value can be used as a probability parameter
        /// </summary>
        /// <param name="prob">The value to be verified</param>
        /// <returns>True for success or False for failure</returns>
        public static bool IsProbability(double prob)
        {
            if (prob >= 0.0 && prob <= 1.0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Resets the group unique id counter to 1.
        /// </summary>
        public static void Reset()
        {
            Group.IdCounter = 0;
        }

        #endregion

    }
}
