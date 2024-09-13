using System;
using System.Collections.Generic;
using Galaktika.ERP.Interop;
using Galaktika.ESB.Adapter.Atlantis.Api;
using Galaktika.ESB.ServiceAdapterApi.AttributeService;
using Galaktika.ESB.Utils.LogService;

namespace Galaktika.ERP.Adapter.Atlantis
{
    /// <summary>
    /// AtlantisAttributeService
    /// </summary>
	public class AtlantisAttributeService : AttributeServiceBase
	{
		private Dictionary<string, List<AttributeInfo>> _attributeInfos;

        /// <summary>
        /// AtlantisAttributeService constructor
        /// </summary>
        /// <param name="serviceProvider">serviceProvider</param>
        public AtlantisAttributeService(IServiceProvider serviceProvider) : base(serviceProvider) { }

        /// <summary>
        /// Creates dictionary of attribute info
        /// </summary>
        protected override Dictionary<string, List<AttributeInfo>> AttributeInfos
		{
			get
			{
				if (_attributeInfos == null)
				{
					using (var ctx = new DictionaryApi.DictionaryContext())
					{
						_attributeInfos = new Dictionary<string, List<AttributeInfo>>();
						foreach (var file in ctx.GetMeta.GetTableInfos())
						{
							if (file.Fields.Count == 0)
								continue;
							var name = TableName(file);

							_attributeInfos[name] = new List<AttributeInfo>();

							foreach (var field in file.Fields)
							{
								Enum.TryParse(field.XE_DATATYPE.ToString(), out AtlTypes res);
								if (res == AtlTypes.String)
								{
									var ai = new AttributeInfo
									{
										MaxLength = field.XE_SIZE,
										ColumnName = ColumnName(field, file)
									};
									_attributeInfos[name].Add(ai);
								}
							}
						}
					}
				}
				return _attributeInfos;
			}
		}

		readonly string[] _tablesDict = new string[66000];
		readonly HashSet<string> _tables = new HashSet<string>();

        /// <summary>
        /// Gets table name by FILE
        /// </summary>
        /// <param name="tbl">FILE</param>
        /// <returns>Table name</returns>
		public string TableName(FILE tbl)
		{
			var tblCode = tbl.XF_CODE;
			var tblName = _tablesDict[tblCode];
			if (tblName != null)
				return tblName;

			tblName = tbl.XF_NAME.Replace("$", "_").Replace(".dat", "").Replace(".adf", "").Replace(".", "_").Replace("_DAT", "").ToUpper();

			if (_tables.Contains(tblName))
			{
				tblName = tbl.XF_NAME.Replace("$", "_").ToUpper();
			}

			//if (_changeTableName.ContainsKey(tbl.XF_CODE))
			//	return _changeTableName[tbl.XF_CODE];

			_tables.Add(tblName);
			_tablesDict[tblCode] = tblName;

			return tblName;
		}
        /// <summary>
        /// Returns column name by FILE and FIELD
        /// </summary>
        /// <param name="field">FIELD</param>
        /// <param name="tbl">FILE</param>
        /// <returns>Column name</returns>
		public string ColumnName(FIELD field, FILE tbl)
		{

			var str = field.XE_NAME.Replace('[', '_').Replace(']', '_').Replace("$", "_");
			if (str == TableName(tbl))
				str += "_";
			return str;
		}
	}
}
