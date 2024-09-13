using System.IO;
using System.Text;
using Galaktika.ESB.Build.Managers;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace Galaktika.ESB.Adapter.ERP.Source.Expansions
{
	/*Используется c# 5.0*/
	public class CsprojExpansions
	{
		public void ChangeCsproj(string csprojFullPath)
		{
			var csprojFile = GetCsprojFileByFullPath(csprojFullPath);
			csprojFile = ChangePackagePath(csprojFile);
			csprojFile = ChangeAuxiliaryFilesPath(csprojFile);
			WriteNewCsprojFile(csprojFullPath, csprojFile);
		}
		public string ChangeAuxiliaryFilesPath(string csprojFile)
		{
			var auxiliaryDirectoryInfo = new DirectoryInfo(SearchManager.GetSourcePath() + "\\AuxiliaryFiles");
			if (!auxiliaryDirectoryInfo.Exists) return csprojFile;

			var auxiliaryFilesInfo = auxiliaryDirectoryInfo.GetFiles();
			if (auxiliaryFilesInfo.Length == 0) return csprojFile;
			var pattern = "";
			for (var i = 0; i < auxiliaryFilesInfo.Length; i++)
			{
				pattern += @"""([^\""]+." + auxiliaryFilesInfo[i].Name + @")""";
				if (i != auxiliaryFilesInfo.Length - 1)
					pattern += "|";
			}
			var regex = new Regex(pattern, RegexOptions.IgnoreCase);
			var matches = regex.Matches(csprojFile);
			if (matches.Count == 0) return csprojFile;
			var changeTasksPath = new List<string>();
			foreach (Match match in matches)
			{
				for (var i = 1; i < match.Groups.Count; i++)
				{
					if (!string.IsNullOrEmpty(match.Groups[i].Value))
						changeTasksPath.Add(match.Groups[i].Value);
				}
			}
			foreach (var changeTaskPath in changeTasksPath)
			{
				foreach (var auxiliaryFileInfo in auxiliaryFilesInfo)
				{
					if (changeTaskPath.IndexOf(auxiliaryFileInfo.Name, StringComparison.OrdinalIgnoreCase) == -1) continue;
					csprojFile = csprojFile.Replace(changeTaskPath, auxiliaryFileInfo.FullName);
				}
			}
			return csprojFile;
		}
		public string ChangePackagePath(string csprojFile)
		{
			var regex = new Regex(@"Project=[""](.*packages\\.*)[""]+?\sCondition|[>](.*packages\\.*)[<]+?");
			var matches = regex.Matches(csprojFile);
			if (matches.Count == 0) return csprojFile;
			var changePackagePath = new List<string>();
			foreach (Match match in matches)
			{
				for (var i = 1; i < match.Groups.Count; i++)
				{
					if (!string.IsNullOrEmpty(match.Groups[i].Value))
						changePackagePath.Add(match.Groups[i].Value);
				}
			}
			var packagePath = SearchManager.GetSourcePath() + "\\Package\\";
			var findName = @"packages\";
			foreach (var packegePath in changePackagePath)
			{
				var endIndex = packegePath.IndexOf(findName, StringComparison.Ordinal) + findName.Length;
				var newPackagePath = packegePath.Remove(0, endIndex);
				csprojFile = csprojFile.Replace(packegePath, packagePath + newPackagePath);
			}
			return csprojFile;
		}
		private string GetCsprojFileByFullPath(string csprojFullPath)
		{
			string csprojFile;
			using (var reader = new StreamReader(csprojFullPath))
				csprojFile = reader.ReadToEnd();
			return csprojFile;
		}

		private void WriteNewCsprojFile(string csprojFullPath, string csprojFileDate)
		{
			using (var fs = new FileStream(csprojFullPath, FileMode.Create))
			{
				var array = Encoding.UTF8.GetBytes(csprojFileDate);
				fs.Write(array, 0, array.Length);
			}
		}
	}
}
