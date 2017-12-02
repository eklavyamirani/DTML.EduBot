using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;
using System.Threading.Tasks;


namespace DTML.Common.Helpers
{
    public class KeyVaultHelper
    {
        //Initialize KeyVaultClient
        private static readonly string VaultUrl = ConfigurationManager.AppSettings["VaultUrl"];
        private static readonly string ClientId = ConfigurationManager.AppSettings["ClientId"];
        private static readonly string ApplicationKey = ConfigurationManager.AppSettings["ClientSecret"];
        private static KeyVaultClient vaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));

        private static string FetchSecret(string SecretName)
        {
            var result = AsyncHelper.RunSync<SecretBundle>(() => vaultClient.GetSecretAsync(VaultUrl, SecretName));
            return result.Value;
        }

        private async static Task<string> GetToken(string authority, string resource, string scope)
        {
            var credential = new ClientCredential(ClientId, ApplicationKey);
            var authenticationContext = new AuthenticationContext(authority, TokenCache.DefaultShared);
            return (await authenticationContext.AcquireTokenAsync(resource, credential)).AccessToken;
        }
        
        private static string GetCachedItem(string key)
        {
            if (CacheProvider.Cache.cacheStorage.Getitem<string>(key) == null)
            {
                CacheProvider.Cache.cacheStorage.Additem<string>(key, FetchSecret(ConfigurationManager.AppSettings[key]));
            }

            return CacheProvider.Cache.cacheStorage.Getitem<string>(key);
        }

        public static string RediscacheKey
        {
            get
            {
                string key = "RediscacheKey";
                return GetCachedItem(key);
            }
        }

        public static string AuditConnection
        {
            get
            {
                string key = "AuditConnection";
                return GetCachedItem(key);
            }
        }

        public static string AzureQuery
        {
            get
            {
                string key = "azurequery";
                return GetCachedItem(key);
            }
        }

        public static string AzureSearch
        {
            get
            {
                string key = "azuresearch";
                return GetCachedItem(key);
            }
        }

        public static string BlobStorageConnectionString
        {
            get
            {
                string key = "BlobStorageConnectionString";
                return GetCachedItem(key);
            }
        }

        public static string BotAPI
        {
            get
            {
                string key = "BotAPI";
                return GetCachedItem(key);
            }
        }

        public static string BotSecretKey
        {
            get
            {
                string key = "BotSecretKey";
                return GetCachedItem(key);
            }
        }

        public static string DefaultConnection
        {
            get
            {
                string key = "DefaultConnection";
                return GetCachedItem(key);
            }
        }

        public static string MailPassword
        {
            get
            {
                string key = "MailPassword";
                return GetCachedItem(key);
            }
        }

        public static string MicrosoftClientId
        {
            get
            {
                string key = "MicrosoftClientId";
                return GetCachedItem(key);
            }
        }

        public static string MicrosoftClientSecret
        {
            get
            {
                string key = "MicrosoftClientSecret";
                return GetCachedItem(key);
            }
        }

        public static string ServiceBusConnectionString
        {
            get
            {
                string key = "ServiceBusConnectionString";
                return GetCachedItem(key);
            }
        }

        public static string SpeechAPI
        {
            get
            {
                string key = "SpeechAPI";
                return GetCachedItem(key);
            }
        }

        public static string StorageConnectionString
        {
            get
            {
                string key = "StorageConnectionString";
                return GetCachedItem(key);
            }
        }

        public static string StripePublishableKey
        {
            get
            {
                string key = "StripePublishableKey";
                return GetCachedItem(key);
            }
        }

        public static string StripeSecretKey
        {
            get
            {
                string key = "StripeSecretKey";
                return GetCachedItem(key);
            }
        }

        public static string TokboxApiKey
        {
            get
            {
                string key = "TokboxApiKey";
                return GetCachedItem(key);
            }
        }

        public static string TokboxApiSecret
        {
            get
            {
                string key = "TokboxApiSecret";
                return GetCachedItem(key);
            }
        }

        public static string Twilio2FactorAPIKey
        {
            get
            {
                string key = "Twilio2FactorAPIKey";
                return GetCachedItem(key);
            }
        }

        public static string TwilioAccountSid
        {
            get
            {
                string key = "TwilioAccountSid";
                return GetCachedItem(key);
            }
        }

        public static string TwilioToken
        {
            get
            {
                string key = "TwilioToken";
                return GetCachedItem(key);
            }
        }
    }
}