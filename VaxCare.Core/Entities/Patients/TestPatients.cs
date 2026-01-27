namespace VaxCare.Core.Entities.Patients
{
    public class TestPatient
    {
        public string Name => LastName + ", " + FirstName;
        public string NameNoComma => LastName + FirstName;
        public string FullNameInCaps => (FirstName + " " + LastName).ToUpper();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DoB { get; set; } = string.Empty;
        public int Gender { get; set; } = 0;
        public string Ssn { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string EligibilityHeader { get; set; } = string.Empty;
        public string RiskDescription { get; set; } = string.Empty;
        public string RiskDetails { get; set; } = string.Empty;
        public string FadeRiskDetails { get; set; } = string.Empty;
        public string MedDCopay { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public string ExpectedProductPrice { get; set; } = string.Empty;

        // Legacy Properties. TODO: Delete them if not needed

        public string EligibilityMessage { get; set; } = string.Empty;
        public string PatientRespHeader { get; set; } = string.Empty;
        public string PatientRespMessage { get; set; } = string.Empty;
        public string ProductAmountExpected2 { get; set; } = string.Empty;


        public PatientType PatientType;
        public EligibilityResponse Eligibility;
        public MedDResponse MedDResponse;
        public EligibilityStatus EligibilityStatus;
    }

    public enum PatientType
    {
        Unknown,
        Eligible,
        NotEligible,
        MedD,
        Vfc,
        Uninsured,
        SelfPay
    }

    public enum MedDResponse
    {
        EligibleWithCoverage,
        IneligibleWithCoverage,
        EligibleWithRejectedCoverage
    }

    public enum EligibilityResponse
    {
        Unknown,
        Warning,
        Eligible,
        IncompletePatientData,
        LimitedImmunizationCoverage,
        NotEligiblePolicyNotActive,
        NotEligibleInsuranceProvidedIsNotPrimary,
        NotEligibleInvalidMemberId,
        NotEligiblePatientNotFound,
        OnlineEligibilityError,
        OnlineEligibilityNotSupported,
        PossiblePatientResponsibility,
        OutsideOfVaxCareNetwork
    }
    public enum EligibilityStatus
    {
        GreenCheckmark,
        RedX,
        Exclamation,
        Loading,
        RiskFullMoon,
        RiskFreePreShot,
        RiskHalfMoon,
        AtRiskDataComplete,
        AtRiskDataMissing,
        AtRiskDataIncorrect,
        MissingOrInvalidPayerName,
        PartnerBill,
        Undetermined,
        RiskFree,
        SelfPay
    }

    // TODO: Remove patients from this class and move them to their individual files as needed. 
    public class TestPatients
    {
        /// <summary>
        /// 'Eligible' patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient DruryChristie()
        {
            TestPatient person = new()
            {
                FirstName = "Christie",
                LastName = "Drury",
                PatientType = PatientType.Eligible,
                Eligibility = EligibilityResponse.Eligible,
                EligibilityHeader = "Eligible",
                EligibilityMessage = "This patient’s check in is Risk Free. " +
                "Please ensure that vaccines administered are within the product licensure for the patient’s age."
            };
            return person;
        }

        /// <summary>
        /// 'RiskFree' patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient MayahMiller()
        {
            TestPatient person = new()
            {
                FirstName = "Mayah",
                LastName = "Miller",
                PatientType = PatientType.Eligible,
                Eligibility = EligibilityResponse.Eligible,
                EligibilityHeader = "Eligible",
                EligibilityMessage = "Ready to Vaccinate"
            };
            return person;
        }

        /// <summary>
        /// 'RiskFree' patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient SharonRiskFree()
        {
            TestPatient person = new()
            {
                FirstName = "Sharon",
                LastName = "Riskfree",
                PatientType = PatientType.Eligible,
                Eligibility = EligibilityResponse.Eligible,
                EligibilityHeader = "Eligible",
                EligibilityMessage = "Ready to Vaccinate",
                RiskDescription = "Ready to Vaccinate",
                RiskDetails = "N/A",
                FadeRiskDetails = "N/A",
                DoB = "10/12/1974",
                PhoneNumber = "5125675543"
            };
            return person;
        }

        /// <summary>
        /// 'Med D Copay' patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient HorneSrPaul()
        {
            TestPatient person = new()
            {
                FirstName = "Paul",
                LastName = "Horne Sr",
                PatientType = PatientType.MedD,
                Eligibility = EligibilityResponse.NotEligiblePatientNotFound,
                EligibilityHeader = "Not Eligible",
                EligibilityMessage = "Justin, the payer cannot confirm eligibility due an invalid or incomplete Member ID, " +
                    "Member Name, or Insured Name. Please review the insurance information and update if you can. If you don't have " +
                    "new information, we recommend requiring the patient to Self Pay.",
                PatientRespHeader = "Medicare Part D",
                PatientRespMessage = "If the patient has indicated interest in receiving Zoster or Tdap through Medicare Part D, " +
                    "please ensure the SSN is accurate before continuing.",
                ExpectedProductPrice = "$47.65",
                ProductAmountExpected2 = "$47.65"
            };
            return person;
        }

        /// <summary>
        /// 'Med D Copay' patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient SkaggsConnie()
        {
            TestPatient person = new()
            {
                FirstName = "Connie",
                LastName = "Skaggs",
                PatientType = PatientType.MedD,
                Eligibility = EligibilityResponse.NotEligibleInsuranceProvidedIsNotPrimary,
                EligibilityHeader = "Not Eligible",
                EligibilityMessage = "Justin, Medicare indicates that the patient has other primary insurance. If you have this " +
                    "insurance information, please update it here. If not, please require the patient to Self Pay.",
                PatientRespHeader = "Medicare Part D",
                PatientRespMessage = "If the patient has indicated interest in receiving Zoster or Tdap through Medicare Part D, " +
                    "please ensure the SSN is accurate before continuing.",
                ExpectedProductPrice = "$99.16"
            };
            return person;
        }

        public static TestPatient OlsonMelissa()
        {
            TestPatient person = new()
            {
                FirstName = "Melissa",
                LastName = "Olson",
                PatientType = PatientType.MedD,
                RiskDescription = "Ready to Vaccinate",
                RiskDetails = "Limited Coverage: Flu, Pneumonia, and COVID Only",
                FadeRiskDetails = "Medicare Part B plans cover Flu, Pneumonia, and Covid shots only."
            };
            return person;
        }

        public static TestPatient OlsonMelissaUninsured()
        {
            TestPatient person = new()
            {
                FirstName = "Melissa",
                LastName = "Olson",
                PatientType = PatientType.NotEligible,
                RiskDescription = "Ready to Vaccinate",
                //RiskDetails = "Policy/Plan is out of VaxCare network",
                RiskDetails = "N/A",
                FadeRiskDetails = "Please file your claim along with the patient encounter to be reimbursed. VaxCare's Market Access team " +
                    "is working hard to contract with new Insurance providers."
            };
            return person;
        }

        public static TestPatient PowerRonda()
        {
            TestPatient person = new()
            {
                FirstName = "Ronda",
                LastName = "Power",
                PatientType = PatientType.MedD
            };
            return person;
        }

        public static TestPatient JeanBrian()
        {
            TestPatient person = new()
            {
                FirstName = "Brian",
                LastName = "Jean",
                PatientType = PatientType.MedD
            };
            return person;
        }

        /// <summary>
        /// 'Med D Copay' patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient ScottRudolph()
        {
            TestPatient person = new()
            {
                FirstName = "Rudolph",
                LastName = "Scott",
                PatientType = PatientType.MedD,
                EligibilityMessage = "Justin, Medicare Part B plans cover flu and pneumonia shots only. " +
                    "If administering any other immunizations, please require the patient to Self Pay.",
                PatientRespHeader = "Medicare Part D",
                PatientRespMessage = "If the patient has indicated interest in receiving Zoster or Tdap through " +
                    "Medicare Part D, please ensure the SSN is accurate before continuing."
            };

            return person;
        }

        /// <summary>
        /// 'Online Elig Not Supported' patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient JaquezMary()
        {
            TestPatient person = new()
            {
                FirstName = "Mary",
                LastName = "Jaquez",
                PatientType = PatientType.Uninsured,
                Eligibility = EligibilityResponse.OnlineEligibilityNotSupported,
                EligibilityHeader = "Online Eligibility Not Supported",
                EligibilityMessage = "Justin, this insurance company doesn't have an electronic eligibility check, " +
                    "so we can't tell you if the patient is eligible. We recommend you send this to VaxCare to bill. " +
                    "We traditionally collect over 95% of claims like these. If we need any additional information for " +
                    "collection, we will reach out to your office and/or the patient.",
                RiskDescription = "Patient Information Complete",
                RiskDetails = "Payer does not support online eligibility",
                FadeRiskDetails = "This insurance provider does not support online eligibility. "
            };
            return person;
        }


        /// <summary>
        /// Newly Created 'Online Elig Not Supported' patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient NewlyCreatedPatient()
        {
            TestPatient person = new()
            {
                FirstName = "",
                LastName = "",
                PatientType = PatientType.Uninsured,
                Eligibility = EligibilityResponse.OnlineEligibilityNotSupported,
                EligibilityHeader = "Online Eligibility Not Supported",
                EligibilityMessage = "Justin, this insurance company doesn't have an electronic eligibility check, " +
                    "so we can't tell you if the patient is eligible. We recommend you send this to VaxCare to bill. " +
                    "We traditionally collect over 95% of claims like these. If we need any additional information for collection, " +
                    "we will reach out to your office and/or the patient.",
                RiskDescription = "Payer Info Required",
                RiskDetails = "Insurance Card Photo Needed for Billing",
                FadeRiskDetails = "This insurance provider does not support online eligibility. "
            };
            return person;
        }

        /// <summary>
        /// 'Partner Billed' patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient VaxcareJustin()
        {
            TestPatient person = new()
            {
                FirstName = "Justin",
                LastName = "Vaxcare",
                PatientType = PatientType.Uninsured,
                Eligibility = EligibilityResponse.OutsideOfVaxCareNetwork,
                EligibilityHeader = "Online Eligibility Not Supported",
                EligibilityMessage = "",
                RiskDescription = "Partner Bill",
                RiskDetails = "Policy/Plan is out of VaxCare network",
                FadeRiskDetails = "Partner Bill"
            };
            return person;
        }

        /// <summary>
        /// 'Self Pay' patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient JonesClara()
        {
            TestPatient person = new()
            {
                FirstName = "Clara",
                LastName = "Jones",
                PatientType = PatientType.SelfPay,
                Eligibility = EligibilityResponse.OutsideOfVaxCareNetwork,
                EligibilityHeader = "Online Eligibility Not Supported",
                EligibilityMessage = " ",
                RiskDescription = "Partner Bill",
                RiskDetails = "Patient is Self-Pay",
                FadeRiskDetails = "Patient is Self-Pay"
            };
            return person;
        }

        /// <summary>
        /// 'Eligible' female VaxDemo patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient VaxDemoEliza()
        {
            TestPatient person = new()
            {
                FirstName = "Eliza",
                LastName = "Vaxdemo",
                PatientType = PatientType.Eligible,
                Eligibility = EligibilityResponse.Eligible,
                EligibilityHeader = "Eligible",
                EligibilityMessage = "This patient’s check in is Risk Free. Please ensure that vaccines " +
                    "administered are within the product licensure for the patient’s age."
            };
            return person;
        }

        /// <summary>
        /// 'Eligible' patient.
        /// </summary>
        /// <returns>The patient object.</returns>
        public static TestPatient MedDEligible()
        {
            TestPatient person = new()
            {
                FirstName = "Med",
                LastName = "Drury",
                PatientType = PatientType.Eligible,
                Eligibility = EligibilityResponse.Eligible,
                EligibilityHeader = "Eligible",
                EligibilityMessage = "This patient’s check in is Risk Free. " +
                "Please ensure that vaccines administered are within the product licensure for the patient’s age."
            };
            return person;
        }
    }
}
