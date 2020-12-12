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

alter proc sp_SEC_Add_Team_Maintenance
@teamCode varchar(5),
@teamDescription varchar(100),
@ctrlCount int,
@reports varchar(255),
@regions varchar(255),
@subsegments varchar(255)
as
begin
	insert into BTT_SEC_TEAM_LKP_N values (@teamCode, @teamDescription,@ctrlCount,'System',GETDATE(),'System',GETDATE())
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
	select @id, rpt.Report_CD,'System',GETDATE(),'System',GETDATE() 
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
			'System',GETDATE(),'System',GETDATE() from (select * from #regions cross join #subSegments) tmp
			where (select REGION_SEGMENT_MAPPING_ID from BTT_SEC_REG_SEG_MAPPING_N where REGION_ID = tmp.REGION_ID and SUB_SEGMENT_ID = tmp.SUB_SEGMENT_ID)
			is not null

	drop table #regions
	drop table #subSegments
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

create proc sp_SEC_Add_Region_Maintenance
@regionCode varchar(5),
@regionDescription varchar(30)
as
begin
	insert into BTT_SEC_REG_LKP_N values (@regionCode, @regionDescription,'System',GETDATE(),'System',GETDATE())
	declare @id int = @@identity

	insert into BTT_SEC_REG_SEG_MAPPING_N
	select @id, SUB_SEGMENT_ID, 'System', GETDATE(), 'System', GETDATE()
	from BTT_SEC_SUB_SEGMENT_LKP_N reg
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

create proc sp_SEC_Add_Subsegment_Maintenance
@subSegmentCode varchar(5),
@subSegmentDescription varchar(30)
as
begin
	insert into BTT_SEC_SUB_SEGMENT_LKP_N values (@subSegmentCode, @subSegmentDescription, 'N', 'System',GETDATE(),'System',GETDATE())
	declare @id int = @@identity

	insert into BTT_SEC_REG_SEG_MAPPING_N
	select REGION_ID, @id, 'System', GETDATE(), 'System', GETDATE()
	from BTT_SEC_REG_LKP_N reg

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
