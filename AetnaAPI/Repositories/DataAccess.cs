using AetnaAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AetnaAPI.Repositories
{
    public class DataAccess
    {
        public List<TeamMaintenance> GetTeamMaintenanceData()
        {
            List<TeamMaintenance> output = new List<TeamMaintenance>();
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";

            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Team_Maintenance"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        TeamMaintenance tmObj = new TeamMaintenance();
                        tmObj.TeamMaintenanceID = Convert.ToInt32(reader["TeamMaintenanceID"]);
                        tmObj.TeamCode = Convert.ToString(reader["TeamCode"]);
                        tmObj.TeamName = Convert.ToString(reader["TeamName"]);
                        tmObj.CtrlCnt = Convert.ToInt32(reader["CtrlCnt"]);
                        tmObj.Reports = Convert.ToString(reader["Reports"]);
                        tmObj.Region = Convert.ToString(reader["Region"]);
                        tmObj.Subsegment = Convert.ToString(reader["Subsegment"]);
                        output.Add(tmObj);
                    }
                    reader.NextResult();
                    con.Close();
                }
            }
            return output;
        }

        public bool AddTeamMaintenance(TeamMaintenance teamMaintenance)
        {
            var output = false;
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";
            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Add_Team_Maintenance"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();

                    var teamCodeparam = new SqlParameter("@teamCode", teamMaintenance.TeamCode);
                    var teamDescriptionParam = new SqlParameter("@teamDescription", teamMaintenance.TeamName);
                    var ctrlCountParam = new SqlParameter("@ctrlCount", teamMaintenance.CtrlCnt);
                    var reportsParam = new SqlParameter("@reports", teamMaintenance.Reports);
                    var regionsParam = new SqlParameter("@regions", teamMaintenance.Region);
                    var subsegmentsParam = new SqlParameter("@subsegments", teamMaintenance.Subsegment);
                    cmd.Parameters.Add(teamCodeparam);
                    cmd.Parameters.Add(teamDescriptionParam);
                    cmd.Parameters.Add(ctrlCountParam);
                    cmd.Parameters.Add(reportsParam);
                    cmd.Parameters.Add(regionsParam);
                    cmd.Parameters.Add(subsegmentsParam);

                    int result = cmd.ExecuteNonQuery();
                    output = true;

                    con.Close();
                }
            }
            return output;
        }

        public bool EditTeamMaintenance(TeamMaintenance teamMaintenance)
        {
            var output = false;
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";
            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Edit_Team_Maintenance"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();

                    var teamIdparam = new SqlParameter("@teamId", teamMaintenance.TeamMaintenanceID);
                    var columnParam = new SqlParameter("@column", teamMaintenance.Column);
                    var valueParam = new SqlParameter("@value", teamMaintenance.Value);
                    cmd.Parameters.Add(teamIdparam);
                    cmd.Parameters.Add(columnParam);
                    cmd.Parameters.Add(valueParam);

                    int result = cmd.ExecuteNonQuery();
                    output = true;

                    con.Close();
                }
            }
            return output;
        }

        public bool AddRegionMaintenance(RegionModel region)
        {
            var output = false;
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";
            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Add_Region_Maintenance"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();

                    var regionCodeparam = new SqlParameter("@regionCode", region.REGION_CD);
                    var regionDescriptionParam = new SqlParameter("@regionDescription", region.REGION_DESCR);
                    cmd.Parameters.Add(regionCodeparam);
                    cmd.Parameters.Add(regionDescriptionParam);

                    int result = cmd.ExecuteNonQuery();
                    output = true;

                    con.Close();
                }
            }
            return output;
        }

        public bool EditRegionMaintenance(RegionModel region)
        {
            var output = false;
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";
            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Edit_Region_Maintenance"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();

                    var regionIdparam = new SqlParameter("@regionId", region.REGION_ID);
                    var columnParam = new SqlParameter("@column", region.Column);
                    var valueParam = new SqlParameter("@value", region.Value);
                    cmd.Parameters.Add(regionIdparam);
                    cmd.Parameters.Add(columnParam);
                    cmd.Parameters.Add(valueParam);

                    int result = cmd.ExecuteNonQuery();
                    output = true;

                    con.Close();
                }
            }
            return output;
        }

        public bool AddSubsegmentMaintenance(SubsegmentModel subsegment)
        {
            var output = false;
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";
            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Add_Subsegment_Maintenance"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();

                    var subsegmentCodeparam = new SqlParameter("@subSegmentCode", subsegment.SUB_SEGMENT_CD);
                    var subsegmentDescriptionParam = new SqlParameter("@subSegmentDescription", subsegment.SUB_SEGMENT_DESCR);
                    cmd.Parameters.Add(subsegmentCodeparam);
                    cmd.Parameters.Add(subsegmentDescriptionParam);

                    int result = cmd.ExecuteNonQuery();
                    output = true;

                    con.Close();
                }
            }
            return output;
        }

        public bool EditSubsegmentMaintenance(SubsegmentModel subsegment)
        {
            var output = false;
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";
            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Edit_Subsegment_Maintenance"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();

                    var subsegmentIdparam = new SqlParameter("@subsegmentId", subsegment.SUB_SEGMENT_ID);
                    var columnParam = new SqlParameter("@column", subsegment.Column);
                    var valueParam = new SqlParameter("@value", subsegment.Value);
                    cmd.Parameters.Add(subsegmentIdparam);
                    cmd.Parameters.Add(columnParam);
                    cmd.Parameters.Add(valueParam);

                    int result = cmd.ExecuteNonQuery();
                    output = true;

                    con.Close();
                }
            }
            return output;
        }

        public List<UserTeamMapping> GetUserTeamMappingData()
        {
            List<UserTeamMapping> output = new List<UserTeamMapping>();
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";

            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_User_Team_Mapping"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        UserTeamMapping utmObj = new UserTeamMapping();
                        utmObj.USER_ID = Convert.ToString(reader["USER_ID"]);
                        utmObj.FIRST_NAM = Convert.ToString(reader["FIRST_NAM"]);
                        utmObj.LAST_NAM = Convert.ToString(reader["LAST_NAM"]);
                        utmObj.EMP_STS_CD = Convert.ToString(reader["EMP_STS_CD"]);
                        utmObj.TEAMS = Convert.ToString(reader["TEAMS"]);
                        output.Add(utmObj);
                    }
                    reader.NextResult();
                    con.Close();
                }
            }
            return output;
        }

        public bool AddUserTeamMapping(UserTeamMapping userTeamMapping)
        {
            var output = false;
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";
            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Add_User_Team_Mapping"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();

                    var userIdParam = new SqlParameter("@userId", userTeamMapping.USER_ID);
                    var firstNameParam = new SqlParameter("@firstName", userTeamMapping.FIRST_NAM);
                    var lastNameParam = new SqlParameter("@LastName", userTeamMapping.LAST_NAM);
                    var employeeStatusParam = new SqlParameter("@employeeStatus", userTeamMapping.EMP_STS_CD);
                    var teamsParam = new SqlParameter("@teams", userTeamMapping.TEAMS);
                    cmd.Parameters.Add(userIdParam);
                    cmd.Parameters.Add(firstNameParam);
                    cmd.Parameters.Add(lastNameParam);
                    cmd.Parameters.Add(employeeStatusParam);
                    cmd.Parameters.Add(teamsParam);

                    int result = cmd.ExecuteNonQuery();
                    output = true;

                    con.Close();
                }
            }
            return output;
        }

        public bool EditUserTeamMapping(UserTeamMapping userTeamMapping)
        {
            var output = false;
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";
            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Edit_User_Team_Mapping"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();

                    var userIdParam = new SqlParameter("@userId", userTeamMapping.USER_ID);
                    var columnParam = new SqlParameter("@column", userTeamMapping.Column);
                    var valueParam = new SqlParameter("@value", userTeamMapping.Value);
                    cmd.Parameters.Add(userIdParam);
                    cmd.Parameters.Add(columnParam);
                    cmd.Parameters.Add(valueParam);

                    int result = cmd.ExecuteNonQuery();
                    output = true;

                    con.Close();
                }
            }
            return output;
        }

        public List<ReportModel> GetReports()
        {
            List<ReportModel> output = new List<ReportModel>();
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";

            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Reports"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ReportModel repObj = new ReportModel();
                        repObj.REPORT_CD = Convert.ToString(reader["REPORT_CD"]);
                        repObj.REPORT_DESC = Convert.ToString(reader["REPORT_DESC"]);
                        repObj.APP_RTGRP_ACC_CD = Convert.ToString(reader["APP_RTGRP_ACC_CD"]);
                        repObj.CREAT_BY_ID = Convert.ToString(reader["CREAT_BY_ID"]);
                        repObj.CREAT_TMSTMP = Convert.ToDateTime(reader["CREAT_TMSTMP"]);
                        repObj.UPDT_BY_ID = Convert.ToString(reader["UPDT_BY_ID"]);
                        repObj.UPDT_TMSTMP = Convert.ToDateTime(reader["UPDT_TMSTMP"]);
                        repObj.SEL_RPT_ACRONYM = Convert.ToString(reader["SEL_RPT_ACRONYM"]);
                        if(!DBNull.Value.Equals(reader["REPORT_SORT_ORDER"]))
                        {
                            repObj.REPORT_SORT_ORDER = Convert.ToInt32(reader["REPORT_SORT_ORDER"]);
                        }        
                        output.Add(repObj);
                    }
                    reader.NextResult();
                    con.Close();
                }
            }
            return output;
        }

        public List<RegionModel> GetRegions()
        {
            List<RegionModel> output = new List<RegionModel>();
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";

            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Regions"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        RegionModel repObj = new RegionModel();
                        repObj.REGION_ID = Convert.ToInt32(reader["REGION_ID"]);
                        repObj.REGION_CD = Convert.ToString(reader["REGION_CD"]);
                        repObj.REGION_DESCR = Convert.ToString(reader["REGION_DESCR"]);
                        if (!DBNull.Value.Equals(reader["CREAT_BY_ID"]))
                        {
                            repObj.CREAT_BY_ID = Convert.ToString(reader["CREAT_BY_ID"]);
                        }
                        if (!DBNull.Value.Equals(reader["CREAT_TMSTMP"]))
                        {
                            repObj.CREAT_TMSTMP = Convert.ToDateTime(reader["CREAT_TMSTMP"]);
                        }
                        if (!DBNull.Value.Equals(reader["UPDT_BY_ID"]))
                        {
                            repObj.UPDT_BY_ID = Convert.ToString(reader["UPDT_BY_ID"]);
                        }
                        if (!DBNull.Value.Equals(reader["UPDT_TMSTMP"]))
                        {
                            repObj.UPDT_TMSTMP = Convert.ToDateTime(reader["UPDT_TMSTMP"]);
                        }
                        output.Add(repObj);
                    }
                    reader.NextResult();
                    con.Close();
                }
            }
            return output;
        }

        public List<SubsegmentModel> GetSubsegments()
        {
            List<SubsegmentModel> output = new List<SubsegmentModel>();
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";

            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Subsegments"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        SubsegmentModel repObj = new SubsegmentModel();
                        repObj.SUB_SEGMENT_ID = Convert.ToInt32(reader["SUB_SEGMENT_ID"]);
                        repObj.SUB_SEGMENT_CD = Convert.ToString(reader["SUB_SEGMENT_CD"]);
                        repObj.SUB_SEGMENT_DESCR = Convert.ToString(reader["SUB_SEGMENT_DESCR"]);
                        if (!DBNull.Value.Equals(reader["FOOTNOTE_IND"]))
                        {
                            repObj.FOOTNOTE_IND = Convert.ToString(reader["FOOTNOTE_IND"]);
                        }
                        if (!DBNull.Value.Equals(reader["CREAT_BY_ID"]))
                        {
                            repObj.CREAT_BY_ID = Convert.ToString(reader["CREAT_BY_ID"]);
                        }
                        if (!DBNull.Value.Equals(reader["CREAT_TMSTMP"]))
                        {
                            repObj.CREAT_TMSTMP = Convert.ToDateTime(reader["CREAT_TMSTMP"]);
                        }
                        if (!DBNull.Value.Equals(reader["UPDT_BY_ID"]))
                        {
                            repObj.UPDT_BY_ID = Convert.ToString(reader["UPDT_BY_ID"]);
                        }
                        if (!DBNull.Value.Equals(reader["UPDT_TMSTMP"]))
                        {
                            repObj.UPDT_TMSTMP = Convert.ToDateTime(reader["UPDT_TMSTMP"]);
                        }
                        output.Add(repObj);
                    }
                    reader.NextResult();
                    con.Close();
                }
            }
            return output;
        }

        public List<TeamModel> GetTeams()
        {
            List<TeamModel> output = new List<TeamModel>();
            var conn = @"Server=USHYDYMANIDEE12;Database=Aetna;Integrated Security=SSPI;";

            using (var con = new SqlConnection(conn))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SEC_Teams"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        TeamModel teamObj = new TeamModel();
                        teamObj.TEAM_ID = Convert.ToInt32(reader["TEAM_ID"]);
                        teamObj.TEAM_CD = Convert.ToString(reader["TEAM_CD"]);
                        teamObj.TEAM_DESCR = Convert.ToString(reader["TEAM_DESCR"]);
                        teamObj.CTRL_COUNT = Convert.ToInt32(reader["CTRL_COUNT"]);
                        if (!DBNull.Value.Equals(reader["CREAT_BY_ID"]))
                        {
                            teamObj.CREAT_BY_ID = Convert.ToString(reader["CREAT_BY_ID"]);
                        }
                        if (!DBNull.Value.Equals(reader["CREAT_TMSTMP"]))
                        {
                            teamObj.CREAT_TMSTMP = Convert.ToDateTime(reader["CREAT_TMSTMP"]);
                        }
                        if (!DBNull.Value.Equals(reader["UPDT_BY_ID"]))
                        {
                            teamObj.UPDT_BY_ID = Convert.ToString(reader["UPDT_BY_ID"]);
                        }
                        if (!DBNull.Value.Equals(reader["UPDT_TMSTMP"]))
                        {
                            teamObj.UPDT_TMSTMP = Convert.ToDateTime(reader["UPDT_TMSTMP"]);
                        }
                        output.Add(teamObj);
                    }
                    reader.NextResult();
                    con.Close();
                }
            }
            return output;
        }
    }
}