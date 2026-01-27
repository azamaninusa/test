using Bogus;
using VaxCare.Core.ApiRequests.PatientsApi.Models;

namespace VaxCare.Core.Entities.Patients
{
    public static class FakePatient
    {
        public static PatientRequest GetPayload(int clinicId = 0, string? date = null)
        {
            var fakePatientInfo = new Faker<PatientInfo>()
                .RuleFor(p => p.FirstName, v => v.Name.FirstName())
                .RuleFor(p => p.LastName, v => "VaxCareTest")
                .RuleFor(p => p.DoB, v => v.Date.Past(80).ToString("yyy-MM-dd"))
                .RuleFor(p => p.Gender, v => v.Random.Int(0, 2))
                .RuleFor(p => p.PhoneNumber, v => v.Phone.PhoneNumber("##########"))
                .RuleFor(p => p.Address1, v => v.Address.StreetAddress())
                .RuleFor(p => p.City, v => v.Address.City())
                .RuleFor(p => p.State, v => v.Address.StateAbbr())
                .RuleFor(p => p.Zip, v => v.Address.ZipCode()[..5])
                .RuleFor(p => p.Race, v => "Unspecified")
                .RuleFor(p => p.Ethnicity, v => "Unspecified");

            var fakePatient = fakePatientInfo.Generate();

            var fakePi = new Faker<PaymentInformation>()
                .RuleFor(p => p.PrimaryInsuranceId, v => 2)
                .RuleFor(p => p.PrimaryMemberId, v => "QAT18894939");

            var fakePaymentInfo = fakePi.Generate();

            var patient = new PatientRequest()
            {
                NewPatient =
                {
                    FirstName = fakePatient.FirstName,
                    LastName = fakePatient.LastName,
                    DoB = fakePatient.DoB,
                    Gender = fakePatient.Gender,
                    PhoneNumber = fakePatient.PhoneNumber,
                    Address1 = fakePatient.Address1,
                    City = fakePatient.City,
                    State = fakePatient.State,
                    Zip = fakePatient.Zip,
                    PaymentInformation =
                    {
                        PrimaryInsuranceId = fakePaymentInfo.PrimaryInsuranceId,
                        PrimaryMemberId = fakePaymentInfo.PrimaryMemberId
                    },
                    Race = fakePatient.Race,
                    Ethnicity = fakePatient.Ethnicity,
                },
                ClinicId = clinicId != 0 ? clinicId : 89534,
                Date = !string.IsNullOrEmpty(date) ? date : DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                MetaData = new(),
                VisitType = "Well"
            };

            return patient;
        }
    }
}
