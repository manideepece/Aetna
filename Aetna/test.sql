create proc sp_SEC_Reports
as
begin
	select REPORT_CD,
		   REPORT_DESC,
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
