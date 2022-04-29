package com.servetech.dynarap.vo;

import com.servetech.dynarap.db.type.CryptoField;
import com.servetech.dynarap.db.type.LongDate;
import lombok.Data;

@Data
public class ShortBlockVO {
    private CryptoField seq;
    private CryptoField blockMetaSeq;
    private CryptoField partSeq;
    private String julianStartAt;
    private String julianEndAt;
    private Double offsetStartAt;
    private Double offsetEndAt;
    private Integer blockNo;
    private CryptoField.NAuth registerUid;

    private Meta blockMetaInfo;
    private PartVO partInfo;

    @Data
    public static class Meta {
        private CryptoField seq;
        private CryptoField partSeq;
        private Float overlap;
        private Float sliceTime;
        private CryptoField.NAuth registerUid;
        private LongDate createdAt;

        private PartVO partInfo;
    }

    @Data
    public static class Param {
        private CryptoField seq;
        private CryptoField blockSeq;
        private CryptoField paramPack;
        private CryptoField paramSeq;

        private ShortBlockVO shortBlockInfo;
        private ParamVO paramInfo;
    }

    @Data
    public static class Raw implements IFlexibleValue {
        private CryptoField seq;
        private CryptoField blockSeq;
        private CryptoField presetParamSeq;
        private Double paramVal;
        private String paramValStr;
        private String julianTimeAt;
        private Double offsetTimeAt;
        private Double lpf;
        private Double hpf;

        private ShortBlockVO shortBlockInfo;
        private PresetVO.Param presetParamInfo;

        @Override
        public <T> T getValue() {
            if (paramVal == null && paramValStr == null)
                return null;

            if (paramVal != null) return (T) paramVal;

            return (T) paramValStr;
        }
    }
}