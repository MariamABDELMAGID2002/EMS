using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using EMS.Data;
using Microsoft.Data.SqlClient;

namespace EMS.Data
{
	public class EMService
	{
		private SqlConnection conn = null;
		public EMService(string connectionString) {
			conn = new SqlConnection(connectionString);
		}
		public void UserSave(UMSUser objUser)
		{
			string sql = "";
			if (objUser.UserID == 0)
				sql = "INSERT INTO UMSUser ( RealName ,Email ,Password ,PassExpired ,Approved ,RegisterDate ,IsAdmin) VALUES  ( @RealName ,@Email ,@Password ,PassExpired ,@Approved ,@RegisterDate ,@IsAdmin)";
			else
				sql = "Update UMSUser set  RealName=@RealName ,Email=@Email ,Password=@Password ,PassExpired=@PassExpired ,Approved=@Approved ,RegisterDate=@RegisterDate ,IsAdmin=@IsAdmin where UserID = @UserID";
			conn.Execute(sql, objUser);
		}
		public void UserDelete(int id)
		{
			string sql = "delete from UMSUser where UserID = " + id;
			conn.Execute(sql);
		}
		public List<UMSUser> UserGet(int id = 0)
		{
			string sql = "SELECT * FROM UMSUser ";
			if (id != 0)
				sql += " where UserID=" + id;
			return conn.Query<UMSUser>(sql).ToList();
		}
		public UMSUser? UserLogin(string email, string password)
		{
			string sql = "SELECT * FROM UMSUser where email = @Email and Password = @Password";
			return conn.Query<UMSUser>(sql, new { Email = email, Password = password }).FirstOrDefault();
		}

		public void AnnounceSave(EMAnnounce objAnnounce)
		{
			string sql = "";
			if (objAnnounce.AnnounceID == 0)
				sql = "INSERT INTO EMAnnounce ( Title ,Description ,CreatedAt ,UpdatedAt ,Published) VALUES  ( @Title ,@Description ,@CreatedAt ,@UpdatedAt ,@Published)";
			else 
				sql = "Update EMAnnounce set Title=@Title ,Description=@Description ,CreatedAt=@CreatedAt ,UpdatedAt=@UpdatedAt ,Published=@Published where AnnounceID = @AnnounceID";
			conn.Execute(sql, objAnnounce);
		}
		public void AnnounceDelete(int id)
		{
			string sql = "delete from EMAnnounce where AnnounceID = "+id;
			conn.Execute(sql);
		}
		public List<EMAnnounce> AnnounceGet(int id=0)
		{
			string sql = "SELECT * FROM EMAnnounce ";
			if (id != 0)
				sql += " where AnnounceID=" + id;
			sql += " ORDER BY CreatedAt DESC";
			return conn.Query<EMAnnounce>(sql).ToList();
		}
		
		

	}	
}
