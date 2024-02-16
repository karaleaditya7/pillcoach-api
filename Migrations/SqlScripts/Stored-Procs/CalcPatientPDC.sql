set ansi_nulls on
go
set quoted_identifier on
go

--drop procedure if exists [CalcPatientPDC]

create or alter procedure [dbo].[CalcPatientPDC]
(
	@patientId int
	, @months int
	, @reportMonth date
)
as
begin
    set nocount on

	declare @pdc_numbers table (
		id int identity
		, condition varchar(20)
		, pdc decimal(5, 2)
		, startDate date
		, endDate date
		, totalDays int
		, totalFills int
		, hasExclusions bit
		, consumptions int
	)

	insert @pdc_numbers ( condition ) values('Cholesterol'), ('Diabetes'), ('RASA')

	if day(@reportMonth) != 1 set @reportMonth = dateadd(d, -day(@reportMonth) + 1, @reportMonth)

	declare @startDate date, @endDate date, @totalConsumptionDays int, @totalDays int, @minThresholdDays int = 91

	set @endDate = eomonth(dateadd(m, -2, @reportMonth))
	set @startDate = dateadd(m, -@months, dateadd(d, 1, @endDate))

	if @months = 12
	begin
		set @startDate = convert(date, formatmessage('1/1/%i', year(@endDate)))

		if month(@endDate) = 3 set @minThresholdDays = 90 -- min days is 90 for range Jan-Mar, as Feb mostly has 28 days and sum of these three months is usually 90 days
	end

	declare @id int, @lastRefillDate date, @totalFills int, @pdc decimal(5, 2) = 0, @condition varchar(20), @hasExclusions bit

	declare @consumption_calendar table ( [condition] varchar(20), [usage_date] date )
	declare @consumption_ranges table ( id int identity, medication_id int, [condition] varchar(20), [start_date] date, [end_date] date )
	
	declare @consumption_id int, @consumption_start_date date, @consumption_end_date date

	select @id = min(id) from @pdc_numbers

	while not @id is null
	begin
		select @condition = condition from @pdc_numbers where id = @id

		if @months = 6 set @startDate = dateadd(m, -@months, dateadd(d, 1, @endDate))
		else set @startDate = convert(date, formatmessage('1/1/%i', year(@endDate)))

		select @lastRefillDate = null, @totalFills = 0, @pdc = 0, @totalConsumptionDays = 0, @totalDays = 0, @hasExclusions = 0

		select @consumption_id = null, @consumption_start_date = null, @consumption_end_date = null

		select @lastRefillDate = min(lastFillDate), @totalFills = count(distinct lastFillDate) from medication
		where patientId = @patientId and condition = @condition and lastFillDate between @startDate and @endDate

		delete @consumption_ranges

		if not exists (
			-- ensure that there are no exclusions
			select * from Pdc_Medications where category in ('Insulins', 'Sacubitril_Valsartan') and code in (
				select distinct ndcNumber from medication where patientId = @patientId and lastFillDate between @startDate and @endDate
					and condition in (@condition, 
						case @condition
							when 'Diabetes' then 'Insulins'
							when 'RASA' then 'Sacubitril_Valsartan'
							else '-' 
						end
					)
			)
		)
		begin
			if @totalFills >= 2 and not @lastRefillDate is null
			begin
				if @lastRefillDate > @startDate set @startDate = @lastRefillDate

				set @totalDays = datediff(d, @startDate, @endDate) + 1 -- +1 to include both start and end dates

				if @totalDays >= @minThresholdDays
				begin
					;with applicable_medications as (
						select Id, lastFillDate, consumptionEndDate from medication
						where patientId = @patientId and condition = @condition
						and lastFillDate between @startDate and @endDate
					)
					insert @consumption_ranges (
						[medication_id], [condition], [start_date], [end_date]
					)
					select Id, @condition, lastFillDate, case
							when consumptionEndDate > @endDate then @endDate
							else consumptionEndDate
						end
					from
						applicable_medications m

					select @consumption_id = min(id) from @consumption_ranges

					while not @consumption_id is null
					begin
						select @consumption_start_date = [start_date], @consumption_end_date = [end_date] from @consumption_ranges where id = @consumption_id

						insert into @consumption_calendar ( condition, usage_date )
						select @condition, s.[date_value] from dbo.GenerateDateSeries(@consumption_start_date, @consumption_end_date) s
						where not exists (select * from @consumption_calendar where condition = @condition and usage_date = s.date_value)

						select @consumption_id = min(id) from @consumption_ranges where id > @consumption_id
					end
				end
			end
		end
		else
		begin
			set @hasExclusions = 1
		end

		if @totalDays = 0 set @totalDays = datediff(d, @startDate, @endDate) + 1

		select @totalConsumptionDays = count(*) - 1 from @consumption_calendar where condition = @condition

		if @totalConsumptionDays > 0 set @pdc = ((@totalConsumptionDays * 1.0) / @totalDays) * 100

		update @pdc_numbers set
			pdc = @pdc
			, startDate = @startDate
			, endDate = @endDate
			, totalDays = isnull(@totalDays, 0)
			, totalFills = @totalFills
			, hasExclusions = @hasExclusions
			, consumptions = isnull(@totalConsumptionDays, 0)
		where id = @id

		select @id = min(id) from @pdc_numbers where id > @id
	end

	select @patientId, @reportMonth, @months, condition, pdc, startDate, endDate, totalDays, totalFills, hasExclusions, consumptions from @pdc_numbers

	set nocount off
end
go
