using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public enum PromotionType
	{
		Percent = 1,
		Fixed = 2
	}

	public class Promotion
	{
		public int Id { get; set; }

		[Required, StringLength(30)]
		public string Code { get; set; } = string.Empty;

		[Required]
		public PromotionType Type { get; set; }

		[Range(0, 1000000)]
		public decimal Value { get; set; }

		[Required]
		public DateTime StartDate { get; set; }

		[Required]
		public DateTime EndDate { get; set; }

		public bool IsActive { get; set; } = true;

		public ICollection<Ride> Rides { get; set; } = new List<Ride>();
	}
} 