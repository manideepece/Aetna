CREATE proc  dbo.sp_Team_Maintenance
as
begin

	select team.Team_ID as 'TeamMaintenanceID',
	team.TEAM_CD as 'TeamCode',
	LTRIM(RTRIM(team.TEAM_DESCR)) as 'TeamName',
	team.CTRL_COUNT as 'CtrlCnt',
	STUFF((SELECT DISTINCT ',' + LTRIM(RTRIM(rpt.REPORT_DESC)) FROM BTT_SEC_TEAM_RPT_MAPPING_N trpt join BTTRPT_NAM_LB_N rpt on trpt.REPORT_CD = rpt.REPORT_CD WHERE trpt.TEAM_ID = team.TEAM_ID
			FOR XML PATH('')),1,1,'') [Reports],
	STUFF((SELECT DISTINCT ',' + LTRIM(RTRIM(reg.REGION_DESCR)) FROM BTT_SEC_TEAM_REG_SEG_MAPPING_N tgs join BTT_SEC_REG_SEG_MAPPING_N rs on tgs.REGION_SEGMENT_MAPPING_ID = rs.REGION_SEGMENT_MAPPING_ID
			join BTT_SEC_REG_LKP_N reg on rs.REGION_ID = reg.REGION_ID WHERE tgs.TEAM_ID = team.TEAM_ID
			FOR XML PATH('')),1,1,'') [Region],
	STUFF((SELECT DISTINCT ',' + LTRIM(RTRIM(seg.SUB_SEGMENT_DESCR)) FROM BTT_SEC_TEAM_REG_SEG_MAPPING_N tgs join BTT_SEC_REG_SEG_MAPPING_N rs on tgs.REGION_SEGMENT_MAPPING_ID = rs.REGION_SEGMENT_MAPPING_ID
			join BTT_SEC_SUB_SEGMENT_LKP_N seg on rs.SUB_SEGMENT_ID = seg.SUB_SEGMENT_ID WHERE tgs.TEAM_ID = team.TEAM_ID
			FOR XML PATH('')),1,1,'') [Subsegment]
	from BTT_SEC_TEAM_LKP_N team

end
----------------------------------------------------------------------------
alter proc sp_SEC_Reports
as
begin
	select REPORT_CD,
		   LTRIM(RTRIM(REPORT_DESC)) as 'REPORT_DESC',
		   APP_RTGRP_ACC_CD,
		   CREAT_BY_ID,
		   CREAT_TMSTMP,
		   UPDT_BY_ID,
		   UPDT_TMSTMP,
		   SEL_RPT_ACRONYM,
		   REPORT_SORT_ORDER
		   from BTTRPT_NAM_LB_N
end

---------------------------------------------------------------------------

create proc sp_SEC_Regions
as
begin
	select REGION_ID,
		   LTRIM(RTRIM(REGION_CD)) AS 'REGION_CD',
		   LTRIM(RTRIM(REGION_DESCR)) AS 'REGION_DESCR',
		   CREAT_BY_ID,
		   CREAT_TMSTMP,
		   UPDT_BY_ID,
		   UPDT_TMSTMP
		   from BTT_SEC_REG_LKP_N
end

-------------------------------------------------------------------------------

CREATE proc sp_SEC_Subsegments
as
begin
	select SUB_SEGMENT_ID,
		   LTRIM(RTRIM(SUB_SEGMENT_CD)) AS 'SUB_SEGMENT_CD',
		   LTRIM(RTRIM(SUB_SEGMENT_DESCR)) AS 'SUB_SEGMENT_DESCR',
		   FOOTNOTE_IND,
		   LTRIM(RTRIM(CREAT_BY_ID)) AS 'CREAT_BY_ID',
		   CREAT_TMSTMP,
		   UPDT_BY_ID,
		   UPDT_TMSTMP
		   from BTT_SEC_SUB_SEGMENT_LKP_N
end

-------------------------------------------------------------------------------------

