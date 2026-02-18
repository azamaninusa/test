namespace NightlyBillingData
{
    public static class SqlData
    {
		public static readonly string getBillableClaims =
			$@"EXEC [DataEntry].[Billing].[GatherBillableClaims];";

        public static readonly string getClaimData =
			$@"SELECT
				CF.ConsentFormId,
				CF.ClaimPaymentMode,
				CF.PaymentModeReason,
				CF.Created CreatedDate,
				CK.DoS DateOfService,
				CASE WHEN CK.[Sent] IS NULL THEN 0 ELSE 1 END IsSent,
				CS.CompensationStatus,
				CSS.CompensationSubStatus,
				FRD.Decision FinalReviewDecision,
				FRDR.Reason FinalReviewDecisionReason,
				CFCS.IsInReview,
				DA.DenialActionName DenialAction,
				ISNULL(CF.ClaimActionId, 0) ClaimActionId,
				CF.ConsentNumberInt PatientVisitId,
				CF.Stock,
				CF.ResearchStatus,
				CF.BillingStatus,
				CF.BatchId,
				CF.EligibilityId,
				CF.ConsentFormExceptionId,
				CASE WHEN TP.PartnerId IS NULL THEN 0 ELSE 1 END IsTestPartner,
				(SELECT COUNT(*) FROM [DataEntry].[dbo].[ConsentFormVaccinations] CFV WHERE CF.ConsentFormId = CFV.ConsentFormId) ProductCount,
				ISNULL(E.ValidationTypeId, 0) AS ValidationLevel
			FROM [DataEntry].[dbo].[ConsentForms] CF
				INNER JOIN [DataEntry].[dbo].[ConsentKeys] CK ON CF.ConsentFormId = CK.SourceId
				INNER JOIN [Sales].[dbo].[Clinics] C ON CF.ClinicId = C.ClinicId
				INNER JOIN [DataEntry].[Billing].[ConsentFormCompensationStatus] CFCS ON CF.ConsentFormId = CFCS.ConsentFormId
				LEFT JOIN [DataEntry].[Billing].[CompensationStatuses] CS ON CFCS.CompensationStatusId = CS.Id
				LEFT JOIN [DataEntry].[Billing].[CompensationSubStatuses] CSS ON CFCS.CompensationSubStatusId = CSS.Id
				LEFT JOIN [DataEntry].[Billing].[FinalReviewDecisions] FRD ON CFCS.FinalReviewDecisionId = FRD.Id
				LEFT JOIN [DataEntry].[Billing].[FinalReviewDecisionReasons] FRDR ON CFCS.FinalReviewDecisionReasonId = FRDR.Id
				LEFT JOIN [DataEntry].[dbo].[DenialActions] DA ON CF.DenialAction = DA.DenialActionId
				LEFT JOIN [DataEntry].[dbo].[TestPartners] TP ON C.PartnerId = TP.PartnerId
				OUTER APPLY (SELECT MAX(ValidationTypeId) AS ValidationTypeId
							FROM [DataEntry].[dbo].[QAExceptions] Q INNER JOIN [DataEntry].[dbo].[QAExceptionTypes] QT ON Q.QAExceptionTypeId = QT.QAExceptionTypeId
							WHERE Q.ConsentFormId = CF.ConsentFormId) E
			WHERE CF.ConsentFormId = @id;";

		public static readonly string getClaimableServices =
			$@"SELECT 
				CFV.ConsentFormId,
				CS.Id,
				CS.Quantity,
				CS.ConsentFormVaccinationId,
				CS.CptCode,
				CS.ExpectedPay
			FROM [DataEntry].[Billing].[ClaimableServices] CS
				INNER JOIN [DataEntry].[dbo].[ConsentFormVaccinations] CFV ON CS.ConsentFormVaccinationId = CFV.ConsentFormVaccinationId
			WHERE CFV.ConsentFormId = @id;";

		public static readonly string getClaimExceptions =
			$@"SELECT
				Q.ConsentFormId,
            	Q.QaExceptionId,
            	Q.QaExceptionTypeId,
            	QT.QaExceptionTypeName,
            	QT.QaExceptionCategoryId,
				QT.ValidationTypeId
            FROM [DataEntry].[dbo].[QaExceptions] Q
				INNER JOIN [DataEntry].[dbo].[QAExceptionTypes] QT ON Q.QAExceptionTypeId = QT.QAExceptionTypeId
            WHERE V.ConsentFormId = @id;";

        public static readonly string getClaimMultiQueryData = $"{getClaimData} {getClaimExceptions}";

		public static readonly string getMessageOut =
			$@"SELECT
				MM.MsgId,
				MM.ConsentFormId,
				MM.ClaimTypeQualifierId,
				MM.ControlNumber,
				MM.BillingProcessorId,
				MM.CreatedOnUtc,
				M.ISAID,
				M.MessageData
			FROM [RealMed].[dbo].[MessageOutMetadata] MM
				INNER JOIN [RealMed].[dbo].[MessagesOut] M ON MM.MsgId = M.MsgId
			WHERE MM.ConsentFormId = @id
			ORDER BY MM.CreatedOnUtc DESC;";

        public static string getTestScenarioData =
			$@"DROP TABLE IF EXISTS #Claims
			SELECT DISTINCT
				S.ConsentFormId,
				S.PayerEntityId,
				CONVERT(date, S.DateofService) DoS,
				CONVERT(date, S.PatientDob) DoB,
				FLOOR(DATEDIFF(day, S.PatientDob, S.DateofService)/365.25) AS AgeDoS,
				[RealMed].[dbo].[AgeToMonths] (S.PatientDob, S.DateofService) AS AgeMonths,
				S.Relationship RelationshipToInsured,
				(SELECT COUNT(*) FROM [DataEntry].[dbo].[ConsentFormVaccinations] CFV WHERE CFV.ConsentFormId = S.ConsentFormId) DoseCount,
				CFV.VaccineCodeName,
				P.ProductId,
				P.Antigen,
				P.Components,
				NULL TotalComponents,
				S.Stock,
				ISNULL(CAST(NULL AS varchar(50)), '') VisitType,
				NULL PediatricSingleComponentBillingFF,
				ISNULL(PR.RuleName, 'None') FilingRuleName
			INTO #Claims
			FROM [RealMed].[Claims].[Standard837Summary] S
				INNER JOIN [DataEntry].[dbo].[ConsentFormVaccinations] CFV ON S.ConsentFormId = CFV.ConsentFormId
				INNER JOIN [Sales].[dbo].[LotNumbers] LN ON CFV.LotNumber = LN.LotNumberName
				INNER JOIN [Sales].[dbo].[Products] P ON LN.ProductId = P.ProductId
				LEFT JOIN [RealMed].[Claims].[PayerFilingRuleMapping] PM ON S.PayerEntityId = PM.PayerEntityId
				LEFT JOIN [RealMed].[Claims].[PayerFilingRules] PR ON PM.PayerFilingRuleId = PR.Id
			WHERE CONVERT(date, S.DateofService) >= DATEADD(day, -90, GETDATE())
				AND ISNULL(PR.RuleName, 'None') = @Id

			DROP TABLE IF EXISTS #EligDetails
			SELECT BE.ConsentFormId,
				MAX(CASE WHEN (ISNULL(RejectionReason,'') NOT LIKE '%10001:%' OR
					ISNULL(RejectionReason,'') NOT LIKE '%10005:%' OR
					ISNULL(RejectionReason,'') NOT LIKE '%41:%' OR
					ISNULL(RejectionReason,'') NOT LIKE '%42:%' OR
					ISNULL(RejectionReason,'') NOT LIKE '%80:%')
					THEN 1 ELSE 0 END) AS EligCheck,
				MAX(CASE WHEN (ISNULL(RejectionReason,'') LIKE '%1019:%' OR
					ISNULL(RejectionReason,'') LIKE '%1114:%' OR
					ISNULL(RejectionReason,'') LIKE '%1113:%' OR
					ISNULL(RejectionReason,'') LIKE '%MEDADVBILLING:%')
					THEN 1 ELSE 0 END) AS MedDRejectCode
			INTO #EligDetails
			FROM [DataEntry].[dbo].[BatchEligibilityCheckResults] BE
				INNER JOIN #Claims C ON BE.ConsentFormId = C.ConsentFormId
			GROUP BY BE.ConsentFormId
					
			DROP TABLE IF EXISTS #Scenarios
			SELECT
				CASE WHEN AgeDoS <19 THEN 'Pediatric'
					 WHEN ISNULL(MedDRejectCode, 0) = 1 THEN 'Medicare'
					 ELSE 'Adult' END AgeGroup,
				CASE WHEN Components > 1 THEN 'Multi-Component'
					 WHEN Components = 1 AND Antigen = 'COVID-19' THEN 'Private COVID'
					 WHEN Components = 1 AND Antigen = 'COVID' THEN 'Public COVID'
					 WHEN Components = 1 AND Antigen LIKE 'RSV%' THEN 'RSV'
					 WHEN Components = 1 AND Antigen = 'Implant' THEN 'LARC - Implant'
					 WHEN Components = 1 AND Antigen = 'Injection' THEN 'LARC - Injection'
					 WHEN Components = 1 AND Antigen = 'IUD' THEN 'LARC - IUD'
					 ELSE 'Single-Component' END AdminScenario,
					 C.*
			INTO #Scenarios
			FROM #Claims C
				LEFT JOIN #EligDetails E ON C.ConsentFormId = E.ConsentFormId
			ORDER BY PayerEntityId
			
			DROP TABLE IF EXISTS #Components
			SELECT S.ConsentFormId, SUM(P.Components) TotalComponents
			INTO #Components
			FROM #Scenarios S
				INNER JOIN [Sales].[dbo].[Products] P ON S.ProductId = P.ProductId
			GROUP BY S.ConsentFormId
			
			UPDATE S
			SET S.TotalComponents = C.TotalComponents
			FROM #Scenarios S
				INNER JOIN #Components C ON S.ConsentFormId = C.ConsentFormId

			DROP TABLE IF EXISTS #VisitDetails
			SELECT
				S.ConsentFormId,
				PV.VisitType,
				CASE WHEN PFF.FeatureFlagId IS NULL THEN 0 ELSE 1 END 'PediatricSingleComponentBillingFF'
			INTO #VisitDetails
			FROM #Scenarios S
				INNER JOIN [DataEntry].[dbo].[ConsentForms] CF ON S.ConsentFormId = CF.ConsentFormId
				INNER JOIN [Sales].[dbo].[PatientVisits] PV ON CF.ConsentNumberInt = PV.PatientVisitId
				INNER JOIN [Sales].[dbo].[Clinics] C ON CF.ClinicId = C.ClinicId
				LEFT JOIN [Sales].[dbo].[vwPartnerActiveFeatureFlags] PFF ON C.PartnerId = PFF.PartnerId AND PFF.FeatureFlagName = 'PediatricSingleComponentBilling'
			
			UPDATE S
			SET S.VisitType = V.VisitType, S.PediatricSingleComponentBillingFF = V.PediatricSingleComponentBillingFF
			FROM #Scenarios S
				INNER JOIN #VisitDetails V ON S.ConsentFormId = V.ConsentFormId
			
			SELECT *
			FROM #Scenarios
			ORDER BY ConsentFormId;";

        public static readonly string getExpectedClaimData =
			$@"DECLARE @Consents table (ConsentFormId int)
				INSERT INTO @Consents
				SELECT value 
				FROM String_Split(@Id, ',')

				SELECT
				CF.ConsentFormId,
				CF.FirstName PatientFirstName,
				CF.LastName PatientLastName,
				CF.DoB PatientDoB,
				CF.Gender PatientGender,
				TRIM(ISNULL(CF.Address1, '') + ' ' + ISNULL(CF.Address2, '')) PatientAddressStreet,
				CF.City PatientAddressCity,
				CF.[State] PatientAddressState,
				CF.ZipCode PatientAddressZipCode,
				TRIM(PM.OutPayerName) PrimaryInsuranceName,
				CF.PrimaryInsuranceMemberId MemberId,
				TRIM(CF.PrimaryInsuranceGroupId) GroupId,
				CASE WHEN CF.RelationshipToInsured = 'Dependent' THEN 'Child' ELSE CF.RelationshipToInsured END RelationshipToInsured,
				PM.PayerEntityId,
			    TRIM(ISNULL(EMB.FirstName,'') + ' ' + ISNULL(EMB.LastNameOrOrganizationName,'')) BillingProviderName, 
			    EMB.IdentificationCode BillingProviderNPI,
				EMB.ReferenceIdentification BillingProviderEIN,
			    EMB.EntityType BillingProviderEntityTypeCode,
				ISNULL(IESB.ReferenceIdentification, '') BillingProviderTaxonomyCode,
				TRIM(ISNULL(EMB.Address1, '') + ' ' + ISNULL(EMB.Address2, '')) BillingProviderAddressStreet,
				ISNULL(EMB.City, '') BillingProviderAddressCity,
				ISNULL(EMB.[State], '') BillingProviderAddressState,
				ISNULL(EMB.ZipCode, '') BillingProviderAddressZipCode,
				ISNULL(PSM.PlaceOfServiceCode, '11') PlaceOfServiceCode,
				ISNULL(F.FilingIndicator, 'ZZ') ClaimFilingIndicatorCode,
				TRIM(ISNULL(EMR.FirstName,'')) RenderingProviderFirstName,
				TRIM(ISNULL(EMR.LastNameOrOrganizationName,'')) RenderingProviderLastName,
			    EMR.IdentificationCode RenderingProviderNPI, 
			    EMR.EntityType RenderingProviderEntityTypeCode,
				ISNULL(IESR.ReferenceIdentification, '') RenderingProviderTaxonomyCode,
				TRIM(ISNULL(EMS.FirstName,'') + ' ' + ISNULL(EMS.LastNameOrOrganizationName,'')) ServicingProviderName,
			    EMS.IdentificationCode ServicingProviderNPI, 
			    EMS.EntityType ServicingProviderEntityTypeCode,
				CASE WHEN EMS.EntityAddressId IS NULL THEN TRIM(ISNULL(CL.Address1, '') + ' ' + ISNULL(CL.Address2, '')) ELSE TRIM(ISNULL(EMS.Address1, '') + ' ' + ISNULL(EMS.Address2, '')) END ServicingProviderAddressStreet,
				CASE WHEN EMS.EntityAddressId IS NULL THEN ISNULL(CL.City, '') ELSE ISNULL(EMS.City, '') END ServicingProviderAddressCity,
				CASE WHEN EMS.EntityAddressId IS NULL THEN ISNULL(CL.[State], '') ELSE ISNULL(EMS.[State], '') END ServicingProviderAddressState,
				CASE WHEN EMS.EntityAddressId IS NULL THEN ISNULL(CL.ZipCode, '') ELSE ISNULL(EMS.ZipCode, '') END ServicingProviderAddressZipCode
			FROM @Consents AC
				INNER JOIN [DataEntry].[dbo].[ConsentForms] CF ON CF.ConsentFormId = AC.ConsentFormId
				INNER JOIN [Sales].[dbo].[Clinics] CL ON CF.ClinicId = CL.ClinicId
				INNER JOIN [RealMed].[dbo].[VwPayerMapping] PM on PM.[State] = CL.[State] AND PM.InPayerName = [RealMed].[dbo].[BillingInsuranceName] (CF.PrimaryInsuranceName, CF.PrimaryInsuranceOtherName, CF.PrimaryInsuranceOther)
			    LEFT JOIN [RealMed].[dbo].[VwEntityMapping] EMB ON PM.PayerEntityID = EMB.PayerEntityID AND EMB.EntityIdentifierCode = '85' --BillingProvider
				LEFT JOIN [RealMed].[dbo].[Information_Entity_Specialty_Info] IESB ON PM.PayerEntityId = IESB.PayerEntityId AND IESB.ProviderCode = 'BI' --BillingProviderTaxonomy
				LEFT JOIN [RealMed].[dbo].[VwEntityMapping] EMR ON PM.PayerEntityID = EMR.PayerEntityID AND EMR.EntityIdentifierCode = '82' --RenderingProvider
				LEFT JOIN [RealMed].[dbo].[Information_Entity_Specialty_Info] IESR ON PM.PayerEntityId = IESR.PayerEntityId AND IESR.ProviderCode = 'PE' --RenderingProviderTaxonomy
				LEFT JOIN [RealMed].[dbo].[VwEntityMapping] EMS ON PM.PayerEntityID = EMS.PayerEntityID AND EMS.EntityIdentifierCode = '77' --ServicingProvider
				LEFT JOIN [RealMed].[dbo].[PlaceOfServiceMapping] PSM ON PM.PayerEntityId = PSM.PayerEntityId
				LEFT JOIN [RealMed].[dbo].[PayerEntityFilingIndicatorMapping] FM ON PM.PayerEntityId = FM.PayerEntityId
				LEFT JOIN [RealMed].[dbo].[FilingIndicators] F ON FM.FilingIndicatorId = F.FilingIndicatorId;";

		public static readonly string getProviderOverrides =
			$@"DROP TABLE IF EXISTS #BaseData
			SELECT CF.ConsentFormId, C.PartnerId, C.ClinicId, PM.PayerEntityId, CF.ProviderId
			INTO #BaseData
			FROM [DataEntry].[dbo].[ConsentForms] CF
				INNER JOIN [Sales].[dbo].[Clinics] C ON CF.ClinicId = C.ClinicId
				INNER JOIN [RealMed].[dbo].[VwPayerMapping] PM on PM.[State] = C.[State] AND PM.InPayerName = [RealMed].[dbo].[BillingInsuranceName] (CF.PrimaryInsuranceName, CF.PrimaryInsuranceOtherName, CF.PrimaryInsuranceOther)
			WHERE CF.ConsentFormId = @id
			
			DROP TABLE IF EXISTS #ProviderData
			SELECT
				B.ConsentFormId,
				B.PartnerId,
				B.ClinicId,
				B.PayerEntityId,
				B.ProviderId,
				PO.EicId, 
				PO.InformationEntityId,
				PO.EntityAddressId,
				U.RenderingNPI RenderingNPISwitchNPI,
				U.FirstName RenderingNPISwitchFirstName,
				U.LastName RenderingNPISwitchLastName,
				R.Identification_Code FacilityRenderingProviderNPI,
				R.First_Name FacilityRenderingProviderFirstName,
				R.Last_Name_or_Organization_Name FacilityRenderingProviderLastName,
				BEL.Identification_Code BillingProviderOverrideNPI,
				IEA.Reference_Identification BillingProviderOverrideEIN,
				BEL.Last_Name_or_Organization_Name BillingProviderOverrideLastName,
				TRIM(BEA.Address_Information_1 + ' ' + BEA.Address_Information_2) BillingProviderOverrideAddressStreet,
				BEA.City_Name BillingProviderOverrideCity,
				BEA.State_or_Province_Code BillingProviderOverrideState,
				BEA.Postal_Code BillingProviderOverrideZipCode,
				REL.Identification_Code RenderingProviderOverrideNPI,
				REL.First_Name RenderingProviderOverrideFirstName,
				REL.Last_Name_or_Organization_Name RenderingProviderOverrideLastName,
				SEL.Identification_Code ServicingProviderOverrideNPI,
				SEL.Last_Name_or_Organization_Name ServicingProviderOverrideLastName,
				TRIM(SEA.Address_Information_1 + ' ' + SEA.Address_Information_2) ServicingProviderOverrideAddressStreet,
				SEA.City_Name ServicingProviderOverrideCity,
				SEA.State_or_Province_Code ServicingProviderOverrideState,
				SEA.Postal_Code ServicingProviderOverrideZipCode,
				IESB.ReferenceIdentification BillingProviderTaxonomyCode,
				IESR.ReferenceIdentification RenderingProviderTaxonomyCode
			INTO #ProviderData
			FROM #BaseData B
				LEFT JOIN [RealMed].[dbo].[InformationEntityPartnerOverrides] PO ON B.PartnerId = PO.PartnerId AND B.PayerEntityId = PO.PayerEntityId
				LEFT JOIN [Sales].[dbo].[Users] U ON B.ProviderId = U.UserId --Rendering NPI Switch
				LEFT JOIN [RealMed].[dbo].[Information_Entity_List] BEL ON BEL.Information_Entity_ID = PO.InformationEntityId AND PO.EicId = 3 --BillingProviderInformationOverride
				LEFT JOIN [RealMed].[dbo].[Information_Entity_Additional_Identifiers] IEA ON BEL.Identification_Code = IEA.Identification_Code AND IEA.Entity_Identifier_Code = 85 --BillingProviderInformationOverride
				LEFT JOIN [RealMed].[dbo].[Information_Entity_Address_List] BEA ON PO.EntityAddressId = BEA.Entity_Address_ID AND PO.EicId = 3 --BillingProviderInformationOverride
				LEFT JOIN [RealMed].[dbo].[Information_Entity_List] REL ON REL.Information_Entity_ID = PO.InformationEntityId AND PO.EicId = 2 --RenderingProviderInformationOverride
				LEFT JOIN [RealMed].[dbo].[Information_Entity_List] SEL ON SEL.Information_Entity_ID = PO.InformationEntityId AND PO.EicId = 1 --ServicingProviderInformationOverride
				LEFT JOIN [RealMed].[dbo].[Information_Entity_Address_List] SEA ON PO.EntityAddressId = SEA.Entity_Address_ID AND PO.EicId = 1 --ServicingProviderInformationOverride
				LEFT JOIN (SELECT TOP 1 FRPM.ClinicId, FRP.Entity_Type_Qualifier, FRP.Last_Name_or_Organization_Name, FRP.First_Name, FRP.Identification_Code
							FROM [RealMed].[dbo].[FacilityRenderingProviderMap] FRPM 
								INNER JOIN [RealMed].[dbo].[FacilityRenderingProvider] FRP ON FRP.RenderingID = FRPM.RenderingID
							WHERE FRPM.ClinicId = (SELECT ClinicId FROM #BaseData)
							) R ON B.ClinicID = R.ClinicID --FacilityBasedRenderingProvider
				LEFT JOIN [RealMed].[dbo].[Information_Entity_Specialty_Info] IESB ON B.PayerEntityId = IESB.PayerEntityId AND IESB.ProviderCode = 'BI' --BillingProviderTaxonomy
				LEFT JOIN [RealMed].[dbo].[Information_Entity_Specialty_Info] IESR ON B.PayerEntityId = IESR.PayerEntityId AND IESR.ProviderCode = 'PE' --RenderingProviderTaxonomy
			
			SELECT * FROM #ProviderData;";

		public static readonly string getPayerSwitchData =
			$@"DROP TABLE IF EXISTS #BaseData
			SELECT CF.ConsentFormId, C.PartnerId, C.ClinicId, PM.PayerEntityId, CF.ProviderId
			INTO #BaseData
			FROM [DataEntry].[dbo].[ConsentForms] CF
				INNER JOIN [Sales].[dbo].[Clinics] C ON CF.ClinicId = C.ClinicId
				INNER JOIN [RealMed].[dbo].[VwPayerMapping] PM on PM.[State] = C.[State] AND PM.InPayerName = [RealMed].[dbo].[BillingInsuranceName] (CF.PrimaryInsuranceName, CF.PrimaryInsuranceOtherName, CF.PrimaryInsuranceOther)
			WHERE CF.ConsentFormId = @id

			DROP TABLE IF EXISTS #PayerSwitchData
			SELECT
				B.ConsentFormId,
				B.PartnerId,
				B.ClinicId,
				B.PayerEntityId PayerSwitchMapFromPayerEntityId,
				PES.MapToPayerEntityId PayerSwitchMapToPayerEntityId,
				PM.OutPayerName PayerSwitchMapToPayerName,
				TRIM(EMB.FirstName + ' ' + EMB.LastNameOrOrganizationName) BillingProviderName,
				EMB.IdentificationCode BillingProviderNPI,
				EMB.ReferenceIdentification BillingProviderEIN,
				IESB.ReferenceIdentification BillingProviderTaxonomyCode,
				TRIM(EMB.Address1 + ' ' + EMB.Address2) BillingProviderAddressStreet,
				EMB.City BillingProviderAddressCity,
				EMB.[State] BillingProviderAddressState,
				EMB.ZipCode BillingProviderAddressZipCode,
				TRIM(EMS.FirstName + ' ' + EMS.LastNameOrOrganizationName) ServicingProviderName,
				EMS.IdentificationCode ServicingProviderNPI,
				CASE WHEN EMS.EntityAddressId IS NULL THEN TRIM(ISNULL(CL.Address1, '') + ' ' + ISNULL(CL.Address2, '')) ELSE TRIM(ISNULL(EMS.Address1, '') + ' ' + ISNULL(EMS.Address2, '')) END ServicingProviderAddressStreet,
				CASE WHEN EMS.EntityAddressId IS NULL THEN ISNULL(CL.City, '') ELSE ISNULL(EMS.City, '') END ServicingProviderAddressCity,
				CASE WHEN EMS.EntityAddressId IS NULL THEN ISNULL(CL.[State], '') ELSE ISNULL(EMS.[State], '') END ServicingProviderAddressState,
				CASE WHEN EMS.EntityAddressId IS NULL THEN ISNULL(CL.ZipCode, '') ELSE ISNULL(EMS.ZipCode, '') END ServicingProviderAddressZipCode
			INTO #PayerSwitchData
			FROM #BaseData B
				INNER JOIN [Sales].[dbo].[Clinics] CL ON B.ClinicId = CL.ClinicId
				LEFT JOIN [RealMed].[dbo].[PayerEntitySwitchMapping] PES ON B.PayerEntityId = PES.MapFromPayerEntityId
				LEFT JOIN [RealMed].[dbo].[VwPayerMapping] PM ON PES.MapToPayerEntityId = PM.PayerEntityId
				LEFT JOIN [RealMed].[dbo].[VwEntityMapping] EMB ON PM.PayerEntityID = EMB.PayerEntityID AND EMB.EntityIdentifierCode = '85' --BillingProvider
				LEFT JOIN [RealMed].[dbo].[Information_Entity_Specialty_Info] IESB ON PES.MapToPayerEntityId = IESB.PayerEntityId AND IESB.ProviderCode = 'BI' --BillingProviderTaxonomy
				LEFT JOIN [RealMed].[dbo].[VwEntityMapping] EMS ON PM.PayerEntityID = EMS.PayerEntityID AND EMS.EntityIdentifierCode = '77' --ServicingProvider
			
			SELECT * FROM #PayerSwitchData;";

		public static readonly string getClaimServicesData =
			$@"EXEC [DataEntry].[Billing].[GetBillableCptCodes] @id";

		public static readonly string getCptModifierData =
			$@"SELECT
				CFV.ConsentFormId,
				CFV.ConsentFormVaccinationId,
				CFV.VaccineCodeName AS CptCode,
				M.PayerEntityId,
				M.ModifierId,
				M.MinAgeMonths,
				M.MaxAgeMonths,
				M.[Priority],
				CM.CptModifierName
			FROM [DataEntry].[dbo].[ConsentFormVaccinations] CFV
				INNER JOIN [RealMed].[dbo].[PayerCptModifierMapping] M ON CFV.VaccineCodeName = M.CptCodeName
				INNER JOIN [RealMed].[dbo].[CptModifiers] CM ON M.ModifierID = CM.ModifierID
			WHERE CFV.ConsentFormId = @id;";
    }
}
