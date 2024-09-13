using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Galaktika.ESB.Adapter.Atlantis.Api;

namespace Galaktika.ESB.Adapter.ERP.AppSpecificERP
{
    /// <summary>
    /// Post
    /// </summary>
	public class Post 
	{
        /// <summary>
        /// Catalogs
        /// </summary>
		public CATALOGS Catalogs { get; set; }

        /// <summary>
        /// NREC
        /// </summary>
		[Column("NREC", TypeName = "Comp")]
		[Key]
		// 1.Номер записи
		public System.Int64 NREC {
			get => Catalogs.NREC;
			set => Catalogs.NREC = value;
		}

        /// <summary>
        /// GroupCode
        /// </summary>
		public System.Int16 GroupCode {
			get => Catalogs.GROUPCODE;
			set => Catalogs.GROUPCODE = value;
		}
        /// <summary>
        /// SysCode
        /// </summary>
		public System.Int16 SysCode {
			get => Catalogs.SYSCODE;
			set => Catalogs.SYSCODE = value;
		}
        /// <summary>
        /// cParent
        /// </summary>
		public System.Int64 cParent {
			get => Catalogs.CPARENT;
			set => Catalogs.CPARENT = value;
		}
        /// <summary>
        /// AddInf
        /// </summary>
		public string AddInf {
			get => Catalogs.ADDINF;
			set => Catalogs.ADDINF = value;
		}
        /// <summary>
        /// Code
        /// </summary>
		public string Code {
			get => Catalogs.CODE;
			set => Catalogs.CODE = value;
		}
        /// <summary>
        /// Name
        /// </summary>
		public string Name {
			get => Catalogs.NAME;
			set => Catalogs.NAME = value;
		}
	}

    /// <summary>
    /// PostTest
    /// </summary>
	public class PostTest
	{
        /// <summary>
        /// Catalogs
        /// </summary>
		public CATALOGS Catalogs { get; set; }

        /// <summary>
        /// NREC
        /// </summary>
		[Column("NREC", TypeName = "Comp")]
		[Key]
		// 1.Номер записи
		public System.Int64 NREC
		{
			get => Catalogs.NREC;
			set => Catalogs.NREC = value;
		}

        /// <summary>
        /// GroupCode
        /// </summary>
		public System.Int16 GroupCode
		{
			get => Catalogs.GROUPCODE;
			set => Catalogs.GROUPCODE = value;
		}
        /// <summary>
        /// SysCode
        /// </summary>
		public System.Int16 SysCode
		{
			get => Catalogs.SYSCODE;
			set => Catalogs.SYSCODE = value;
		}
        /// <summary>
        /// cParent
        /// </summary>
		public System.Int64 cParent
		{
			get => Catalogs.CPARENT;
			set => Catalogs.CPARENT = value;
		}
        /// <summary>
        /// AddInf
        /// </summary>
		public string AddInf
		{
			get => Catalogs.ADDINF;
			set => Catalogs.ADDINF = value;
		}
        /// <summary>
        /// Code
        /// </summary>
		public string Code
		{
			get => Catalogs.CODE;
			set => Catalogs.CODE = value;
		}
        /// <summary>
        /// Name
        /// </summary>
		public string Name
		{
			get => Catalogs.NAME;
			set => Catalogs.NAME = value;
		}
	}
}
