using System.Configuration;

namespace G3
{
    public class Provider
    {
        private static Provider _instance;

        IConfigurationRoot config;

        protected Provider() {
            this.config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

         public String GetConnectionString()
         {
           return config.GetValue<string>("Connection");
         }

        public int GetTimeout()
        {
            return config.GetValue<int>("Timeout");
        }

        public static Provider Instance()
        {
            

            if (_instance == null)
            {
                _instance = new Provider();
            }

            return _instance;
        }
    }
}
