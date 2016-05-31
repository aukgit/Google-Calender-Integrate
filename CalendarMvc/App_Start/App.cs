using System.Configuration;
using System.Web;
using CalendarMvc.Modules.Calendar.Outlook;
using DevMvcComponent.Encryption;
using DevMvcComponent.Extensions;
using DevMvcComponent.Miscellaneous;

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

        /// <summary>
        ///     Current Directory
        ///     Returns current directory without slash.
        /// </summary>
        public static string Path {
            get { return HttpContext.Current.Server.MapPath("~/"); }
        }

        public static string Email {
            get { return "mauthtest@medavante.com"; }
        }

        public static string EmailDisplay {
            get { return "Test Acc"; }
        }

        public static ExchangeServiceAccess ExchangeServiceAccess {
            get {
                if (_exchangeServiceAccess == null) {
                    //var fileName = Path + @"_service.object";
                    //var serviceObject = BinaryFileExtenstion.ReadfromBinary2<ExchangeServiceAccess>(fileName);
                    //if (serviceObject == null) {
                    _exchangeServiceAccess = new ExchangeServiceAccess(Email, EmailPassword);
                    //_exchangeServiceAccess.SaveAsBinary(fileName);                        //_exchangeServiceAccess.SaveAsBinary(fileName);
                    //} else {
                    //    _exchangeServiceAccess = serviceObject;
                    //}
                }
                return _exchangeServiceAccess;
            }
            private set { _exchangeServiceAccess = value; }
        }
    }
}