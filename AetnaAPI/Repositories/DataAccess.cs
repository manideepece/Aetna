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
                using (SqlCommand cmd = new SqlCommand("sp_aetna_sec_team_maintenace"))
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
    }
}