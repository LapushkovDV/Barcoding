using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ERP_API_Service.Models
{
    public class KatMcItem
    {
        public string NREC { get; set; }
        public DateTime ATL_LASTDATE { get; set; }
        public string ATL_LASTTIME { get; set; }
        public string ATL_LASTUSER { get; set; }
        public int ATL_ORIGINOFFICE { get; set; }
        public string ATL_OWNER { get; set; }
        public string ATL_BRANCH { get; set; }
        public string NAME { get; set; }
        public string OBOZN { get; set; }
        public string BARKOD { get; set; }
        public string BARKOD2 { get; set; }
        public string REMMC { get; set; }
        public string CGROUPMC { get; set; }
        public string KGROUPMC { get; set; }
        public string TNVED { get; set; }
        public string CED { get; set; }
        public string CMASED { get; set; }
        public double MASSA { get; set; }
        public double MTARA { get; set; }
        public double SIZEX { get; set; }
        public double SIZEY { get; set; }
        public double SIZEZ { get; set; }
        public double VOLUME { get; set; }
        public string CRESHR { get; set; }
        public double CLASSGR { get; set; }
        public double ZAPASMIN { get; set; }
        public double ZAPASMAX { get; set; }
        public double NORMUB { get; set; }
        public double PERCENTN { get; set; }
        public string CTYPE { get; set; }
        public string CTECH { get; set; }
        public bool PREUTIL { get; set; }
        public int DIVIDE { get; set; }
        public string BASEDSE { get; set; }
        public string CDEP { get; set; }
        public double PRICEPU { get; set; }
        public string CMCMAT { get; set; }
        public string RET1 { get; set; }
        public string RET2 { get; set; }
        public int PRIOR { get; set; }
        public int SALE { get; set; }
        public double CENAMC { get; set; }
        public double MAXPROCN { get; set; }
        public string CGRNAL { get; set; }
        public string OKDP { get; set; }
        public int KOMPLEKT { get; set; }
        public double NALPROD { get; set; }
        public double MAXON { get; set; }
        public double MAXRN { get; set; }
        public double MAXORN { get; set; }
        public string COKP { get; set; }
        public int ISARCH { get; set; }
        public double KOLDEF { get; set; }
        public double BONUS_PR { get; set; }
        public int KIND { get; set; }
        public int PRMAT { get; set; }
        public string CSTZATR { get; set; }
        public string CKAELEM { get; set; }
        public string CMASKMC { get; set; }
        public string CHASHAN { get; set; }
        public string CSLOJ { get; set; }
        public int POLZAK { get; set; }
        public string CUSL { get; set; }
        public string F_LND_N { get; set; }
        public string F_LND_S_1_ { get; set; }
        public string F_LND_S_2_ { get; set; }
        public string F_LND_S_3_ { get; set; }
        public string F_LND_S_4_ { get; set; }
        public string F_LND_S_5_ { get; set; }
        public int TPPLAN { get; set; }
        public double VHODON { get; set; }
        public int WPLANLEVEL { get; set; }
        public int WQUALITYCONTROL { get; set; }
        public int WINTERVALQC { get; set; }
        public string GOST { get; set; }
        public int DEFGODNDAYS { get; set; }
        public string DEFGODNHOURS { get; set; }
        public int WSERIALREG { get; set; }
        public int WBITFLAGS { get; set; }
        public string SIZES_1_ { get; set; }
        public string SIZES_2_ { get; set; }
        public string SIZES_3_ { get; set; }
        public string SIZES_4_ { get; set; }
        public string SIZES_5_ { get; set; }
        public string SIZES_6_ { get; set; }
        public string SIZES_7_ { get; set; }
        public string SIZES_8_ { get; set; }
        public string SIZES_9_ { get; set; }
        public string SIZES_10_ { get; set; }
        public string SIZES_11_ { get; set; }
        public string SIZES_12_ { get; set; }
        public string SIZES_13_ { get; set; }
        public string SIZES_14_ { get; set; }
        public string SIZES_15_ { get; set; }
        public string CGROUPSFO { get; set; }
        public string CSTATE { get; set; }
        public int WQUESTIONNAIRE { get; set; }
        public string CCOUTRYIMPORT { get; set; }
        public string TRANSTYPE { get; set; }
        public string TRANSVARNAME { get; set; }
        public string OKVED { get; set; }
        public string ARTICLE { get; set; }
    }

    public class KatMcJsonModel
    {
        [JsonProperty("@odata.context")]
        public string @OdataContext { get; set; }
        public List<KatMcItem> Value { get; set; }
    }


}
