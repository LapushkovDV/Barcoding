using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Galaktika.ESB.Core.Storage.Core;

namespace Galaktika.ESB.Adapter.ERP.Adapter
{

	class UpdatesJournal
	{
		public System.Int64 NREC { get; set; }
		public System.Int32 TABLECODE { get; set; }
		public System.Int64 TABLENREC { get; set; }
		public System.DateTime LASTDATE { get; set; }
		public System.TimeSpan LASTTIME { get; set; }
		public System.Byte OPERATION { get; set; }
	}

}
