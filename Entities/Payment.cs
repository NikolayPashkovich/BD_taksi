using System;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public enum PaymentStatus
	{
		Pending = 1,
		Completed = 2,
		Failed = 3
	}

	public class Payment
	{
		public int Id { get; set; }

		[Range(0, 100000000)]
		public decimal Amount { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[Required]
		public PaymentStatus Status { get; set; }

		[Required]
		public int RideId { get; set; }
		public Ride? Ride { get; set; }

		[Required]
		public int CustomerId { get; set; }
		public Customer? Customer { get; set; }

		[Required]
		public int MethodId { get; set; }
		public PaymentMethod? Method { get; set; }
	}
} 