using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ContinuousPing
{
    public partial class ContinuousPing : ServiceBase
    {

        System.Collections.Specialized.OrderedDictionary PingResults;
        List<String> Hosts;

        String _BaseDir = @"C:\ContinuousPing\";
        System.Timers.Timer _PingTimer;

        public ContinuousPing()
        {
            InitializeComponent();
            if (!EventLog.SourceExists("ContinuousPing"))
            {
                EventLog.CreateEventSource("ContinuousPing", "Application");
            }
            Hosts = new List<string>();
            PingResults = new System.Collections.Specialized.OrderedDictionary();
            PingResults.Add("Timestamp", "");
            String _line = "";
            try
            {
                System.IO.FileInfo _testFile = new System.IO.FileInfo(_BaseDir + "Hosts.txt");
                using (System.IO.StreamReader file = new System.IO.StreamReader(_BaseDir + "Hosts.txt"))
                {
                    while ((_line = file.ReadLine()) != null)
                    {
                        Hosts.Add(_line);
                        PingResults.Add(_line, "");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger(ex.ToString());
                throw;
            }

            _PingTimer = new System.Timers.Timer();
            _PingTimer.Elapsed += OnTimedEvent;
            _PingTimer.Interval = 1000;
            _PingTimer.AutoReset = true;
            _PingTimer.Start();
        }

        protected override void OnStart(string[] args)
        {            
        }

        protected override void OnStop()
        {
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            PingResults["Timestamp"] = System.DateTime.Now.ToString("T");
            foreach (String _entry in Hosts)
            {
                Double _replyTime = 1001;
                try
                {
                    using (Ping _ping = new Ping())
                    {
                        PingReply _reply = _ping.Send(_entry);
                        if (_reply.Status == IPStatus.Success) { _replyTime = _reply.RoundtripTime; }
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger(ex.ToString());
                }
                PingResults[_entry] = _replyTime.ToString();
            }
            CSVWriter();
        }

        private void CSVWriter()
        {
            String _TracePath = _BaseDir + @"Traces\" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
            String _LogPath = _BaseDir + @"Logs\";
            StringBuilder _HeaderLine = new StringBuilder();
            StringBuilder _CSVLine = new StringBuilder();
            Boolean _FileExists = System.IO.File.Exists(_TracePath);
            _HeaderLine.Append("\"");
            _CSVLine.Append("\"");

            foreach (System.Collections.DictionaryEntry _entry in PingResults)
            {
                _HeaderLine.Append(_entry.Key + "\",\"");
                _CSVLine.Append(_entry.Value + "\",\"");
            }
            _HeaderLine.Append("\"");
            _CSVLine.Append("\"");

            System.IO.Directory.CreateDirectory(_BaseDir + @"Traces\");
            using (System.IO.StreamWriter LogFile = new System.IO.StreamWriter(_TracePath, true))
            {
                try
                {
                    if (!_FileExists) {LogFile.WriteLine(_HeaderLine.ToString());}
                    LogFile.WriteLine(_CSVLine.ToString());
                }
                catch (Exception ex)
                {
                    ErrorLogger(ex.ToString());
                }
            }
        }   

        private void ErrorLogger(String Message)
        {
            EventLog.WriteEntry("ContinuousPing", Message, EventLogEntryType.Error);
        }
    }
}
