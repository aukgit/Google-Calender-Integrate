namespace CalendarMvc.Models {
    public class OutlookToken {
        public int OutlookTokenID { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool IsRefreshTokenExpired { get; set; }
        public string Email { get; set; }
    }
}