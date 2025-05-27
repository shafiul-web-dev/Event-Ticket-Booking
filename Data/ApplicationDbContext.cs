using Event_Ticket_Booking.Models;
using Microsoft.EntityFrameworkCore;

namespace Event_Ticket_Booking.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		   : base(options)   { }

		public DbSet<Event> Events { get; set; }
		public DbSet<Booking> Bookings { get; set; }
		public DbSet<TicketTier> TicketTiers { get; set; }
		public DbSet<User> Users { get; set; }



	}
}
