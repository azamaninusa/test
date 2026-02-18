using NightlyBillingData.ClaimFormExtensions;
using NightlyBillingData.DbModels;
using VaxCare.Edi.Contracts.Claims;

namespace NightlyBillingData
{
    public class CPTCalculator
    {
        public CptCodeData GetStandardCptCodeData(string ageGroup, string adminScenario, string expectedCptCode, int doseQuantity, int components)
        {
            int baseQuantity;
            string? serviceLineQuantity;
            string? serviceLineCharge;

            if (expectedCptCode == "90471")
            {
                serviceLineQuantity = "1";
                serviceLineCharge = "56.00";
            }
            else if (expectedCptCode == "90472")
            {
                baseQuantity = doseQuantity - 1;
                serviceLineQuantity = baseQuantity.ToString();
                serviceLineCharge = (38.00 * baseQuantity).ToString("F2");
            }
            else if (expectedCptCode == "90460")
            {
                baseQuantity = doseQuantity;
                serviceLineQuantity = baseQuantity.ToString();
                serviceLineCharge = (56.00 * baseQuantity).ToString("F2");
            }
            else if (expectedCptCode == "90461")
            {
                baseQuantity = components - doseQuantity;
                serviceLineQuantity = baseQuantity.ToString();
                serviceLineCharge = (38.00 * baseQuantity).ToString("F2");
            }
            else if (expectedCptCode == "G0008" || expectedCptCode == "G0009")
            {
                serviceLineQuantity = "1";
                serviceLineCharge = "90.00";
            }
            else if (expectedCptCode == "90480" || expectedCptCode == "96380" || expectedCptCode == "96381")
            {
                serviceLineQuantity = "1";
                serviceLineCharge = "50.00";
            }
            else if (expectedCptCode == "58300" || expectedCptCode == "11981")
            {
                serviceLineQuantity = "1";
                serviceLineCharge = "0.00";
            }
            else if (expectedCptCode == string.Empty)
            {
                serviceLineQuantity = string.Empty;
                serviceLineCharge = string.Empty;
            }
            else
                throw new NotImplementedException($"Admin CPT code not implemented for {ageGroup} - {adminScenario}.");

            var cptCodeData = new CptCodeData
            {
                ServiceLineQuantity = serviceLineQuantity,
                ServiceLineCharge = serviceLineCharge
            };
            return cptCodeData;
        }

        public CptCodeData GetQuantityOverride(string filingRule, string cptCode, List<ClaimTestScenarios> testScenarios)
        {
            var cptCodeData = new CptCodeData();

            switch (filingRule)
            {
                case "BCBSKSComponent":
                    if (testScenarios.Any(x => x.DoseCount > 1) && cptCode == "90460")
                    {
                        cptCodeData.ServiceLineQuantity = "1";
                        cptCodeData.ServiceLineCharge = "56.00";
                    }
                    else if (testScenarios.Any(x => x.DoseCount > 1) && cptCode == "90461")
                    {
                        var totalComponents = testScenarios.Select(x => x.TotalComponents).FirstOrDefault();
                        var count90461 = totalComponents - 1;
                        cptCodeData.ServiceLineQuantity = count90461.ToString();
                        cptCodeData.ServiceLineCharge = (38.00 * count90461).ToString("F2");
                    }
                    break;
            }
            return cptCodeData;
        }

        public string GetCptCodeOverride(string filingRule, string cptCode, ClaimForm claimForm, List<ClaimTestScenarios> testScenarios)
        {
            switch (filingRule)
            {
                case "BCBS SC Component":
                    if (cptCode == "90461")
                    {
                        cptCode = string.Empty;
                    }
                    break;
                case "ExcludeAdminCodesForVfcClaims":
                    if (testScenarios.Any(x => x.Stock == "Vfc"))
                    {
                        cptCode = string.Empty;
                    }
                    break;
                case "ExcludeAdultAdminCodes":
                    if (cptCode == "90471" || cptCode == "90472" || cptCode == "99401" || cptCode == "G0008" || cptCode == "G0009")
                    {
                        cptCode = string.Empty;
                    }
                    break;
                case "ExcludePediatricAdminCodes":
                    if (testScenarios.Any(x => x.Stock == "Vfc") && cptCode == "90461")
                    {
                        cptCode = string.Empty;
                    }
                    break;
                case "Kaiser Prevent G Codes":
                    if (cptCode == "G0008" || cptCode == "G0009")
                    {
                        cptCode = "90471";
                    }
                    break;
                case "LARCNexplanonIUDInsertionCode":
                    if (testScenarios.Any(x => x.Antigen == "IUD"))
                    {
                        cptCode = "58300";
                    }
                    else if (testScenarios.Any(x => x.Antigen == "Implant"))
                    {
                        cptCode = "11981";
                    }
                    break;
                case "MedicareCodingOver65":
                    if (testScenarios.Any(x => x.Antigen == "Influenza") && claimForm.GetPatientAgeInMonths() >= 780)
                    {
                        cptCode = "G0008";
                    }
                    else if ((testScenarios.Any(x => x.Antigen == "PCV15") || testScenarios.Any(x => x.Antigen == "PCV20") || testScenarios.Any(x => x.Antigen == "PCV 21") || testScenarios.Any(x => x.Antigen == "PPSV23")) && claimForm.GetPatientAgeInMonths() >= 780)
                    {
                        cptCode = "G0009";
                    }
                    break;
                case "NoAddOnCPT":
                    if (cptCode == "90461")
                    {
                        cptCode = string.Empty;
                    }
                    break;
                case "OverrideAdminCPTs":
                    if (cptCode == "90461")
                    {
                        cptCode = string.Empty;
                    }
                    break;
                case "SwitchVfcPediatricAdminCodesForAdultCodes":
                    if (testScenarios.Any(x => x.Stock == "Vfc") && testScenarios.Any(x => x.PediatricSingleComponentBillingFF == true))
                    {
                        if (cptCode == "90460")
                        {
                            cptCode = "90471";
                        }
                        else if (cptCode == "90461")
                        {
                            cptCode = "90472";
                        }
                    }
                    break;
                case "UseAdultAdminCodesForAllVaccines":
                    if (cptCode == "90460" || cptCode == "G0008" || cptCode == "G0009")
                    {
                        cptCode = "90471";
                    }
                    else if (cptCode == "90461")
                    {
                        cptCode = "90472";
                    }
                    break;
                case "UseGcodeAdminCPT":
                    if (testScenarios.Any(x => x.Antigen == "Influenza") && claimForm.GetPatientAgeInMonths() >= 228)
                    {
                        cptCode = "G0008";
                    }
                    break;
            }
            return cptCode;
        }
    }
}
