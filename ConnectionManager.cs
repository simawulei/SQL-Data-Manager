namespace SQLDM
{
    using System.Collections.Generic;
    using System.Data;

    public sealed class ConnectionManager
    {

        #region Singleton Stuff - ensure this class can only ever have one instance

        static private volatile ConnectionManager _instance;
        static private object _lock = new object();

        private ConnectionManager() { }

        /// <summary>
        /// Gets the instance of a ConnectionManager
        /// </summary>
        /// <returns></returns>
        public static ConnectionManager GetConnectionManager()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = new ConnectionManager();
                }
            }
            return _instance;
        }

        #endregion

        public List<SqlEnvironment> EnvironmentList { get; set; } = new List<SqlEnvironment>();

        /// <summary>
        /// Gets the database connection string for the current environment
        /// </summary>
        /// <returns></returns>
        public string ConnectionString()
        {
            return GetEnvironment().ConnectionString;
        }

        /// <summary>
        /// Gets the connection string for the given environment
        /// </summary>
        /// <param name="environmentName">The name of the environment to get the connection string for</param>
        /// <returns></returns>
        public string ConnectionString(string environmentName)
        {
            return GetEnvironment(environmentName).ConnectionString;
        }

        /// <summary>
        /// Gets the current environment
        /// </summary>
        /// <returns></returns>
        public SqlEnvironment GetEnvironment()
        {
            foreach(SqlEnvironment e in EnvironmentList)
            {
                if(e.IsCurrent) { return e; }
            }
            throw new DataException("Cannot find environment");
        }

        /// <summary>
        /// Gets the requested environment
        /// </summary>
        /// <param name="environmentName">The name of th environment to get</param>
        /// <returns></returns>
        public SqlEnvironment GetEnvironment(string environmentName)
        {
            foreach (SqlEnvironment e in EnvironmentList)
            {
                if (e.EnvironmentName == environmentName) { return e; }
            }
            throw new DataException("Cannot find environment");
        }

        /// <summary>
        /// Adds a new environment into the system
        /// </summary>
        /// <param name="environment"></param>
        public void Add(SqlEnvironment environment)
        {
            if (!EnvironmentList.Contains(environment))
            {
                string n = environment.EnvironmentName;
                foreach(SqlEnvironment e in EnvironmentList)
                {
                    if(e.EnvironmentName == n) { throw new DataException("Environment " + n + " already exists"); }
                }

                EnvironmentList.Add(environment);
                if(environment.IsCurrent) {
                    SetCurrentEnvironment(environment.EnvironmentName);
                    SQLDMGlobal.Initialise();
                }
            }
        }

        /// <summary>
        /// Sets the specified environment to be the current one in use
        /// </summary>
        /// <param name="environmentName">The environment to be assigned as current</param>
        public void SetCurrentEnvironment(string environmentName)
        {
            SqlEnvironment e = GetEnvironment(environmentName);
            foreach(SqlEnvironment env in EnvironmentList)
            {
                env.IsCurrent = (env == e);
            }
        }

    }
}
