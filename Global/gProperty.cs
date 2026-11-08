

using System.Data;

namespace BaseUI
{
    public class gProperty
    {
        /// <summary>
        /// Struct for UI Button Tags
        /// UI에 사용하는 버튼 태그 키워드 구조체
        /// </summary>
        public struct TEXT
        {
            public struct LANG
            {
                public const string English = "English";
                public const string Korean = "한국어";
            }
            public struct LANG_CODE
            {
                public const string English = "en-US";
                public const string Korean = "ko-KR";
            }
            public struct THEMA
            {
                public const string Light = "Light";
                public const string Dark = "Dark";
            }
        }

        /// <summary>프로그램 설치 경로 기반 BasePath</summary>
        public static string BaseDir = string.Empty;

        /// <summary>프로그램 Resource 경로</summary>
        public static string ResourceDir = string.Empty;

        /// <summary>프로그램 구동용 Data 경로</summary>
        public static string ConfigDir = string.Empty;

        /// <summary>UI 에서 사용하는 텍스트 폰트들 경로</summary>
        public static string FontsDir = string.Empty;

        /// <summary>Log4net.config 파일 경로</summary>
        public static string Log4NetConfigPath = string.Empty;

        /// <summary>프로그램에서 편집하는 Data 경로</summary>
        public static string ProgramDataDir = string.Empty;

        /// <summary>사용자 설정 파일 경로</summary>
        public static string UserDataPath = string.Empty;

        /// <summary>UI 로그 폴더 경로</summary>
        public static string LogDir = string.Empty;

        /// <summary>Job 파일 출력 로그 폴더 경로</summary>
        public static string BuildLogDir = string.Empty;

        /// <summary>UI 시스템 로그 폴더 경로</summary>
        public static string SysLogDir = string.Empty;

        /// <summary>UI 치명 로그 파일 경로</summary>
        public static string FatalLogPath = string.Empty;

        /// <summary>AppData 경로</summary>
        public static string AppDataDir = string.Empty;

        /// <summary>Build 파일 업로드/보관 경로</summary>
        public static string BuildFilesDir = string.Empty;

        /// <summary>장비 상태 체크하는 SharedModel 내의 스레드 반복 주기</summary>
        public static int StatusCheckerInterval = 100;

        /// <summary>장비 빌드 시 로거 반복 주기</summary>
        public static int BuildLoggerInterval = 1000;

        /// <summary>Logger 클래스에 적용할 필터용 Attribute 이름 중 PrintLogger용</summary>
        public const string BuildLoggerAttributeName = "LoggingValue";

        /// <summary>UI 시스템 전체에서 사용하는 로거. 시스템 백그라운드 동작 관련 로그</summary>
        public static Logger SystemLogger;

        /// <summary>UI 시스템 비정상 종료 및 치명적인 에러 발생 기록</summary>
        public static Logger FatalLogger;

        /// <summary>날짜 변환 포맷 하나로 고정.</summary>
        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        /// <summary>파일에 사용하는 날짜 변환 포맷 하나로 고정.</summary>
        public static string DateTimeFormatForFileName = "yyyy-MM-dd HH-mm-ss";

        /// <summary>Job Log 생성할 때, 사용하는 로그 파일 이름 고정</summary>
        public static string BuildLogName = "build.log";

        /// <summary>Job Log List 에서 조회할 때 사용하는 파일 이름 고정</summary>
        public static string BuildDataName = "BuildData.json";

        /// <summary>System Log 생성할 때, 기본 생성되는 로거 이름</summary>
        public static string SysLogName = "SysLog.log";

        /// <summary>Job 로그 보관 기간</summary>
        public static int BuildLogRetentionPeriod;

        /// <summary>System 로그 보관 기간</summary>
        public static int SystemLogRetentionPeriod;
    }
}
