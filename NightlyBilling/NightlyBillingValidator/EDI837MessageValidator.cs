using FluentValidation;
using NightlyBillingData.DbModels;
using NightlyBillingData.ClaimFormExtensions;

namespace NightlyBillingValidators
{
    public class EDI837MessageValidator : AbstractValidator<ClaimDataContainer>
    {
        public EDI837MessageValidator()
        {
            RuleFor(x => x.ActualClaimDataDto.Patient.FirstName).NotNull().Equal(x => x.ExpectedClaimDataDto.PatientFirstName)
                .WithMessage("Patient FirstName does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.Patient.LastName).NotNull().Equal(x => x.ExpectedClaimDataDto.PatientLastName)
                .WithMessage("Patient LastName does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.Patient.DoB).NotNull().Equal(x => x.ExpectedClaimDataDto.PatientDoB)
                .WithMessage("Patient DoB does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.Patient.Sex).NotNull().Equal(x => x.ExpectedClaimDataDto.PatientGender)
                .WithMessage("Patient Gender does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.PatientAddress.Address1).NotNull().Equal(x => x.ExpectedClaimDataDto.PatientAddressStreet)
                .WithMessage("Patient Address (Street) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.PatientAddress.City).NotNull().Equal(x => x.ExpectedClaimDataDto.PatientAddressCity)
                .WithMessage("Patient Address (City) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.PatientAddress.State).NotNull().Equal(x => x.ExpectedClaimDataDto.PatientAddressState)
                .WithMessage("Patient Address (State) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.PatientAddress.ZipCode.Substring(0, 5)).NotNull().Equal(x => x.ExpectedClaimDataDto.PatientAddressZipCode.Substring(0, 5))
                .WithMessage("Patient Address (ZipCode) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.PayerName.ToUpper()).NotNull().Equal(x => x.ExpectedClaimDataDto.PrimaryInsuranceName.ToUpper())
                .WithMessage("Primary Insurance Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.MemberId).NotNull().Equal(x => x.ExpectedClaimDataDto.MemberId)
                .WithMessage("MemberId does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.PatientRelationshipToInsured).NotNull().Equal(x => x.ExpectedClaimDataDto.RelationshipToInsured)
                .WithMessage("Relationship To Insured does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.Patient.LastName).NotNull().Equal(x => x.ExpectedClaimDataDto.PatientLastName)
                .WithMessage("Patient LastName does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.LastNameOrOrganizationName).NotNull().Equal(x => x.ExpectedClaimDataDto.BillingProviderName)
                .WithMessage("Billing Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.Npi).NotNull().Equal(x => x.ExpectedClaimDataDto.BillingProviderNpi)
                .WithMessage("Billing Provider NPI does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.FederalTaxIDNumber).NotNull().Equal(x => x.ExpectedClaimDataDto.BillingProviderEin)
                .WithMessage("Billing Provider EIN does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.EntityType).NotNull().Equal(x => x.ExpectedClaimDataDto.BillingProviderEntityTypeCode)
                .WithMessage("Billing Provider Entity Type Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.TaxonomyCode.IdentificationCode ?? string.Empty).Equal(x => x.ExpectedClaimDataDto.BillingProviderTaxonomyCode ?? string.Empty)
                .WithMessage("Billing Provider Taxonomy Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.Address1).NotNull().Equal(x => x.ExpectedClaimDataDto.BillingProviderAddressStreet)
                .WithMessage("Billing Provider Address (Street) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.City).NotNull().Equal(x => x.ExpectedClaimDataDto.BillingProviderAddressCity)
                .WithMessage("Billing Provider Address (City) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.State).NotNull().Equal(x => x.ExpectedClaimDataDto.BillingProviderAddressState)
                .WithMessage("Billing Provider Address (State) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.BillingProvider.ZipCode.Substring(0, 5)).NotNull().Equal(x => x.ExpectedClaimDataDto.BillingProviderAddressZipCode.Substring(0, 5))
                .WithMessage("Billing Provider Address (ZipCode) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.PlaceOfService).NotNull().Equal(x => x.ExpectedClaimDataDto.PlaceOfServiceCode)
                .WithMessage("Place Of Service Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ClaimFilingIndicatorCode).NotNull().Equal(x => x.ExpectedClaimDataDto.ClaimFilingIndicatorCode)
                .WithMessage("Claim Filing Indicator Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.FirstName ?? string.Empty).Equal(x => x.ExpectedClaimDataDto.RenderingProviderFirstName ?? string.Empty)
                .WithMessage("Rendering Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.LastNameOrOrganizationName ?? string.Empty).Equal(x => x.ExpectedClaimDataDto.RenderingProviderLastName ?? string.Empty)
                .WithMessage("Rendering Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.Npi ?? string.Empty).Equal(x => x.ExpectedClaimDataDto.RenderingProviderNpi ?? string.Empty)
                .WithMessage("Rendering Provider NPI does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.EntityType).Equal(x => x.ExpectedClaimDataDto.RenderingProviderEntityTypeCode)
                .WithMessage("Rendering Provider Entity Type Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.RenderingProvider.TaxonomyCode.IdentificationCode ?? string.Empty).Equal(x => x.ExpectedClaimDataDto.RenderingProviderTaxonomyCode ?? string.Empty)
                .WithMessage("Rendering Provider Taxonomy Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.LastNameOrOrganizationName).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderName)
                .WithMessage("Servicing Provider Name does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.Npi ?? string.Empty).Equal(x => x.ExpectedClaimDataDto.ServicingProviderNpi ?? string.Empty)
                .WithMessage("Servicing Provider Npi does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.EntityType).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderEntityTypeCode)
                .WithMessage("Servicing Provider Entity Type Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.Address1).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderAddressStreet)
                .WithMessage("Servicing Provider Address (Street) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.City).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderAddressCity)
                .WithMessage("Servicing Provider Address (City) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.State).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderAddressState)
                .WithMessage("Servicing Provider Address (State) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.ServiceFacilityLocation.ZipCode.Substring(0, 5)).NotNull().Equal(x => x.ExpectedClaimDataDto.ServicingProviderAddressZipCode.Substring(0, 5))
                .WithMessage("Servicing Provider Address (ZipCode) does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
            
            RuleFor(x => x.ActualClaimDataDto.GetCptCodeHelper(x.ExpectedCptCode)).NotNull().Equal(x => x.ExpectedCptCode)
                .WithMessage("Admin CPT Code does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.GetServiceLineQuantityHelper(x.ExpectedCptCode)).NotNull().Equal(x => x.ExpectedQuantity)
                .WithMessage("Admin CPT Quantity does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");

            RuleFor(x => x.ActualClaimDataDto.GetServiceLineChargeHelper(x.ExpectedCptCode)).NotNull().Equal(x => x.ExpectedServiceLineCharge)
                .WithMessage("Admin CPT Billed Amount does not match. Expected value: {ComparisonValue}; Actual Value: {PropertyValue}.");
        }
    }
}
