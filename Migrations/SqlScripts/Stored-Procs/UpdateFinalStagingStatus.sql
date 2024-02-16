set ansi_nulls on
go
set quoted_identifier on
go

--drop procedure if exists [UpdateFinalStagingStatus]

create or alter procedure [dbo].[UpdateFinalStagingStatus]
(
	@sourceFileId int
	, @resultStatusId int output
)
as
begin
    set nocount on

	set @resultStatusId = null

	if exists (select * from ImportSourceFiles where Id = @sourceFileId and ImportStatusId = 3) -- Staging In Progress
	begin
		declare
			@totalRecords int
			, @errorsJson nvarchar(max)
		
		set @errorsJson = (
			select
				[row] = RowNo, [error] = ErrorsJson
			from
				ImportFileStagingData
			where
				ImportSourceFileId = @sourceFileId
				and not ErrorsJson is null
			for json auto, root ('stagingErrors')
		)

		select @totalRecords = count(*) from ImportFileStagingData where ImportSourceFileId = @sourceFileId

		set @resultStatusId = case when @errorsJson is null then 4 else 7 end

		update ImportSourceFiles set
			TotalRecords = @totalRecords
			, ErrorsJson = @errorsJson
			, ImportStatusId = @resultStatusId
			, StagingEndTimeUTC = sysutcdatetime()
		where Id = @sourceFileId
	end

	set nocount off
end
go
