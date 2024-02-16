using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OntrackDb.Migrations
{
    public partial class SP_ImportPatient_v6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlScript = @"create or alter procedure [dbo].[ImportPatient]
(
	@sourceFileId int
	, @resultStatusId int output
)
as
begin
    set nocount on

	set @resultStatusId = null

	if not exists (select * from ImportSourceFiles where Id = @sourceFileId and ImportStatusId = 4) return;

	declare @errors table ( rowNo int, error nvarchar(max) )
	declare @failedPatients table ( patientIdentifier nvarchar(100) )

	declare @id int, @pharmacyId int, @doctorId int, @doctorContactId int, @patientId int, @patientContactId int, @patientAddressId int, @rowError nvarchar(max), @nextFillDate datetime2
		, @medicationId int, @lastRefillNumber int, @drugSubGroup varchar(max), @medicationCategory varchar(50), @excludeMedication bit, @surplusDays int, @drugGenericName nvarchar(max)
		, @successCount int = 0

	declare
		@rowNo int, @pharmacyName nvarchar(100), @pharmacyNPI nvarchar(50), @patientIdentifier nvarchar(100), @patientFirstName nvarchar(50)
		, @patientLastName nvarchar(50), @patientDateofBirth datetime2, @patientPrimaryAddress nvarchar(100), @patientPrimaryCity nvarchar(50), @patientPrimaryState nvarchar(50)
		, @patientPrimaryZipCode nvarchar(10), @patientPrimaryPhone nvarchar(20), @patientEmail nvarchar(100), @patientGender nvarchar(10), @patientLanguage nvarchar(20)
		, @patientRace nvarchar(20), @prescriberFirstName nvarchar(50), @prescriberLastName nvarchar(50), @prescriberNPI nvarchar(50), @prescriberPrimaryAddress nvarchar(100)
		, @prescriberPrimaryCity nvarchar(50), @prescriberPrimaryState nvarchar(50), @prescriberPrimaryZip nvarchar(10), @prescriberPrimaryPhone nvarchar(20), @prescriberFaxNumber nvarchar(20)
		, @rxNumber nvarchar(50), @refillNumber int, @dateFilled datetime2, @daysSupply int, @refillsRemaining int, @dispensedQuantity decimal(18, 2), @dispensedItemNDC nvarchar(50)
		, @dispensedItemName nvarchar(max), @directions nvarchar(max), @patientPaidAmount decimal(18, 2)

	select @pharmacyId = PharmacyId from ImportSourceFiles where Id = @sourceFileId

	select @id = min(Id) from ImportFileStagingData where ImportSourceFileId = @sourceFileId and IsProcessed = 0

	update ImportSourceFiles set ImportStatusId = 5, ImportStartTimeUTC = getutcdate() where Id = @sourceFileId

	while not @id is null
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
				, @dispensedItemName = DispensedItemName, @directions = Directions, @patientPaidAmount = PatientPaidAmount
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

				set @nextFillDate = dateadd(d, @daysSupply, @dateFilled)
				set @excludeMedication = case when isnull(@medicationCategory, '') in ('Cholesterol', 'Diabetes', 'RASA') then 0 else 1 end

				select top 1 @drugGenericName = genericName from rxNavMedication where ndcNumber = @dispensedItemNDC

				select @medicationId = Id, @lastRefillNumber = rfNumber from medication where patientId = @patientId and rxNumber = @rxNumber

				if @medicationId is null or @lastRefillNumber is null or @lastRefillNumber <> @refillNumber
				begin
					insert medication (
						patientId, rxNumber, rfNumber, refillsRemaining, rxDate, drugName, genericName, direction, quantity, supply
						, lastFillDate, nextFillDate, payDue, ndcNumber, drugSubGroup, condition, importSourceFileId
						, DoctorPrescribedId, isExclude, isInclude
					) values (
						@patientId, @rxNumber, @refillNumber, @refillsRemaining, @dateFilled, @dispensedItemName, @drugGenericName, @directions, @dispensedQuantity, @daysSupply
						, @dateFilled, @nextFillDate, @patientPaidAmount, @dispensedItemNDC, @drugSubGroup, @medicationCategory, @sourceFileId
						, @doctorId, @excludeMedication, convert(bit, 1 - @excludeMedication)
					)

					-- insert/update medication consumptions

					if @excludeMedication = 0 and isnull(@drugSubGroup, '') <> '' and isnull(@medicationCategory, '') <> ''
					begin
						declare @prevSubGroup varchar(max), @prevScheduledFillDate date

						select top 1 @prevSubGroup = drugSubGroup, @prevScheduledFillDate = [date] from medicationConsumptions where 
							patientId = @patientId and condition = @medicationCategory and [date] between @dateFilled and @nextFillDate and [status] = 1
							order by [Id] desc

						if not @prevSubGroup is null
						begin
							if @prevSubGroup = @drugSubGroup and @dateFilled < @prevScheduledFillDate
							begin
								set @surplusDays = datediff(day, @dateFilled, @prevScheduledFillDate)
							end

							-- mark overlapping records of previous consumptions as inactive/invalid
							update medicationConsumptions set [status] = 0 where
								patientId = @patientId and condition = @medicationCategory and [date] >= @dateFilled
						end

						declare @date date = @dateFilled
						declare @endDate date = dateadd(day, isnull(@surplusDays, 0), @nextFillDate)

						while @date < @endDate
						begin
							insert medicationConsumptions (
								patientId, rxNumber, [date], [status], drugSubGroup, ndcNumber, condition, importSourceFileId
							) values (
								@patientId, @rxNumber, @date, 1, @drugSubGroup, @dispensedItemNDC, @medicationCategory, @sourceFileId
							)

							set @date = dateadd(day, 1, @date)
						end
					end
				end
			end

			update ImportFileStagingData set IsProcessed = 1, ErrorsJson = @rowError where Id = @id

			set @successCount = @successCount + 1

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

		select @id = min(Id) from ImportFileStagingData where ImportSourceFileId = @sourceFileId and IsProcessed = 0 and Id > @id
	end

	declare @errorsJson nvarchar(max)
	declare @importStatusId int = 6 -- Imported

	if exists (select * from @errors)
	begin
		select @errorsJson = (select [row] = rowNo, error from @errors for json auto, root('importErrors'))
		set @importStatusId = 8 -- Partially Imported
	end

	if @successCount = 0 set @importStatusId = 7 -- Failed

	update ImportSourceFiles set ImportStatusId = @importStatusId, TotalImported = @successCount, ImportEndTimeUTC = getutcdate(), ErrorsJson = @errorsJson where Id = @sourceFileId

	set @resultStatusId = @importStatusId

	-- update medication status of this pharmacy
	exec [DeactivateUnusedMedications] @pharmacyId = @pharmacyId, @patientId = null

	set nocount off
end";

            migrationBuilder.Sql(sqlScript);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
