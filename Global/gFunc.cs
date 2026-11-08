using Microsoft.Extensions.Configuration;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using Application = System.Windows.Application;
using static BaseUI.Function;
using static BaseUI.gProperty;
using System.Diagnostics;

namespace BaseUI
{
    public class gFunc
    {
        /// <summary>
        /// Application 시작 때 Mutex 이후로 UI의 전역 설정을 위한 함수.
        /// </summary>
        public static void RunAtStartUp()
        {
            /// App.cfg 파일 읽어서 UI 세팅값 추출
            LoadCfg();

            /// log4net Repository 생성
            var logRepos = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());

            /// log4net.config 파일 내부 토큰 명칭과 경로 연결
            log4net.GlobalContext.Properties[nameof(LogDir)] = LogDir;
            log4net.GlobalContext.Properties[nameof(BuildLogDir)] = BuildLogDir;
            log4net.GlobalContext.Properties[nameof(SysLogDir)] = SysLogDir;
            log4net.GlobalContext.Properties[nameof(FatalLogPath)] = FatalLogPath;

            /// log4net.config 파일로 Configure 실행.
            log4net.Config.XmlConfigurator.Configure(logRepos, new FileInfo(Log4NetConfigPath));

            /// 시스템 로거, 치명 로거 둘 다 이 시점에서 초기화. => static이라, 생성하면서 초기화하면, Cfg 읽어오기 전에 초기화하면서 에러 발생.
            SystemLogger = new Logger("SysLogger", "SysLog");
            FatalLogger = new Logger("FatalLogger", "FatalLog");
            
            /// 시스템 로거 시작점.
            SystemLogger.Write($"UI Started");

            if(!DirExists(ProgramDataDir))
            {
                Directory.CreateDirectory(ProgramDataDir);
            }
            if(!DirExists(LogDir))
            {
                Directory.CreateDirectory(LogDir);
            }
            if(!DirExists(BuildLogDir))
            {
                Directory.CreateDirectory(BuildLogDir);
            }
            if(!DirExists(SysLogDir))
            {
                Directory.CreateDirectory(SysLogDir);
            }
            if(!DirExists(AppDataDir))
            {
                Directory.CreateDirectory(AppDataDir);
            }
            //if(!DirExists(JobFilesDir))
            //{
            //    Directory.CreateDirectory(JobFilesDir);
            //}
            if(!FileExits(UserDataPath))
            {
                Debug.WriteLine($"ConfigDir : {ConfigDir}");
                File.Copy(ConfigDir + "\\UserData.json", UserDataPath);
            }
        }

        /// <summary>
        /// App.cfg 파일 읽어서 초기 설정 해주는 함수
        /// </summary>
        public static void LoadCfg()
        {
            /// 어플리케이션 Context 에서 기본경로 추출
            BaseDir = AppContext.BaseDirectory;
            /// ConfigBuilder 사용해서 ini 파일 해석.
            var cfg = new ConfigurationBuilder().SetBasePath(BaseDir).AddIniFile(Path.Combine(BaseDir, "app.cfg"), optional:false, reloadOnChange: true).Build();
            /// cfg 파일 내 토큰 사전 생성
            /// => BaseDir, ResourceDir 등 키워드 값 어떻게 연결할건지 
            var tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["BaseDir"] = BaseDir
            };

            /// [ResourcePaths]
            ResourceDir = ExpandCfgTokens(cfg["ResourcePaths:ResourceDir"], tokens);
            tokens["ResourceDir"] = ResourceDir;

            ConfigDir = ExpandCfgTokens(cfg["ResourcePaths:ConfigDir"], tokens);
            tokens["ConfigDir"] = ConfigDir;

            FontsDir = ExpandCfgTokens(cfg["ResourcePaths:FontsDir"], tokens);
            Log4NetConfigPath = ExpandCfgTokens(cfg["ResourcePaths:Log4netConfigPath"], tokens);

            /// [ProgramDataPaths]
            ProgramDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Lincsolution\\MBJ";
            tokens["ProgramDataDir"] = ProgramDataDir;

            UserDataPath = ExpandCfgTokens(cfg["ProgramDataPaths:UserDataPath"], tokens);
            LogDir = ExpandCfgTokens(cfg["ProgramDataPaths:LogDir"], tokens);
            tokens["LogDir"] = LogDir;

            BuildLogDir = ExpandCfgTokens(cfg["ProgramDataPaths:BuildLogDir"], tokens);
            SysLogDir = ExpandCfgTokens(cfg["ProgramDataPaths:SysLogDir"], tokens);
            FatalLogPath = ExpandCfgTokens(cfg["ProgramDataPaths:FatalLogPath"], tokens);

            /// [AppDataPaths]
            AppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Lincsolution\\MBJ";
            tokens["AppDataDir"] = AppDataDir;

            /// [UIDatas]
            StatusCheckerInterval = cfg.GetSection("UIDatas").GetValue<int>("StatusCheckerInterval", 100);
            BuildLoggerInterval = cfg.GetSection("UIDatas").GetValue<int>("BuildLoggerInterval", 1000);
        }

        /// <summary>
        /// LoadCfg 함수에서 사용하는 Cfg 파일 내 토큰-경로 반환 함수
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="tokens"></param>
        /// <returns></returns>
        private static string ExpandCfgTokens(string raw, IDictionary<string, string> tokens)
        {
            if(string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }
            /// OS 환경변수 확인
            var cfgDatas = Environment.ExpandEnvironmentVariables(raw);

            foreach(var keyValue in tokens)
            {
                cfgDatas = cfgDatas.Replace($"%{keyValue.Key}%", keyValue.Value, StringComparison.OrdinalIgnoreCase);
            }
            /// Uri 데이터인지 확인
            var isUri = Uri.TryCreate(cfgDatas.Trim(), UriKind.Absolute, out var uri);
            /// Uri 데이터이고, 파일이 아니라면
            if(isUri && !uri.IsFile)
            {
                return cfgDatas;
            }

            return Path.GetFullPath(cfgDatas);
        }
    }
}
