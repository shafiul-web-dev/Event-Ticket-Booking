using Event_Ticket_Booking.Models;
using System.ComponentModel.DataAnnotations;

namespace Event_Ticket_Booking.DTO
{
	public class CreateBookingDto
	{
		[Required(ErrorMessage = "EventId is required.")]
		public int EventId { get; set; }

		[Required(ErrorMessage = "UserId is required.")]
		public int UserId { get; set; }

		[Required(ErrorMessage = "TicketTierId is required.")]
		public int TicketTierId { get; set; } // Standard, VIP, Premium

		[Required(ErrorMessage = "Ticket quantity is required.")]
		[Range(1, 10, ErrorMessage = "You can book between 1 and 10 tickets.")]
		public int TicketQuantity { get; set; }
	}
}
