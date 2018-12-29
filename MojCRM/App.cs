using System.Configuration;

namespace MojCRM
{
    /// <summary>
    /// Global App variables, etc.
    /// </summary>
    public class App
    {
        /// <summary>
        /// MerEndpoint is the URL for the different services.
        /// We use this variable so that we can use same source in all countries,
        /// and only change Web.config
        /// </summary>
        public static string MerEndpoint
        {
            get
            {
                return ConfigurationManager.AppSettings["MerEndpoint"];
            }
        }
    }
}