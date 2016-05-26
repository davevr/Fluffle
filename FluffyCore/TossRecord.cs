using System;
using System.Collections.Generic;

namespace Fluffimax.Core
{
	public class TossRecord {
		public long id { get; set; }
		public long tosserId { get; set; }
		public long catcherId { get; set; }
		public DateTime startTossDate { get; set; }
		public DateTime endTossDate { get; set; }
		public long bunnyId { get; set; }
		public Boolean isValid { get; set; }
		public int price { get; set; }

		public TossRecord() {
			
		}


	}
}

