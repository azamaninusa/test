using VaxCare.Core.ApiRequests.PatientsApi.Models;

namespace VaxCare.Core.Entities.Patients
{
    public static class AlGreenTestRisk
    {
        public static TestPatient GetPatient()
        {
            TestPatient patient = new()
            {
                FirstName = "AlGreen",
                LastName = "TestRisk",
                DoB = "1998-01-29",
                Gender = 0,
                PhoneNumber = "8888298550",
                Address1 = "123 MAIN STREET",
                City = "Orlando",
                State = "FL",
                Zip = "32886",
                PatientType = PatientType.Eligible,
                Eligibility = EligibilityResponse.Eligible,
                EligibilityHeader = "Eligible",
                EligibilityMessage = "This patient’s check in is Risk Free. " +
                "Please ensure that vaccines administered are within the product licensure for the patient’s age."
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
                    PhoneNumber = patient.PhoneNumber,
                    Address1 = patient.Address1,
                    City = patient.City,
                    State = patient.State,
                    Zip = patient.Zip,
                    PaymentInformation =
                    {
                        PrimaryInsuranceId = 1000028601,
                        PrimaryInsurancePlanId = 74114,
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