using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class Tariff
	{
		public int Id { get; set; }

		[Required, StringLength(50)]
		public string Name { get; set; } = string.Empty;

		[Range(0, 1000000)]
		public decimal BaseFare { get; set; }

		[Range(0, 1000000)]
		public decimal PricePerKm { get; set; }

		[Range(0, 1000000)]
		public decimal PricePerMinute { get; set; }

		public ICollection<Ride> Rides { get; set; } = new List<Ride>();
	}
} 