using System;
using System.Collections.Generic;
using System.IO;
using GameTuner.Framework.Properties;

namespace GameTuner.Framework
{
    /// <summary>
    /// Stub implementation of SourceControl that provides no-op functionality
    /// to replace P4API dependency. All methods return safe defaults.
    /// </summary>
    public class SourceControl : ISourceControl
    {
        private class SourceControlLabel : ISourceControlLabel
        {
            public string Name { get; private set; }
            public DateTime Date { get; private set; }

            public SourceControlLabel(string name, DateTime date)
            {
                Name = name;
                Date = date;
            }
        }

        private string password;
        private string client;

        public string Port { get; set; }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string Client
        {
            get { return client; }
            set { client = value; }
        }

        public bool IsConnected
        {
            get { return false; } // Always return false since we have no real connection
        }

        public SourceControl() : this("", "")
        {
        }

        public SourceControl(string client) : this(client, "")
        {
        }

        public SourceControl(string client, string password)
        {
            this.client = client;
            this.password = password;
            Port = ""; // No default port since we're not connecting
        }

        public void Submit(string description, string[] files)
        {
            // No-op: Source control submission disabled
            // In a real implementation, this could log the attempt or show a message
        }

        public string GetLocalPathFromDepot(string depot)
        {
            // Return the depot path as-is since we can't resolve it
            return depot;
        }

        public bool IsSourceControlPath(string szPath)
        {
            // Return false since we have no source control integration
            return false;
        }

        public List<ISourceControlLabel> CollectLabels()
        {
            // Return empty list since we have no source control connection
            return new List<ISourceControlLabel>();
        }

        public List<ISourceControlLabel> CollectLabels(string pattern)
        {
            // Return empty list since we have no source control connection
            return new List<ISourceControlLabel>();
        }

        /// <summary>
        /// Stub method for P4API error handling - no-op since we don't use P4API
        /// </summary>
        public static void ThrowExecptionOnError(string command, object result)
        {
            // No-op: P4API error handling disabled
        }
    }
}