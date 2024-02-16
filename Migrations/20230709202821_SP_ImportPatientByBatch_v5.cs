using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class SP_ImportPatientByBatch_v5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"create or alter procedure [dbo].[ImportPatientByBatch]
(
	@sourceFileId int
	, @batchSize int
	, @resultStatusId int output
)
as
begin
    set nocount on

	declare @existingErrorsJson nvarchar(max)

	select @existingErrorsJson = ErrorsJson from ImportSourceFiles where Id = @sourceFileId

	declare @errors table ( rowNo int, error nvarchar(max) )
	declare @failedPatients table ( patientIdentifier nvarchar(100) )
	declare @deactivatedMedications table ( medicationId int )

	declare @id int, @pharmacyId int, @doctorId int, @doctorContactId int, @patientId int, @patientContactId int, @patientAddressId int, @rowError nvarchar(max), @nextFillDate datetime2
		, @medicationId int, @lastRefillNumber int, @drugSubGroup varchar(max), @medicationCategory varchar(50), @excludeMedication bit, @surplusDays int, @drugGenericName nvarchar(max)
		, @batchCount int = 0

	declare
		@rowNo int, @pharmacyName nvarchar(100), @pharmacyNPI nvarchar(50), @patientIdentifier nvarchar(100), @patientFirstName nvarchar(50)
		, @patientLastName nvarchar(50), @patientDateofBirth datetime2, @patientPrimaryAddress nvarchar(100), @patientPrimaryCity nvarchar(50), @patientPrimaryState nvarchar(50)
		, @patientPrimaryZipCode nvarchar(10), @patientPrimaryPhone nvarchar(20), @patientEmail nvarchar(100), @patientGender nvarchar(10), @patientLanguage nvarchar(20)
		, @patientRace nvarchar(20), @prescriberFirstName nvarchar(50), @prescriberLastName nvarchar(50), @prescriberNPI nvarchar(50), @prescriberPrimaryAddress nvarchar(100)
		, @prescriberPrimaryCity nvarchar(50), @prescriberPrimaryState nvarchar(50), @prescriberPrimaryZip nvarchar(10), @prescriberPrimaryPhone nvarchar(20), @prescriberFaxNumber nvarchar(20)
		, @rxNumber nvarchar(50), @refillNumber int, @dateFilled datetime2, @daysSupply int, @refillsRemaining int, @dispensedQuantity decimal(18, 2), @dispensedItemNDC nvarchar(50)
		, @dispensedItemName nvarchar(max), @directions nvarchar(max), @patientPaidAmount decimal(18, 2), @isPdcMedication bit, @drugSbdcName nvarchar(max), @refillDue bit

	select @pharmacyId = PharmacyId from ImportSourceFiles where Id = @sourceFileId

	select @id = min(Id) from ImportFileStagingData where ImportSourceFileId = @sourceFileId and IsProcessed = 0

	update ImportSourceFiles set ImportStatusId = 5, ImportStartTimeUTC = getutcdate() where Id = @sourceFileId and ImportStatusId = 4 -- Staging Completed

	while not @id is null and @batchCount < @batchSize
	begin
		begin tran t1

		begin try
			select
				@rowNo = RowNo, @pharmacyName = PharmacyName, @pharmacyNPI = PharmacyNPI, @patientIdentifier = PatientIdentifier, @patientFirstName = PatientFirstName
				, @patientLastName = PatientLastName, @patientDateofBirth = PatientDateofBirth, @patientPrimaryAddress = PatientPrimaryAddress, @patientPrimaryCity = PatientPrimaryCity
				, @patientPrimaryState = PatientPrimaryState, @patientPrimaryZipCode = PatientPrimaryZipCode, @patientPrimaryPhone = PatientPrimaryPhone, @patientEmail = PatientEmail
				, @patientGender = PatientGender, @patientLanguage = PatientLanguage, @patientRace = PatientRace, @prescriberFirstName = PrescriberFirstName, @prescriberLastName = PrescriberLastName
				, @prescriberNPI = PrescriberNPI, @prescriberPrimaryAddress = PrescriberPrimaryAddress, @prescriberPrimaryCity = PrescriberPrimaryCity, @prescriberPrimaryState = PrescriberPrimaryState
				, @prescriberPrimaryZip = PrescriberPrimaryZip, @prescriberPrimaryPhone = PrescriberPrimaryPhone, @prescriberFaxNumber = PrescriberFaxNumber, @rxNumber = RxNumber, @refillNumber = RefillNumber
				, @dateFilled = DateFilled, @daysSupply = DaysSupply, @refillsRemaining = RefillsRemaining, @dispensedQuantity = DispensedQuantity, @dispensedItemNDC = DispensedItemNDC
				, @dispensedItemName = trim(DispensedItemName), @directions = Directions, @patientPaidAmount = PatientPaidAmount
				, @drugSbdcName = trim(DrugSbdcName)
			from
				ImportFileStagingData
			where
				Id = @id

			if trim(isnull(@patientIdentifier, '')) = ''
			begin
				set @rowError = 'invalid patient identifier'
				insert @errors ( rowNo, error ) values ( @rowNo, @rowError )
			end
			else if not exists (select * from @failedPatients where patientIdentifier = @patientIdentifier)
			begin
				-- insert/update doctor

				select @doctorId = Id, @doctorContactId = contactId from doctor where npi = @prescriberNPI

				if @doctorId is null
				begin
					insert into contact ( firstName, lastName, primaryPhone, fax, dob )
					values ( @prescriberFirstName, @prescriberLastName, @prescriberPrimaryPhone, @prescriberFaxNumber, '01/01/0001' )

					set @doctorContactId = scope_identity()

					insert doctor ( npi, contactId, importSourceFileId )
					values ( @prescriberNPI, @doctorContactId, @sourceFileId )

					set @doctorId = scope_identity()
				end
				else
				begin
					update contact set
						firstName = @prescriberFirstName
						, lastName = @prescriberLastName
						, primaryPhone = @prescriberPrimaryPhone
						, fax = @prescriberFaxNumber
					where Id = @doctorContactId
				end

				-- insert/update patient

				select @patientId = Id, @patientContactId = contactId, @patientAddressId = addressId
				from patient where patientVendorRxID = @patientIdentifier and pharmacyId = @pharmacyId

				if @patientId is null
				begin
					insert [address] ( addressLineOne, city, [state], zipCode )
					values ( @patientPrimaryAddress, @patientPrimaryCity, @patientPrimaryState, @patientPrimaryZipCode )

					set @patientAddressId = scope_identity()

					insert into contact ( firstName, lastName, primaryPhone, primaryEmail, dob )
					values ( @patientFirstName, @patientLastName, @patientPrimaryPhone, @patientEmail, @patientDateofBirth )

					set @patientContactId = scope_identity()

					insert patient ( pharmacyId, addressId, contactId, [status], importSourceFileId, patientVendorRxID )
					values ( @pharmacyId, @patientAddressId, @patientContactId, 'New Patient', @sourceFileId, @patientIdentifier )

					set @patientId = scope_identity()
				end
				else
				begin
					-- set status to 'New Patient', for the user(s) to know that medications has been updated
					update patient set [status] = 'New Patient' where Id = @patientId

					-- donot update contact details of existing patient
					update [address] set
						addressLineOne = @patientPrimaryAddress
						, city = @patientPrimaryCity
						, [state] = @patientPrimaryState
						, zipCode = @patientPrimaryZipCode
					where Id = @patientAddressId
				end

				-- insert/update medication

				select top 1
					@drugSubGroup = trim(isnull(value_set_item, value_set_subgroup))
					, @medicationCategory = trim(case category when 'Statins' then 'Cholesterol' else category end)
				from
					Pdc_Medications where code = @dispensedItemNDC
				order by Id desc

				set @medicationCategory = nullif(isnull(trim(@medicationCategory), ''), '')

				set @nextFillDate = dateadd(d, @daysSupply, @dateFilled)

				set @refillDue = case when @nextFillDate <= getdate() then 1 else 0 end

				select top 1 @drugGenericName = trim(genericName) from rxNavMedication where ndcNumber = @dispensedItemNDC

				select @medicationId = Id, @lastRefillNumber = rfNumber from medication where patientId = @patientId and rxNumber = @rxNumber

				set @isPdcMedication = case when @medicationCategory in ('Cholesterol', 'Diabetes', 'RASA') then 1 else 0 end
				set @excludeMedication = case when @isPdcMedication = 1 then 0 else 1 end

				if @medicationId is null or @lastRefillNumber is null or @lastRefillNumber <> @refillNumber
				begin
					insert medication (
						patientId, rxNumber, rfNumber, refillsRemaining, rxDate, drugName, genericName
						, direction, quantity, supply
						, lastFillDate, nextFillDate, payDue, ndcNumber, drugSubGroup, condition
						, importSourceFileId, DoctorPrescribedId, isExclude, isInclude
						, isActive, inUse, refillDue, consumptionEndDate, sbdcName
					) values (
						@patientId, @rxNumber, @refillNumber, @refillsRemaining, @dateFilled, @dispensedItemName, nullif(@drugGenericName, '')
						, @directions, @dispensedQuantity, @daysSupply
						, @dateFilled, @nextFillDate, @patientPaidAmount, @dispensedItemNDC, @drugSubGroup, @medicationCategory
						, @sourceFileId, @doctorId, @excludeMedication, convert(bit, 1 - @excludeMedication)
						, 1, 1, @refillDue, @nextFillDate, @drugSbdcName
					)

					set @medicationId = scope_identity()

					-- insert/update medication consumptions

					if @isPdcMedication = 1
					begin
						declare @previousFillId int, @scheduledRefillDate date

						select top 1
							@previousFillId = Id
							, @scheduledRefillDate = nextFillDate
						from medication where
							patientId = @patientId
							and Id < @medicationId
							and condition = @medicationCategory
							and genericName = @drugGenericName
						order by
							lastFillDate desc

						if not @previousFillId is null and @scheduledRefillDate > @dateFilled
						begin
							update medication set consumptionEndDate = dateadd(d, -1, @dateFilled)
							where Id = @previousFillId
						end

						-- update refillDue for patient

						if @refillDue = 1
						begin
							update patient set
								cholesterolRefillDue = case @medicationCategory when 'Cholesterol' then 1 else cholesterolRefillDue end
								, diabetesRefillDue = case @medicationCategory when 'Diabetes' then 1 else diabetesRefillDue end
								, rasaRefillDue = case @medicationCategory when 'RASA' then 1 else rasaRefillDue end
							where
								Id = @patientId
								and (rasaRefillDue = 0 or cholesterolRefillDue = 0 or diabetesRefillDue = 0)
						end
					end

					-- deactivate old matching medications

					update medication set isActive = 0
					output deleted.Id into @deactivatedMedications ( medicationId )
					where
						patientId = @patientId -- this patient
						and Id <> @medicationId -- not this medication
						and isActive = 1 -- is currently active
						and (
							coalesce(sbdcName, genericName, drugName) = coalesce(@drugSbdcName, @drugGenericName, @dispensedItemName)
							or (
								(@isPdcMedication = 1 and genericName = @drugGenericName) -- matching generic name for PDC medications
								or (@isPdcMedication = 0 and drugName = @dispensedItemName) -- matching drug name for NON-PDC medications
							)
						)

					-- insert refill due activation queue for PDC medication

					if @isPdcMedication = 1 and @refillDue = 1 and dateadd(D, -5, @nextFillDate) >= getdate()
					begin
						insert RefillDueActivationQueue (
							PatientId, MedicationId, ActivationDate
						) values (
							@patientId, @medicationId, convert(date, dateadd(D, -5, @nextFillDate))
						)
					end
				end
			end

			update ImportFileStagingData set IsProcessed = 1, ErrorsJson = @rowError where Id = @id

			commit tran t1

		end try

		begin catch
			rollback tran t1

			set @rowError = error_message()
			insert @errors ( rowNo, error ) values ( @rowNo, @rowError )
			
			update ImportFileStagingData set IsProcessed = 1, ErrorsJson = @rowError where Id = @id

			insert into @failedPatients values ( @patientIdentifier )
		end catch

		select
			@rowNo = null, @pharmacyName = null, @pharmacyNPI = null, @patientIdentifier = null, @patientFirstName = null, @patientLastName = null, @patientDateofBirth = null
			, @patientPrimaryAddress = null, @patientPrimaryCity = null, @patientPrimaryState = null, @patientPrimaryZipCode = null, @patientPrimaryPhone = null, @patientEmail = null, @patientGender = null
			, @patientLanguage = null, @patientRace = null, @prescriberFirstName = null, @prescriberLastName = null, @prescriberNPI = null, @prescriberPrimaryAddress = null, @prescriberPrimaryCity = null
			, @prescriberPrimaryState = null, @prescriberPrimaryZip = null, @prescriberPrimaryPhone = null, @prescriberFaxNumber = null, @rxNumber = null, @refillNumber = null, @dateFilled = null, @daysSupply = null
			, @refillsRemaining = null, @dispensedQuantity = null, @dispensedItemNDC = null, @dispensedItemName = null, @directions = null, @patientPaidAmount = null
			, @doctorId = null, @doctorContactId = null, @patientId = null, @patientContactId = null, @patientAddressId = null, @rowError = null, @nextFillDate = null
			, @drugSubGroup = null, @medicationCategory = null, @excludeMedication = null, @surplusDays = null, @drugGenericName = null, @lastRefillNumber = null, @medicationId = null
			, @isPdcMedication = 0, @drugSbdcName = null, @refillDue = null

		select @id = min(Id) from ImportFileStagingData where ImportSourceFileId = @sourceFileId and IsProcessed = 0 and Id > @id

		set @batchCount = @batchCount + 1
	end

	-- remove refill queue items for deactivated medications

	if exists (select * from @deactivatedMedications)
	begin
		delete from RefillDueActivationQueue
		where MedicationId in (select medicationId from @deactivatedMedications)
	end

	-- update ImportSourceFiles based on processing results

	declare @batchErrorsJson nvarchar(max)
	declare @importStatusId int = 5 -- Import In Progress

	if exists (select * from @errors)
	begin
		select @batchErrorsJson = (select [row] = rowNo, error from @errors for json auto, root('importErrors'))

		if not @existingErrorsJson is null
		begin
			select @existingErrorsJson = json_modify(
			   @existingErrorsJson,
			   'append $.importErrors',
			   json_query([value])
			)
			from openjson(@batchErrorsJson, '$.importErrors')
		end
		else
		begin
			set @existingErrorsJson = @batchErrorsJson
		end
	end

	declare @successCount int = 0

	select @successCount = count(*) from ImportFileStagingData where ImportSourceFileId = @sourceFileId and IsProcessed = 1

	if not exists (select * from ImportFileStagingData where ImportSourceFileId = @sourceFileId and IsProcessed = 0)
	begin
		update ImportSourceFiles set
			ImportEndTimeUTC = getutcdate()
			, TotalImported = @successCount
			, ErrorsJson = isnull(@existingErrorsJson, ErrorsJson)
		where
			Id = @sourceFileId

		update ImportSourceFiles set
			ImportStatusId = case
				when TotalImported = TotalRecords and ErrorsJson is null then 6 -- Imported
				when TotalImported = 0 then 7 -- Failed
				else 8 -- Partially Imported
			end
		where
			Id = @sourceFileId

		-- update medication status of this pharmacy
		exec [DeactivateUnusedMedications] @pharmacyId = @pharmacyId, @patientId = null
	end
	else
	begin
		update ImportSourceFiles set
			TotalImported = @successCount
			, ErrorsJson = isnull(@existingErrorsJson, ErrorsJson)
		where
			Id = @sourceFileId
	end

	select @resultStatusId = ImportStatusId from ImportSourceFiles where Id = @sourceFileId

	set nocount off
end";

            migrationBuilder.Sql(sqlScript);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
