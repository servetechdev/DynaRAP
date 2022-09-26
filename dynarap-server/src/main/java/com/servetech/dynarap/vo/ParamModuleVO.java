package com.servetech.dynarap.vo;

import com.servetech.dynarap.db.type.CryptoField;
import com.servetech.dynarap.db.type.LongDate;
import com.servetech.dynarap.db.type.String64;
import lombok.Data;

import java.util.List;
import java.util.Map;

@Data
public class ParamModuleVO {
    private CryptoField seq;
    private String64 moduleName;
    private CryptoField copyFromSeq;
    private LongDate createdAt;
    private boolean referenced;
    private transient boolean deleted;

    private ParamModuleVO originInfo;
    private Map<String, String> dataProp;

    private List<Equation> equations;
    private List<Source> sources;

    @Data
    public static class DataSource {
        protected String sourceType = "parammodule";
        protected CryptoField sourceSeq = null;
        protected CryptoField paramPack;
        protected CryptoField paramSeq;
        protected String julianStartAt;
        protected String julianEndAt;
        protected double offsetStartAt;
        protected double offsetEndAt;

        public static ParamModuleVO.DataSource getSource(ParamModuleVO paramModule, Equation equation) {
            ParamModuleVO.DataSource source = new ParamModuleVO.DataSource();
            source.setSourceType("parammodule");
            source.setSourceSeq(equation.getSeq());
            source.setJulianStartAt(equation.getJulianStartAt());
            source.setJulianEndAt(equation.getJulianEndAt());
            source.setOffsetStartAt(equation.getOffsetStartAt());
            source.setOffsetEndAt(equation.getOffsetEndAt());
            return source;
        }
    }

    @Data
    public static class Source {
        private CryptoField seq;
        private CryptoField moduleSeq;
        private String sourceType;
        private CryptoField sourceSeq;
        private CryptoField paramPack;
        private CryptoField paramSeq;
        private String julianStartAt;
        private String julianEndAt;
        private double offsetStartAt;
        private double offsetEndAt;

        private String64 sourceName;
        private String paramKey;
        private int dataCount;
        private String useTime;

        private transient boolean mark;
    }

    @Data
    public static class Equation {
        private CryptoField seq;
        private CryptoField moduleSeq;
        private String64 eqName;
        private String equation;
        private int eqOrder;
        private String julianStartAt;
        private String julianEndAt;
        private double offsetStartAt;
        private double offsetEndAt;
        private String useTime;
        private int dataCount;

        private Map<String, String> dataProp;
        private transient boolean mark;
    }

    @Data
    public static class Plot {
        private CryptoField seq;
        private CryptoField moduleSeq;
        private String64 plotName;
        private String plotType;
        private LongDate createdAt;
        private int plotOrder;

        private List<Source> plotSourceList;
        private List<PlotSource> plotSources;
        private Map<String, String> dataProp;

        @Data
        public static class Source {
            private CryptoField seq;
            private CryptoField moduleSeq;
            private CryptoField plotSeq;
            private String sourceType;
            private CryptoField sourceSeq;
            private CryptoField paramPack;
            private CryptoField paramSeq;
            private String julianStartAt;
            private String julianEndAt;
            private double offsetStartAt;
            private double offsetEndAt;

            public static PlotSource getSimple(Source source) {
                PlotSource plotSource = new PlotSource();
                plotSource.setSeq(source.getSeq());
                plotSource.setSourceType(source.getSourceType());
                plotSource.setSourceSeq(source.getSourceSeq());
                return plotSource;
            }
        }

        @Data
        public static class PlotSource {
            private CryptoField seq;
            private String sourceType;
            private CryptoField sourceSeq; /* source-list의 seq, eq-list 의 seq */
        }
    }
}
