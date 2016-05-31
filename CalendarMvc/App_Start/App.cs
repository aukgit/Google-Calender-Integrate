using System.Configuration;
using System.Web;
using CalendarMvc.Modules.Calendar.Outlook;
using DevMvcComponent.Encryption;
using DevMvcComponent.Extensions;
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

        public static string Email {
            get {
                return "mauthtest@medavante.com";
            }
        }
        public static string EmailDisplay {
            get {
                return "Test Acc";
            }
        }
        public static ExchangeServiceAccess ExchangeServiceAccess {
            get {
                if (_exchangeServiceAccess == null) {
                    var fileName = "_service.object";
                    var serviceObject = BinaryFileExtenstion.ReadfromBinary2<ExchangeServiceAccess>(fileName);
                    if (serviceObject == null) {
                        _exchangeServiceAccess = new ExchangeServiceAccess(Email, EmailPassword);
                        _exchangeServiceAccess.SaveAsBinary(fileName);
                    } else {
                        _exchangeServiceAccess = serviceObject;
                    }
                }
                return _exchangeServiceAccess;
            }
            private set { _exchangeServiceAccess = value; }
        }
    }
}