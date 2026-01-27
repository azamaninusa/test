using VaxCare.Core.ApiRequests.PatientsApi.Models;

namespace VaxCare.Core.Entities.Patients
{
    public static class OnlineEligNotSupported
    {
        // TODO: Complete patient info
        public static TestPatient GetPatient()
        {
            TestPatient patient = new()
            {
                FirstName = "Oon",
                LastName = "Insurance",
                DoB = "1967-12-04",
                Gender = 0,
                PhoneNumber = "5128906654",
                Address1 = "1289 PARROT TER",
                City = "Orlando",
                State = "AZ",
                Zip = "34567",
                PatientType = PatientType.Uninsured,
                Eligibility = EligibilityResponse.OnlineEligibilityNotSupported,
                EligibilityStatus = EligibilityStatus.PartnerBill,
                EligibilityHeader = "Online Eligibility Not Supported",
                EligibilityMessage = "",
                RiskDescription = "Ready",
                RiskDetails = "Plan Out of Network",
                FadeRiskDetails = "VaxCare is not in network with this plan. Please file your claim along with the patient encounter to be reimbursed."
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
                        PrimaryInsuranceId = 1000023110,
                        PrimaryMemberId = "Z67656765"
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
