using Newtonsoft.Json;
using System;
using System.Text;

namespace Galaktika.ESB.Adapter.ERP.RequestResults
{
	public class RequestResult
	{
		[JsonProperty("RESULT_CODE")]
		public int ResultCode { get; set; }
		
		[JsonProperty("RESULT_MSG")]
		public string ResultMsg { get; set; }
		
		[JsonProperty("RESULT_MSG_RUS")]
		public string ResultMsgRus { get; set; }
	}
}