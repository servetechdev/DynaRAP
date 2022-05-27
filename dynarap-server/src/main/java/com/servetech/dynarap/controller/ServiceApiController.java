package com.servetech.dynarap.controller;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
import com.servetech.dynarap.config.ServerConstants;
import com.servetech.dynarap.db.mapper.DirMapper;
import com.servetech.dynarap.db.service.*;
import com.servetech.dynarap.db.type.CryptoField;
import com.servetech.dynarap.db.type.String64;
import com.servetech.dynarap.ext.HandledServiceException;
import com.servetech.dynarap.ext.ResponseHelper;
import com.servetech.dynarap.vo.*;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.web.servlet.error.ErrorController;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.servlet.ModelAndView;

import javax.servlet.RequestDispatcher;
import javax.servlet.http.HttpServletRequest;
import java.util.*;

@Controller
@RequestMapping(value = "/api/{serviceVersion}")
public class ServiceApiController extends ApiController {
    @Value("${neoulsoft.auth.client-id}")
    private String authClientId;

    @Value("${neoulsoft.auth.client-secret}")
    private String authClientSecret;

    @RequestMapping(value = "/dir")
    @ResponseBody
    public Object apiDir(HttpServletRequest request, @PathVariable String serviceVersion,
                         @RequestBody JsonObject payload, Authentication authentication) throws HandledServiceException {
        /*
        String accessToken = request.getHeader("Authorization");
        if (accessToken == null || (!accessToken.startsWith("bearer") && !accessToken.startsWith("Bearer")))
            return ResponseHelper.error(403, "권한이 없습니다.");

        String username = authentication.getPrincipal().toString();
        */
        UserVO user = getService(UserService.class).getUser("admin@dynarap@dynarap");

        if (checkJsonEmpty(payload, "command"))
            throw new HandledServiceException(404, "파라미터를 확인하세요.");

        String command = payload.get("command").getAsString();

        if (command.equals("list")) {
            JsonObject result = getService(DirService.class).getDirList(user.getUid());
            return ResponseHelper.response(200, "Success - Dir List", result);
        }

        if (command.equals("add")) {
            DirVO added = getService(DirService.class).insertDir(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Dir Added", added);
        }

        if (command.equals("modify")) {
            DirVO updated = getService(DirService.class).updateDir(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Dir Updated", updated);
        }

        if (command.equals("remove")) {
            getService(DirService.class).deleteDir(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Dir Deleted", "");
        }

        throw new HandledServiceException(411, "명령이 정의되지 않았습니다.");
    }

    @RequestMapping(value = "/flight")
    @ResponseBody
    public Object apiFlight(HttpServletRequest request, @PathVariable String serviceVersion,
                            @RequestBody JsonObject payload, Authentication authentication) throws HandledServiceException {
        /*
        String accessToken = request.getHeader("Authorization");
        if (accessToken == null || (!accessToken.startsWith("bearer") && !accessToken.startsWith("Bearer")))
            return ResponseHelper.error(403, "권한이 없습니다.");

        String username = authentication.getPrincipal().toString();
        */
        UserVO user = getService(UserService.class).getUser("admin@dynarap@dynarap");

        if (checkJsonEmpty(payload, "command"))
            throw new HandledServiceException(404, "파라미터를 확인하세요.");

        String command = payload.get("command").getAsString();

        if (command.equals("list")) {
            List<FlightVO> flights = getService(FlightService.class).getFlightList();
            return ResponseHelper.response(200, "Success - Flight List", flights);
        }

        if (command.equals("add")) {
            FlightVO flight = getService(FlightService.class).insertFlight(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Flight Added", flight);
        }

        if (command.equals("modify")) {
            FlightVO flight = getService(FlightService.class).updateFlight(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Flight Updated", flight);
        }

        if (command.equals("remove")) {
            getService(FlightService.class).deleteFlight(payload);
            return ResponseHelper.response(200, "Success - Flight Deleted", "");
        }

        throw new HandledServiceException(411, "명령이 정의되지 않았습니다.");
    }

    @RequestMapping(value = "/param")
    @ResponseBody
    public Object apiParam(HttpServletRequest request, @PathVariable String serviceVersion,
                           @RequestBody JsonObject payload, Authentication authentication) throws HandledServiceException {
        /*
        String accessToken = request.getHeader("Authorization");
        if (accessToken == null || (!accessToken.startsWith("bearer") && !accessToken.startsWith("Bearer")))
            return ResponseHelper.error(403, "권한이 없습니다.");

        String username = authentication.getPrincipal().toString();
        */
        UserVO user = getService(UserService.class).getUser("admin@dynarap@dynarap");

        if (checkJsonEmpty(payload, "command"))
            throw new HandledServiceException(404, "파라미터를 확인하세요.");

        String command = payload.get("command").getAsString();

        if (command.equals("list")) {
            Integer pageNo = 1;
            if (!checkJsonEmpty(payload, "pageNo"))
                pageNo = payload.get("pageNo").getAsInt();

            Integer pageSize = 15;
            if (!checkJsonEmpty(payload, "pageSize"))
                pageSize = payload.get("pageSize").getAsInt();

            List<ParamVO> params = getService(ParamService.class).getParamList(pageNo, pageSize);
            int paramCount = getService(ParamService.class).getParamCount();

            return ResponseHelper.response(200, "Success - Param List", paramCount, params);
        }

        if (command.equals("info")) {
            CryptoField paramSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "seq"))
                paramSeq = CryptoField.decode(payload.get("seq").getAsString(), 0L);

            CryptoField paramPack = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "paramPack"))
                paramPack = CryptoField.decode(payload.get("paramPack").getAsString(), 0L);

            if (paramPack == null || paramPack.isEmpty())
                throw new HandledServiceException(404, "파라미터를 확인하세요.");

            ParamVO param = null;
            if (paramSeq == null || paramSeq.isEmpty())
                param = getService(ParamService.class).getActiveParam(paramPack);
            else
                param = getService(ParamService.class).getParamBySeq(paramSeq);

            return ResponseHelper.response(200, "Success - Param Info", param);
        }

        if (command.equals("add")) {
            ParamVO param = getService(ParamService.class).insertParam(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Param Added", param);
        }

        if (command.equals("add-bulk")) {
            JsonArray jarrBulk = payload.get("params").getAsJsonArray();
            List<ParamVO> params = new ArrayList<>();
            long msec = System.currentTimeMillis();
            for (int i = 0; i < jarrBulk.size(); i++) {
                JsonObject jobjParam = jarrBulk.get(i).getAsJsonObject();
                jobjParam.addProperty("paramName", new String64("param_" + (msec + i)).valueOf());
                ParamVO param = getService(ParamService.class).insertParam(user.getUid(), jobjParam);
                params.add(param);
            }
            return ResponseHelper.response(200, "Success - Param Added (Bulk) ", params);
        }

        if (command.equals("modify")) {
            ParamVO param = getService(ParamService.class).updateParam(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Param Updated", param);
        }

        if (command.equals("remove")) {
            getService(ParamService.class).deleteParam(payload);
            return ResponseHelper.response(200, "Success - Param Deleted", "");
        }

        if (command.equals("prop-list")) {
            String propType = null;
            if (!checkJsonEmpty(payload, "propType"))
                propType = payload.get("propType").getAsString();

            List<ParamVO.Prop> paramProps = getService(ParamService.class).getParamPropList(propType);
            return ResponseHelper.response(200, "Success - Param Prop List", paramProps);
        }

        if (command.equals("prop-add")) {
            ParamVO.Prop paramProp = getService(ParamService.class).insertParamProp(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Param Prop Add", paramProp);
        }

        if (command.equals("prop-modify")) {
            ParamVO.Prop paramProp = getService(ParamService.class).updateParamProp(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Param Prop Modify", paramProp);
        }

        throw new HandledServiceException(411, "명령이 정의되지 않았습니다.");
    }

    @RequestMapping(value = "/preset")
    @ResponseBody
    public Object apiPreset(HttpServletRequest request, @PathVariable String serviceVersion,
                            @RequestBody JsonObject payload, Authentication authentication) throws HandledServiceException {
        /*
        String accessToken = request.getHeader("Authorization");
        if (accessToken == null || (!accessToken.startsWith("bearer") && !accessToken.startsWith("Bearer")))
            return ResponseHelper.error(403, "권한이 없습니다.");

        String username = authentication.getPrincipal().toString();
        */
        UserVO user = getService(UserService.class).getUser("admin@dynarap@dynarap");

        if (checkJsonEmpty(payload, "command"))
            throw new HandledServiceException(404, "파라미터를 확인하세요.");

        String command = payload.get("command").getAsString();

        if (command.equals("list")) {
            Integer pageNo = 1;
            if (!checkJsonEmpty(payload, "pageNo"))
                pageNo = payload.get("pageNo").getAsInt();

            Integer pageSize = 15;
            if (!checkJsonEmpty(payload, "pageSize"))
                pageSize = payload.get("pageSize").getAsInt();

            List<PresetVO> presets = getService(ParamService.class).getPresetList(pageNo, pageSize);
            int presetCount = getService(ParamService.class).getPresetCount();

            return ResponseHelper.response(200, "Success - Preset List", presetCount, presets);
        }

        if (command.equals("info")) {
            CryptoField presetSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "seq"))
                presetSeq = CryptoField.decode(payload.get("seq").getAsString(), 0L);

            CryptoField presetPack = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "presetPack"))
                presetPack = CryptoField.decode(payload.get("presetPack").getAsString(), 0L);

            if (presetPack == null || presetPack.isEmpty())
                throw new HandledServiceException(404, "파라미터를 확인하세요.");

            PresetVO preset = null;
            if (presetSeq == null || presetSeq.isEmpty())
                preset = getService(ParamService.class).getActivePreset(presetPack);
            else
                preset = getService(ParamService.class).getPresetBySeq(presetSeq);

            return ResponseHelper.response(200, "Success - Preset Info", preset);
        }

        if (command.equals("add")) {
            PresetVO preset = getService(ParamService.class).insertPreset(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Preset Added", preset);
        }

        if (command.equals("modify")) {
            PresetVO preset = getService(ParamService.class).updatePreset(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Preset Updated", preset);
        }

        if (command.equals("remove")) {
            getService(ParamService.class).deletePreset(payload);
            return ResponseHelper.response(200, "Success - Preset Deleted", "");
        }

        if (command.equals("param-list")) {
            CryptoField presetPack = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "presetPack"))
                presetPack = CryptoField.decode(payload.get("presetPack").getAsString(), 0L);

            CryptoField presetSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "presetSeq"))
                presetSeq = CryptoField.decode(payload.get("presetSeq").getAsString(), 0L);

            CryptoField paramPack = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "paramPack"))
                paramPack = CryptoField.decode(payload.get("paramPack").getAsString(), 0L);

            CryptoField paramSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "paramSeq"))
                paramSeq = CryptoField.decode(payload.get("paramSeq").getAsString(), 0L);

            Integer pageNo = 1;
            if (!checkJsonEmpty(payload, "pageNo"))
                pageNo = payload.get("pageNo").getAsInt();

            Integer pageSize = 15;
            if (!checkJsonEmpty(payload, "pageSize"))
                pageSize = payload.get("pageSize").getAsInt();

            List<ParamVO> paramList = getService(ParamService.class).getPresetParamList(
                    presetPack, presetSeq, paramPack, paramSeq, pageNo, pageSize);
            int paramCount = getService(ParamService.class).getPresetParamCount(
                    presetPack, presetSeq, paramPack, paramSeq);

            return ResponseHelper.response(200, "Success - Preset Param List", paramCount, paramList);
        }

        if (command.equals("param-add")) {
            getService(ParamService.class).insertPresetParam(payload);
            return ResponseHelper.response(200, "Success - Preset Param Add", "");
        }

        if (command.equals("param-add-bulk")) {
            JsonArray jarrBulk = payload.get("params").getAsJsonArray();

            for (int i = 0; i < jarrBulk.size(); i++) {
                JsonObject jobjParam = jarrBulk.get(i).getAsJsonObject();
                getService(ParamService.class).insertPresetParam(jobjParam);
            }
            return ResponseHelper.response(200, "Success - Preset Param Add Bulk", "");
        }

        if (command.equals("param-remove")) {
            getService(ParamService.class).deletePresetParam(payload);
            return ResponseHelper.response(200, "Success - Preset Param Remove", "");
        }

        throw new HandledServiceException(411, "명령이 정의되지 않았습니다.");
    }

    @RequestMapping(value = "/dll")
    @ResponseBody
    public Object apiDLL(HttpServletRequest request, @PathVariable String serviceVersion,
                         @RequestBody JsonObject payload, Authentication authentication) throws HandledServiceException {
        /*
        String accessToken = request.getHeader("Authorization");
        if (accessToken == null || (!accessToken.startsWith("bearer") && !accessToken.startsWith("Bearer")))
            return ResponseHelper.error(403, "권한이 없습니다.");

        String username = authentication.getPrincipal().toString();
        */
        UserVO user = getService(UserService.class).getUser("admin@dynarap@dynarap");

        if (checkJsonEmpty(payload, "command"))
            throw new HandledServiceException(404, "파라미터를 확인하세요.");

        String command = payload.get("command").getAsString();

        if (command.equals("list")) {
            List<DLLVO> dlls = getService(DLLService.class).getDLLList();
            return ResponseHelper.response(200, "Success - DLL List", dlls);
        }

        if (command.equals("add")) {
            DLLVO dll = getService(DLLService.class).insertDLL(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - DLL Added", dll);
        }

        if (command.equals("modify")) {
            DLLVO dll = getService(DLLService.class).updateDLL(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - DLL Updated", dll);
        }

        if (command.equals("remove")) {
            getService(DLLService.class).deleteDLL(payload);
            return ResponseHelper.response(200, "Success - DLL Deleted", "");
        }

        if (command.equals("param-list")) {
            CryptoField dllSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "dllSeq"))
                dllSeq = CryptoField.decode(payload.get("dllSeq").getAsString(), 0L);

            if (dllSeq == null || dllSeq.isEmpty())
                throw new HandledServiceException(411, "요청 파라미터 오류입니다. [필수 파라미터 누락]");

            List<DLLVO.Param> paramList = getService(DLLService.class).getDLLParamList(dllSeq);
            return ResponseHelper.response(200, "Success - DLL Param List", paramList);
        }

        if (command.equals("param-add")) {
            DLLVO.Param dllParam = getService(DLLService.class).insertDLLParam(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - DLL Param Add", dllParam);
        }

        if (command.equals("param-modify")) {
            DLLVO.Param dllParam = getService(DLLService.class).updateDLLParam(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - DLL Param Modify", dllParam);
        }

        if (command.equals("param-remove")) {
            getService(DLLService.class).deleteDLLParam(payload);
            return ResponseHelper.response(200, "Success - DLL Param Remove", "");
        }

        if (command.equals("param-remove-multi")) {
            getService(DLLService.class).deleteDLLParamByMulti(payload);
            return ResponseHelper.response(200, "Success - DLL Param All Remove", "");
        }

        if (command.equals("data-list")) {
            CryptoField dllSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "dllSeq"))
                dllSeq = CryptoField.decode(payload.get("dllSeq").getAsString(), 0L);

            if (dllSeq == null || dllSeq.isEmpty())
                throw new HandledServiceException(411, "요청 파라미터 오류입니다. [필수 파라미터 누락]");

            CryptoField dllParamSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "dllParamSeq"))
                dllParamSeq = CryptoField.decode(payload.get("dllParamSeq").getAsString(), 0L);

            List<DLLVO.Raw> rawData = getService(DLLService.class).getDLLData(dllSeq, dllParamSeq);
            return ResponseHelper.response(200, "Success - DLL Data List", rawData);
        }

        if (command.equals("data-add")) {
            DLLVO.Raw dllRaw = getService(DLLService.class).insertDLLData(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - DLL Data Add", dllRaw);
        }

        if (command.equals("data-modify")) {
            DLLVO.Raw dllRaw = getService(DLLService.class).updateDLLData(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - DLL Data Modify", dllRaw);
        }

        if (command.equals("data-remove-row")) {
            getService(DLLService.class).deleteDLLDataByRow(payload);
            return ResponseHelper.response(200, "Success - DLL Data Remove By Row", "");
        }

        if (command.equals("data-remove-param")) {
            getService(DLLService.class).deleteDLLDataByParam(payload);
            return ResponseHelper.response(200, "Success - DLL Data Remove By Param or All", "");
        }

        throw new HandledServiceException(411, "명령이 정의되지 않았습니다.");
    }

    @RequestMapping(value = "/raw")
    @ResponseBody
    public Object apiRaw(HttpServletRequest request, @PathVariable String serviceVersion,
                         @RequestBody JsonObject payload, Authentication authentication) throws HandledServiceException {
        /*
        String accessToken = request.getHeader("Authorization");
        if (accessToken == null || (!accessToken.startsWith("bearer") && !accessToken.startsWith("Bearer")))
            return ResponseHelper.error(403, "권한이 없습니다.");

        String username = authentication.getPrincipal().toString();
        */
        UserVO user = getService(UserService.class).getUser("admin@dynarap@dynarap");

        if (checkJsonEmpty(payload, "command"))
            throw new HandledServiceException(404, "파라미터를 확인하세요.");

        String command = payload.get("command").getAsString();

        if (command.equals("import")) {
            // need - uploadSeq, presetPack, presetSeq, flightSeq, flightAt
            CryptoField uploadSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "uploadSeq"))
                uploadSeq = CryptoField.decode(payload.get("uploadSeq").getAsString(), 0L);

            CryptoField presetPack = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "presetPack"))
                presetPack = CryptoField.decode(payload.get("presetPack").getAsString(), 0L);

            CryptoField presetSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "presetSeq"))
                presetSeq = CryptoField.decode(payload.get("presetSeq").getAsString(), 0L);

            CryptoField flightSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "flightSeq"))
                flightSeq = CryptoField.decode(payload.get("flightSeq").getAsString(), 0L);

            String flightAt = "";
            if (!checkJsonEmpty(payload, "flightAt"))
                flightAt = payload.get("flightAt").getAsString();

            String dataType = "";
            if (!checkJsonEmpty(payload, "dataType"))
                dataType = payload.get("dataType").getAsString();

            if (uploadSeq == null || uploadSeq.isEmpty() || presetPack == null || presetPack.isEmpty() || dataType == null || dataType.isEmpty())
                throw new HandledServiceException(404, "필요 파라미터가 누락됐습니다.");

            JsonObject jobjResult = getService(RawService.class).runImport(user.getUid(), uploadSeq, presetPack, presetSeq, flightSeq, flightAt, dataType);

            return ResponseHelper.response(200, "Success - Import request done", jobjResult);
        }

        if (command.equals("create-cache")) {
            // need - uploadSeq, presetPack, presetSeq, flightSeq, flightAt
            CryptoField uploadSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "uploadSeq"))
                uploadSeq = CryptoField.decode(payload.get("uploadSeq").getAsString(), 0L);

            if (uploadSeq == null || uploadSeq.isEmpty())
                throw new HandledServiceException(404, "필요 파라미터가 누락됐습니다.");

            JsonObject jobjResult = getService(RawService.class).createCache(user.getUid(), uploadSeq);

            return ResponseHelper.response(200, "Success - Create cache done", jobjResult);
        }

        if (command.equals("check-done")) {
            CryptoField uploadSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "uploadSeq"))
                uploadSeq = CryptoField.decode(payload.get("uploadSeq").getAsString(), 0L);

            RawVO.Upload rawUpload = getService(RawService.class).getUploadBySeq(uploadSeq);
            return ResponseHelper.response(200, "Success - Check Import Done", rawUpload.isImportDone());
        }

        // 업로드 리스트 가져가기
        if (command.equals("upload")) {
            RawVO.Upload rawUpload = getService(RawService.class).doUpload(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Upload Raw Data", rawUpload);
        }

        if (command.equals("progress")) {
            RawVO.Upload rawUpload = getService(RawService.class).getProgress(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Get Upload Progress", rawUpload);
        }

        // 업로드 리스트 가져가기
        if (command.equals("upload-list")) {
            List<RawVO.Upload> rawUploadList = getService(RawService.class).getUploadList();
            return ResponseHelper.response(200, "Success - Upload list", rawUploadList);
        }

        // 파트 쪼개는 작업 진행해야함.
        if (command.equals("create-part")) {
            List<PartVO> partList = getService(PartService.class).createPartList(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Create raw data part", partList);
        }

        throw new HandledServiceException(411, "명령이 정의되지 않았습니다.");
    }

    @RequestMapping(value = "/part")
    @ResponseBody
    public Object apiPart(HttpServletRequest request, @PathVariable String serviceVersion,
                         @RequestBody JsonObject payload, Authentication authentication) throws HandledServiceException {
        /*
        String accessToken = request.getHeader("Authorization");
        if (accessToken == null || (!accessToken.startsWith("bearer") && !accessToken.startsWith("Bearer")))
            return ResponseHelper.error(403, "권한이 없습니다.");

        String username = authentication.getPrincipal().toString();
        */
        UserVO user = getService(UserService.class).getUser("admin@dynarap@dynarap");

        if (checkJsonEmpty(payload, "command"))
            throw new HandledServiceException(404, "파라미터를 확인하세요.");

        String command = payload.get("command").getAsString();

        if (command.equals("list")) {
            CryptoField.NAuth registerUid = null;
            if (!checkJsonEmpty(payload, "registerUid"))
                registerUid = CryptoField.NAuth.decode(payload.get("registerUid").getAsString(), 0L);

            CryptoField uploadSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "uploadSeq"))
                uploadSeq = CryptoField.decode(payload.get("uploadSeq").getAsString(), 0L);

            Integer pageNo = 1;
            if (!checkJsonEmpty(payload, "pageNo"))
                pageNo = payload.get("pageNo").getAsInt();

            Integer pageSize = 15;
            if (!checkJsonEmpty(payload, "pageSize"))
                pageSize = payload.get("pageSize").getAsInt();

            List<PartVO> partList = getService(PartService.class).getPartList(registerUid, uploadSeq, pageNo, pageSize);
            int partCount = getService(PartService.class).getPartCount(registerUid, uploadSeq);

            return ResponseHelper.response(200, "Success - Part List", partCount, partList);
        }

        if (command.equals("info")) {
            CryptoField partSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "partSeq"))
                partSeq = CryptoField.decode(payload.get("partSeq").getAsString(), 0L);

            PartVO partInfo = getService(PartService.class).getPartBySeq(partSeq);

            return ResponseHelper.response(200, "Success - Part Info", partInfo);
        }

        // 업로드 리스트 가져가기
        if (command.equals("create-shortblock")) {
            ShortBlockVO.Meta shortBlockMeta = getService(PartService.class).doCreateShortBlock(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Create ShortBlock Data", shortBlockMeta);
        }

        if (command.equals("progress")) {
            ShortBlockVO.Meta shortBlockMeta = getService(PartService.class).getProgress(user.getUid(), payload);
            return ResponseHelper.response(200, "Success - Get Create ShortBlock Progress", shortBlockMeta);
        }

        if (command.equals("row-data")) {
            CryptoField partSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "partSeq"))
                partSeq = CryptoField.decode(payload.get("partSeq").getAsString(), 0L);

            PartVO partInfo = getService(PartService.class).getPartBySeq(partSeq);

            // 요청 파라미터 셋.
            JsonObject jobjResult = new JsonObject();
            JsonArray jarrParams = null;
            if (!checkJsonEmpty(payload, "paramSet"))
                jarrParams = payload.get("paramSet").getAsJsonArray();
            List<ParamVO> params = new ArrayList<>();

            if (jarrParams == null || jarrParams.size() == 0) {
                List<String> paramSet = listOps.range("P" + partInfo.getSeq().originOf(), 0, Integer.MAX_VALUE);
                for (String p : paramSet) {
                    ParamVO param = getService(ParamService.class).getPresetParamBySeq(
                            new CryptoField(Long.parseLong(p.substring(1))));
                    if (param == null) continue;
                    params.add(param);
                }
            }
            else {
                for (int i = 0; i < jarrParams.size(); i++) {
                    Long paramKey = jarrParams.get(i).getAsLong();
                    ParamVO param = getService(ParamService.class).getPresetParamBySeq(new CryptoField(paramKey));
                    if (param == null) continue;
                    params.add(param);
                }
            }

            JsonArray jarrJulian = payload.get("julianRange").getAsJsonArray();
            String julianFrom = jarrJulian.get(0).getAsString();
            if (julianFrom == null || julianFrom.isEmpty())
                julianFrom = zsetOps.rangeByScore("P" + partInfo.getSeq().originOf() + ".R", 0, Integer.MAX_VALUE).iterator().next();
            String julianTo = jarrJulian.get(1).getAsString();
            if (julianTo == null || julianTo.isEmpty())
                julianTo = zsetOps.reverseRangeByScore("P" + partInfo.getSeq().originOf() + ".R", 0, Integer.MAX_VALUE).iterator().next();

            String julianStart = zsetOps.rangeByScore("P" + partInfo.getSeq().originOf() + ".R", 0, Integer.MAX_VALUE).iterator().next();
            Long startRowAt = zsetOps.score("P" + partInfo.getSeq().originOf() + ".R", julianStart).longValue();

            Long rankFrom = zsetOps.rank("P" + partInfo.getSeq().originOf() + ".R", julianFrom);
            if (rankFrom == null) {
                julianFrom = zsetOps.rangeByScore("P" + partInfo.getSeq().originOf() + ".R", 0, Integer.MAX_VALUE).iterator().next();
                rankFrom = zsetOps.rank("P" + partInfo.getSeq().originOf() + ".R", julianFrom);
            }
            Long rankTo = zsetOps.rank("P" + partInfo.getSeq().originOf() + ".R", julianTo);
            if (rankTo == null) {
                julianTo = zsetOps.reverseRangeByScore("P" + partInfo.getSeq().originOf() + ".R", 0, Integer.MAX_VALUE).iterator().next();
                rankTo = zsetOps.rank("P" + partInfo.getSeq().originOf() + ".R", julianTo);
            }

            LinkedHashMap<String, List<Double>> rowData = new LinkedHashMap<>();
            for (ParamVO p : params) {
                Set<String> listSet = zsetOps.rangeByScore(
                        "P" + partInfo.getSeq().originOf() + ".N" + p.getPresetParamSeq(), startRowAt + rankFrom, startRowAt + rankTo);

                Iterator<String> iterListSet = listSet.iterator();
                while (iterListSet.hasNext()) {
                    String rowVal = iterListSet.next();
                    String julianTime = rowVal.substring(0, rowVal.lastIndexOf(":"));
                    List<Double> rowList = rowData.get(julianTime);
                    if (rowList == null) {
                        rowList = new ArrayList<>();
                        rowData.put(julianTime, rowList);
                    }
                    Double dblVal = Double.parseDouble(rowVal.substring(rowVal.lastIndexOf(":") + 1));
                    rowList.add(dblVal);
                }
            }

            jobjResult.add("paramSet", ServerConstants.GSON.toJsonTree(params));
            jobjResult.add("julianSet", ServerConstants.GSON.toJsonTree(Arrays.asList(rowData.keySet())));
            jobjResult.add("data", ServerConstants.GSON.toJsonTree(rowData.values()));

            return ResponseHelper.response(200, "Success - rowData", jobjResult);
        }

        if (command.equals("column-data")) {
            CryptoField partSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "partSeq"))
                partSeq = CryptoField.decode(payload.get("partSeq").getAsString(), 0L);

            PartVO partInfo = getService(PartService.class).getPartBySeq(partSeq);

            // 요청 파라미터 셋.
            JsonObject jobjResult = new JsonObject();
            JsonArray jarrParams = null;
            if (!checkJsonEmpty(payload, "paramSet"))
                jarrParams = payload.get("paramSet").getAsJsonArray();
            List<ParamVO> params = new ArrayList<>();

            if (jarrParams == null || jarrParams.size() == 0) {
                List<String> paramSet = listOps.range("P" + partInfo.getSeq().originOf(), 0, Integer.MAX_VALUE);
                for (String p : paramSet) {
                    ParamVO param = getService(ParamService.class).getPresetParamBySeq(
                            new CryptoField(Long.parseLong(p.substring(1))));
                    if (param == null) continue;
                    params.add(param);
                }
            }
            else {
                for (int i = 0; i < jarrParams.size(); i++) {
                    Long paramKey = jarrParams.get(i).getAsLong();
                    ParamVO param = getService(ParamService.class).getPresetParamBySeq(new CryptoField(paramKey));
                    if (param == null) continue;
                    params.add(param);
                }
            }

            List<String> julianData = new ArrayList<>();
            LinkedHashMap<String, List<Double>> paramData = new LinkedHashMap<>();
            for (ParamVO p : params) {
                Set<String> listSet = zsetOps.rangeByScore(
                        "P" + partInfo.getSeq().originOf() + ".N" + p.getPresetParamSeq(), 0, Integer.MAX_VALUE);
                List<Double> rowData = new ArrayList<>();
                paramData.put(p.getParamKey(), rowData);

                Iterator<String> iterListSet = listSet.iterator();
                while (iterListSet.hasNext()) {
                    String rowVal = iterListSet.next();
                    String julianTime = rowVal.substring(0, rowVal.lastIndexOf(":"));
                    if (!julianData.contains(julianTime)) julianData.add(julianTime);

                    Double dblVal = Double.parseDouble(rowVal.substring(rowVal.lastIndexOf(":") + 1));
                    rowData.add(dblVal);
                }
            }

            jobjResult.add("julianSet", ServerConstants.GSON.toJsonTree(Arrays.asList(julianData)));
            jobjResult.add("data", ServerConstants.GSON.toJsonTree(paramData.values()));

            return ResponseHelper.response(200, "Success - columnData", jobjResult);
        }

        throw new HandledServiceException(411, "명령이 정의되지 않았습니다.");
    }

    @RequestMapping(value = "/shortblock")
    @ResponseBody
    public Object apiShortBlock(HttpServletRequest request, @PathVariable String serviceVersion,
                          @RequestBody JsonObject payload, Authentication authentication) throws HandledServiceException {
        /*
        String accessToken = request.getHeader("Authorization");
        if (accessToken == null || (!accessToken.startsWith("bearer") && !accessToken.startsWith("Bearer")))
            return ResponseHelper.error(403, "권한이 없습니다.");

        String username = authentication.getPrincipal().toString();
        */
        UserVO user = getService(UserService.class).getUser("admin@dynarap@dynarap");

        if (checkJsonEmpty(payload, "command"))
            throw new HandledServiceException(404, "파라미터를 확인하세요.");

        String command = payload.get("command").getAsString();

        if (command.equals("list")) {
            CryptoField.NAuth registerUid = null;
            if (!checkJsonEmpty(payload, "registerUid"))
                registerUid = CryptoField.NAuth.decode(payload.get("registerUid").getAsString(), 0L);

            CryptoField partSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "partSeq"))
                partSeq = CryptoField.decode(payload.get("partSeq").getAsString(), 0L);

            Integer pageNo = 1;
            if (!checkJsonEmpty(payload, "pageNo"))
                pageNo = payload.get("pageNo").getAsInt();

            Integer pageSize = 15;
            if (!checkJsonEmpty(payload, "pageSize"))
                pageSize = payload.get("pageSize").getAsInt();

            List<ShortBlockVO> shortBlockList = getService(PartService.class).getShortBlockList(registerUid, partSeq, pageNo, pageSize);
            int shortBlockCount = getService(PartService.class).getShortBlockCount(registerUid, partSeq);

            return ResponseHelper.response(200, "Success - Short Block List", shortBlockCount, shortBlockList);
        }

        if (command.equals("info")) {
            CryptoField blockSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "blockSeq"))
                blockSeq = CryptoField.decode(payload.get("blockSeq").getAsString(), 0L);

            ShortBlockVO shortBlockInfo = getService(PartService.class).getShortBlockBySeq(blockSeq);

            return ResponseHelper.response(200, "Success - ShortBlock Info", shortBlockInfo);
        }

        if (command.equals("remove-meta")) {
            CryptoField blockMetaSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "blockMetaSeq"))
                blockMetaSeq = CryptoField.decode(payload.get("blockMetaSeq").getAsString(), 0L);

            //getService(PartService.class).deleteShortBlockMeta(blockMetaSeq);

            return ResponseHelper.response(200, "Success - Remove ShortBlock Meta", "");
        }

        if (command.equals("row-data")) {
            CryptoField blockSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "blockSeq"))
                blockSeq = CryptoField.decode(payload.get("blockSeq").getAsString(), 0L);

            ShortBlockVO blockInfo = getService(PartService.class).getShortBlockBySeq(blockSeq);

            // 요청 파라미터 셋.
            JsonObject jobjResult = new JsonObject();
            JsonArray jarrParams = null;
            if (!checkJsonEmpty(payload, "paramSet"))
                jarrParams = payload.get("paramSet").getAsJsonArray();
            List<ParamVO> params = new ArrayList<>();

            if (jarrParams == null || jarrParams.size() == 0) {
                List<String> paramSet = listOps.range("S" + blockInfo.getSeq().originOf(), 0, Integer.MAX_VALUE);
                for (String p : paramSet) {
                    // FIXME : unmapped params 처리
                    ParamVO param = getService(ParamService.class).getPresetParamBySeq(
                            new CryptoField(Long.parseLong(p.substring(1))));
                    if (param == null) continue;
                    params.add(param);
                }
            }
            else {
                for (int i = 0; i < jarrParams.size(); i++) {
                    Long paramKey = jarrParams.get(i).getAsLong();
                    ParamVO param = getService(ParamService.class).getPresetParamBySeq(new CryptoField(paramKey));
                    if (param == null) continue;
                    params.add(param);
                }
            }

            JsonArray jarrJulian = payload.get("julianRange").getAsJsonArray();
            String julianFrom = jarrJulian.get(0).getAsString();
            if (julianFrom == null || julianFrom.isEmpty())
                julianFrom = zsetOps.rangeByScore("S" + blockInfo.getSeq().originOf() + ".R", 0, Integer.MAX_VALUE).iterator().next();
            String julianTo = jarrJulian.get(1).getAsString();
            if (julianTo == null || julianTo.isEmpty())
                julianTo = zsetOps.reverseRangeByScore("S" + blockInfo.getSeq().originOf() + ".R", 0, Integer.MAX_VALUE).iterator().next();

            String julianStart = zsetOps.rangeByScore("S" + blockInfo.getSeq().originOf() + ".R", 0, Integer.MAX_VALUE).iterator().next();
            Long startRowAt = zsetOps.score("S" + blockInfo.getSeq().originOf() + ".R", julianStart).longValue();

            Long rankFrom = zsetOps.rank("S" + blockInfo.getSeq().originOf() + ".R", julianFrom);
            if (rankFrom == null) {
                julianFrom = zsetOps.rangeByScore("S" + blockInfo.getSeq().originOf() + ".R", 0, Integer.MAX_VALUE).iterator().next();
                rankFrom = zsetOps.rank("S" + blockInfo.getSeq().originOf() + ".R", julianFrom);
            }
            Long rankTo = zsetOps.rank("S" + blockInfo.getSeq().originOf() + ".R", julianTo);
            if (rankTo == null) {
                julianTo = zsetOps.reverseRangeByScore("S" + blockInfo.getSeq().originOf() + ".R", 0, Integer.MAX_VALUE).iterator().next();
                rankTo = zsetOps.rank("S" + blockInfo.getSeq().originOf() + ".R", julianTo);
            }

            LinkedHashMap<String, List<Double>> rowData = new LinkedHashMap<>();
            for (ParamVO p : params) {
                Set<String> listSet = zsetOps.rangeByScore(
                        "S" + blockInfo.getSeq().originOf() + ".N" + p.getPresetParamSeq(), startRowAt + rankFrom, startRowAt + rankTo);

                Iterator<String> iterListSet = listSet.iterator();
                while (iterListSet.hasNext()) {
                    String rowVal = iterListSet.next();
                    String julianTime = rowVal.substring(0, rowVal.lastIndexOf(":"));
                    List<Double> rowList = rowData.get(julianTime);
                    if (rowList == null) {
                        rowList = new ArrayList<>();
                        rowData.put(julianTime, rowList);
                    }
                    Double dblVal = Double.parseDouble(rowVal.substring(rowVal.lastIndexOf(":") + 1));
                    rowList.add(dblVal);
                }
            }

            jobjResult.add("paramSet", ServerConstants.GSON.toJsonTree(params));
            jobjResult.add("julianSet", ServerConstants.GSON.toJsonTree(Arrays.asList(rowData.keySet())));
            jobjResult.add("data", ServerConstants.GSON.toJsonTree(rowData.values()));

            return ResponseHelper.response(200, "Success - rowData", jobjResult);
        }

        if (command.equals("column-data")) {
            CryptoField blockSeq = CryptoField.LZERO;
            if (!checkJsonEmpty(payload, "blockSeq"))
                blockSeq = CryptoField.decode(payload.get("blockSeq").getAsString(), 0L);

            ShortBlockVO blockInfo = getService(PartService.class).getShortBlockBySeq(blockSeq);

            // 요청 파라미터 셋.
            JsonObject jobjResult = new JsonObject();
            JsonArray jarrParams = null;
            if (!checkJsonEmpty(payload, "paramSet"))
                jarrParams = payload.get("paramSet").getAsJsonArray();
            List<ParamVO> params = new ArrayList<>();

            if (jarrParams == null || jarrParams.size() == 0) {
                List<String> paramSet = listOps.range("S" + blockInfo.getSeq().originOf(), 0, Integer.MAX_VALUE);
                for (String p : paramSet) {
                    ParamVO param = getService(ParamService.class).getPresetParamBySeq(
                            new CryptoField(Long.parseLong(p.substring(1))));
                    if (param == null) continue;
                    params.add(param);
                }
            }
            else {
                for (int i = 0; i < jarrParams.size(); i++) {
                    Long paramKey = jarrParams.get(i).getAsLong();
                    ParamVO param = getService(ParamService.class).getPresetParamBySeq(new CryptoField(paramKey));
                    if (param == null) continue;
                    params.add(param);
                }
            }

            List<String> julianData = new ArrayList<>();
            LinkedHashMap<String, List<Double>> paramData = new LinkedHashMap<>();
            for (ParamVO p : params) {
                Set<String> listSet = zsetOps.rangeByScore(
                        "S" + blockInfo.getSeq().originOf() + ".N" + p.getPresetParamSeq(), 0, Integer.MAX_VALUE);
                List<Double> rowData = new ArrayList<>();
                paramData.put(p.getParamKey(), rowData);

                Iterator<String> iterListSet = listSet.iterator();
                while (iterListSet.hasNext()) {
                    String rowVal = iterListSet.next();
                    String julianTime = rowVal.substring(0, rowVal.lastIndexOf(":"));
                    if (!julianData.contains(julianTime)) julianData.add(julianTime);

                    Double dblVal = Double.parseDouble(rowVal.substring(rowVal.lastIndexOf(":") + 1));
                    rowData.add(dblVal);
                }
            }

            jobjResult.add("julianSet", ServerConstants.GSON.toJsonTree(Arrays.asList(julianData)));
            jobjResult.add("data", ServerConstants.GSON.toJsonTree(paramData.values()));

            return ResponseHelper.response(200, "Success - columnData", jobjResult);
        }

        throw new HandledServiceException(411, "명령이 정의되지 않았습니다.");
    }
}
