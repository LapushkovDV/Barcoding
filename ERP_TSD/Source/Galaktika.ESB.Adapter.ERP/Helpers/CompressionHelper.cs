using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Galaktika.ESB.Adapter.ERP.Helpers
{    
    public static class CompressionHelper
    {
        public static string Compress(byte[] data)
		{
			using (var compressStream = new MemoryStream())
			{
				using (var compressor = new DeflateStream(compressStream, CompressionMode.Compress))
				{
					compressor.Write(data, 0, data.Length);
					compressor.Close();
					return Convert.ToBase64String(compressStream.ToArray());
				}
			}			
		}		
    }
}