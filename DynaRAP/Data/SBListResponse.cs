﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynaRAP.Data
{
    public class SBListResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<ResponseSBList> response { get; set; }
        public ResponseAt responseAt { get; set; }
        public int resultCount { get; set; }
    }

    public class ResponseSBList
    {
        public string seq { get; set; }
        public string blockMetaSeq { get; set; }
        public string partSeq { get; set; }
        public int blockNo { get; set; }
        public string blockName { get; set; }
        public string julianStartAt { get; set; }
        public string julianEndAt { get; set; }
        public double offsetStartAt { get; set; }
        public double offsetEndAt { get; set; }
        public string registerUid { get; set; }
    }

    public class SBInfoResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public ResponseSBList response { get; set; }
        public ResponseAt responseAt { get; set; }
        public int resultCount { get; set; }
    }

    public class SBPartInfoResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public SBResponsePartInfo response { get; set; }
        public ResponseAt responseAt { get; set; }
        public int resultCount { get; set; }
    }

    public class SBResponsePartInfo
    {
        public string seq { get; set; }
        public string uploadSeq { get; set; }
        public string partName { get; set; }
        public string presetPack { get; set; }
        public string presetSeq { get; set; }
        public string julianStartAt { get; set; }
        public string julianEndAt { get; set; }
        public double offsetStartAt { get; set; }
        public double offsetEndAt { get; set; }
        public string registerUid { get; set; }
        public bool lpfDone { get; set; }
        public bool hpfDone { get; set; }
    } 
}
