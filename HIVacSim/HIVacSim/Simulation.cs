// ----------------------------------------------------------------------------
// <copyright file="Simulation.cs" company="HIVacSim">
//   Copyright (c) 2014 HIVacSim Contributors
// </copyright>
// <author>Israel Vieira</author>
// ----------------------------------------------------------------------------

namespace HIVacSim
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using HIVacSim.Utility;
    using System.Runtime.CompilerServices;

    #region Simulation Event Handlers

    /// <summary>
    /// Defines the simulation event argument
    /// </summary>
    public class SimEventArgs : EventArgs
    {
        private string _msg;

        /// <summary>
        /// Simulation event argument
        /// </summary>
        /// <param name="msg">Argument message</param>
        public SimEventArgs(string msg)
        {
            this._msg = msg;
        }

        /// <summary>
        /// Simulation event argument
        /// </summary>
        public string Mensage
        {
            get { return this._msg; }
        }
    }
    #endregion

    /// <summary>
    /// HIVacSim run-time exception message class.
    /// </summary>
    public class SimulationException : System.ApplicationException
    {
        /// <summary>
        /// Simulation application exception
        /// </summary>
        /// <param name="msg"></param>
        public SimulationException(string msg) : base(msg) { }
    }
    
    /// <summary>
    /// Implements the simulation executive, the HIVacSim model kernel.
    /// </summary>
    [Serializable]
    public class Simulation : Scenario
    {
        #region Local variables
        // Simulation runtime variables
        private ESimStatus _status;	// Simulation status
        private int _run;		    // Current simulation run
        private int _clock;		    // Current simulation clock (time)
        private bool _waiting;	    // Wait the user to continue - pause | step
        private bool _warmup;	    // Identify if the simulation is warming up
        private bool _stdzero;	    // STD prevalence zero condition
        private Stopwatch _timer;	// Simulation internal clock

        // Simulation data
        private SimData[, ,] _data;     // Results data [Runs,Duration,Groups]
        private SWNInfo[,] _netdata;	// Network characteristics data

        // Control
        private int _idxVisit;	// Index of the current visit to a person

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Simulation()
            : base()
        {
            this._status = ESimStatus.Ready;
            this._run = 0;
            this._clock = 0;
            this._waiting = false;
            this._warmup = false;
            this._timer = new Stopwatch();
        }

        #endregion

        #region Delegates, Events and Handlers

        /// <summary>
        /// This event will be fired when the simulation has 
        /// started running a trial.
        /// </summary>
        public event EventHandler OnStartTrial;

        /// <summary>
        /// This event will be fired when the simulation starts a new run.
        /// </summary>
        public event EventHandler OnStartRun;

        /// <summary>
        /// This event will be fired when the simulation 
        /// start the warm-up period.
        /// </summary>
        public event EventHandler OnStartWarmUp;

        /// <summary>
        /// This event will be fired when the end of warm-up time is completed.
        /// </summary>
        public event EventHandler OnEndWarmUp;

        /// <summary>
        /// This event will be fired when the simulation run 
        /// starts (at any clock time).
        /// </summary>
        public event EventHandler OnStartClock;

        /// <summary>
        /// This event will be fired if the simulation animation is on to
        /// request the UI to animate.
        /// </summary>
        public event EventHandler OnAnimate;

        /// <summary>
        /// This event will be fired when the simulation clock 
        /// stops (at any clock time).
        /// </summary>
        public event EventHandler OnEndClock;

        /// <summary>
        /// This event will be fired when the simulation is stopped 
        /// by a pause or step request.
        /// </summary>
        public event EventHandler OnStopRun;

        /// <summary>
        /// This event will be fired when the simulation continues a 
        /// run after a pause or step request.
        /// </summary>
        public event EventHandler OnContinueRun;

        /// <summary>
        /// This event will be fired before the simulation reset is obeyed.
        /// </summary>
        public event EventHandler OnBeforeReset;

        /// <summary>
        /// This event will be fired when the simulation reaches 
        /// the end of the run.
        /// (Results collection period).
        /// </summary>
        public event EventHandler<SimEventArgs> OnEndRun;

        /// <summary>
        /// This event will be fired upon completion of a 
        /// simulation reset request.
        /// </summary>
        public event EventHandler OnReset;

        /// <summary>
        /// This event will be fired when the simulation has finished 
        /// running all the runs in a trial.
        /// </summary>
        public event EventHandler OnEndTrial;

        /// <summary>
        /// This event will be fired when an error has occurred 
        /// within the simulation.
        /// </summary>
        public event EventHandler<SimEventArgs> OnSimError;

        #endregion

        #region Public Properties
        /// <summary>
        /// Get the current simulation status
        /// </summary>
        [CategoryAttribute("Simulation"),
        DefaultValueAttribute(ESimStatus.Ready),
        BrowsableAttribute(false),
        DescriptionAttribute("Get the current simulation status.")]
        public ESimStatus Status
        {
            get
            {
                lock (this)
                {
                    return this._status;
                }
            }
        }
        
        /// <summary>
        /// Get the simulation warm up status
        /// </summary>
        [CategoryAttribute("Simulation"),
        DefaultValueAttribute(false),
        BrowsableAttribute(false),
        DescriptionAttribute("Get the simulation warm up status.")]
        public bool WarmingUp
        {
            get
            {
                lock (this)
                {
                    return this._warmup;
                }
            }
        }
        
        /// <summary>
        /// Gets the current simulation run
        /// </summary>
        [CategoryAttribute("Simulation"),
        DefaultValueAttribute(0),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the current simulation run")]
        public int Run
        {
            get
            {
                lock (this)
                {
                    if (this._status != ESimStatus.Ready)
                    {
                        return this._run + 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the current simulation clock
        /// </summary>
        [CategoryAttribute("Simulation"),
        DefaultValueAttribute(0),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the current simulation clock")]
        public int Clock
        {
            get
            {
                lock (this)
                {
                    if (this._status != ESimStatus.Ready)
                    {
                        return this._clock;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the current simulation clock as date
        /// </summary>
        [CategoryAttribute("Simulation"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the current simulation clock as date")]
        public DateTime ClockToDate
        {
            get
            {
                switch (this._simClock)
                {
                    case ESimClock.Month:
                        {
                            return this._simDate.AddMonths(this._clock).Date;
                        }

                    case ESimClock.Trimester:
                        {
                            return this._simDate.AddMonths(3 * this._clock).Date;
                        }

                    case ESimClock.Semester:
                        {
                            return this._simDate.AddMonths(6 * this._clock).Date;
                        }

                    case ESimClock.Year:
                        {
                            return this._simDate.AddYears(this._clock).Date;
                        }
                    default:
                        {
                            return this._simDate;
                        }
                }
            }
        }
        
        /// <summary>
        /// Gets the current simulation clock as a formatted string
        /// </summary>
        [CategoryAttribute("Simulation"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the current simulation clock as a formatted string")]
        public string ClockToString
        {
            get { return this.ClockToDate.ToString("dddd, dd MMMM yyyy"); }
        }

        /// <summary>
        /// Gets the simulation run-time in seconds
        /// </summary>
        [CategoryAttribute("Simulation"),
        BrowsableAttribute(false),
        DescriptionAttribute("Gets the simulation run-time in seconds")]
        public double Time
        {
            get { return this._timer.Elapsed.TotalSeconds; }
        }

        #endregion //Public properties

        #region Public Methods
        /// <summary>
        /// Launch the simulation worker thread. This method will return 
        /// immediately, the model starts asynchronously on a worker thread.
        /// </summary>
        public void SimRun()
        {
            lock (this)
            {
                if (this._status == ESimStatus.Ready ||
                    this._status == ESimStatus.Completed)
                {
                    this._status = ESimStatus.Running;
                    new Thread(new ThreadStart(SimStart)).Start();
                }
                else if (this._status == ESimStatus.Stepping ||
                    this._status == ESimStatus.Paused)
                {
                    this._status = ESimStatus.Running;
                    this._waiting = false;
                    Monitor.Pulse(this);
                }
                else
                {
                    this.OnSimError(this, new SimEventArgs(
                                    "This simulation is already running..."));
                }
            }
        }

        /// <summary>
        /// Start or step the simulation
        /// </summary>
        public void SimStep()
        {
            lock (this)
            {
                if (this._status == ESimStatus.Ready || 
                    this._status == ESimStatus.Completed)
                {
                    this._waiting = true;
                    this._status = ESimStatus.Stepping;
                    new Thread(new ThreadStart(SimStart)).Start();
                }
                else if (this._status == ESimStatus.Stepping)
                {
                    this._waiting = false;
                    Monitor.Pulse(this);
                }
                else if (this._status == ESimStatus.Paused)
                {
                    this._status = ESimStatus.Stepping;
                    this._waiting = false;
                    Monitor.Pulse(this);
                }
                else
                {
                    this.OnSimError(this, new SimEventArgs(
                                "Simulation is already ready running..."));
                }
            }
        }

        /// <summary>
        /// Pause current continuous simulation
        /// </summary>
        public void SimPause()
        {
            lock (this)
            {
                if (this._status == ESimStatus.Running)
                {
                    this._waiting = true;
                    this._status = ESimStatus.Paused;
                    Monitor.Pulse(this);
                }
                else if (this._status == ESimStatus.Paused)
                {
                    this._waiting = false;
                    this._status = ESimStatus.Running;
                    Monitor.Pulse(this);
                }
                else
                {
                    this.OnSimError(this, new SimEventArgs(
                        "The simulation is already stepping or not running..."));
                }
            }
        }

        /// <summary>
        /// Reset / Cancel simulation
        /// </summary>
        public void SimReset()
        {
            lock (this)
            {
                if (this._status == ESimStatus.Completed)
                {
                    this.ClearData(true);
                    this._status = ESimStatus.Ready;
                }
                else if (this._status == ESimStatus.Resetting)
                {
                    this.OnSimError(this, new SimEventArgs(
                        "The simulation is already resetting..."));
                }
                else if (this._status != ESimStatus.Ready)
                {
                    this._status = ESimStatus.Resetting;
                    this._waiting = false;
                    Monitor.Pulse(this);
                }
                else
                {
                    this.OnSimError(this, new SimEventArgs(
                                          "The simulation is not running."));
                }
            }
        }

        /// <summary>
        /// Simulation results data per run, clock and group
        /// </summary>
        /// <param name="run">The simulation run.</param>
        /// <param name="clock">The simulation clock.</param>
        /// <param name="group">The population group.</param>
        /// <returns>The simulation clock result.</returns>
        public SimData SimResult(int run, int clock, int group)
        {
            try
            {
                return this._data[run, clock, group];
            }
            catch (Exception err)
            {
                throw new SimulationException(err.Message);
            }
        }

        /// <summary>
        /// Small-World Network characteristics per trial for each group
        /// </summary>
        /// <param name="run">The simulation run</param>
        /// <param name="group">The population group</param>
        /// <returns>The network characteristics</returns>
        public SWNInfo Network(int run, int group)
        {
            try
            {
                return this._netdata[run, group];
            }
            catch (Exception err)
            {
                throw new SimulationException(err.Message);
            }
        }
        
        #endregion

        #region File I/O Methods

        /// <summary>
        /// Save the network characteristics as comma separated values (*.csv)
        /// </summary>
        /// <remarks>
        /// One should use Try...Catch when calling this procedure
        /// </remarks>
        /// <param name="fileName">
        /// The file path and name to export the results
        /// </param>
        /// <returns>True for success and False for failure.</returns>
        public bool ExportNetToCSV(string fileName)
        {
            int r, g, gCount;
            StreamWriter csvFile = File.CreateText(fileName);
            csvFile.WriteLine("Run,Group," + SWNInfo.ToCSVTitle);

            //Gets the number of simulation groups
            gCount = this._pop.Count;
            for (r = 0; r < this._simRuns; r++)
            {
                for (g = 0; g < gCount; g++)
                {
                    csvFile.Write(r + 1 + ",");	//Run
                    csvFile.Write(g + ",");		//Group
                    csvFile.WriteLine(this._netdata[r, g].ToCSV(false));
                }
            }
            csvFile.Close();
            return true;
        }//ExportNetToCSV

        /// <summary>
        /// Save the simulation results as comma separated values (*.csv)
        /// </summary>
        /// <remarks>
        /// One should use Try...Catch when calling this procedure
        /// </remarks>
        /// <param name="fileName">
        /// The file path and name to export the results
        /// </param>
        /// <returns>True for success and False for failure.</returns>
        public bool ExportToCSV(string fileName)
        {
            int r, c, g, gCount;
            StreamWriter csvFile = File.CreateText(fileName);

            csvFile.WriteLine("Run,Clock,Date,Group Id,Group Name,Topology,Size,Female,Male,Homosexual," +
                    "Friends,From Friends,Partners,Stable,From Acquaintances,Casual,Internal,External,Concurrent," +
                    "STDPrevalence,STDIncidence,STDFemale,STDMale,STDHomosexual,STDMale2Female,STDFemale2Male," +
                    "STDMale2Male,STDStable,STDCasual,STDInternal,STDExternal,STDRecovered,STDProtected,Deaths,STDDeaths, UserData");

            //Gets the number of simulation groups
            gCount = this._pop.Count;

            for (r = 0; r < this._simRuns; r++)
            {
                for (c = 0; c <= this._simDuration; c++)
                {
                    for (g = 0; g < gCount; g++)
                    {
                        csvFile.Write(r + 1 + ",");						//Run
                        csvFile.Write(c + ",");							//Clock
                        csvFile.Write(this._data[r, c, g].Date.ToShortDateString() + ",");

                        csvFile.Write(this._pop[g].Id.ToString() + ",");	//Group id
                        csvFile.Write(this._pop[g].Name + ",");	//Group name
                        csvFile.Write(this._pop[g].Topology.ToString() + ",");
                        csvFile.Write(this._data[r, c, g].Size + ",");
                        csvFile.Write(this._data[r, c, g].Female + ",");
                        csvFile.Write(this._data[r, c, g].Male + ",");
                        csvFile.Write(this._data[r, c, g].Homosexual + ",");
                        csvFile.Write(this._data[r, c, g].Friends + ",");
                        csvFile.Write(this._data[r, c, g].FromFriends + ",");
                        csvFile.Write(this._data[r, c, g].Partners + ",");
                        csvFile.Write(this._data[r, c, g].Stable + ",");
                        csvFile.Write(this._data[r, c, g].StbFromList + ",");
                        csvFile.Write(this._data[r, c, g].Casual + ",");
                        csvFile.Write(this._data[r, c, g].Internal + ",");
                        csvFile.Write(this._data[r, c, g].External + ",");
                        csvFile.Write(this._data[r, c, g].Concurrent + ",");
                        csvFile.Write(this._data[r, c, g].STDPrevalence + ",");
                        csvFile.Write(this._data[r, c, g].STDIncidence + ",");
                        csvFile.Write(this._data[r, c, g].STDFemale + ",");
                        csvFile.Write(this._data[r, c, g].STDMale + ",");
                        csvFile.Write(this._data[r, c, g].STDHomosexual + ",");
                        csvFile.Write(this._data[r, c, g].STDMale2Female + ",");
                        csvFile.Write(this._data[r, c, g].STDFemale2Male + ",");
                        csvFile.Write(this._data[r, c, g].STDMale2Male + ",");
                        csvFile.Write(this._data[r, c, g].STDStable + ",");
                        csvFile.Write(this._data[r, c, g].STDCasual + ",");
                        csvFile.Write(this._data[r, c, g].STDInternal + ",");
                        csvFile.Write(this._data[r, c, g].STDExternal + ",");
                        csvFile.Write(this._data[r, c, g].STDRecovered + ",");
                        csvFile.Write(this._data[r, c, g].STDProtected + ",");
                        csvFile.Write(this._data[r, c, g].Deaths + ",");
                        csvFile.Write(this._data[r, c, g].STDDeaths + ",");
                        csvFile.WriteLine(this._data[r, c, g].UserData);
                    }
                }
            }
            csvFile.Close();
            return true;
        }//Export As CSV

        /// <summary>
        /// Exports the simulation population to be used by the
        /// matlab create movie script
        /// </summary>
        /// <remarks>
        /// This code should be called from inside and at the end of the 
        /// simulation clock loop. The simulation should be configured to run 
        /// ONLY for a single replication as this function will output a file 
        /// for each simulation clock. 
        /// </remarks>
        /// <param name="filename">
        /// The full name of the output file <c>WITHOUT</c> extension
        /// </param>
        public void ExportToMatlabMovie(string filename)
        {
            int g, gCount, p, pCount;
            string fname;
            Group grp;
            Person prs;
            StreamWriter sw;

            gCount = this._pop.Count;
            for (g = 0; g < gCount; g++)
            {
                grp = this._pop[g];
                fname = filename + "Movie-" + grp.Topology.ToString() + grp.Id.ToString() +
                        "-" + this._run.ToString() + "-" + this._clock.ToString() + ".csv";

                //Creates the matlab file
                pCount = grp.Size;
                sw = new StreamWriter(fname);
                if (grp.Topology == ETopology.Circle)
                {
                    for (p = 0; p < pCount; p++)
                    {
                        if (grp[p, "STD status"].STDStatus == ESTDStatus.Infected)
                        {
                            sw.WriteLine("1");
                        }
                        else
                        {
                            sw.WriteLine("0");
                        }
                    }//Person's loop
                }
                else if (grp.Topology == ETopology.Sphere)
                {
                    for (p = 0; p < pCount; p++)
                    {
                        prs = grp[p, "STD status"];
                        if (prs.STDStatus == ESTDStatus.Infected)
                        {
                            sw.WriteLine("{0},{1}", prs.Location.ToCSV(), 1);
                        }
                        else
                        {
                            sw.WriteLine("{0},{1}", prs.Location.ToCSV(), 0);
                        }
                    }//Person's loop
                }
                else
                {
                    sw.WriteLine("Metric network, there is no topological representation.");
                }
                sw.Close();
            }//Group's loop
        }

        /// <summary>
        /// Exports the simulation population network for plotting purpose
        /// </summary>
        /// <remarks>
        /// This code should be called from inside and at the end of the 
        /// simulation clock loop. The simulation should be configured to run
        /// ONLY for a single replication as this function will output a file 
        /// for each simulation clock. 
        /// </remarks>
        /// <param name="filename">
        /// The full name of the output file <c>WITHOUT</c> extension
        /// </param>
        public void ExportNetwork(string filename)
        {
            int g, gCount, p, pCount;
            string fname;
            Group grp;
            Person prs;

            //Defines the file name
            fname = string.Format("{0}Network-{1}-{2}.csv", filename, this._run + 1, this._clock);
            //Output file
            StreamWriter sw = new StreamWriter(fname);

            //Print out file header
            sw.WriteLine("Group Id,Group,Topology,Person Id,STDStatus|Location|Acquaintances Ids|Partners Ids");

            gCount = this._pop.Count;
            for (g = 0; g < gCount; g++)
            {
                //Gets the group
                grp = this._pop[g];

                //Creates the matlab file
                pCount = grp.Size;
                if (grp.Topology == ETopology.Circle)
                {
                    for (p = 0; p < pCount; p++)
                    {
                        prs = grp[p, "STD status"];
                        sw.WriteLine("{0},{1},{2},{3},{4}|{5}|{6}|{7}",
                                     grp.Id, grp.Name, grp.Topology.ToString(),
                                     prs.Id, prs.STDStatus.ToString(), p,
                                     prs.Friends.ToCSV(), prs.Partners.ToCSV());
                    }//Person's loop
                }
                else if (grp.Topology == ETopology.Sphere)
                {
                    for (p = 0; p < pCount; p++)
                    {
                        prs = grp[p, "STD status"];
                        sw.WriteLine("{0},{1},{2},{3},{4}|{5}|{6}|{7}",
                                     grp.Id, grp.Name, grp.Topology.ToString(),
                                     prs.Id, prs.STDStatus.ToString(), prs.Location.ToCSV(),
                                     prs.Friends.ToCSV(), prs.Partners.ToCSV());
                    }//Person's loop
                }
                else //Topology free
                {
                    for (p = 0; p < pCount; p++)
                    {
                        prs = grp[p, "STD status"];
                        sw.WriteLine("{0},{1},{2},{3},{4}|{5}|{6}|{7}",
                                     grp.Id, grp.Name, grp.Topology.ToString(),
                                     prs.Id, prs.STDStatus.ToString(), 0,
                                     prs.Friends.ToCSV(), prs.Partners.ToCSV());
                    }//Person's loop
                }
            }//Group's loop

            sw.Close();
        }

        /// <summary>
        /// Tabulates the simulation results
        /// </summary>
        /// <returns>A <see cref="DataTable"/> with the tabulated results</returns>
        public DataTable TabulateResults()
        {
            return TabulateResults(false);
        }

        /// <summary>
        /// Tabulates the simulation results or network propeties
        /// </summary>
        /// <param name="netProperties">
        /// If <b>true</b> returns the network properties, otherwise the 
        /// model simulation results.
        /// </param>
        /// <returns>
        /// A <see cref="DataTable"/> with the tabulated results
        /// </returns>
        public DataTable TabulateResults(bool netProperties)
        {
            int r, c, g, gCount;
            DataTable result = new DataTable();

            //Gets the number of groups
            gCount = this._pop.Count;

            //Check the output type
            if (netProperties)
            {
                #region Export Network Properties
                //Creates table structure
                result.TableName = "SWN Properties";

                //Adds columns
                result.Columns.Add("Run", typeof(int));
                result.Columns.Add("GroupId", typeof(int));
                result.Columns.Add("Group", typeof(string));
                result.Columns.Add("Vertices", typeof(int));
                result.Columns.Add("Edges", typeof(int));
                result.Columns.Add("Connected", typeof(bool));
                result.Columns.Add("Degree", typeof(double));
                result.Columns.Add("Diameter", typeof(double));
                result.Columns.Add("PathLength", typeof(double));
                result.Columns.Add("Clustering", typeof(double));
                result.Columns.Add("GConnectivity", typeof(double));
                result.Columns.Add("LConnectivity", typeof(double));
                result.Columns.Add("GEfficiency", typeof(double));
                result.Columns.Add("LEfficiency", typeof(double));
                result.Columns.Add("Cost", typeof(double));
                result.Columns.Add("LRegular", typeof(double));
                result.Columns.Add("CRegular", typeof(double));
                result.Columns.Add("LRandom", typeof(double));
                result.Columns.Add("CRandom", typeof(double));

                //main output loop
                string tmpData;
                for (r = 0; r < this._simRuns; r++)
                {
                    for (g = 0; g < gCount; g++)
                    {
                        tmpData = string.Format("{0},{1},{2},", r + 1, g, this._pop[g].Name);
                        tmpData += this._netdata[r, g].ToCSV(false);
                        result.Rows.Add(tmpData.Split(','));
                    }
                }
                #endregion
            }
            else
            {
                #region Export Network Properties
                result.TableName = "Model Results";

                //Creates table
                result.Columns.Add("Run", typeof(int));
                result.Columns.Add("Clock", typeof(int));
                result.Columns.Add("Date", typeof(DateTime));
                result.Columns.Add("GroupId", typeof(int));
                result.Columns.Add("Group", typeof(string));
                result.Columns.Add("Topology", typeof(string));
                result.Columns.Add("Size", typeof(int));
                result.Columns.Add("Female", typeof(int));
                result.Columns.Add("Male", typeof(int));
                result.Columns.Add("Homosexual", typeof(int));
                result.Columns.Add("Friends", typeof(int));
                result.Columns.Add("FromFriends", typeof(int));
                result.Columns.Add("Partners", typeof(int));
                result.Columns.Add("Stable", typeof(int));
                result.Columns.Add("FromAcquaintances", typeof(int));
                result.Columns.Add("Casual", typeof(int));
                result.Columns.Add("Internal", typeof(int));
                result.Columns.Add("External", typeof(int));
                result.Columns.Add("Concurrent", typeof(int));
                result.Columns.Add("STDPrevalence", typeof(double));
                result.Columns.Add("STDIncidence", typeof(int));
                result.Columns.Add("STDFemale", typeof(int));
                result.Columns.Add("STDMale", typeof(int));
                result.Columns.Add("STDHomosexual", typeof(int));
                result.Columns.Add("STDMale2Female", typeof(int));
                result.Columns.Add("STDFemale2Male", typeof(int));
                result.Columns.Add("STDMale2Male", typeof(int));
                result.Columns.Add("STDStable", typeof(int));
                result.Columns.Add("STDCasual", typeof(int));
                result.Columns.Add("STDInternal", typeof(int));
                result.Columns.Add("STDExternal", typeof(int));
                result.Columns.Add("STDRecovered", typeof(int));
                result.Columns.Add("STDProtected", typeof(int));
                result.Columns.Add("Deaths", typeof(int));
                result.Columns.Add("STDDeaths", typeof(int));
                result.Columns.Add("UserData", typeof(string));

                //Main loop
                SimData dataCell;
                DataRow tmpRow;
                for (r = 0; r < this._simRuns; r++)
                {
                    for (c = 0; c <= this._simDuration; c++)
                    {
                        for (g = 0; g < gCount; g++)
                        {
                            //Creates a new row
                            tmpRow = result.NewRow();

                            //Gets the simulation data cell
                            dataCell = this._data[r, c, g];

                            //Populates row
                            tmpRow["Run"] = r + 1;
                            tmpRow["Clock"] = c;
                            tmpRow["Date"] = dataCell.Date;
                            tmpRow["GroupId"] = this._pop[g].Id;
                            tmpRow["Group"] = this._pop[g].Name;
                            tmpRow["Topology"] = this._pop[g].Topology.ToString();
                            tmpRow["Size"] = dataCell.Size;
                            tmpRow["Female"] = dataCell.Female;
                            tmpRow["Male"] = dataCell.Male;
                            tmpRow["Homosexual"] = dataCell.Homosexual;
                            tmpRow["Friends"] = dataCell.Friends;
                            tmpRow["FromFriends"] = dataCell.FromFriends;
                            tmpRow["Partners"] = dataCell.Partners;
                            tmpRow["Stable"] = dataCell.Stable;
                            tmpRow["FromAcquaintances"] = dataCell.StbFromList;
                            tmpRow["Casual"] = dataCell.Casual;
                            tmpRow["Internal"] = dataCell.Internal;
                            tmpRow["External"] = dataCell.External;
                            tmpRow["Concurrent"] = dataCell.Concurrent;
                            tmpRow["STDPrevalence"] = dataCell.STDPrevalence;
                            tmpRow["STDIncidence"] = dataCell.STDIncidence;
                            tmpRow["STDFemale"] = dataCell.STDFemale;
                            tmpRow["STDMale"] = dataCell.STDMale;
                            tmpRow["STDHomosexual"] = dataCell.STDHomosexual;
                            tmpRow["STDMale2Female"] = dataCell.STDMale2Female;
                            tmpRow["STDFemale2Male"] = dataCell.STDFemale2Male;
                            tmpRow["STDMale2Male"] = dataCell.STDMale2Male;
                            tmpRow["STDStable"] = dataCell.STDStable;
                            tmpRow["STDCasual"] = dataCell.STDCasual;
                            tmpRow["STDInternal"] = dataCell.STDInternal;
                            tmpRow["STDExternal"] = dataCell.STDExternal;
                            tmpRow["STDRecovered"] = dataCell.STDRecovered;
                            tmpRow["STDProtected"] = dataCell.STDProtected;
                            tmpRow["Deaths"] = dataCell.Deaths;
                            tmpRow["STDDeaths"] = dataCell.STDDeaths;
                            tmpRow["UserData"] = dataCell.UserData;

                            //Adds row to table
                            result.Rows.Add(tmpRow);
                        }
                    }
                }

                #endregion
            }

            //Retuns the output table
            return result;
        }

        #endregion //File I/O

        #region Private Methods

        // This method is called on a worker thread (via asynchronous delegate 
        // invocation). This is where we call the operation (as defined in the 
        // deriving class's DoWork method).
        private void SimStart()
        {
            try
            {
                Simulate();
            }
            catch (Exception e)
            {
                this._timer.Stop();
                this.OnSimError(this, new SimEventArgs(e.Message));
            }

            lock (this)
            {
                // If the operation wasn't cancelled (or if the UI thread
                // tried to cancel it, but the method ran to completion
                // anyway before noticing the cancellation) and it
                // didn't fail with an exception, then we complete the
                // operation - if the UI thread was blocked waiting for
                // cancellation to complete it will be unblocked, and
                // the Completion event will be raised.
                if (this._status == ESimStatus.Resetting)
                {
                    this.ClearData(true);
                    this._status = ESimStatus.Ready;
                }
                else
                {
                    this._status = ESimStatus.Completed;
                }
            }

            //Sends end all trials event
            FireSimEvent(this.OnEndTrial);
        }

        /// <summary>
        /// Runs the simulation. This is where the simulation takes place.
        /// </summary>
        private void Simulate()
        {
            //Start the simulation time
            this._timer.Reset();
            this._timer.Start();

            //Sets the run-time data structures to their initial status
            this.ClearData(true);

            //Resizes the simulation data structure according to the scenario settings
            this._data = new SimData[this._simRuns, this._simDuration + 1, this._pop.Count];
            this._netdata = new SWNInfo[this._simRuns, this._pop.Count];

            //Checks the random number generator's seeding options 
            if (!this.AutoSeed)
            {
                this._rnd.ReSeed(this.CustomSeed);
            }

            //Save the initial prevalence
            this._pop.SavePrevalence();

            //Sends the start trial event
            FireSimEvent(this.OnStartTrial);

            //Gets the number of groups in the population 
            int gCount = this._pop.Count;
            if (gCount <= 0)
            {
                return;
            }

            //Resets the person Id counter
            Person.ResetId();

            #region Simulation Runs (Trials)
            //Start simulation trials (runs) loop ans sends the event 
            for (this._run = 0; this._run < this._simRuns; this._run++)
            {
                /*Initialise the simulation clock
                 * and sends the start run event*/
                this._clock = 0;
                this._stdzero = false;
                FireSimEvent(this.OnStartRun);

                //Reset the person's visit counter
                this._idxVisit = 0;

                //Restore initial prevalence
                this._pop.RestorePrevalence();

                #region Create Initial Population

                //Creates the initial population
                if (this._simWarmType == EWarmup.None)
                {
                    //Creates population with HIV status
                    for (int g = 0; g < gCount; g++)
                    {
                        //this._pop[g].CreatePopulation(this._info,true);
                        this._pop[g].CreatePopulation(this._info, false);
                    }

                    InitialiseSTDInfection();
                }
                else
                {
                    //Creates population without HIV status
                    for (int g = 0; g < gCount; g++)
                    {
                        this._pop[g].CreatePopulation(this._info, false);
                    }
                }

                #endregion //Initial population

                #region Simulation Warm-up
                if (this._simWarmType == EWarmup.None)
                {
                    this._warmup = false;
                }
                else
                {
                    //Updates warm-up status and concurrency value
                    this._warmup = true;
                    this._pop.SaveConcurrency(this._simWMaxConc, this._simWPrConc);

                    //Sends warm-up start event 
                    FireSimEvent(this.OnStartWarmUp);

                    //Defines the warm-up type
                    if (this._simWarmType == EWarmup.Traditional)
                    {
                        //Call traditional warm-up and checks for reset request
                        if (!this.TraditionalWarmup())
                        {
                            //Restore original concurrency values
                            this._pop.RestoreConcurrency();
                            break;
                        }
                    }
                    else
                    {
                        if (this._simWarmType == EWarmup.Temporal)
                        {
                            //Call traditional warm-up and checks for reset request
                            if (!this.TemporalWarmup())
                            {
                                //Restore original concurrency values
                                this._pop.RestoreConcurrency();
                                break;
                            }
                        }
                        else 
                        {
                            //Call traditional warm-up and checks for reset request
                            if (!this.ConditionalWarmup())
                            {
                                //Restore original concurrency values
                                this._pop.RestoreConcurrency();
                                break;
                            }
                        }
                    }

                    //Restore original concurrency values
                    this._pop.RestoreConcurrency();

                    // Reset clock after warm-up and sends end warm-up event
                    this._warmup = false;
                    this._clock = 0;
                    FireSimEvent(this.OnEndWarmUp);
                }

                #endregion //Warm-up

                #region Clock zero setup and data collection

                /*==================================
                 * Collects the initial data  
                 *================================*/
                FireSimEvent(this.OnStartClock);
                ClockZeroData();

                //Checks for animation
                if (this._simAnimate)
                {
                    FireSimEvent(this.OnAnimate);
                }

                //Sends the end clock event
                FireSimEvent(this.OnEndClock);

                //Checks for simulation stopping request before next clock
                if (this._status == ESimStatus.Stepping ||
                    this._status == ESimStatus.Paused)
                {
                    //Sends simulation stop and continue events
                    FireSimEvent(this.OnStopRun);
                    WaitToContinue();
                    //Checks for reset request
                    if (this._status != ESimStatus.Resetting)
                    {
                        FireSimEvent(this.OnContinueRun);
                    }
                }
                else
                {
                    Thread.Sleep(this._simDelay);
                }

                //Checks for a reset request
                if (this._status == ESimStatus.Resetting)
                {
                    break;
                }

                #endregion //Clock zero

                /*====================================
                 * SIMULATION CLOCK STARTS
                 * ==================================*/
                //Checks for HIV Prevalence zero
                if (!this._stdzero)
                {
                    #region Simulation Clock
                    //Starts the simulation clock from 1
                    for (this._clock = 1; this._clock <= this._simDuration; this._clock++)
                    {
                        #region Simulation body goes here
                        // 1 - Increments simulation clock 
                        FireSimEvent(this.OnStartClock);

                        // 2 - Update Population
                        UpdatePopulation();

                        // 3 - Update Partnerships
                        UpdatePartners();

                        // 4 - Create new partnerships
                        CreatePartners();

                        // 5 - Execute intervention strategies (TODO)
                        ExecuteIntervention();

                        // 6 - Update STD transmission within current partnerships
                        UpdateTransmission();

                        // 7 - Update STD prevalence for all groups and check for STD prevalence zero
                        if (UpdateSTDPrevalence() <= 0)
                        {
                            this._stdzero = true;
                            FireSimEvent(this.OnEndClock);
                            break;
                        }

                        // 8 - Checks for animation
                        if (this._simAnimate)
                        {
                            FireSimEvent(OnAnimate);
                        }

                        /*#####################################
                         * User defined data collection point
                         * ------------------------------------
                         * 
                         * You code goes here
                         * 
                         *####################################*/

                        // 9 - Sends the end clock event
                        FireSimEvent(this.OnEndClock);

                        // 10 - Checks for simulation stopping request before next clock
                        if (this._status == ESimStatus.Stepping ||
                            this._status == ESimStatus.Paused)
                        {
                            //Sends simulation stop and continue events
                            FireSimEvent(this.OnStopRun);
                            WaitToContinue();

                            //Checks for reset request
                            if (this._status != ESimStatus.Resetting)
                            {
                                FireSimEvent(this.OnContinueRun);
                            }
                        }
                        else
                        {
                            // 11 - Controls the simulation speed
                            Thread.Sleep(this._simDelay);
                        }

                        // 12 - Checks for simulation reset request before next clock
                        if (this._status == ESimStatus.Resetting)
                        {
                            break;
                        }

                        #endregion //Simulation body

                    }//Next clock

                    // 13 - Avoids wrong information on the GUI
                    if (this._clock > this._simDuration)
                    {
                        this._clock = this._simDuration;
                    }

                    #endregion //Simulation Clock

                } //########## END OF SIMULATIN CLOCK ##########

                // 14 - Calculates the network characteristics
                CalculateNetwork();

                // 15 - Clear temporary data 
                ClearData(false);

                // 16 - Checks for user request to reset
                if (this._status == ESimStatus.Resetting)
                {
                    //Sends the before reset
                    FireSimEvent(this.OnBeforeReset);
                    break;
                }

                // 17 - Sends the end run event
                if (this.OnEndRun != null)
                {
                    if (this._stdzero)
                    {
                        FireSimEvent(this.OnEndRun, new object[] 
                        {
                            this,
                            new SimEventArgs("STD Prevalence Zero.") 
                        });
                    }
                    else
                    {
                        FireSimEvent(this.OnEndRun, new object[] 
                        {
                            this,
                            new SimEventArgs("Completed.") 
                        });
                    }
                }

            } //########## END OF SIMULATIN TRIALS ##########

            // 18 - Avoids wrong information on the GUI
            if (this._run > this._simRuns)
            {
                this._run = this._simRuns;
            }

            #endregion Simulation Runs (Trials)

            // 19 - Checks for user request to reset
            if (this._status == ESimStatus.Resetting)
            {
                //Send the reset event
                FireSimEvent(this.OnReset);
            }

            // 20 - Restores the initial prevalence
            this._pop.RestorePrevalence();

            // 21 - Stops the simulation internal clock
            this._timer.Stop();
        }//End Simulate

        #region Simulation warm-up procedures

        /// <summary>
        /// Runs the simulation traditional warm-up
        /// </summary>
        /// <returns>
        /// <c>false</c> for any error or reset request, otherwise <c>true</c>
        /// </returns>
        private bool TraditionalWarmup()
        {
            //Local variable
            int gCount;

            //Gets the number of groups
            gCount = this._pop.Count;

            //Starts the simulation warm-up at clock
            for (this._clock = 1; this._clock <= this._simWarmupLen; this._clock++)
            {
                #region Warm-up body goes here

                // 1 - Increments simulation clock 
                FireSimEvent(this.OnStartClock);

                // 2 - Update Population
                UpdatePopulation();

                // 3 - Update Partnerships
                UpdatePartners();

                // 4 - Make available individuals at the end of their transitory period
                //UpdateTransitory();

                // 5 - Create new partnerships
                CreatePartners();

                // 6 - Checks for animation
                if (this._simAnimate)
                {
                    FireSimEvent(OnAnimate);
                }

                // 7 - Sends the end clock event
                FireSimEvent(this.OnEndClock);

                // 8 - Checks for simulation stopping request before next clock
                if (this._status == ESimStatus.Stepping ||
                    this._status == ESimStatus.Paused)
                {
                    //Sends simulation stop and continue events
                    FireSimEvent(this.OnStopRun);
                    WaitToContinue();

                    //Checks for reset request
                    if (this._status != ESimStatus.Resetting)
                    {
                        FireSimEvent(this.OnContinueRun);
                    }
                }
                else
                {
                    // Sets simulation to run at the highest speed.
                    Thread.Sleep(this._simDelay);
                }

                // 9 - Checks for simulation reset request before next clock
                if (this._status == ESimStatus.Resetting)
                {
                    return false;
                }

                #endregion

            }//Simulation warm-up clock

            // 10 - Starts initial epidemic
            InitialiseSTDInfection();

            // 11 - Updates STD Prevalence
            if (UpdateSTDPrevalence() <= 0.0)
            {
                this._stdzero = true;
            }

            //Return no error or reset
            return true;
        }

        /// <summary>
        /// Runs the simulation temporal warm-up
        /// </summary>
        /// <returns>
        /// False for any error or reset request, True otherwise
        /// </returns>
        private bool TemporalWarmup()
        {
            //Local Variables
            int gIdx, pIdx;	            // Group and person indexes
            double hivCount, initHIV;	// HIV infection counter and # of initial HIV infections
            Group grp;					// Temporary holds the selected group
            Person pZe;				    // Temporary person
            int gCount, pCount;			// Group and person counters

            //Selects a random group
            gCount = this._pop.Count;

            //Select a random group
            gIdx = 0;
            if (gCount > 1)
            {
                gIdx = (int)this._rnd.Uniform(0, gCount);
            }

            //Gets the random selected group 
            grp = this._pop[gIdx];
            pCount = grp.Size;

            //Defines the initial number of HIV infections
            initHIV = this._rnd.Sample(this.WarmUpInfected);
            if (initHIV < 1.0)
            {
                initHIV = 1.0;
            }

            //Population size check
            if (pCount < initHIV)
            {
                initHIV = pCount;
            }

            //Initial HIV infection loop
            hivCount = 0.0;
            while (hivCount < initHIV)
            {
                //Select a random person (pCount > 1, group definition)
                pIdx = (int)this._rnd.Uniform(0, pCount);
                pZe = grp[pIdx, "Temporal warm-up 1"];

                if (pZe.STDStatus == ESTDStatus.Susceptable)
                {
                    //Creates the initial infected person
                    pZe.STDStatus = ESTDStatus.Infected;
                    pZe.STDAge = 0;
                    //Checks if the STD can cause death
                    if (this._rnd.Bernoulli(this._info.Mortality))
                    {
                        pZe.STDDeath = (int)this._rnd.Sample(this._info.LifeExpectancy);
                    }

                    //Sets the duration of infection for non lifelong STDs.
                    if (!this._info.LifeInfection)
                    {
                        pZe.STDDuration = (int)this._rnd.Sample(this._info.STDDuration);
                    }

                    hivCount++;
                }
            }//While loop

            //Update HIV prevalence for the selected group
            grp.STDPrevalence = hivCount / pCount;
            if (grp.STDPrevalence <= 0.0)
            {
                this._stdzero = true;
                return true;
            }

            //Starts the simulation warm-up at clock 1
            for (this._clock = 1; this._clock <= this._simWarmupLen; this._clock++)
            {

                #region Temporal warm-up body goes here

                // 1 - Increments simulation clock 
                FireSimEvent(this.OnStartClock);

                // 2 - Update Population
                UpdatePopulation();

                // 3 - Update Partnerships
                UpdatePartners();

                // 4 - Create new partnerships
                CreatePartners();

                // 5 - Update STD transmission within current partnerships
                UpdateTransmission();

                // 6 - Update STD prevalence for all groups and check for HIV prevalence zero
                if (UpdateSTDPrevalence() <= 0.0)
                {
                    this._stdzero = true;
                    FireSimEvent(this.OnEndClock);
                    break;
                }

                // 7 - Checks for animation
                if (this._simAnimate)
                {
                    FireSimEvent(OnAnimate);
                }

                // 9 - Sends the end clock event
                FireSimEvent(this.OnEndClock);

                // 10 - Checks for simulation stopping request before next clock
                if (this._status == ESimStatus.Stepping ||
                    this._status == ESimStatus.Paused)
                {
                    //Sends simulation stop and continue events
                    FireSimEvent(this.OnStopRun);
                    WaitToContinue();
                    //Checks for reset request
                    if (this._status != ESimStatus.Resetting)
                    {
                        FireSimEvent(this.OnContinueRun);
                    }
                }
                else
                {
                    // 11 - Controls the simulation speed
                    Thread.Sleep(this._simDelay);
                }

                // 12 - Checks for simulation reset request before next clock
                if (this._status == ESimStatus.Resetting)
                {
                    return false;
                }

                #endregion
            }//Simulation clock

            //No error or reset
            return true;
        }

        /// <summary>
        /// Runs the simulation conditional warm-up
        /// </summary>
        /// <returns>
        /// False for any error or reset request, True otherwise
        /// </returns>
        private bool ConditionalWarmup()
        {
            //Local Variables
            int g, pIdx;				//Group indexer and person's index
            int gCount, pCount;			//Group and person counters
            Person pZe;				    //Temporary person
            Group grp;					//Temporary group variable
            double gSTDPrev, pSTDSum;	//Temporary STD prevalence and sum of STD prevalences
            double stdCount, initSTD;	//HIV infection counter and # of initial STD infections	
            bool done;					//Warmup completion flag

            //Defines the number of groups within the population
            gCount = this._pop.Count;

            //Setup the initial population
            pSTDSum = 0.0;
            for (g = 0; g < gCount; g++)
            {
                //Initial setup
                grp = this._pop[g];
                grp.WarmedUp = false;
                pCount = grp.Size;

                //Defines the initial number of HIV infections
                initSTD = this._rnd.Sample(this.WarmUpInfected);
                if (initSTD < 1.0)
                {
                    initSTD = 1.0;
                }

                //Checks for population size and initial HIV infection
                if (pCount < initSTD)
                {
                    initSTD = pCount;
                }

                //Initial STD Infection loop
                stdCount = 0.0;
                while (stdCount < initSTD)
                {
                    //Select a random person
                    if (pCount == 1)
                    {
                        pIdx = 0;
                    }
                    else
                    {
                        pIdx = (int)this._rnd.Uniform(0, pCount);
                    }

                    pZe = grp[pIdx, "Conditional warm-up 1"];
                    if (pZe.STDStatus == ESTDStatus.Susceptable)
                    {
                        //Creates the initial infected person
                        pZe.STDStatus = ESTDStatus.Infected;
                        pZe.STDAge = 0;

                        //Checks if STD can cause death
                        if (this._rnd.Bernoulli(this._info.Mortality))
                        {
                            pZe.STDDeath = (int)this._rnd.Sample(this._info.LifeExpectancy);
                        }

                        //Sets the duration of infection for non lifelong STDs.
                        if (!this._info.LifeInfection)
                        {
                            pZe.STDDuration = (int)this._rnd.Sample(this._info.STDDuration);
                        }

                        stdCount++;
                    }
                }

                //Sum STD prevalence
                stdCount = stdCount / pCount;
                pSTDSum += stdCount;

                //Checks the completed precondition
                if (grp.STDPrevalence <= stdCount)
                {
                    grp.WarmedUp = true;
                }
                else
                {
                    grp.WarmedUp = false;
                }
            }

            //Reset simulation clock and check prevalence zero
            this._clock = 0;
            pSTDSum = pSTDSum / gCount;
            if (pSTDSum <= 0.0)
            {
                this._stdzero = true;
                return true;
            }

            //Initial condition
            done = true;

            //Start simulation conditional warm up
            while (!done && (this._simWarmupLen > this._clock))
            {
                #region Conditional warmup body goes here

                // 1 - Increments simulation clock 
                this._clock++;
                FireSimEvent(this.OnStartClock);

                // 2 - Update Population
                UpdatePopulation();

                // 3 - Update Partnerships
                UpdatePartners();

                // 4 - Create new partnerships
                CreatePartners();

                // 5 - Update HIV transmission within current partnerships 
                UpdateTransmission();

                /* 6 - Evaluate the completion condition, calculate HIV 
                 *     prevalence and check for HIV prevalence zero */
                done = true;
                pSTDSum = 0.0;
                for (g = 0; g < gCount; g++)
                {
                    gSTDPrev = this.CalcPrevalence(this._pop[g]);
                    pSTDSum += gSTDPrev;

                    if (!this._pop[g].WarmedUp)
                    {
                        if (this._pop[g].STDPrevalence <= gSTDPrev)
                        {
                            this._pop[g].WarmedUp = true;
                        }
                        else
                        {
                            done = false;
                        }
                    }
                }

                pSTDSum = pSTDSum / gCount;
                if (pSTDSum <= 0.0)
                {
                    this._stdzero = true;
                    FireSimEvent(this.OnEndClock);
                    break;
                }

                // 8 - Checks for animation
                if (this._simAnimate)
                {
                    FireSimEvent(OnAnimate);
                }

                // 9 - Sends the end clock event
                FireSimEvent(this.OnEndClock);

                // 10 - Checks for simulation stopping request before next clock
                if (this._status == ESimStatus.Stepping ||
                    this._status == ESimStatus.Paused)
                {
                    //Sends simulation stop and continue events
                    FireSimEvent(this.OnStopRun);
                    WaitToContinue();

                    //Checks for reset request
                    if (this._status != ESimStatus.Resetting)
                    {
                        FireSimEvent(this.OnContinueRun);
                    }
                }
                else
                {
                    // 11 - Controls the simulation speed
                    Thread.Sleep(this._simDelay);
                }

                //12 - Checks for simulation reset request before next clock
                if (this._status == ESimStatus.Resetting)
                {
                    return false;
                }

                #endregion
            }//Finishes conditional warm-up loop

            //No error or reset
            return true;
        }
        #endregion //warm-up

        #region Simulation internal routines

        /// <summary>
        /// Updates the population size and structure definition- (growth and deaths).
        /// </summary>
        private void UpdatePopulation()
        {
            //Local Variables
            int g, p;			//Group and person indexers
            int gCount, pCount;	//Group and person counters
            Person pZe;		    //Temporary person
            Point3d loc;		//Location of the new person

            //Group loop
            gCount = this._pop.Count;
            for (g = 0; g < gCount; g++)
            {
                //Check the conditional warm-up
                if (this._warmup && this._pop[g].WarmedUp &&
                    this._simWarmType == EWarmup.Conditional)
                {
                    pCount = 0;
                }
                else
                {
                    pCount = this._pop[g].Size;
                }

                //Collect dimulation clock and group size data
                if (!this._warmup)
                {
                    this._data[this._run, this._clock, g].Date = this.ClockToDate;
                    this._data[this._run, this._clock, g].Size = pCount;
                }

                //Person loop
                for (p = 0; p < pCount; p++)
                {

                    pZe = this._pop[g][p, "Update population 1"];

                    //Update Age, STD infection and protection
                    pZe.Age++;
                    if (pZe.STDStatus == ESTDStatus.Infected)
                    {
                        pZe.STDAge++;

                        //Update non lifelong infection
                        if (!this._info.LifeInfection)
                        {
                            if (pZe.STDAge > pZe.STDDuration)
                            {
                                if (this._info.AllowReinfection)
                                {
                                    pZe.STDStatus = ESTDStatus.Susceptable;
                                }
                                else
                                {
                                    pZe.STDStatus = ESTDStatus.Protected;

                                    //Collect data
                                    if (!this._warmup)
                                    {
                                        this._data[this._run, this._clock, g].STDProtected++;
                                    }
                                }

                                pZe.STDAge = 0;
                                pZe.STDDeath = int.MaxValue;
                                pZe.STDDuration = int.MaxValue;

                                //Collect data
                                if (!this._warmup)
                                {
                                    this._data[this._run, this._clock, g].STDRecovered++;
                                }
                            }
                        }
                    }
                    else if (pZe.STDStatus == ESTDStatus.Protected)
                    {
                        if (pZe.STDDuration < this._clock)
                        {
                            pZe.STDStatus = ESTDStatus.Susceptable;
                            pZe.STDDuration = int.MaxValue;
                            if (!this._warmup)
                            {
                                this._data[this._run, this._clock, g].STDProtected--;
                            }
                        }
                    }//STD Status

                    //Update individual's death by natural causes or STD infection
                    if (pZe.Age > pZe.LifeExpectancy || pZe.STDAge > pZe.STDDeath)
                    {
                        if (!this._warmup) //Collect data
                        {
                            this._data[this._run, this._clock, g].Deaths++;

                            //HIV caused death
                            if (pZe.Age <= pZe.LifeExpectancy)
                            {
                                this._data[this._run, this._clock, g].STDDeaths++;
                            }
                        }

                        //Kills the old individual and create a new person
                        loc = pZe.Location;
                        pZe.SayGoodBye();
                        pZe = this._pop[g].CreatePerson(this._info);
                        pZe.Location = loc;
                        VaccinationByPerson(pZe, g); //Vaccine intervention
                        this._pop[g][p, "Update Population 3"] = pZe;
                    }

                    if (!this._warmup) //Collect gender data
                    {
                        //Checks the gender
                        if (pZe.Gender == EGender.Female)
                        {
                            this._data[this._run, this._clock, g].Female++;
                        }
                        else //Male
                        {
                            this._data[this._run, this._clock, g].Male++;
                            if (pZe.Homosexual)
                            {
                                this._data[this._run, this._clock, g].Homosexual++;
                            } //Homosexual
                        } //End gender
                    } //Collect data
                } //Next person
            } //Next group
        } //End UpdatePopulation

        /// <summary>
        /// Updates the status of existing partneships
        /// </summary>
        private void UpdatePartners()
        {
            //Local Variables
            int g, p, pp;			//Group and person indexers
            int gCount, pCount;		//Group and person counters
            Person pZe, pLia;		//Temporary persons
            EPartners tPartner;		//Temporary partnership

            //Group loop
            gCount = this._pop.Count;
            for (g = 0; g < gCount; g++)
            {
                //Check the conditional warm-up
                if (this._warmup && this._pop[g].WarmedUp &&
                    this._simWarmType == EWarmup.Conditional)
                {
                    pCount = 0;
                }
                else
                {
                    pCount = this._pop[g].Size;
                }

                //Person loop
                for (p = 0; p < pCount; p++)
                {
                    pZe = this._pop[g][p, "Update partners 1"];

                    //Checks the size of the individual's partnership list
                    if (pZe.Partners.Count > 0)
                    {
                        //Parthenrships loop
                        for (pp = 0; pp < pZe.Partners.Count; pp++)
                        {
                            //Checks the end of the partnership
                            if (pZe.Partners[pp].Duration <= this._clock)
                            {
                                tPartner = pZe.Partners[pp].Partnership;
                                pLia = pZe.Partners[pp].ToPerson;

                                //Finish partnership
                                if (pZe.EndPartnership(pLia))
                                {
                                    //Sets the transitory status for Stable partners
                                    if (tPartner == EPartners.Stable)
                                    {
                                        pZe.Partnership = ERelation.Transitory;
                                        pZe.Transitory = this._clock + (int)this._rnd.Sample(
                                            pZe.MyGroup.StbTransitory);

                                        pLia.Partnership = ERelation.Transitory;
                                        pLia.Transitory = this._clock + (int)this._rnd.Sample(
                                            pLia.MyGroup.StbTransitory);
                                    }
                                    else //Casual, checks the availability status
                                    {
                                        if (pZe.Partners.Count == 0)
                                        {
                                            pZe.Partnership = ERelation.Available;
                                        }

                                        if (pLia.Partners.Count == 0)
                                        {
                                            pLia.Partnership = ERelation.Available;
                                        }
                                    } //If partners type
                                } //If finishes
                            } //If end
                        } //Partner loop
                    } //If List

                    //Update Transitory Status
                    if (pZe.Partnership == ERelation.Transitory)
                    {
                        //Evaluate the end of transition status
                        if (pZe.Transitory <= this._clock)
                        {
                            pZe.Partnership = ERelation.Available;
                            pZe.Transitory = 0;
                        }
                    } //If transitory
                } //Person loop
            } //Group loop
        }

        /// <summary>
        /// Execute preventive vaccine intervention strategies
        /// </summary>
        private void ExecuteIntervention()
        {
            int i, g;
            for (i = 0; i < this._inteven.Count; i++)
            {
                if (this._inteven[i].Active && this._inteven[i].Clock == this._clock)
                {
                    if (this._inteven[i].Intevention == EStrategy.AllGroups)
                    {
                        for (g = 0; g < this._pop.Count; g++)
                        {
                            VaccinationByGroup(this._inteven[i], g);
                        }
                    }
                    else if (this._inteven[i].Intevention == EStrategy.Custom)
                    {
                        for (g = 0; g < this._pop.Count; g++)
                        {
                            if (this._inteven[i].Groups.IndexOf(this._pop[g].Id) >= 0)
                            {
                                VaccinationByGroup(this._inteven[i], g);
                            }
                        }
                    }
                } 
            } 
        } 

        /// <summary>
        /// Execute a preventive vaccination intervention to a group
        /// </summary>
        /// <param name="stg">The intervention <see cref="Strategy"/></param>
        /// <param name="gIdx">The index of the group to be intervened</param>
        private void VaccinationByGroup(Strategy stg, int gIdx)
        {
            int npop, pIdx, j, duration;
            RIntArray rndIdx;		//Array of random indexes
            Group grp;				//Group to intervene

            grp = this._pop[gIdx];

            npop = (int)(grp.Size * stg.Population);
            rndIdx = new RIntArray(grp.Size, this._rnd);
            for (j = 0; j < npop; j++)
            {
                pIdx = rndIdx.Next();
                if (grp[pIdx, "Intervention"].STDStatus == ESTDStatus.Susceptable)
                {
                    if (this._rnd.Bernoulli(stg.UseVaccine.Effectiveness))
                    {
                        grp[pIdx, "Intervention"].STDStatus = ESTDStatus.Protected;
                        if (stg.UseVaccine.Lifetime)
                        {
                            grp[pIdx, "Intervention"].STDDuration = this._simDuration;
                        }
                        else
                        {
                            duration = this._clock + stg.UseVaccine.Length;
                            grp[pIdx, "Intervention"].STDDuration = duration;
                        }

                        this._data[_run, _clock, gIdx].STDProtected++;
                    }//If effectiveness
                }//If HIV status
            }//For j
        }//Execute Intervention by group

        /// <summary>
        /// Execute preventive vaccine intervention to new person
        /// </summary>
        /// <param name="prs">The <see cref="Person"/> to be vaccinated</param>
        /// <param name="gIdx">The person's group index</param>
        private void VaccinationByPerson(Person prs, int gIdx)
        {
            int i, duration;
            for (i = 0; i < this._inteven.Count; i++)
            {
                if (this._inteven[i].Active && this._inteven[i].Clock <= this._clock)
                {
                    if (this._inteven[i].Intevention == EStrategy.AllGroups)
                    {
                        //Evaluate intervention
                        if (this._rnd.Bernoulli(this._inteven[i].Population))
                        {
                            if (prs.STDStatus == ESTDStatus.Susceptable)
                            {
                                if (this._rnd.Bernoulli(this._inteven[i].UseVaccine.Effectiveness))
                                {
                                    prs.STDStatus = ESTDStatus.Protected;
                                    if (this._inteven[i].UseVaccine.Lifetime)
                                    {
                                        prs.STDDuration = this._simDuration;
                                    }
                                    else
                                    {
                                        duration = this._clock + this._inteven[i].UseVaccine.Length;
                                        prs.STDDuration = duration;
                                    }

                                    this._data[_run, _clock, gIdx].STDProtected++;
                                }//If effectiveness
                            }//If HIV status
                        }//If population
                    }
                    else if (this._inteven[i].Intevention == EStrategy.Custom)
                    {
                        if (this._inteven[i].Groups.IndexOf(prs.MyGroup.Id) >= 0)
                        {
                            //Evaluate intervention
                            if (this._rnd.Bernoulli(this._inteven[i].Population))
                            {
                                if (prs.STDStatus == ESTDStatus.Susceptable)
                                {
                                    if (this._rnd.Bernoulli(this._inteven[i].UseVaccine.Effectiveness))
                                    {
                                        prs.STDStatus = ESTDStatus.Protected;
                                        if (this._inteven[i].UseVaccine.Lifetime)
                                        {
                                            prs.STDDuration = this._simDuration;
                                        }
                                        else
                                        {
                                            duration = this._clock + this._inteven[i].UseVaccine.Length;
                                            prs.STDDuration = duration;
                                        }

                                        this._data[_run, _clock, gIdx].STDProtected++;
                                    }//If effectiveness
                                }//If HIV status
                            }//If population
                        }//If group in list
                    }//If strategy
                }//If active and on time
            }//For i
        }//Execute Intervention by person

        /// <summary>
        /// Creates new partnerships for each core group
        /// </summary>
        private void CreatePartners()
        {
            //Local variables
            Group grp;			    //Current selected group
            Person pZe, pLia;		//Temporary persons
            int g, p, pp, step;		//Group and person indexers, circle searching position
            int gCount, pCount;		//Group and person counters
            int gIdx, pIdx, pRnd;	//Current group, persons indexes
            int maxTrial, nTrial;	//Maximum number of trials
            bool found;				//Identifies if a new paernership has been found
            double DC;				//Distance constant
            RIntArray rndIdx;		//Array of random indexes

            //Defines the initial group
            gCount = this._pop.Count;
            if (gCount == 1)
            {
                gIdx = 0;
            }
            else
            {
                gIdx = (int)this._rnd.Uniform(0, gCount);
            }

            //Group loop
            for (g = 0; g < gCount; g++)
            {
                //Check the conditional warm-up
                if (this._warmup && this._pop[g].WarmedUp &&
                    this._simWarmType == EWarmup.Conditional)
                {
                    //do nothing, group already completed
                }
                else
                {
                    #region Setup Conditions

                    //Current group
                    grp = this._pop[gIdx];

                    //Setup
                    pCount = grp.Size;
                    DC = Math.Cos(grp.Distance / grp.Radius);
                    maxTrial = grp.MaxTrials;

                    //Setup the temporary index array
                    rndIdx = new RIntArray(pCount, this._rnd);

                    //Chooses a random starting person
                    pIdx = rndIdx.Next();
                    #endregion //Setup conditions

                    //Individuals main loop
                    for (p = 0; p < pCount; p++)
                    {
                        //Gets the current person;
                        pZe = grp[pIdx, "Create Partner 1"];

                        //Resets search flag
                        found = false;

                        //What are you looking for
                        if (this._rnd.Bernoulli(grp.PrNewPartner)) //Partnership
                        {
                            #region Looking for a new sexual partner
                            //Which type of partnership
                            if (this._rnd.Bernoulli(grp.PrCasual)) //Casual
                            {
                                #region Looking for a new casual partner
                                //Test precondition for casual partnership
                                if (AllowPartnership(pZe, EPartners.Casual))
                                {
                                    //Where to start searching for a casual partnership?
                                    if (this._rnd.Bernoulli(grp.PrInternal)) //Internal first
                                    {
                                        #region Searches internal first for casual partner
                                        //Searches first internal
                                        found = CasualInternalSearch(pZe, pIdx, gIdx);

                                        //If not found, searches external
                                        if (!found && this._pop.Count > 1)
                                        {
                                            if (!this._warmup || (this._warmup && this._simWarmType != EWarmup.Conditional))
                                            {
                                                found = CasualExternalSearch(pZe, pIdx, gIdx);
                                            }
                                        }// Found

                                        #endregion
                                    } //Internal first
                                    else //External first
                                    {
                                        #region Searches external first for casual partner
                                        if (this._pop.Count > 1)
                                        {
                                            if (!this._warmup || (this._warmup && this._simWarmType != EWarmup.Conditional))
                                            {
                                                found = CasualExternalSearch(pZe, pIdx, gIdx);
                                            }
                                        }// Found

                                        //If not found, searches internal
                                        if (!found)
                                        {
                                            found = CasualInternalSearch(pZe, pIdx, gIdx);
                                        }//Not found

                                        #endregion
                                    } //here to start searching

                                }//Allow Casual Partnership
                                #endregion //Casual partner

                            }//Casual
                            else //Stable
                            {
                                #region Looking for a new stable partner

                                //Test precondition for stable
                                if (AllowPartnership(pZe, EPartners.Stable))
                                {
                                    #region Searches list of acquaintances

                                    //Checks the acquaintaces list size
                                    if (pZe.Friends.Count > 0)
                                    {
                                        //Searches for a new partner within the acquaintaces list
                                        pLia = SearchAcquaintances(pZe, true);

                                        //Checks if a person was found
                                        if (pLia != null)
                                        {
                                            //Create the new partnership
                                            if (CreatePartnership(pZe, pLia, EPartners.Stable))
                                            {
                                                //Update simulation data
                                                if (!this._warmup)
                                                {
                                                    this._data[this._run, this._clock, gIdx].StbFromList += 2;
                                                }

                                                //Sets the found flag
                                                found = true;
                                            }
                                        }//Found
                                    }//List of acquaintaces size

                                    #endregion //List of acquaintances search

                                    #region Search by geography up to MaxTrials times
                                    //If not found in the list of acquaintances
                                    if (!found)
                                    {
                                        //Number of trials
                                        nTrial = 0;

                                        //Define topological search
                                        if (grp.Topology == ETopology.Circle)
                                        {
                                            #region Circle Topology Search

                                            //If MaxTrials is not even, round it up to the nearest even integer
                                            nTrial = grp.MaxTrials;
                                            if (this.Mod(nTrial, 2) != 0)
                                            {
                                                nTrial++;
                                            }

                                            step = 0;
                                            for (pp = 0; pp < nTrial; pp++)
                                            {
                                                //Generates a next target's index
                                                if (this.Mod(pp, 2) == 0) //Move forward
                                                {
                                                    step++;
                                                    pRnd = this.Mod(pIdx + step, pCount);
                                                }
                                                else //Move backward
                                                {
                                                    pRnd = this.Mod(pIdx - step, pCount);
                                                }

                                                pLia = grp[pRnd, "Circle Structure 1"];

                                                //Tests if the selected person's can be a stable partner
                                                if (AllowPartnership(pLia, EPartners.Stable))
                                                {
                                                    //Checks if the target person can be a partner
                                                    if (CanBePartners(pZe, pLia, EPartners.Stable))
                                                    {
                                                        if (CreatePartnership(pZe, pLia, EPartners.Stable))
                                                        {
                                                            //Found a partner, exit
                                                            break;
                                                        }
                                                    }//If accepted, "pooling"
                                                }//Allow Partnership
                                            }//End for loop

                                            #endregion //Circle Topology Search
                                        }
                                        else if (grp.Topology == ETopology.Free)
                                        {
                                            #region Topology Free Search
                                            //Start trials
                                            for (pp = 0; pp < maxTrial; pp++)
                                            {
                                                //Generates a random target person
                                                do
                                                {
                                                    pRnd = (int)this._rnd.Uniform(0, pCount);
                                                } while (pRnd == pIdx);

                                                pLia = grp[pRnd, "Free Structure 1"];

                                                //Tests if the selected person's can be a stable partner
                                                if (AllowPartnership(pLia, EPartners.Stable))
                                                {
                                                    //Checks if the target person can be a partner
                                                    if (CanBePartners(pZe, pLia, EPartners.Stable))
                                                    {
                                                        if (CreatePartnership(pZe, pLia, EPartners.Stable))
                                                        {
                                                            //Found a partner, exit
                                                            break;
                                                        }
                                                    }//If accepted, "pooling"
                                                }//Allow Partnership
                                            }//End trials loop

                                            #endregion //Topology Free Search
                                        }
                                        else //Sphere
                                        {
                                            #region Spherical Topology Search

                                            //Geographical search loop
                                            for (pp = 0; pp < pCount; pp++)
                                            {
                                                //Checks for self loop
                                                if (pIdx != pp)
                                                {
                                                    //Pick up the person at index pp
                                                    pLia = grp[pp, "Sphere Structure stable 1"];

                                                    //Checks geographical distance
                                                    if (Point3d.Dot3d(pZe.Location, pLia.Location) >= DC)
                                                    {
                                                        //Tests if the selected person's can be a stable partner
                                                        if (AllowPartnership(pLia, EPartners.Stable))
                                                        {
                                                            //Checks if the target person can be a partner
                                                            if (CanBePartners(pZe, pLia, EPartners.Stable))
                                                            {
                                                                if (CreatePartnership(pZe, pLia, EPartners.Stable))
                                                                {
                                                                    //Found a partner, exit
                                                                    break;
                                                                }
                                                            }//If accepted, "pooling"
                                                        }//Allow Partnership

                                                        //Evaluate the maximum number of trial
                                                        nTrial++;
                                                        if (nTrial > maxTrial)
                                                        {
                                                            break;
                                                        }//Trials
                                                    }//Distance
                                                }//Self loop
                                            }//Spherical search loop

                                            #endregion //Spherical Topology Search
                                        } //Topological search
                                    }//Not found.

                                    #endregion
                                } //Allow Stable Partnership

                                #endregion //Stable partner
                            }//Stable

                            #endregion //New sexual partner
                        }//Partnership
                        else //Friendship
                        {
                            #region Looking for a new friend

                            //Test for more friends precondition
                            if (MoreFriends(pZe))
                            {
                                #region Searches list of acquaintances

                                //Checks the acquaintaces list size
                                if (pZe.Friends.Count > 0)
                                {
                                    //Searches for a new partner within the acquaintaces list
                                    pLia = SearchAcquaintances(pZe, false);

                                    //Checks if a person was found
                                    if (pLia != null)
                                    {
                                        //Create the new friendship
                                        if (pZe.AddFriend(pLia))
                                        {
                                            //Update friendship data
                                            if (!this._warmup)
                                            {
                                                this._data[this._run, this._clock, gIdx].Friends += 2;
                                                this._data[this._run, this._clock, gIdx].FromFriends += 2;
                                            }

                                            //Sets the found flag
                                            found = true;
                                        }
                                    }//Found
                                }//Acquaintaces list size
                                #endregion

                                #region Search by geography up to MaxTrials times

                                if (!found)
                                {
                                    //Number of trials
                                    nTrial = 0;

                                    //Define topological search
                                    if (grp.Topology == ETopology.Circle)
                                    {
                                        #region Circle Topology Search
                                        //If MaxTrials is not even, round it up to the nearest even integer
                                        nTrial = maxTrial;
                                        if (this.Mod(nTrial, 2) != 0)
                                        {
                                            nTrial++;
                                        }

                                        //Main search loop
                                        step = 0;
                                        for (pp = 0; pp < nTrial; pp++)
                                        {
                                            //Generates a next target's index
                                            if (this.Mod(pp, 2) == 0) //Move forward
                                            {
                                                step++;
                                                pRnd = this.Mod(pIdx + step, pCount);
                                            }
                                            else //Move backward
                                            {
                                                pRnd = this.Mod(pIdx - step, pCount);
                                            }

                                            pLia = grp[pRnd, "Circle Structure 3"];

                                            //Checks the friends list precondition
                                            if (MoreFriends(pLia))
                                            {
                                                //Tries to add current selection to list
                                                if (pZe.AddFriend(pLia))
                                                {
                                                    //Update friendship data
                                                    if (!this._warmup)
                                                    {
                                                        this._data[this._run, this._clock, gIdx].Friends += 2;
                                                    }
                                                    break;
                                                }//Add to list
                                            }//More friends
                                        }//End for loop

                                        #endregion //Topology Free Search
                                    }
                                    else if (grp.Topology == ETopology.Free)
                                    {
                                        #region Topology Free Search
                                        //Geographical search loop
                                        for (pp = 0; pp < maxTrial; pp++)
                                        {
                                            //Generates a random target person
                                            do
                                            {
                                                pRnd = (int)this._rnd.Uniform(0, pCount);
                                            } while (pRnd == pIdx);

                                            pLia = grp[pRnd, "Free Structure 2"];

                                            //Checks the friends list precondition
                                            if (MoreFriends(pLia))
                                            {
                                                //Tries to add the surrent selection to list
                                                if (pZe.AddFriend(pLia))
                                                {
                                                    //Update friendship data
                                                    if (!this._warmup)
                                                    {
                                                        this._data[this._run, this._clock, gIdx].Friends += 2;
                                                    }

                                                    break;
                                                }//Add to list
                                            }//More friends
                                        }//End for loop

                                        #endregion //Topology Free Search
                                    }//Free
                                    else //Sphere
                                    {
                                        #region Spherical Topology Search
                                        //Geographical search loop
                                        for (pp = 0; pp < pCount; pp++)
                                        {
                                            //Checks for self loop
                                            if (pIdx != pp)
                                            {
                                                //Pick up the person at index pp
                                                pLia = grp[pp, "Sphere Structure stable 1"];

                                                //Checks geographical distance
                                                if (Point3d.Dot3d(pZe.Location, pLia.Location) >= DC)
                                                {
                                                    //Checks the friends list precondition
                                                    if (MoreFriends(pLia))
                                                    {
                                                        //Tries to add the current selection to the list
                                                        if (pZe.AddFriend(pLia))
                                                        {
                                                            //Update friendship data
                                                            if (!this._warmup)
                                                            {
                                                                this._data[this._run, this._clock, gIdx].Friends += 2;
                                                            }

                                                            break;
                                                        }//Add to list
                                                    }//More friends

                                                    //Evaluate the maximum number of trial
                                                    nTrial++;
                                                    if (nTrial > maxTrial)
                                                    {
                                                        break;
                                                    }//Trials
                                                }
                                            }
                                        }//End for loop

                                        #endregion //Spherical Topology Search
                                    }//Topological search
                                }//Not found

                                #endregion
                            } //More friends precondition

                            #endregion
                        } //Friendship

                        //Selects next random person
                        pIdx = rndIdx.Next();
                    }//Individuals main loop

                    //Clears up the temporary data
                    GC.Collect();
                }//Warm-up check

                // The next group
                gIdx = this.Mod(gIdx + 1, gCount);
            }//Group loop
        }//CreatePartners

        /// <summary>
        /// Updates HIV transmission within current partnerships
        /// </summary>
        private void UpdateTransmission()
        {
            //Local variables
            Relation tRel;			//Temporarily holds the current relation
            double prSafe, prTrans;	//Probabilities of safe sex practice and STD transmission
            double nContact;		//Number of sexual contacts
            int idTrans, rCount;	//Type of transmission 1=MF, 2=FM, 3=MM, partners counter
            int g, gIdx, p, pp;		//Group and person indexers
            int gCount, pCount;		//Group and person counters
            Person pZe, pLia;		//Temporary persons

            //Group loop
            gCount = this._pop.Count;
            for (g = 0; g < gCount; g++)
            {
                //Check the conditional warm-up
                if (this._warmup && this._pop[g].WarmedUp &&
                    this._simWarmType == EWarmup.Conditional)
                {
                    pCount = 0;
                }
                else
                {
                    pCount = this._pop[g].Size;
                }

                //Person loop
                for (p = 0; p < pCount; p++)
                {
                    pZe = this._pop[g][p, "Update transmission 1"];

                    //Checks for list of partners
                    rCount = pZe.Partners.Count;
                    if (rCount > 0)
                    {
                        //Evaluate each partnership
                        for (pp = 0; pp < rCount; pp++)
                        {
                            //Checks if this relation has been visited this clock
                            tRel = pZe.Partners[pp];
                            if (tRel.Visited != this._clock)
                            {
                                pLia = tRel.ToPerson;
                                //Evaluate possibility of transmission
                                if ((pZe.STDStatus == ESTDStatus.Infected && pLia.STDStatus == ESTDStatus.Susceptable) ||
                                    (pZe.STDStatus == ESTDStatus.Susceptable && pLia.STDStatus == ESTDStatus.Infected))
                                {
                                    //Defines probability of safe sex
                                    if (tRel.Partnership == EPartners.Stable)
                                    {
                                        prSafe = pZe.MyGroup.StbSafeSex; //Internal partnership
                                    }
                                    else //Casual
                                    {
                                        prSafe = GMean(pZe.MyGroup.CslSafeSex, pLia.MyGroup.CslSafeSex);
                                    }//If prob. of safe sex

                                    //Check for safe sex practice
                                    if (!this._rnd.Bernoulli(prSafe))
                                    {
                                        //Define the number of sexual contacs
                                        if (tRel.Partnership == EPartners.Stable)
                                        {
                                            nContact = (int)this._rnd.Sample(pZe.MyGroup.StbContacts); //Internal
                                        }
                                        else //Casual
                                        {
                                            nContact = (int)GMean(this._rnd.Sample(pZe.MyGroup.CslContacts),
                                                this._rnd.Sample(pLia.MyGroup.CslContacts));
                                        }//Number of contacs

                                        //Define the transmission probability
                                        //Male => Female
                                        if ((pZe.Gender == EGender.Male && pZe.STDStatus == ESTDStatus.Infected &&
                                            pLia.Gender == EGender.Female) || (pLia.Gender == EGender.Male &&
                                            pLia.STDStatus == ESTDStatus.Infected && pZe.Gender == EGender.Female))
                                        {
                                            prTrans = this._info.Male2Female;
                                            idTrans = 1;
                                        }
                                        else if ((pZe.Gender == EGender.Female && pZe.STDStatus == ESTDStatus.Infected &&
                                            pLia.Gender == EGender.Male) || (pLia.Gender == EGender.Female &&
                                            pLia.STDStatus == ESTDStatus.Infected && pZe.Gender == EGender.Male))
                                        {
                                            //Female => Male
                                            prTrans = this._info.Female2Male;
                                            idTrans = 2;
                                        }
                                        else
                                        {
                                            //Male => Male
                                            prTrans = this._info.Male2Male;
                                            idTrans = 3;
                                        }//Prob. of transmission

                                        //Calculate the final probability Pr = 1 - ((1 - p)^n)
                                        prTrans = (1.0 - Math.Pow(1.0 - prTrans, nContact));

                                        //Evaluates the HIV transmission
                                        if (this._rnd.Bernoulli(prTrans))
                                        {

                                            //Update HIV infection
                                            if (pZe.STDStatus == ESTDStatus.Infected)
                                            {
                                                pLia.STDStatus = ESTDStatus.Infected;
                                                pLia.STDAge = 0;
                                                //Checks if STD can cause death
                                                if (this._rnd.Bernoulli(this._info.Mortality))
                                                {
                                                    pLia.STDDeath = (int)this._rnd.Sample(
                                                        this._info.LifeExpectancy);
                                                }

                                                //Sets the duration of infection for non lifelong STDs.
                                                if (!this._info.LifeInfection)
                                                {
                                                    pLia.STDDuration = (int)this._rnd.Sample(
                                                        this._info.STDDuration);
                                                }

                                                //Gets the group index for data collection
                                                gIdx = this._pop.IndexOf(pLia.MyGroup);
                                            }
                                            else //pLia => pZe
                                            {
                                                pZe.STDStatus = ESTDStatus.Infected;
                                                pZe.STDAge = 0;
                                                //Evaluates mortality from HIV infection
                                                if (this._rnd.Bernoulli(this._info.Mortality))
                                                {
                                                    pZe.STDDeath = (int)this._rnd.Sample(
                                                        this._info.LifeExpectancy);
                                                }

                                                //Gets the group index for data collection
                                                gIdx = this._pop.IndexOf(pZe.MyGroup);

                                            }// Update Infection

                                            //Update simulation data, if not warming up
                                            if (!this._warmup)
                                            {
                                                this._data[_run, _clock, gIdx].STDIncidence++;

                                                //Defines the HIV infection direction
                                                if (idTrans == 1) //Male => Female
                                                {
                                                    this._data[_run, _clock, gIdx].STDFemale++;
                                                    this._data[_run, _clock, gIdx].STDMale2Female++;
                                                }
                                                else if (idTrans == 2) //Female => Male
                                                {
                                                    this._data[_run, _clock, gIdx].STDMale++;
                                                    this._data[_run, _clock, gIdx].STDFemale2Male++;
                                                }
                                                else //Male => Male
                                                {
                                                    this._data[_run, _clock, gIdx].STDMale++;
                                                    this._data[_run, _clock, gIdx].STDMale2Male++;
                                                    this._data[_run, _clock, gIdx].STDHomosexual++;
                                                }//HIV Direcion

                                                //Defines the HIV infection scope
                                                if (tRel.Partnership == EPartners.Stable)
                                                {
                                                    this._data[_run, _clock, gIdx].STDStable++;
                                                    this._data[_run, _clock, gIdx].STDInternal++;
                                                }
                                                else //Causal
                                                {
                                                    this._data[_run, _clock, gIdx].STDCasual++;
                                                    if (pZe.MyGroup.Id == pLia.MyGroup.Id)
                                                    {
                                                        this._data[_run, _clock, gIdx].STDInternal++;
                                                    }
                                                    else
                                                    {
                                                        this._data[_run, _clock, gIdx].STDExternal++;
                                                    }
                                                }//HIV Scope
                                            }//update data
                                        }//HIV Transmission
                                    }//Safe sex
                                }//If transmission

                                //Mark Relations as visited for this clock
                                tRel.Visited = this._clock;
                                pLia.Partners[pZe].Visited = this._clock;
                            }//If visited
                        }//Partner loop
                    }//If partners
                }//Person loop
            }//Group loop
        }//Update Transmission

        #endregion //Simulation internal routines

        #region Partnership Utility Functions

        /// <summary>
        /// Test the preconditions for one to be available for partnership
        /// </summary>
        /// <param name="prs">The person to test</param>
        /// <param name="ptype">The type of partnership</param>
        /// <returns>True, if the person is available, false otherwise</returns>
        private bool AllowPartnership(Person prs, EPartners ptype)
        {
            //Checks the maximum number of concurrent precondition
            if (prs.Partners.Count >= prs.MyGroup.MaxConcurrent)
            {
                return false;
            }
            else if (prs.Partners.Count >= 1)
            {
                //Checks the probability of concurrent partnership precondition
                if (!this._rnd.Bernoulli(prs.MyGroup.PrConcurrent))
                {
                    return false;
                }
            }

            //Identify the kind of pratnership
            if (ptype == EPartners.Stable)
            {
                if (prs.Partnership == ERelation.Available)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else //Casual
            {
                return true;
            }
        }
        
        /// <summary>
        /// Evaluate if two selected individuals can stablesh a new partnership
        /// </summary>
        /// <param name="pFrom">
        /// The person who is looking for a new partner
        /// </param>
        /// <param name="pTo">
        /// The person who has been asked to a a partner
        /// </param>
        /// <param name="ptype">The proposed type of partnership</param>
        /// <returns>
        /// True, if a new partnership can be established, false otherwise
        /// </returns>
        private bool CanBePartners(Person pFrom, Person pTo, EPartners ptype)
        {
            /*===================================
             * Rule # 2 - pFrom must not be pTo
             *=================================*/
            if (pFrom == pTo)
            {
                return false;
            }

            /*=================================
             * Rule # 2 - Gender compatibility
             *===============================*/

            //Two females can't be partners (at least here)
            if ((pFrom.Gender == EGender.Female) &&
                (pTo.Gender == EGender.Female))
            {
                return false;
            }
            //Two males must be homosexual to be partners
            else if ((pFrom.Gender == pTo.Gender) && 
                     (!pFrom.Homosexual || !pTo.Homosexual))
            {
                return false;
            }
            else //Rule # 2 passed
            {
                /*================================================
                 * Rule # 3 - Are not they already partners
                 * =============================================*/
                if (pFrom.Partners.Contains(pTo))
                {
                    return false;
                }
                else //Rule #3 passed
                {
                    /*================================================
                    * Rule # 4 - Is the target looking for a partner?
                    *===============================================*/
                    if (this._rnd.Bernoulli(pTo.MyGroup.PrNewPartner))
                    { //Rule # 4 passed

                        /*====================================
                        * Rule # 5 - The type of partnership?
                        *===================================*/
                        if (ptype == EPartners.Stable)
                        {
                            if (this._rnd.Bernoulli(pTo.MyGroup.PrCasual))
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else //Casual
                        {
                            if (this._rnd.Bernoulli(pTo.MyGroup.PrCasual))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }//End rule # 5
                    }
                    else //Rule # 4 failed
                    {
                        return false;
                    }//End rule # 4
                }//End rule # 3
            }//End rule # 2
        }//End CanBePartners
        
        /// <summary>
        /// Searches for a casual partner within the same group
        /// </summary>
        /// <param name="prs">The individual who is looking for a partner</param>
        /// <param name="pIdx">The individual's index</param>
        /// <param name="gIdx">The individual group's index</param>
        /// <returns>True if a new partner is found, False otherwise</returns>
        private bool CasualInternalSearch(Person prs, int pIdx, int gIdx)
        {
            //Local variables
            int idx, pRnd;			//Loop index and random selected person index
            int maxTrial, pCount;	//The maximum number of trials and person counter
            Person pZe;				//Temporary person


            //Gets the population size and maximum number of trials
            pCount = prs.MyGroup.Size;
            maxTrial = prs.MyGroup.MaxTrials;

            //Start trials loop
            for (idx = 0; idx < maxTrial; idx++)
            {
                //Generates a random person
                do
                {
                    pRnd = (int)this._rnd.Uniform(0, pCount);
                }
                while (pRnd == pIdx);

                pZe = prs.MyGroup[pRnd, "Internal search 1"];

                //Tests if the selected person's can be a casual partner
                if (AllowPartnership(pZe, EPartners.Casual))
                {
                    //Checks if the target person can be a partner
                    if (CanBePartners(prs, pZe, EPartners.Casual))
                    {
                        if (CreatePartnership(prs, pZe, EPartners.Casual))
                        {
                            //Luck one, found a partner
                            return true;
                        }
                    }//If accepted, "pooling"
                }//Allow Partnership
            }//Random search loop

            //No luck, returns false
            return false;

        }//Casual internal search
        
        /// <summary>
        /// Searches for a casual partner within the same group
        /// </summary>
        /// <param name="prs">The individual who is looking for a partner</param>
        /// <param name="pIdx">The individual's index</param>
        /// <param name="gIdx">The individual group's index</param>
        /// <returns>True if a new partner is found, False otherwise</returns>
        private bool CasualExternalSearch(Person prs, int pIdx, int gIdx)
        {
            //Local Variables
            int idx, maxTrial;		//Loop index and maximum # of trials
            int nGrp, egIdx, pRnd;	//Number of groups, external group's index and random person's index
            double prSel, prSum;	//Selection probability and comulative sum of probabilities
            Group sGrp;			    //Selected group
            Person pZe;				//Temporary person

            //Set initial variables
            nGrp = this._pop.Count;
            prSel = this._rnd.NextDouble();
            prSum = 0.0;
            egIdx = 0;
            sGrp = null;

            //Tries to select a external group
            for (idx = 0; idx < nGrp; idx++)
            {
                //Checks if the selected index isn't the original group
                if (gIdx != idx)
                {
                    prSum += this._pop.AdjMatrix[gIdx, idx];
                    if (prSel <= prSum)
                    {
                        sGrp = this._pop[idx];
                        egIdx = idx;
                        break;
                    }
                }
            }

            //Check if an external group was selected
            if (sGrp != null)
            {
                //Calculates the maximun number of search trials
                maxTrial = sGrp.MaxTrials;
                if (maxTrial > sGrp.Size)
                {
                    maxTrial = sGrp.Size;
                }

                //Person loop
                for (idx = 0; idx < maxTrial; idx++)
                {
                    //Pick up a random individual
                    pRnd = (int)this._rnd.Uniform(0, sGrp.Size);
                    pZe = sGrp[pRnd, "External Search 1"];

                    //Are you looking for a new sexual partner?
                    if (this._rnd.Bernoulli(sGrp.PrNewPartner))
                    {
                        //Do you fancy a casual partner?
                        if (this._rnd.Bernoulli(sGrp.PrCasual))
                        {
                            //Test precondition for casual
                            if (AllowPartnership(pZe, EPartners.Casual))
                            {
                                //Can their be partners?
                                if (CanBePartners(prs, pZe, EPartners.Casual))
                                {
                                    //Create partnership
                                    if (CreatePartnership(prs, pZe, EPartners.Casual))
                                    {
                                        //Lock one, found a partner
                                        return true;
                                    }
                                }//Can be partners
                            }// Allow partnership
                        }//If accepts casual
                    }//If looking for new partner
                }//End person loop
            } // If group selected

            //No luck, returns false
            return false;
        }//Casual internal search

        /// <summary>
        /// Creates a new partnership between two individuals
        /// </summary>
        /// <param name="pFrom">The person who is proposing the partnership</param>
        /// <param name="pTo">The person who has accepted the proposed partnership</param>
        /// <param name="ptype">The type of partnership</param>
        /// <returns>True, if the partnership was created, false otherwise</returns>
        private bool CreatePartnership(Person pFrom, Person pTo, EPartners ptype)
        {
            //Local Variables
            Relation objRel;		//Temporarily hold a new partnership
            double frmLen, toLen;	//Duration of partnerships for pFrom and pTo
            int rLen;				//The duration of the new partnership
            int gIdx, egIdx;		//Indexes of the population groups
            bool casualExt = false;	//Identify an external casual partnerships

            //Population group index for origin person
            gIdx = this._pop.IndexOf(pFrom.MyGroup);

            //What is the type of Partnership?;
            if (ptype == EPartners.Casual)
            {
                //Casual Partnership. Checks if internal or external
                if (pFrom.MyGroup.Id == pTo.MyGroup.Id)
                {
                    //Internal partnership
                    rLen = this._clock + (int)this._rnd.Sample(pFrom.MyGroup.CslDuration);

                    //Target person's group index
                    egIdx = gIdx;
                }
                else //External
                {
                    //External partnership
                    casualExt = true;
                    frmLen = this._rnd.Sample(pFrom.MyGroup.CslDuration);
                    toLen = this._rnd.Sample(pTo.MyGroup.CslDuration);
                    rLen = this._clock + (int)this.GMean(frmLen, toLen);

                    //Target person's group index
                    egIdx = this._pop.IndexOf(pTo.MyGroup);
                }

                //Creates the Relation object    
                objRel = new Relation(pTo, EPartners.Casual, rLen);
            }
            else //Stable, internal
            {
                //Internal partnership
                rLen = this._clock + (int)this._rnd.Sample(pFrom.MyGroup.StbDuration);

                //Creates the Relation object    
                objRel = new Relation(pTo, EPartners.Stable, rLen);

                //Target person's group index
                egIdx = gIdx;

            }//Duration of partnership

            //Creates the new partnership
            if (pFrom.AddPartnership(objRel))
            {
                //Set the partnership status
                if (ptype == EPartners.Stable)
                {
                    pFrom.Partnership = ERelation.Engaged;
                    pTo.Partnership = ERelation.Engaged;
                }

                //Update simulation results dataset
                if (!this._warmup)
                {
                    //Update stable, internal partners
                    if (ptype == EPartners.Stable)
                    {
                        this._data[this._run, this._clock, gIdx].Partners += 2;
                        this._data[this._run, this._clock, gIdx].Stable += 2;
                    }
                    else //Casual internal or external partners
                    {
                        if (casualExt) //External
                        {
                            //Internal group 
                            this._data[this._run, this._clock, gIdx].Partners++;
                            this._data[this._run, this._clock, gIdx].Casual++;
                            this._data[this._run, this._clock, gIdx].External++;

                            //External group
                            this._data[this._run, this._clock, egIdx].Partners++;
                            this._data[this._run, this._clock, egIdx].Casual++;
                            this._data[this._run, this._clock, egIdx].External++;
                        }
                        else //Internal
                        {
                            this._data[this._run, this._clock, gIdx].Partners += 2;
                            this._data[this._run, this._clock, gIdx].Casual += 2;
                            this._data[this._run, this._clock, gIdx].Internal += 2;
                        }
                    }//Type of partnership

                    //Concurrent partnerships
                    if (pFrom.Partners.Count > 1)
                    {
                        this._data[this._run, this._clock, gIdx].Concurrent++;
                    }
                    if (pTo.Partners.Count > 1)
                    {
                        this._data[this._run, this._clock, egIdx].Concurrent++;
                    }
                }//If not warming-up

                return true;
            }
            else
            {
                return false;
            }//If add partner

        }// Create partnership

        /// <summary>
        /// Search the list of acquaintances of a given individual for a new 
        /// partner or friend up to three degrees of separation.
        /// </summary>
        /// <param name="prs">The individual who list is to be searched</param>
        /// <param name="partner">
        /// True for partnership or False for friendship
        /// </param>
        /// <returns>The person reference, if found, or null</returns>
        private Person SearchAcquaintances(Person prs, bool partner)
        {
            //Local variables
            int maxDegree, trials, count, i, j;
            Person d1Prs, d2Prs, d3Prs;
            RPrsArray ngb;
            PQueue q, d3q;

            //Defines the maximum searching distance
            maxDegree = prs.MyGroup.Degrees;

            //Setup
            this._idxVisit = this.Mod(this._idxVisit + 1, int.MaxValue);
            prs.Visited = this._idxVisit;
            trials = prs.MyGroup.MaxTrials;
            count = trials;

            //Verify the friends list and degrees of separation
            if (prs.Friends.Count < 1 || maxDegree < 1)
            {
                return null;
            }

            //Get the list of neighbours
            ngb = new RPrsArray(prs.Friends.ToArray(), this._rnd);
            if (count > ngb.Count)
            {
                count = ngb.Count;
            }

            //Create queues
            q = new PQueue();
            d3q = new PQueue();

            //Defines what to search
            if (partner)
            {
                #region Search for a partner
                //Degree #1 search
                for (i = 0; i < count; i++)
                {
                    d1Prs = ngb.Next();
                    d1Prs.Visited = this._idxVisit;

                    //Are you looking for a new sexual partner?
                    if (this._rnd.Bernoulli(d1Prs.MyGroup.PrNewPartner))
                    {
                        //Do you fancy a stable partner?
                        if (!this._rnd.Bernoulli(d1Prs.MyGroup.PrCasual))
                        {
                            //Test precondition for stable
                            if (AllowPartnership(d1Prs, EPartners.Stable))
                            {
                                //Can they be partners?
                                if (CanBePartners(prs, d1Prs, EPartners.Stable))
                                {
                                    return d1Prs;
                                }//Can be partners
                            }// Allow partnership
                        }//If accepts casual
                    }//If looking for new partner

                    q.Enqueue(d1Prs);
                }//End degree # 1

                //Degree #2 search
                if (maxDegree > 1)
                {
                    while (!q.IsEmpty())
                    {
                        d1Prs = q.Dequeue();
                        if (d1Prs.Friends.Count > 0)
                        {
                            ngb = new RPrsArray(d1Prs.Friends.ToArray(), this._rnd);
                            count = trials;
                            if (count > ngb.Count)
                            {
                                count = ngb.Count;
                            }
                            for (j = 0; j < count; j++)
                            {
                                d2Prs = ngb.Next();
                                if (d2Prs.Visited != this._idxVisit)
                                {
                                    d2Prs.Visited = this._idxVisit;
                                    //Are you looking for a new sexual partner?
                                    if (this._rnd.Bernoulli(d2Prs.MyGroup.PrNewPartner))
                                    {
                                        //Do you fancy a stable partner?
                                        if (!this._rnd.Bernoulli(d2Prs.MyGroup.PrCasual))
                                        {
                                            //Test precondition for stable
                                            if (AllowPartnership(d2Prs, EPartners.Stable))
                                            {
                                                //Can they be partners?
                                                if (CanBePartners(prs, d2Prs, EPartners.Stable))
                                                {
                                                    return d2Prs;
                                                }//Can be partners
                                            }// Allow partnership
                                        }//If accepts casual
                                    }//If looking for new partner

                                    //Add d2Prs to degree #3 queue
                                    d3q.Enqueue(d2Prs);
                                }//If visited
                            }//For size
                        }//If count
                    }//while no empty
                }//End degree #2

                //Degree #3 search
                if (maxDegree > 2)
                {
                    while (!d3q.IsEmpty())
                    {
                        d2Prs = d3q.Dequeue();
                        if (d2Prs.Friends.Count > 0)
                        {
                            ngb = new RPrsArray(d2Prs.Friends.ToArray(), this._rnd);
                            count = trials;
                            if (count > ngb.Count)
                            {
                                count = ngb.Count;
                            }
                            for (j = 0; j < count; j++)
                            {
                                d3Prs = ngb.Next();
                                if (d3Prs.Visited != this._idxVisit)
                                {
                                    d3Prs.Visited = this._idxVisit;

                                    //Are you looking for a new sexual partner?
                                    if (this._rnd.Bernoulli(d3Prs.MyGroup.PrNewPartner))
                                    {
                                        //Do you fancy a stable partner?
                                        if (!this._rnd.Bernoulli(d3Prs.MyGroup.PrCasual))
                                        {
                                            //Test precondition for stable
                                            if (AllowPartnership(d3Prs, EPartners.Stable))
                                            {
                                                //Can they be partners?
                                                if (CanBePartners(prs, d3Prs, EPartners.Stable))
                                                {
                                                    return d3Prs;
                                                }//Can be partners
                                            }// Allow partnership
                                        }//If accepts casual
                                    }//If looking for new partner
                                }//If visited
                            }//For size
                        }//If friend
                    }//while not empty
                }//End degree #3

                //Not found
                return null;
                #endregion //Search for partner
            }
            else //Friend
            {
                #region Search for a friend

                //Create degree #1 queue
                for (i = 0; i < count; i++)
                {
                    d1Prs = ngb.Next();
                    d1Prs.Visited = this._idxVisit;
                    q.Enqueue(d1Prs);
                }

                //Search degree #1 => 2 for a friend
                while (!q.IsEmpty())
                {
                    d1Prs = q.Dequeue();
                    if (d1Prs.Friends.Count > 0)
                    {
                        ngb = new RPrsArray(d1Prs.Friends.ToArray(), this._rnd);
                        count = trials;
                        if (count > ngb.Count)
                        {
                            count = ngb.Count;
                        }
                        for (j = 0; j < count; j++)
                        {
                            d2Prs = ngb.Next();
                            if (d2Prs.Visited != this._idxVisit)
                            {
                                //Set the visited index
                                d2Prs.Visited = this._idxVisit;

                                //Checks the more friends precondition
                                if (MoreFriends(d2Prs))
                                {
                                    //Checks if d2Prs is already a friend of prs
                                    if (!prs.Friends.Contains(d2Prs))
                                    {
                                        return d2Prs;
                                    }
                                }//More friends

                                //Add d2Prs to degree #3 queue
                                d3q.Enqueue(d2Prs);

                            }//If visited
                        }//For count
                    }//If count
                }//While no empty

                //Search degree #2 => 3 for a friend
                if (maxDegree > 1)
                {
                    while (!d3q.IsEmpty())
                    {
                        d2Prs = d3q.Dequeue();
                        if (d2Prs.Friends.Count > 0)
                        {
                            ngb = new RPrsArray(d2Prs.Friends.ToArray(), this._rnd);
                            count = trials;
                            if (count > ngb.Count)
                            {
                                count = ngb.Count;
                            }
                            for (j = 0; j < count; j++)
                            {
                                d3Prs = ngb.Next();
                                if (d3Prs.Visited != this._idxVisit)
                                {
                                    //Set the visited index
                                    d3Prs.Visited = this._idxVisit;

                                    //Checks the more friends precondition
                                    if (MoreFriends(d3Prs))
                                    {
                                        //Checks if d2Prs is already a friend of prs
                                        if (!prs.Friends.Contains(d3Prs))
                                        {
                                            return d3Prs;
                                        }
                                    }//More friends
                                }//If visited
                            }//For count
                        }//If friend
                    }//while not empty
                }//End degree #3

                #endregion //Search for a friend

                //Not found
                return null;
            }//If partner/friend
        }//End of SearchAcquaintances

        #endregion //Partnership Utility Functions

        #region Simulation utility Functions

        /// <summary>
        /// Starts the initial HIV epidemic according to each group's prevalence
        /// </summary>
        private void InitialiseSTDInfection()
        {
            int g, p, gCount, pCount, iCount, done;
            Group grp;
            Person prs;
            Point3d pnt;
            double DC, dist;
            RIntArray rndIdx;		//Array of random indexes

            //Add STD Infection to population
            gCount = this._pop.Count;
            for (g = 0; g < gCount; g++)
            {
                grp = this._pop[g];
                iCount = (int)(grp.Size * grp.STDPrevalence);
                done = 0;
                if (!this._uinfect && grp.Topology == ETopology.Circle)
                {
                    #region Circle

                    //Gets the index of the starting node
                    done = (int)this._rnd.Uniform(0, grp.Size);
                    for (p = 0; p < iCount; p++)
                    {
                        prs = grp[done, "Initial STD"];

                        prs.STDStatus = ESTDStatus.Infected;
                        prs.STDAge = (int)this._rnd.Sample(grp.STDAge);

                        //Checks if the STD can cause death
                        if (this._rnd.Bernoulli(this._info.Mortality))
                        {
                            prs.STDDeath = (int)this._rnd.Sample(this._info.LifeExpectancy);
                            if (prs.STDAge >= prs.STDDeath)
                            {
                                prs.STDAge = 0;
                            }
                        }//STD death

                        //Sets the duration of infection for non lifelong STDs.
                        if (!this._info.LifeInfection)
                        {
                            prs.STDDuration = (int)this._rnd.Sample(this._info.STDDuration);
                            if (prs.STDAge >= prs.STDDuration)
                            {
                                prs.STDDuration += prs.STDAge;
                            }
                        }//STD duration

                        done++;
                        //Next index
                        done = this.Mod(done, grp.Size);
                    }//For iCount

                    #endregion //Circle
                }
                else if (!this._uinfect && grp.Topology == ETopology.Sphere)
                {
                    #region Sphere
                    //Gets the index of the starting node
                    pnt = grp[(int)this._rnd.Uniform(0, grp.Size), "Initial STD"].Location;

                    //Defines the searching distance
                    dist = Simulation.SphericalCap(grp.Size, iCount, grp.Radius);
                    if (dist < grp.Distance)
                    {
                        dist = grp.Distance;
                    }

                    DC = Math.Cos(dist / grp.Radius);
                    pCount = grp.Size;
                    for (p = 0; p < pCount; p++)
                    {
                        prs = grp[p, "Initial STD"];
                        //Checks geographical distance
                        if (Point3d.Dot3d(pnt, prs.Location) >= DC)
                        {
                            prs.STDStatus = ESTDStatus.Infected;
                            prs.STDAge = (int)this._rnd.Sample(grp.STDAge);

                            //Checks if the STD can cause death
                            if (this._rnd.Bernoulli(this._info.Mortality))
                            {
                                prs.STDDeath = (int)this._rnd.Sample(this._info.LifeExpectancy);
                                if (prs.STDAge >= prs.STDDeath)
                                {
                                    prs.STDAge = 0;
                                }
                            }//STD death

                            //Sets the duration of infection for non lifelong STDs.
                            if (!this._info.LifeInfection)
                            {
                                prs.STDDuration = (int)this._rnd.Sample(this._info.STDDuration);
                                if (prs.STDAge >= prs.STDDuration)
                                {
                                    prs.STDDuration += prs.STDAge;
                                }
                            }//STD duration

                            done++;
                            if (done >= iCount)
                            {
                                break;
                            }
                        }//If in the neighbourhood
                    }//For pCount
                    #endregion //Sphere
                }
                else
                {
                    #region Free
                    //Setup the temporary index array
                    rndIdx = new RIntArray(grp.Size, this._rnd);

                    //Chooses a random starting person
                    done = rndIdx.Next();
                    for (p = 0; p < iCount; p++)
                    {
                        prs = this._pop[g][done, "Traditional Warm-up"];
                        //STD Infection
                        prs.STDStatus = ESTDStatus.Infected;
                        prs.STDAge = (int)this._rnd.Sample(grp.STDAge);

                        //Checks if the STD can cause death
                        if (this._rnd.Bernoulli(this._info.Mortality))
                        {
                            prs.STDDeath = (int)this._rnd.Sample(this._info.LifeExpectancy);
                            if (prs.STDAge >= prs.STDDeath)
                            {
                                prs.STDAge = 0;
                            }
                        }//STD death

                        //Sets the duration of infection for non lifelong STDs.
                        if (!this._info.LifeInfection)
                        {
                            prs.STDDuration = (int)this._rnd.Sample(this._info.STDDuration);
                            if (prs.STDAge >= prs.STDDuration)
                            {
                                prs.STDDuration += prs.STDAge;
                            }
                        }//STD duration

                        done = rndIdx.Next();
                    }//iCount loop

                    #endregion //Free
                }//IF Topology
            }//Group's loop*/
        }//InitialiseSTDInfection

        /// <summary>
        /// Clear the simulation and/or population run-time data from memory
        /// </summary>
        /// <param name="result">If true clears both the simulation and population
        /// run-time data, otherwise, clears only the population run-time data.
        /// </param>
        private void ClearData(bool result)
        {
            for (int i = 0; i < this._pop.Count; i++)
            {
                this._pop[i].ClearPopulation();
            }

            //Clear the simulation results.
            if (result) 
            {
                this._data = null;
            }

            GC.Collect();
        } // end ClearData
        
        /// <summary>
        /// Updates the STD prevalence for all groups
        /// </summary>
        private double UpdateSTDPrevalence()
        {
            //Local Variables
            int g, gCount;			//Group indexer and counter
            double theSum = 0.0;	//Sum of STD prevalences 

            gCount = this._pop.Count;
            for (g = 0; g < gCount; g++)
            {
                this._pop[g].STDPrevalence = CalcPrevalence(this._pop[g]);
                theSum += this._pop[g].STDPrevalence;

                //Update data
                if (!this._warmup)
                {
                    this._data[_run, _clock, g].STDPrevalence = this._pop[g].STDPrevalence;
                }
            }

            //Evaluates the return value
            if (gCount > 0)
            {
                return theSum / gCount;
            }
            else
            {
                return theSum;
            }
        }
        
        /// <summary>
        /// Calculates the STD prevalence within a given group.
        /// </summary>
        /// <param name="grp">The group to calculate the STD prevalence.</param>
        private double CalcPrevalence(Group grp)
        {
            //Local variables
            int p, pCount;		//Person indexer and counter
            int stdCount = 0;	//STD infected counter

            pCount = grp.Size;
            for (p = 0; p < pCount; p++)
            {
                if (grp[p, "Calc Prevalence"].STDStatus == ESTDStatus.Infected)
                {
                    stdCount++;
                }
            }

            //Evaluates the results
            if (pCount > 0)
            {
                return (double)stdCount / pCount;
            }
            else
            {
                return 0.0;
            }
        }
        
        /// <summary>
        /// Collects simulation clock zero data
        /// </summary>
        private void ClockZeroData()
        {
            //Local variables
            int g, p, pp, gIdx;	//Group and person indexers
            int gCount, pCount;	//Group and person counters
            Person pZe;		//Temporary person
            int lCount;			//List items counter
            Relation rTmp;		//Temporary partnership

            //Checks for clock zero
            if (this._clock != 0)
            {
                return;
            }

            //Start data collection
            gCount = this._pop.Count;
            for (g = 0; g < gCount; g++)
            {
                //Simulation clock
                this._data[this._run, this._clock, g].Date = this.ClockToDate;

                //Population size
                pCount = this._pop[g].Size;
                this._data[this._run, this._clock, g].Size = pCount;

                //Person's loop
                for (p = 0; p < pCount; p++)
                {
                    pZe = this._pop[g][p, "Clock Zero Data 1"];

                    //Gender data
                    if (pZe.Gender == EGender.Female)
                    {
                        this._data[this._run, this._clock, g].Female++;
                    }
                    else //Male
                    {
                        this._data[this._run, this._clock, g].Male++;
                        if (pZe.Homosexual)
                        {
                            this._data[this._run, this._clock, g].Homosexual++;
                        }
                    }//Gender

                    //Update partnerships
                    lCount = pZe.Partners.Count;

                    //Concurrent partnerships
                    if (lCount > 1)
                    {
                        this._data[this._run, this._clock, g].Concurrent++;
                    }

                    //Partnerships loop
                    for (pp = 0; pp < lCount; pp++)
                    {
                        rTmp = pZe.Partners[pp];

                        //Checks if this partnership has not been visited
                        if (rTmp.Visited != this._clock)
                        {
                            //Sets the visited status
                            rTmp.Visited = this._clock;
                            rTmp.ToPerson.Partners[pZe].Visited = this._clock;

                            if (rTmp.Partnership == EPartners.Stable)
                            {
                                //Update dataset
                                this._data[this._run, this._clock, g].Partners += 2;
                                this._data[this._run, this._clock, g].Stable += 2;
                                this._data[this._run, this._clock, g].Internal += 2;
                            }
                            else
                            {
                                //Internal partnership
                                if (rTmp.ToPerson.MyGroup.Id == pZe.MyGroup.Id)
                                {
                                    this._data[this._run, this._clock, g].Partners += 2;
                                    this._data[this._run, this._clock, g].Casual += 2;
                                    this._data[this._run, this._clock, g].Internal += 2;
                                }
                                else
                                {
                                    this._data[this._run, this._clock, g].Partners++;
                                    this._data[this._run, this._clock, g].Casual++;
                                    this._data[this._run, this._clock, g].External++;

                                    gIdx = this._pop.IndexOf(rTmp.ToPerson.MyGroup);
                                    this._data[this._run, this._clock, gIdx].Partners++;
                                    this._data[this._run, this._clock, gIdx].Casual++;
                                    this._data[this._run, this._clock, gIdx].External++;
                                }//Scope
                            }//Type of partnership
                        }//Visited
                    }//Partnership loop

                    //HIV infection
                    this._data[this._run, this._clock, g].STDPrevalence = this.CalcPrevalence(this._pop[g]);
                    if (pZe.STDStatus == ESTDStatus.Infected)
                    {
                        this._data[this._run, this._clock, g].STDIncidence++;
                        if (pZe.Gender == EGender.Female)
                        {
                            this._data[this._run, this._clock, g].STDFemale++;
                        }
                        else
                        {
                            this._data[this._run, this._clock, g].STDMale++;
                            if (pZe.Homosexual)
                            {
                                this._data[this._run, this._clock, g].STDHomosexual++;
                            }
                        }//Gender
                    }//HIV infection
                }//Person loop
            }//Group loop
        }
        
        /// <summary>
        /// Wait for the user to continue the simulaton run.
        /// </summary>
        private void WaitToContinue()
        {
            lock (this)
            {
                while (this._waiting)
                {
                    Monitor.Wait(this, 1000);
                }

                if (this._status == ESimStatus.Stepping)
                {
                    this._waiting = true;
                }
            }
        } 

        /// <summary>
        /// Calculate the geometric mean of two real numbers
        /// </summary>
        /// <param name="x1">The fir</param>
        /// <param name="x2"></param>
        /// <returns>The geometric mean of the two numbers</returns>
        public double GMean(double x1, double x2)
        {
            if (x1 >= 0.0 && x2 >= 0.0)
            {
                return Math.Sqrt(x1 * x2);
            }
            else
            {
                throw new SimulationException(
                    "GMean invalid parameters [x1 < 0.0 or x2 < 0.0].");
            }
        }

        /// <summary>
        /// Conditional test for adding new friends to one's book
        /// </summary>
        /// <param name="pZe">
        /// The <see cref="Person"/> who wants a new friend.
        /// </param>
        /// <returns>
        /// True, if it's allowed to add a new friend, false otherwise.
        /// </returns>
        private bool MoreFriends(Person pZe)
        {
            double fx = Math.Log(pZe.MyGroup.Size) * 
                        Math.Exp(-pZe.MyGroup.Alpha * pZe.Friends.Count);
            if (this._rnd.NextDouble() <= fx)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Custom Mod function. Divide two numbers and return only the remainder, 
        /// which has the same sign as the divisor.
        /// </summary>
        /// <param name="number">
        /// The number for which you want to find the remainder.
        /// </param>
        /// <param name="divisor">
        /// The number by which you want to divide number.
        /// </param>
        /// <returns>
        /// The remainder after number is divided by divisor.
        /// </returns>
        public int Mod(double number, double divisor)
        {
            try
            {
                //MOD(n, d) = n - d*INT(n/d)
                return (int)(number - divisor * Math.Floor(number / divisor));
            }
            catch (Exception err)
            {
                throw new SimulationException(err.Message);
            }
        }

        /// <summary>
        /// Calculates the network characteristics
        /// </summary>
        public void CalculateNetwork()
        {
            int g, gCount, p, pCount, i, lsize;
            Group grp;
            Person prs;

            gCount = this._pop.Count;
            for (g = 0; g < gCount; g++)
            {
                grp = this._pop[g];

                //Finish all partnerships
                pCount = grp.Size;
                for (p = 0; p < pCount; p++)
                {
                    lsize = grp[p, "Partner list size"].Partners.Count;
                    for (i = 0; i < lsize; i++)
                    {
                        prs = grp[p, "Partner list size"].Partners[0].ToPerson;
                        grp[p, "Partner list size"].EndPartnership(prs);
                    }
                }//Person's loop

                //Defines the network efficiency calculation
                if (grp.Size <= this._plnumeric) //Numerically
                {
                    this._netdata[this._run, g] = grp.SWNProperties(this._plalgo);
                }
                else //Estimates
                {
                    this._netdata[this._run, g] = grp.SWNProperties(this._plalgo, false, this._plsample);
                }
            }
        }
        
        /// <summary>
        /// Calculate the spherical cap defining a neighbourhood on 
        /// the surface of a unit sphere 
        /// </summary>
        /// <param name="popSize">The size of the population.</param>
        /// <param name="neighbors">
        /// The expected number of neighbours within the neighbourhood.
        /// </param>
        /// <param name="wradius">The real world radius to transform.</param>
        /// <returns>The spherical distance from a reference point</returns>
        public static double SphericalCap(int popSize, int neighbors, double wradius)
        {
            double sarea, scap, retval;

            sarea = 4.0 * Math.PI;						    //Area of the unit sphere
            scap = (sarea * neighbors) / (double)popSize;	//Spherical cap

            retval = Math.Acos(((2.0 * Math.PI) - scap) / (2.0 * Math.PI)) * wradius;
            return retval;
        }
        
        #endregion //Utility Functions

        #endregion //Private methods

        #region Raise Events

        /// <summary>
        /// Fires simulation events
        /// </summary>
        /// <param name="dlg">The event to fire</param>
        /// <param name="args">The event arguments</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void FireSimEvent(Delegate dlg, params object[] args)
        {
            //Checks for subscribers
            if (dlg == null)
            {
                return;
            }

            //Get subscribers
            Delegate[] subscribers = dlg.GetInvocationList();
            ISynchronizeInvoke synchronizer;

            //Event subscribers loop
            foreach (Delegate sink in subscribers)
            {
                try
                {
                    //Checks for thread affinity
                    synchronizer = sink.Target as ISynchronizeInvoke;
                    if (synchronizer != null)
                    {
                        if (synchronizer.InvokeRequired)
                        {
                            synchronizer.Invoke(sink, args);
                            continue;
                        }
                    }

                    //Not requiring thread affinity or invoke is not required
                    sink.DynamicInvoke(args);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    //TODO: Log error
                }
            }
        }

        /// <summary>
        /// Fires simulation events
        /// </summary>
        /// <param name="dlg">The event to fire</param>
        private void FireSimEvent(Delegate dlg)
        {
            this.FireSimEvent(dlg, this, EventArgs.Empty);
        }

        #endregion
    }//Simulation
}
