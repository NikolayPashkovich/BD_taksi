using System;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class Maintenance
	{
		public int Id { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[Required, StringLength(50)]
		public string Type { get; set; } = string.Empty;

		[Range(0, 100000000)]
		public decimal Cost { get; set; }

		[Range(0, 2000000)]
		public int Odometer { get; set; }

		[StringLength(200)]
		public string? Comment { get; set; }

		[Required]
		public int VehicleId { get; set; }
		public Vehicle? Vehicle { get; set; }
	}
} 