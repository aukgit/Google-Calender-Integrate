using System.Configuration;
using CalendarMvc.Modules.Calendar.Outlook;
using DevMvcComponent.Encryption;

namespace CalendarMvc {
    public static class App {
        private static ExchangeServiceAccess _exchangeServiceAccess;

        private static string EmailPassword {
            get {
                var encryptedPassword = ConfigurationManager.AppSettings["EmailPassword"];
                var encrptionPassPhase = ConfigurationManager.AppSettings["EncryptedPassPhrase"];
                var actualPassword = Pbkdf2Encryption.Decrypt(encryptedPassword, encrptionPassPhase);
                return actualPassword;
            }
        }
        public static ExchangeServiceAccess ExchangeServiceAccess {
            get {
                if (_exchangeServiceAccess == null) {
                    _exchangeServiceAccess = new ExchangeServiceAccess("mauthtest@medavante.com", EmailPassword);
                }
                return _exchangeServiceAccess;
            }
            private set { _exchangeServiceAccess = value; }
        }
    }
}