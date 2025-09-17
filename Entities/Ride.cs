using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class Ride
	{
		public int Id { get; set; }

		[Required]
		public DateTime StartTime { get; set; }

		public DateTime? EndTime { get; set; }

		[Range(0, 10000)]
		public double DistanceKm { get; set; }

		[Range(0, 100000000)]
		public decimal Cost { get; set; }

		[StringLength(200)]
		public string? Comment { get; set; }

		[Required]
		public int DriverId { get; set; }
		public Driver? Driver { get; set; }

		[Required]
		public int VehicleId { get; set; }
		public Vehicle? Vehicle { get; set; }

		[Required]
		public int CustomerId { get; set; }
		public Customer? Customer { get; set; }

		[Required]
		public int StatusId { get; set; }
		public RideStatus? Status { get; set; }

		[Required]
		public int TariffId { get; set; }
		public Tariff? Tariff { get; set; }

		public int? PromotionId { get; set; }
		public Promotion? Promotion { get; set; }

		public ICollection<Payment> Payments { get; set; } = new List<Payment>();
	}
} 