using System;
using System.IO;
using System.Text;

namespace Galaktika.ESB.Adapter.ERP.Helpers
{    
    public static class FileHelper
    {
        public static string ReadAllText(string fileName, Encoding encoding)
		{
			string result;
			using (var streamReader = new StreamReader(fileName, encoding))
			{
				result = streamReader.ReadToEnd();
			}
			return result;
		}
		
		public static void WriteAllText(string fileName, string content, Encoding encoding)
		{
			using (var streamWriter = new StreamWriter(fileName, false, encoding))
			{
				streamWriter.WriteLine(content);
			}
		}
		
		public static byte[] ReadAllBytes(string fileName)
		{
			return File.ReadAllBytes(fileName);
		}
    }
}