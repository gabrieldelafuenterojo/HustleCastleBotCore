using System.IO;
using Microsoft.Extensions.Configuration;

namespace HustleCastleBotCore
{
    /// <summary>
    /// Obtiene los valores del archivo de configuración
    /// </summary>
    public class ConfigurationFile
    {
        private IConfigurationRoot configuration;
        public ConfigurationFile()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true);

            configuration = builder.Build();
        }

        /// <summary>
        /// Obtiene el parámetro GetPortalLevel del settings.json
        /// </summary>
        /// <returns></returns>
        public int GetPortalLevel()
        {
            int result;
            int.TryParse((configuration["PortalLevel"]), out result);
            return result;
        }

        /// <summary>
        /// Obtiene el parámetro GetMaxRetryPortal del settings.json
        /// </summary>
        /// <returns></returns>
        public int GetMaxRetryPortal()
        {
            int result;
            int.TryParse((configuration["MaxRetryPortal"]), out result);
            return result;
        }

        public int GetMaxDarkSouls()
        {
            int result;
            int.TryParse((configuration["MaxDarkSouls"]), out result);
            return result;
        }

        /// <summary>
        /// Obtiene el parámetro BuyFoodInMarket del settings.json
        /// </summary>
        /// <returns></returns>
        public bool BuyFoodInMarket()
        {
            int result;
            int.TryParse((configuration["BuyFoodInMarket"]), out result);

            if (result == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene el parámetro char_whitelist_battle_power del settings.json
        /// </summary>
        /// <returns></returns>
        public string GetCharWhitelistBattlePower()
        {
            return configuration["char_whitelist_battle_power"];
        }

        /// <summary>
        /// Obtiene el parámetro char_whitelist_portal del settings.json
        /// </summary>
        /// <returns></returns>
        public string GetCharWhitelistPortal()
        {
            return configuration["char_whitelist_portal"];
        }
    }
}
