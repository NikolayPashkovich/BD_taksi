using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BD_taksi.Entities
{
	public class PaymentMethod
	{
		public int Id { get; set; }

		[Required, StringLength(30)]
		public string Name { get; set; } = string.Empty; // Cash, Card, Online

		public ICollection<Payment> Payments { get; set; } = new List<Payment>();
	}
} 