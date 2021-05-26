using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DatabaseTest
{
    public partial class DatabaseTest : ServiceBase
    {
        #region Declaration of instances
        //defined to count all the times the service repeats an action.
        private int eventId = 1;
        //defined to set the timer that the service uses.
        Timer timer;
        //defined to log events that occur in the service.
        EventLog eventLog;
        //defined to set the source of the service and where the logs will be showing.
        const string source = "DatabaseTestLog";
        const string log = "Application";
        #endregion

        /// <summary>
        /// Initialize a new instance of the DatabaseTest class.
        /// </summary>
        public DatabaseTest()
        {
            InitializeComponent();

            eventLog = new EventLog();

            //Checks if the event source exist in the local computer.
            if (!EventLog.SourceExists(source))
            {
                //Creates a new custom log in the computer.
                EventLog.CreateEventSource(source, log);
            }

            //Sets the source name to register and use when writing the event log.
            eventLog.Source = source;

            //Sets the name of the log to write to.
            eventLog.Log = log;
        }

        /// <summary>
        /// Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                //This timer calls the OnTimer method every 30 seconds.
                timer = new Timer();
                timer.Interval = 30000; // 30 seconds
                timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
                timer.Start();
            }

            catch (Exception ex)
            {
                //log event in case of error.
                eventLog.WriteEntry($"Error: {ex.Message}. Found at: {ex.StackTrace}", EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// Specifies actions to take when the service stops.
        /// </summary>
        protected override void OnStop()
        {
            //log event.
            eventLog.WriteEntry("In OnStop.", EventLogEntryType.Warning);
        }

        /// <summary>
        /// This method is in charge of calling the UpdateDatabase method and log the event everytime it repeats itself.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                DatabaseTestService databaseTestService = new DatabaseTestService();

                var time = DateTime.Now;

                eventLog.WriteEntry("Checking database for updates... at time: " + time.ToString(), EventLogEntryType.Information, eventId++);

                databaseTestService.UpdateDatabase();                
            }

            catch (Exception ex)
            {
                //log event in case of error.
                eventLog.WriteEntry(ex.Message, EventLogEntryType.Error, 1);
            }
        }
    }
}
