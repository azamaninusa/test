namespace VaxCare.Core.Entities
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int PartnerId { get; set; } = 0;
        public int ClinicId { get; set; } = 0;
        public string ClinicName { get; set; } = string.Empty;
    }
    // TODO: Consider moving these to appsettings
    public class Users
    {
        /// <summary>
        /// Test user account for VaxCare2.0 partner used to log in to Portal.
        /// </summary>
        public static User SecondBaptistChurch()
        {
            User user = new()
            {
                Username = "Vaxcare@aol.com",
                Password = "1111",
                PartnerId = 100001,
                ClinicId = 10808,
                ClinicName = "Curch"
            };
            return user;
        }


        /// <summary>
        /// Test user account for VaxCare3.0 partner used to log in to Portal.
        /// </summary>
        public static User QaRobot()
        {
            User user = new()
            {
                Username = "qarobot@vaxcare.com",
                Password = "Q4R0b0t123",
                PartnerId = 178764,
                ClinicId = 89534,
                ClinicName = "QA One"
            };
            return user;
        }
    }
}
