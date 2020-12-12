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

                    var teamCodeparam = new SqlParameter("@teamId", teamMaintenance.TeamMaintenanceID);
                    var teamDescriptionParam = new SqlParameter("@column", teamMaintenance.Column);
                    var ctrlCountParam = new SqlParameter("@value", teamMaintenance.Value);
                    cmd.Parameters.Add(teamCodeparam);
                    cmd.Parameters.Add(teamDescriptionParam);
                    cmd.Parameters.Add(ctrlCountParam);

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
    }
}