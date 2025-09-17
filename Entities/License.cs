using System;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class License
	{
		public int Id { get; set; }

		[Required]
		public int DriverId { get; set; }

		[Required, StringLength(30)]
		public string Number { get; set; } = string.Empty;

		[Required, StringLength(10)]
		public string Category { get; set; } = string.Empty;

		[Required]
		public DateTime IssueDate { get; set; }

		[Required]
		public DateTime ExpiryDate { get; set; }

		public Driver? Driver { get; set; }
	}
} 