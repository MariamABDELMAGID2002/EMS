using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.Serialization.Formatters;
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
		public enum UsedTypes { eventTypes,ticketTypes,events,orders,users }
		public int GetUsedCount(UsedTypes type, int id)
		{
			string sql = "select 0";
			if (type == UsedTypes.eventTypes)
				sql = "SELECT COUNT(*) FROM dbo.EMEvent WHERE EventTypeID = @id ";
			else if (type == UsedTypes.ticketTypes)
				sql = "SELECT COUNT(*) FROM dbo.EMEventPrice WHERE TicketTypeID = @id";
			else if (type == UsedTypes.users)
				sql = "SELECT (SELECT COUNT(*) FROM dbo.EMOrder WHERE UserID = @id)+ (SELECT COUNT(*) FROM dbo.UMInterest WHERE UserID= @id)";
			else if (type == UsedTypes.events)
				sql = "SELECT COUNT(*) FROM EMEventPrice INNER JOIN EMOrderItem ON EMEventPrice.PriceID = EMOrderItem.PriceID WHERE  EMEventPrice.EventID = @id";


			return conn.ExecuteScalar<int>(sql,new { id= id});
		}
		public void SaveUser(UMSUser objUser)
		{
			string sql = "";
			if (objUser.UserID == 0)
				sql = "INSERT INTO UMSUser ( RealName ,Email ,Password ,PassExpired ,Approved ,RegisterDate ,IsAdmin) VALUES  ( @RealName ,@Email ,@Password ,@PassExpired ,@Approved ,@RegisterDate ,@IsAdmin)";
			else
				sql = "Update UMSUser set  RealName=@RealName ,Email=@Email ,Password=@Password ,PassExpired=@PassExpired ,Approved=@Approved ,RegisterDate=@RegisterDate ,IsAdmin=@IsAdmin where UserID = @UserID";
			conn.Execute(sql, objUser);
		}
		public void DeleteUser(int id)
		{
			string sql = "delete from UMSUser where UserID = " + id;
			conn.Execute(sql);
		}
		public List<UMSUser> GetUser(int id = 0)
		{
			string sql = "SELECT * FROM UMSUser ";
			if (id != 0)
				sql += " where UserID=" + id;
			return conn.Query<UMSUser>(sql).ToList();
		}
		public bool IsAdmin(int id)
		{
			string sql = "SELECT count(*) FROM UMSUser where isadmin = 1 and userid ="+id;
			return conn.ExecuteScalar<int>(sql) == 1;
		}
		public UMSUser? Login(string email, string password)
		{
			string sql = "SELECT * FROM UMSUser where email = @Email and Password = @Password";
			return conn.Query<UMSUser>(sql, new { Email = email, Password = password }).FirstOrDefault();
		}

		public void SaveAnnounce(EMAnnounce objAnnounce)
		{
			string sql = "";
			if (objAnnounce.AnnounceID == 0)
				sql = "INSERT INTO EMAnnounce ( Title ,Description ,CreatedAt ,UpdatedAt ,Published) VALUES  ( @Title ,@Description ,@CreatedAt ,@UpdatedAt ,@Published)";
			else 
				sql = "Update EMAnnounce set Title=@Title ,Description=@Description ,CreatedAt=@CreatedAt ,UpdatedAt=@UpdatedAt ,Published=@Published where AnnounceID = @AnnounceID";
			conn.Execute(sql, objAnnounce);
			if(objAnnounce.AnnounceID==0)
				objAnnounce.AnnounceID = conn.ExecuteScalar<int>("select max(announceid) from emannounce");
		}
		public void DeleteAnnounce(int id)
		{
			string sql = "delete from EMAnnounce where AnnounceID = "+id;
			conn.Execute(sql);
		}
		public List<EMAnnounce> GetAnnounce(int id=0)
		{
			string sql = "SELECT * FROM EMAnnounce ";
			if (id != 0)
				sql += " where AnnounceID=" + id;
			sql += " ORDER BY AnnounceID DESC";
			return conn.Query<EMAnnounce>(sql).ToList();
		}

		public List<EMEventType> GetEventType(int id = 0)
		{
			string sql = "SELECT * FROM EMEventType ";
			if (id != 0)
				sql += " where EventTypeID=" + id;
			sql += " ORDER BY EventTypeID";
			return conn.Query<EMEventType>(sql).ToList();
		}
		public void SaveEventType(EMEventType objEventType)
		{

			string sql = "";
			if (objEventType.EventTypeID == 0)
				sql = "INSERT INTO EMEventType ( TypeName ,Description, Active) VALUES  ( @TypeName ,@Description, @Active)";
			else
				sql = "Update EMEventType set  TypeName=@TypeName ,Description=@Description, Active=@Active where EventTypeID = @EventTypeID";
			conn.Execute(sql, objEventType);
			if (objEventType.EventTypeID == 0)
				objEventType.EventTypeID = conn.ExecuteScalar<int>("select max(EventTypeID) from EMEventType");
		}
		public void DeleteEventType(int id)
		{
			string sql = "delete from EMEventType where EventTypeID = " + id;
			conn.Execute(sql);
		}

		public List<EMTicketType> GetTicketType(int id = 0)
		{
			string sql = "SELECT * FROM EMTicketType ";
			if (id != 0)
				sql += " where TicketTypeID=" + id;
			sql += " ORDER BY TicketTypeID";
			return conn.Query<EMTicketType>(sql).ToList();
		}
		public void SaveTicketType(EMTicketType objTicketType)
		{

			string sql = "";
			if (objTicketType.TicketTypeID == 0)
				sql = "INSERT INTO EMTicketType ( TypeName ,Description, Active) VALUES  ( @TypeName ,@Description, @Active)";
			else
				sql = "Update EMTicketType set  TypeName=@TypeName ,Description=@Description, Active=@Active where TicketTypeID = @TicketTypeID";
			conn.Execute(sql, objTicketType);
			if (objTicketType.TicketTypeID == 0)
				objTicketType.TicketTypeID = conn.ExecuteScalar<int>("select max(TicketTypeID) from EMTicketType");
		}
		public void DeleteTicketType(int id)
		{
			string sql = "delete from EMTicketType where TicketTypeID = " + id;
			conn.Execute(sql);
		}

		public void SaveEvent(EMEvent obEvent)
		{
			string sql = "";
			if (obEvent.EventID == 0)
				sql = "INSERT INTO EMEvent ( Title,Description,EventTypeID,StartDate,EndDate,Location,latitude,longitude,CanPlanned,MaxQuota,CreatedAt,UpdatedAt,Published) VALUES  ( @Title,@Description,@EventTypeID,@StartDate,@EndDate,@Location,@latitude,@longitude,@CanPlanned,@MaxQuota,@CreatedAt,@UpdatedAt,@Published) ";
			else
				sql = "Update EMEvent set Title=@Title,Description=@Description,EventTypeID=@EventTypeID,StartDate=@StartDate,EndDate=@EndDate,Location=@Location,latitude=@latitude,longitude=@longitude,CanPlanned=@CanPlanned,MaxQuota=@MaxQuota,CreatedAt=@CreatedAt,UpdatedAt=@UpdatedAt,Published=@Published where EventID = @EventID ";
				
			conn.Execute(sql, obEvent);
			if (obEvent.EventID == 0)
				obEvent.EventID = conn.ExecuteScalar<int>("select max(eventid) from emevent");
		}
		public void DeleteEvent(int id)
		{
			string sql = "delete from EMEvent where EventID = " + id;
			conn.Execute(sql);
		}
		public List<EMEvent> GetEvent(int id = 0,int userid=0)
		{
			// if userid <> 0 join with userinterests
			string sql = "SELECT * FROM EMEvent ";
			if (id != 0)
				sql += " where EventID=" + id;
			sql += " ORDER BY EventID DESC";
			return conn.Query<EMEvent>(sql).ToList();
		}
		public void SaveEventPrice(EMEventPrice obEventPrice)
		{
			string sql = "";
			if (obEventPrice.PriceID == 0)

				sql = "INSERT INTO EMEventPrice (EventID,TicketTypeID,Price,Quota) VALUES (@EventID,@TicketTypeID,@Price,@Quota) ";	
			else
				sql = "Update EMEventPrice set EventID=@EventID,TicketTypeID=@TicketTypeID,Price=@Price,Quota= @Quota where PriceID = @PriceID ";

			conn.Execute(sql, obEventPrice);
			if (obEventPrice.EventID == 0)
				obEventPrice.EventID = conn.ExecuteScalar<int>("select max(PriceID) from EMEventPrice");
		}
		public List<EMEventPrice> GetEventPrice(int id = 0)
		{

			string sql = "SELECT null AS PriceID, null AS EventID, EMTicketType.TicketTypeID, null AS Price, null AS Quota, EMTicketType.TypeName FROM EMTicketType WHERE Active=1 order by TicketTypeID";
			var generalPrice =conn.Query<EMEventPrice>(sql).ToList();

			if (id != 0)
			{
				sql= "SELECT * from EMEventPrice where EventID="+id + " order by TicketTypeID";
				var eventPrice=conn.Query<EMEventPrice>(sql).ToList();
				for (int i=0;i<eventPrice.Count;i++)
				{
					generalPrice[i].PriceID = eventPrice[i].PriceID;
					generalPrice[i].EventID = eventPrice[i].EventID;
					generalPrice[i].Price = eventPrice[i].Price;
					generalPrice[i].Quota = eventPrice[i].Quota;
				}
			}

			return generalPrice;
		}
		public void DeleteEventPrice(int id)
		{
			string sql = "delete from EMEventPrice where PriceID = " + id;
			conn.Execute(sql);
		}

		public List<EMOrder> GetOrder(int id = 0)
		{
			string sql = "SELECT EMOrder.*, UMSUser.RealName FROM  EMOrder INNER JOIN UMSUser ON EMOrder.UserID = UMSUser.UserID ";
			if (id != 0)
				sql += " where OrderID=" + id;
			sql += " ORDER BY OrderID DESC";
			return conn.Query<EMOrder>(sql).ToList();
		}
		public List<EMOrderItem> GetOrderItem(int id = 0)
		{
			string sql = $@"SELECT TOP (100) PERCENT dbo.EMEvent.Title AS EventName, dbo.EMTicketType.TypeName AS TicketType, dbo.EMOrderItem.UnitPrice, dbo.EMOrderItem.UnitCount
				FROM dbo.EMOrderItem INNER JOIN
										dbo.EMEventPrice ON dbo.EMOrderItem.PriceID = dbo.EMEventPrice.PriceID INNER JOIN
				dbo.EMEvent ON dbo.EMEventPrice.EventID = dbo.EMEvent.EventID INNER JOIN
				dbo.EMTicketType ON dbo.EMEventPrice.TicketTypeID = dbo.EMTicketType.TicketTypeID
				WHERE        (dbo.EMOrderItem.OrderID = {id})
				ORDER BY dbo.EMOrderItem.ItemID ";
			
			return conn.Query<EMOrderItem>(sql).ToList();
		}
		public void DeleteOrder(int id)
		{
			string sql = "delete from emorderitem where orderid = " + id;
			conn.Execute(sql);
			 sql = "delete from emorder where orderid = " + id;
			conn.Execute(sql);
		}
		

	}	
}