ALTER proc [dbo].[sp_SEC_Add_Team_Maintenance]
@teamCode varchar(5),
@teamDescription varchar(100),
@ctrlCount int,
@reports varchar(255),
@regions varchar(255),
@subsegments varchar(255),
@modifiedUser varchar(100)
as
begin
	
	if exists (select * from BTT_SEC_TEAM_LKP_N where LTRIM(RTRIM(TEAM_CD)) = @teamCode)
	begin
		SELECT 'Team Code already exists!'
	end
	else
	begin
		insert into BTT_SEC_TEAM_LKP_N values (@teamCode, @teamDescription,@ctrlCount,@modifiedUser,GETDATE(),@modifiedUser,GETDATE())
		declare @id int = @@identity

		create table #reports (Report_CD varchar(2))

		insert into #reports
		SELECT rcsId = y.i.value('(./text())[1]', 'varchar(2)')             
		  FROM 
		  ( 
			SELECT 
				n = CONVERT(XML, '<i>' 
					+ REPLACE((SELECT STUFF((SELECT ',' + @reports FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
					+ '</i>')
		  ) AS a 
		  CROSS APPLY n.nodes('i') AS y(i)

		insert into BTT_SEC_TEAM_RPT_MAPPING_N
		select @id, rpt.Report_CD,@modifiedUser,GETDATE(),@modifiedUser,GETDATE() 
		from #reports rep join BTTRPT_NAM_LB_N rpt on rep.Report_CD = rpt.Report_CD 

		drop table #reports

		create table #regions (REGION_ID int)

		insert into #regions
		SELECT rcsId = y.i.value('(./text())[1]', 'int')             
		  FROM 
		  ( 
			SELECT 
				n = CONVERT(XML, '<i>' 
					+ REPLACE((SELECT STUFF((SELECT ',' + @regions FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
					+ '</i>')
		  ) AS a 
		  CROSS APPLY n.nodes('i') AS y(i)

		create table #subSegments (SUB_SEGMENT_ID int)

		insert into #subSegments
		SELECT rcsId = y.i.value('(./text())[1]', 'int')             
		  FROM 
		  ( 
			SELECT 
				n = CONVERT(XML, '<i>' 
					+ REPLACE((SELECT STUFF((SELECT ',' + @subsegments FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
					+ '</i>')
		  ) AS a 
		  CROSS APPLY n.nodes('i') AS y(i)

		insert into BTT_SEC_TEAM_REG_SEG_MAPPING_N
		select @id, 
				(select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID = tmp.REGION_ID and SUB_SEGMENT_ID = tmp.SUB_SEGMENT_ID),
				@modifiedUser,GETDATE(),@modifiedUser,GETDATE() from (select * from #regions cross join #subSegments) tmp
				where (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID = tmp.REGION_ID and SUB_SEGMENT_ID = tmp.SUB_SEGMENT_ID)
				is not null

		drop table #regions
		drop table #subSegments

		SELECT 'Saved Successfully!'
	end
	
end

------------------------------------------------------------------------------------------------------

create proc sp_SEC_Edit_Team_Maintenance
@teamId int,
@column varchar(100),
@value varchar(max)
as
begin
	if @column = 'TeamCode'
	begin
		Update BTT_SEC_TEAM_LKP_N set TEAM_CD = @value where TEAM_ID = @teamId
	end
	else if @column = 'TeamName'
	begin
		Update BTT_SEC_TEAM_LKP_N set TEAM_DESCR = @value where TEAM_ID = @teamId
	end
	else if @column = 'CtrlCnt'
	begin
		Update BTT_SEC_TEAM_LKP_N set CTRL_COUNT = @value where TEAM_ID = @teamId
	end
	else if @column = 'Reports'
	begin
		create table #reports (Report_CD varchar(2))

		insert into #reports
		SELECT rcsId = y.i.value('(./text())[1]', 'varchar(2)')             
		  FROM 
		  ( 
			SELECT 
				n = CONVERT(XML, '<i>' 
					+ REPLACE((SELECT STUFF((SELECT ',' + @value FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
					+ '</i>')
		  ) AS a 
		  CROSS APPLY n.nodes('i') AS y(i)

		insert into BTT_SEC_TEAM_RPT_MAPPING_N
		select @teamId, rpt.Report_CD,'System',GETDATE(),'System',GETDATE() 
		from #reports rep join BTTRPT_NAM_LB_N rpt on rep.Report_CD = rpt.Report_CD
		where rpt.Report_CD not in (select distinct REPORT_CD from BTT_SEC_TEAM_RPT_MAPPING_N where TEAM_ID = @teamId)

		delete from BTT_SEC_TEAM_RPT_MAPPING_N where TEAM_ID = @teamId and REPORT_CD not in (select * from #reports)

		drop table #reports
	end
	else if @column = 'Region'
	begin
		create table #regions (REGION_ID int)

		insert into #regions
		SELECT rcsId = y.i.value('(./text())[1]', 'int')             
		  FROM 
		  ( 
			SELECT 
				n = CONVERT(XML, '<i>' 
					+ REPLACE((SELECT STUFF((SELECT ',' + @value FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
					+ '</i>')
		  ) AS a 
		  CROSS APPLY n.nodes('i') AS y(i)

		  insert into BTT_SEC_TEAM_REG_SEG_MAPPING_N
		  select @teamId, 
		  (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID = tmp.REGION_ID and SUB_SEGMENT_ID = tmp.SUB_SEGMENT_ID),
		  'System',GETDATE(),'System',GETDATE() from 
		  (select * from #regions cross join (select distinct SUB_SEGMENT_ID from BTT_SEC_REG_SEG_MAPPING_N rs join BTT_SEC_TEAM_REG_SEG_MAPPING_N trs on rs.REGION_SEGMENT_MAPPING_ID = trs.REGION_SEGMENT_MAPPING_ID where TEAM_ID = @teamId)a) tmp
		  where REGION_ID not in (select distinct REGION_ID from BTT_SEC_REG_SEG_MAPPING_N rs join BTT_SEC_TEAM_REG_SEG_MAPPING_N trs on rs.REGION_SEGMENT_MAPPING_ID = trs.REGION_SEGMENT_MAPPING_ID where TEAM_ID = @teamId) 
		  and (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID = tmp.REGION_ID and SUB_SEGMENT_ID = tmp.SUB_SEGMENT_ID) is not null

		  delete from BTT_SEC_TEAM_REG_SEG_MAPPING_N where TEAM_ID = @teamId and REGION_SEGMENT_MAPPING_ID not in
		  (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N rs join #regions reg on rs.REGION_ID = reg.REGION_ID )

		  drop table #regions
	end
	else if @column = 'Subsegment'
	begin
		create table #subSegments (SUB_SEGMENT_ID int)

		insert into #subSegments
		SELECT rcsId = y.i.value('(./text())[1]', 'int')             
		  FROM 
		  ( 
			SELECT 
				n = CONVERT(XML, '<i>' 
					+ REPLACE((SELECT STUFF((SELECT ',' + @value FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
					+ '</i>')
		  ) AS a 
		  CROSS APPLY n.nodes('i') AS y(i)

		  insert into BTT_SEC_TEAM_REG_SEG_MAPPING_N
		  select @teamId, 
		  (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID = tmp.REGION_ID and SUB_SEGMENT_ID = tmp.SUB_SEGMENT_ID),
		  'System',GETDATE(),'System',GETDATE() from 
		  (select * from #subSegments cross join (select distinct REGION_ID from BTT_SEC_REG_SEG_MAPPING_N rs join BTT_SEC_TEAM_REG_SEG_MAPPING_N trs on rs.REGION_SEGMENT_MAPPING_ID = trs.REGION_SEGMENT_MAPPING_ID where TEAM_ID = @teamId)a) tmp
		  where SUB_SEGMENT_ID not in (select distinct SUB_SEGMENT_ID from BTT_SEC_REG_SEG_MAPPING_N rs join BTT_SEC_TEAM_REG_SEG_MAPPING_N trs on rs.REGION_SEGMENT_MAPPING_ID = trs.REGION_SEGMENT_MAPPING_ID where TEAM_ID = @teamId)
		  AND (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID = tmp.REGION_ID and SUB_SEGMENT_ID = tmp.SUB_SEGMENT_ID) is not null

		 delete from BTT_SEC_TEAM_REG_SEG_MAPPING_N where TEAM_ID = @teamId and REGION_SEGMENT_MAPPING_ID not in
		 (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N rs join #subSegments seg on rs.SUB_SEGMENT_ID = seg.SUB_SEGMENT_ID )

		 drop table #subSegments
	end
end

---------------------------------------------------------------------------------------------------------

ALTER proc [dbo].[sp_SEC_Add_Region_Maintenance]
@regionCode varchar(5),
@regionDescription varchar(30),
@modifiedUser varchar(100)
as
begin
	if exists (select * from BTT_SEC_REG_LKP_N where LTRIM(RTRIM(REGION_CD)) = @regionCode)
	begin
		SELECT 'Region Code already exists'
	end
	else
	begin
		insert into BTT_SEC_REG_LKP_N values (@regionCode, @regionDescription,@modifiedUser,GETDATE(),@modifiedUser,GETDATE())
		declare @id int = @@identity

		insert into BTT_SEC_REG_SEG_MAPPING_N
		select @id, SUB_SEGMENT_ID, @modifiedUser, GETDATE(), @modifiedUser, GETDATE()
		from BTT_SEC_SUB_SEGMENT_LKP_N reg

		SELECT 'Saved Successfully!'
	end
end

-----------------------------------------------------------------------------------------------------------

create proc sp_SEC_Edit_Region_Maintenance
@regionId int,
@column varchar(100),
@value varchar(max)
as
begin
	if @column = 'REGION_CD'
	begin
		Update BTT_SEC_REG_LKP_N set REGION_CD = @value where REGION_ID = @regionId
	end
	else if @column = 'REGION_DESCR'
	begin
		Update BTT_SEC_REG_LKP_N set REGION_DESCR = @value where REGION_ID = @regionId
	end
end

---------------------------------------------------------------------------------------

ALTER proc [dbo].[sp_SEC_Add_Subsegment_Maintenance]
@subSegmentCode varchar(5),
@subSegmentDescription varchar(30),
@modifiedUser varchar(100)
as
begin

	if exists (select * from BTT_SEC_SUB_SEGMENT_LKP_N where LTRIM(RTRIM(SUB_SEGMENT_CD)) = @subSegmentCode)
	begin
		SELECT 'Subsegment Code already exists'
	end
	else
	begin
		insert into BTT_SEC_SUB_SEGMENT_LKP_N values (@subSegmentCode, @subSegmentDescription, 'N', @modifiedUser,GETDATE(),@modifiedUser,GETDATE())
		declare @id int = @@identity

		insert into BTT_SEC_REG_SEG_MAPPING_N
		select REGION_ID, @id, @modifiedUser, GETDATE(), @modifiedUser, GETDATE()
		from BTT_SEC_REG_LKP_N reg

		SELECT 'Saved Successfully!'
	end
-----------------------------------------------------------------------------------------

create proc sp_SEC_Edit_Subsegment_Maintenance
@subsegmentId int,
@column varchar(100),
@value varchar(max)
as
begin
	if @column = 'SUB_SEGMENT_CD'
	begin
		Update BTT_SEC_SUB_SEGMENT_LKP_N set SUB_SEGMENT_CD = @value where SUB_SEGMENT_ID = @subsegmentId
	end
	else if @column = 'SUB_SEGMENT_DESCR'
	begin
		Update BTT_SEC_SUB_SEGMENT_LKP_N set SUB_SEGMENT_DESCR = @value where SUB_SEGMENT_ID = @subsegmentId
	end
end

-------------------------------------------------------------------------------------------------------------

create proc sp_SEC_Teams
as
begin
	select TEAM_ID,
		   LTRIM(RTRIM(TEAM_CD)) AS 'TEAM_CD',
		   LTRIM(RTRIM(TEAM_DESCR)) AS 'TEAM_DESCR',
		   CTRL_COUNT,
		   CREAT_BY_ID,
		   CREAT_TMSTMP,
		   UPDT_BY_ID,
		   UPDT_TMSTMP
		   from BTT_SEC_TEAM_LKP_N
end
------------------------------------------------------------------------------------------------------------

create proc sp_User_Team_Mapping
as
begin
	select distinct LTRIM(RTRIM(USER_ID)) as 'USER_ID',
		   (select top 1 LTRIM(RTRIM(FIRST_NAM)) from BTTSEC_USER_N where UDT_USER_ID = utm.USER_ID) as 'FIRST_NAM',
		   (select top 1 LTRIM(RTRIM(LAST_NAM)) from BTTSEC_USER_N where UDT_USER_ID = utm.USER_ID) as 'LAST_NAM',
		   (select top 1 LTRIM(RTRIM(EMP_STS_CD)) from BTTSEC_USER_N where UDT_USER_ID = utm.USER_ID) as 'EMP_STS_CD',
		   STUFF((SELECT DISTINCT ',' + ISNULL(LTRIM(RTRIM(TEAM_DESCR)),'') FROM BTT_SEC_TEAM_LKP_N team join BTT_SEC_USER_TEAM_MAPPING_N tm on team.TEAM_ID = tm.TEAM_ID  WHERE USER_ID = utm.USER_ID
			FOR XML PATH('')),1,1,'') [TEAMS]
	from BTT_SEC_USER_TEAM_MAPPING_N utm join BTT_SEC_TEAM_LKP_N team on utm.team_id = team.team_id
end

--------------------------------------------------------------------------------------------------------------
ALTER proc [dbo].[sp_SEC_Add_User_Team_Mapping]
@userId varchar(18),
@firstName varchar(15),
@LastName varchar(25),
@employeeStatus varchar(1),
@teams varchar(255),
@modifiedUser varchar(100)
as
begin
	
	if exists (select * from BTTSEC_USER_N where LTRIM(RTRIM(UDT_USER_ID)) = @userId)
	begin
		SELECT 'User Id already exists'
	end
	else
	begin
		insert into BTTSEC_USER_N values (@userId, @firstName,@LastName,'','A','NewAccount', 'NewAccount', 'NewAccount', 'NewAccount', 'NewAccount', 'NewAccount', GETDATE() + 60, 'ALL', GETDATE(),@modifiedUser,GETDATE(),@modifiedUser)

		create table #teams (TEAM_ID int)

		insert into #teams
		SELECT rcsId = y.i.value('(./text())[1]', 'varchar(2)')             
		  FROM 
		  ( 
			SELECT 
				n = CONVERT(XML, '<i>' 
					+ REPLACE((SELECT STUFF((SELECT ',' + @teams FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
					+ '</i>')
		  ) AS a 
		  CROSS APPLY n.nodes('i') AS y(i)

		insert into BTT_SEC_USER_TEAM_MAPPING_N
		select @userId, team.TEAM_ID,@modifiedUser,GETDATE(),@modifiedUser,GETDATE() 
		from #teams team 

		drop table #teams

		SELECT 'Saved Successfully!'
	end
end
--------------------------------------------------------------------------------------------------------------

ALTER proc ddbo.sp_SEC_Add_User_Team_Mapping
@userId varchar(18),
@firstName varchar(15),
@LastName varchar(25),
@employeeStatus varchar(1),
@teams varchar(255)
as
begin
	
	if exists (select * from BTTSEC_USER_N where LTRIM(RTRIM(UDT_USER_ID)) = @userId)
	begin
		SELECT 'User Id already exists'
	end
	else
	begin
		insert into BTTSEC_USER_N values (@userId, @firstName,@LastName,'','A','NewAccount', 'NewAccount', 'NewAccount', 'NewAccount', 'NewAccount', 'NewAccount', GETDATE() + 60, 'ALL', GETDATE(),'System',GETDATE(),'System')

		create table #teams (TEAM_ID int)

		insert into #teams
		SELECT rcsId = y.i.value('(./text())[1]', 'varchar(2)')             
		  FROM 
		  ( 
			SELECT 
				n = CONVERT(XML, '<i>' 
					+ REPLACE((SELECT STUFF((SELECT ',' + @teams FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
					+ '</i>')
		  ) AS a 
		  CROSS APPLY n.nodes('i') AS y(i)

		insert into BTT_SEC_USER_TEAM_MAPPING_N
		select @userId, team.TEAM_ID,'System',GETDATE(),'System',GETDATE() 
		from #teams team 

		drop table #teams

		SELECT 'Saved Successfully!'
	end
end

--------------------------------------------------------------------------------------------------------------------------------------
ALTER proc [dbo].[sp_SEC_Delete_Region_Maintenance]
@regionId varchar(255)
as
begin
	
	create table #IdsToDelete (REGION_ID int)

	insert into #IdsToDelete
	SELECT rcsId = y.i.value('(./text())[1]', 'int')             
	  FROM 
	  ( 
		SELECT 
			n = CONVERT(XML, '<i>' 
				+ REPLACE((SELECT STUFF((SELECT ',' + @regionId FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
				+ '</i>')
	  ) AS a 
	  CROSS APPLY n.nodes('i') AS y(i)

	delete trs from BTT_SEC_TEAM_REG_SEG_MAPPING_N trs join BTT_SEC_REG_SEG_MAPPING_N rs on trs.REGION_SEGMENT_MAPPING_ID = rs.REGION_SEGMENT_MAPPING_ID
	where REGION_ID in (select * from #IdsToDelete)

	delete from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID in (select * from #IdsToDelete)

	delete from BTT_SEC_REG_LKP_N where REGION_ID in (select * from #IdsToDelete)

	drop table #IdsToDelete
end
-------------------------------------------------------------------------------------------------------------------------------------
ALTER proc [dbo].[sp_SEC_Delete_Subsegment_Maintenance]
@subsegmentId varchar(255)
as
begin
	
	create table #IdsToDelete (SUB_SEGMENT_ID int)

	insert into #IdsToDelete
	SELECT rcsId = y.i.value('(./text())[1]', 'int')             
	  FROM 
	  ( 
		SELECT 
			n = CONVERT(XML, '<i>' 
				+ REPLACE((SELECT STUFF((SELECT ',' + @subsegmentId FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
				+ '</i>')
	  ) AS a 
	  CROSS APPLY n.nodes('i') AS y(i)

	delete trs from BTT_SEC_TEAM_REG_SEG_MAPPING_N trs join BTT_SEC_REG_SEG_MAPPING_N rs on trs.REGION_SEGMENT_MAPPING_ID = rs.REGION_SEGMENT_MAPPING_ID
	where SUB_SEGMENT_ID in (select * from #IdsToDelete)

	delete from BTT_SEC_REG_SEG_MAPPING_N where SUB_SEGMENT_ID in (select * from #IdsToDelete)

	delete from BTT_SEC_SUB_SEGMENT_LKP_N where SUB_SEGMENT_ID in (select * from #IdsToDelete)

	drop table #IdsToDelete
end
--------------------------------------------------------------------------------------------------------------------------------------
ALTER proc [dbo].[sp_SEC_Delete_Team_Maintenance]
@teamId varchar(max)
as
begin
	
	create table #IdsToDelete (TEAM_ID int)

	insert into #IdsToDelete
	SELECT rcsId = y.i.value('(./text())[1]', 'int')             
	  FROM 
	  ( 
		SELECT 
			n = CONVERT(XML, '<i>' 
				+ REPLACE((SELECT STUFF((SELECT ',' + @teamId FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
				+ '</i>')
	  ) AS a 
	  CROSS APPLY n.nodes('i') AS y(i)

	delete from BTT_SEC_TEAM_RPT_MAPPING_N where TEAM_ID in (select * from #IdsToDelete)

	delete from BTT_SEC_TEAM_REG_SEG_MAPPING_N where TEAM_ID in (select * from #IdsToDelete)

	delete from BTT_SEC_TEAM_LKP_N where TEAM_ID in (select * from #IdsToDelete)

	drop table #IdsToDelete
end
----------------------------------------------------------------------------------------------------------------------------------------
ALTER proc [dbo].[sp_SEC_Delete_User_Team_Mapping]
@userId varchar(max)
as
begin

	create table #IdsToDelete (USER_ID varchar(100))

	insert into #IdsToDelete
	SELECT rcsId = y.i.value('(./text())[1]', 'varchar(100)')             
	  FROM 
	  ( 
		SELECT 
			n = CONVERT(XML, '<i>' 
				+ REPLACE((SELECT STUFF((SELECT ',' + @userId FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
				+ '</i>')
	  ) AS a 
	  CROSS APPLY n.nodes('i') AS y(i)

	delete from BTT_SEC_USER_TEAM_MAPPING_N where USER_ID in (select * from #IdsToDelete)

	delete from BTTSEC_USER_N where UDT_USER_ID in (select * from #IdsToDelete)

	drop table #IdsToDelete
end
-----------------------------------------------------------------------------------------------------------------------------------------
ALTER proc [dbo].[sp_SEC_Edit_Team_Maintenance_New]
@teamId int,
@teamCode varchar(5),
@teamName varchar(100),
@ctrlCount int,
@reports varchar(255),
@regions varchar(255),
@subsegments varchar(255)
as
begin
	
		create table #reports (Report_CD varchar(2))

		insert into #reports
		SELECT rcsId = y.i.value('(./text())[1]', 'varchar(2)')             
		  FROM 
		  ( 
			SELECT 
				n = CONVERT(XML, '<i>' 
					+ REPLACE((SELECT STUFF((SELECT ',' + @reports FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
					+ '</i>')
		  ) AS a 
		  CROSS APPLY n.nodes('i') AS y(i)


		create table #regions (REGION_ID int)

		insert into #regions
		SELECT rcsId = y.i.value('(./text())[1]', 'int')             
		  FROM 
		  ( 
			SELECT 
				n = CONVERT(XML, '<i>' 
					+ REPLACE((SELECT STUFF((SELECT ',' + @regions FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
					+ '</i>')
		  ) AS a 
		  CROSS APPLY n.nodes('i') AS y(i)


		create table #subSegments (SUB_SEGMENT_ID int)

		insert into #subSegments
		SELECT rcsId = y.i.value('(./text())[1]', 'int')             
		  FROM 
		  ( 
			SELECT 
				n = CONVERT(XML, '<i>' 
					+ REPLACE((SELECT STUFF((SELECT ',' + @subsegments FOR XML PATH('')),1,1,'')), ',' , '</i><i>') 
					+ '</i>')
		  ) AS a 
		  CROSS APPLY n.nodes('i') AS y(i)
		
		
		Update BTT_SEC_TEAM_LKP_N set TEAM_CD = @teamCode, TEAM_DESCR = @teamName, CTRL_COUNT = @ctrlCount where TEAM_ID = @teamId
	
		insert into BTT_SEC_TEAM_RPT_MAPPING_N
		select @teamId, rpt.Report_CD,'System',GETDATE(),'System',GETDATE() 
		from #reports rep join BTTRPT_NAM_LB_N rpt on rep.Report_CD = rpt.Report_CD
		where rpt.Report_CD not in (select distinct REPORT_CD from BTT_SEC_TEAM_RPT_MAPPING_N where TEAM_ID = @teamId)

		delete from BTT_SEC_TEAM_RPT_MAPPING_N where TEAM_ID = @teamId and REPORT_CD not in (select * from #reports) and @reports is not null
		

		  insert into BTT_SEC_TEAM_REG_SEG_MAPPING_N
		  select @teamId, 
		  (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID = tmp.REGION_ID and SUB_SEGMENT_ID = tmp.SUB_SEGMENT_ID),
		  'System',GETDATE(),'System',GETDATE() from 
		  (select * from #regions cross join (select distinct SUB_SEGMENT_ID from (select SUB_SEGMENT_ID from BTT_SEC_REG_SEG_MAPPING_N rs join BTT_SEC_TEAM_REG_SEG_MAPPING_N trs on rs.REGION_SEGMENT_MAPPING_ID = trs.REGION_SEGMENT_MAPPING_ID where TEAM_ID = @teamId union select * from #subSegments)b)a) tmp
		  where REGION_ID not in (select distinct REGION_ID from BTT_SEC_REG_SEG_MAPPING_N rs join BTT_SEC_TEAM_REG_SEG_MAPPING_N trs on rs.REGION_SEGMENT_MAPPING_ID = trs.REGION_SEGMENT_MAPPING_ID where TEAM_ID = @teamId) 
		  and (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID = tmp.REGION_ID and SUB_SEGMENT_ID = tmp.SUB_SEGMENT_ID) is not null

		  delete from BTT_SEC_TEAM_REG_SEG_MAPPING_N where TEAM_ID = @teamId and REGION_SEGMENT_MAPPING_ID not in
		  (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N rs join #regions reg on rs.REGION_ID = reg.REGION_ID )
		  and @regions is not null



		  insert into BTT_SEC_TEAM_REG_SEG_MAPPING_N
		  select @teamId, 
		  (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID = tmp.REGION_ID and SUB_SEGMENT_ID = tmp.SUB_SEGMENT_ID),
		  'System',GETDATE(),'System',GETDATE() from 
		  (select * from #subSegments cross join (select distinct REGION_ID from (select REGION_ID from BTT_SEC_REG_SEG_MAPPING_N rs join BTT_SEC_TEAM_REG_SEG_MAPPING_N trs on rs.REGION_SEGMENT_MAPPING_ID = trs.REGION_SEGMENT_MAPPING_ID where TEAM_ID = @teamId union select * from #regions)b)a) tmp
		  where SUB_SEGMENT_ID not in (select distinct SUB_SEGMENT_ID from BTT_SEC_REG_SEG_MAPPING_N rs join BTT_SEC_TEAM_REG_SEG_MAPPING_N trs on rs.REGION_SEGMENT_MAPPING_ID = trs.REGION_SEGMENT_MAPPING_ID where TEAM_ID = @teamId)
		  AND (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID = tmp.REGION_ID and SUB_SEGMENT_ID = tmp.SUB_SEGMENT_ID) is not null

		 delete from BTT_SEC_TEAM_REG_SEG_MAPPING_N where TEAM_ID = @teamId and REGION_SEGMENT_MAPPING_ID not in
		 (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N rs join #subSegments seg on rs.SUB_SEGMENT_ID = seg.SUB_SEGMENT_ID )
		 and @subsegments is not null

		 drop table #reports
		 drop table #regions
		 drop table #subSegments
end
-------------------------------------------------------------------------------------------------------------------------------------------------------
Create proc [dbo].[sp_SEC_Edit_Subsegment_Maintenance_New]
@subsegmentId int,
@subSegmentCode varchar(5),
@subSegmentDescription varchar(30),
@modifiedUser varchar(100)
as
begin
	Update BTT_SEC_SUB_SEGMENT_LKP_N set SUB_SEGMENT_CD = @subSegmentCode, SUB_SEGMENT_DESCR = @subSegmentDescription, UPDT_BY_ID = @modifiedUser, UPDT_TMSTMP = GETDATE() where SUB_SEGMENT_ID = @subsegmentId
end