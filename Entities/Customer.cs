using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class Customer
	{
		public int Id { get; set; }

		[Required, StringLength(100)]
		public string Name { get; set; } = string.Empty;

		[Required, Phone, StringLength(20)]
		public string Phone { get; set; } = string.Empty;

		[EmailAddress, StringLength(100)]
		public string? Email { get; set; }

			[Range(0, 5)]
	public double Rating { get; set; } = 0;

	public string FullName => Name;

	public ICollection<Ride> Rides { get; set; } = new List<Ride>();
	public ICollection<Payment> Payments { get; set; } = new List<Payment>();
	}
} 