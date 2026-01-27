using VaxCare.Core.ApiRequests.PatientsApi.Models;

namespace VaxCare.Core.Entities.Patients
{
    public static class MedDEligible
    {
        public static TestPatient GetPatient()
        {
            TestPatient patient = new()
            {
                FirstName = "Medd",
                LastName = "Eligible",
                DoB = "1942-03-23",
                Gender = 1,
                Ssn = "123456789",
                PhoneNumber = "8888298550",
                Address1 = "123 MAIN STREET",
                City = "Orlando",
                State = "FL",
                Zip = "32886",
                PatientType = PatientType.MedD,
                Eligibility = EligibilityResponse.Eligible,
                RiskDescription = "Ready",
                FadeRiskDetails = "Payment guaranteed on all eligible doses administered with credit card collected and consent form returned to VaxCare.",
                MedDCopay = "Tdap (Adacel): $0"
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
                    Ssn= patient.Ssn,
                    PhoneNumber = patient.PhoneNumber,
                    Address1 = patient.Address1,
                    City = patient.City,
                    State = patient.State,
                    Zip = patient.Zip,
                    PaymentInformation =
                    {
                        PrimaryInsuranceId = 2,
                        PrimaryMemberId = "QAT18894939"
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
