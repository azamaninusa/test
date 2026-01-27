using VaxCare.Core.ApiRequests.PatientsApi.Models;

namespace VaxCare.Core.Entities.Patients
{
    public static class PowerRonda
    {
        public static TestPatient GetPatient()
        {
            TestPatient patient = new()
            {
                FirstName = "Ronda",
                LastName = "Power",
                DoB = "1972-06-06",
                Gender = 1,
                Ssn = "435454566",
                PhoneNumber = "5129032278",
                Address1 = "56 GDSJJGSDF",
                City = "Orlando",
                State = "FL",
                Zip = "32756",
                PatientType = PatientType.MedD,
                Eligibility = EligibilityResponse.Eligible,
                RiskDescription = "Ready",
                FadeRiskDetails = "Payment guaranteed on all eligible doses administered with credit card collected and consent form returned to VaxCare.",
                MedDCopay = "Tdap (Adacel): $47",
            };

            return patient;
        }

        public static PatientRequest GetPayload()
        {
            var patient = GetPatient();
            var patientRequest = new PatientRequest()
            {
                NewPatient =
                {
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    DoB = patient.DoB,
                    Gender = patient.Gender,
                    Ssn = patient.Ssn,
                    PhoneNumber = patient.PhoneNumber,
                    Address1 = patient.Address1,
                    City = patient.City,
                    State = patient.State,
                    Zip = patient.Zip,
                    PaymentInformation =
                    {
                        PrimaryInsuranceId = 7,
                        PrimaryMemberId = "ZUF893A64799",
                    },
                    Race = "Unspecified",
                    Ethnicity = "Unspecified",
                },
                ClinicId = 89534,
                Date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                MetaData = new(),
                VisitType = "Well"
            };

            return patientRequest;
        }
    }
}
