using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Data
{
	public class EMAnnounce
	{
		public int AnnounceID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public bool Published { get; set; }

	}
	public class EMEvent
	{
		public int EventID { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public int? EventTypeID { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string Location { get; set; }
		public string latitude { get; set; }
		public string longitude { get; set; }
		public bool CanPlanned { get; set; }
		public int? MaxQuota { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
		public bool Published { get; set; }
		public virtual int AvailableQuota { get; set; }

	}
	public class EMEventPrice
	{
		public int PriceID { get; set; }
		public int? EventID { get; set; }
		public int? TicketTypeID { get; set; }
		public double? Price { get; set; }
		public int? Quota { get; set; }
		public virtual string TypeName { get; set; } = "";

	}
	public class EMEventType
	{
		public int EventTypeID { get; set; }
		public string TypeName { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }

	}
	public class EMOrder
	{
		public int OrderID { get; set; }
		public int? UserID { get; set; }
		public DateTime? OrderDate { get; set; }
		public double? TotalAmount { get; set; }
		public string PaymentMethod { get; set; }
		public virtual string RealName { get; set; }

	}
	public class EMOrderItem
	{
		public int ItemID { get; set; }
		public int? OrderID { get; set; }
		public int? PriceID { get; set; }
		public double? UnitPrice { get; set; }
		public int? UnitCount { get; set; }
		public virtual string EventName { get; set; }
		public virtual string TicketType { get; set; }


	}
	public class EMTicketType
	{
		public int TicketTypeID { get; set; }
		public string TypeName { get; set; }
		public string Description { get; set; }
		public bool Active { get; set; }

	}
	public class UMInterest
	{
		public int InterestID { get; set; }
		public int? UserID { get; set; }
		public int? EventType { get; set; }

	}
	public class UMSUser
	{
		public int UserID { get; set; }
		public string RealName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public bool PassExpired { get; set; }
		public bool Approved { get; set; }
		public DateTime? RegisterDate { get; set; }
		public bool IsAdmin { get; set; }

	}

	public class VWItem
	{
		public int EventID { get; set; }
		public string Title { get; set; }
		public int PriceID { get; set; }
		public int? TicketTypeID { get; set; }
		public double? Price { get; set; }
		public int? UnitCount { get; set; }
		public int? MaxQuota { get; set; }
		public int? AvailableQuota { get; set; }
		

	}
}
