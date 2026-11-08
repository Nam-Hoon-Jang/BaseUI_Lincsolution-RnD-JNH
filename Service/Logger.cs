using log4net;
using System.Linq.Expressions;
using System.Reflection;

namespace BaseUI
{
    public class Logger
    {
        #region Variables
        /// <summary>log4net 로그 작성용 인터페이스</summary>
        private readonly ILog Log;

        /// <summary>log4net.config 설정 내 logger 이름</summary>
        private string loggerName = string.Empty;

        /// <summary>log4net.config 설정 내 appender 이름</summary>
        private string appenderName = string.Empty;

        /// <summary>로그 시작 플래그</summary>
        private volatile bool logging = false;

        /// <summary>로그 정지 플래그</summary>
        private volatile bool stopLogging = false;

        /// <summary>로그 일시정지 플래그</summary>
        private volatile bool pauseLogging = false;

        /// <summary>지속 로거 스레드 중복 접근 방지용 변수</summary>
        private readonly object Lock = new object();

        /// <summary>지속 로거 스레드 반복 간격</summary>
        private int loggerInterval = 0;

        /// <summary>로그 일시정지 핸들러</summary>
        private static ManualResetEventSlim pauseHandler = new ManualResetEventSlim(true);

        /// <summary>지속 로거 스레드</summary>
        Thread loggerThread;

        /// <summary>로깅에 사용할 참조 인스턴스 목록</summary>
        private object[] _sources;

        /// <summary>인스턴스 목록에서 Property들 목록.</summary>
        private (Func<object, object> getter, int sourceIndex, string header)[] _columns;
        #endregion

        public Logger(string loggerName, string appenderName)
        {
            this.appenderName = appenderName;
            this.loggerName = loggerName;
            Log = LogManager.GetLogger(loggerName);
        }

        /// <summary>
        /// 로그 파일 이름 변경 용도
        /// </summary>
        /// <param name="filePath">생성하려는 로그파일 명칭</param>
        public void ChangeLogFileName(string filePath)
        {
            var rootRepository = LogManager.GetRepository();
            foreach(var appender in rootRepository.GetAppenders())
            {
                if(appender.Name.Equals(appenderName) && appender is log4net.Appender.FileAppender)
                {
                    var fileAppender = appender as log4net.Appender.FileAppender;
                    fileAppender.File = filePath;
                    fileAppender.ActivateOptions();
                }
            }
        }

        /// <summary>
        /// 로거로 단일 정보 로그로 남길 때, 사용
        /// </summary>
        /// <param name="text">로그로 남길 내용</param>
        /// <param name="writeCase">입력 구분</param>
        public void Write(string text, Case writeCase = Case.Info)
        {
            switch(writeCase)
            {
                case Case.Info:
                    Log.Info("[Information] " + text);
                    break;

                case Case.Warn:
                    Log.Warn("[Warning] " + text);
                    break;

                case Case.Error:
                    Log.Error("[Error] " + text);
                    break;

                case Case.Fatal:
                    Log.Fatal("[Fatal Error] " + text);
                    break;

                case Case.Popup:
                    Log.Info("[Popup Open] " + text);
                    break;

                case Case.Button:
                    Log.Info("[Button Command] " + text);
                    break;

                case Case.Automation:
                    Log.Info("[Automation Command] " + text);
                    break;

                case Case.Exception:
                    Log.Error("[Exception] " + text);
                    break;
            }
        }

        /// <summary>
        /// 로깅에 사용할 인스턴스 목록, 로깅 주기, 
        /// (필요 시)인스턴스 내에서 적용할 필터, (필요 시)로깅 데이터의 데이터 헤더들 추출할 필터 세팅
        /// </summary>
        /// <param name="sources">로깅에 사용할 인스턴스들 목록</param>
        /// <param name="interval">로깅 주기</param>
        /// <param name="filter">로깅에 참조하는 인스턴스들 중, 실제 로깅에 사용할 인스턴스 구분용 필터</param>
        /// <param name="headerSelector">로깅에 참조하는 인스턴스들 중, 실제 로깅에 사용할 인스턴스 이름 추출용 필터</param>
        /// <exception cref="LoggerException"></exception>
        public void SetContinuousLog(object[] sources, int interval = 1000, Func<PropertyInfo, bool> filter = null, Func<PropertyInfo, string> headerSelector = null)
        {
            if(sources == null || sources.Length == 0)
            {
                throw new LoggerException(LoggerExceptionType.InputSourcesEmpty);
            }
            /// 로그 기록 간격 할당
            loggerInterval = interval;
            
            /// 로그 시작 플래그 활성
            logging = true;
            stopLogging = false;
            pauseLogging = false;

            /// 참조 인스턴스 목록 할당
            _sources = sources;

            /// _columns 에 전달할 임시 리스트 생성.
            var cols = new List<(Func<object, object>, int, string)>();

            for(int sourceIndex = 0; sourceIndex < sources.Length; sourceIndex++)
            {
                /// 참조 인스턴스 하나 pick
                var src = sources[sourceIndex];
                /// 인스턴스 내 Property 들 중, instance 또는 public 들만 추출.
                var properties = src.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                /// 읽기 가능하고 IndexParameters 가 있는 property만 pick
                IEnumerable<PropertyInfo> pickedProperties = properties.Where(p => (p.CanRead && p.GetIndexParameters().Length == 0));

                /// 매개변수로 필터 받았을 경우, 해당 필터 기준으로 한 번 더 걸러줌
                if(filter != null)
                {
                    pickedProperties = pickedProperties.Where(filter);
                }

                foreach(var pickedProperty in pickedProperties)
                {
                    /// header: 기본값은 TypeName.Property
                    var header = headerSelector != null ? headerSelector(pickedProperty) : (pickedProperty.DeclaringType.Name + "." + pickedProperty.Name);

                    /// GetValue 쓰면 캐시 메모리 과도하게 사용하므로, getter 사용.
                    var getMethod = pickedProperty.GetGetMethod();
                    /// 일반적인 Delegate로는 Func<object, object> 못만듦. BuldSafeGetter 함수로 처리.
                    var getter = BuildSafeGetter(pickedProperty);
                    /// 위에서 얻은 getter 대리자, 인덱스, 프로퍼티 명칭들 로깅 데이터에 추가.
                    cols.Add((getter, sourceIndex, header));
                }
            }

            _columns = cols.ToArray();
        }

        /// <summary>
        /// 지속 로거 시작
        /// </summary>
        /// <exception cref="LoggerException"></exception>
        public void StartLog()
        {
            if(logging)
            {
                if((_columns.Length <= 0))
                {
                    throw new LoggerException(LoggerExceptionType.FailedToCreateLoggingDataLists);
                }
                loggerThread = new Thread(() => ThreadStartLogging()) { IsBackground = true };
                loggerThread.Start();
            }
        }

        /// <summary>
        /// 지속 로거 정지
        /// </summary>
        public void StopLog()
        {
            if(logging)
            {
                if(!stopLogging)
                {
                    stopLogging = true;
                    logging = false;
                    pauseLogging = false;
                    pauseHandler.Set();
                }
            }
        }

        /// <summary>
        /// 지속 로거 일시정지
        /// </summary>
        public void PauseLog()
        {
            if(logging)
            {
                if(!pauseLogging)
                {
                    pauseLogging = true;
                    pauseHandler.Reset();
                }
            }
        }

        /// <summary>
        /// 지속 로거 재시작
        /// </summary>
        public void ResumeLog()
        {
            if(logging)
            {
                if(pauseLogging)
                {
                    pauseLogging = false;
                    pauseHandler.Set();
                }
            }
        }

        /// <summary>
        /// 자동 지속 로그에서, 지속적으로 데이터 관찰하려는 Property 클래스의 파일 처리하기 위함
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private Func<object, object> BuildSafeGetter(PropertyInfo p)
        {
            /// Expression Parameter 를 object 형태로 강제 고정할 수 있도록 Parameter 생성
            var obj = Expression.Parameter(typeof(object), "o");

            /// pickedProperty 가 속해있는 실제 인스턴스의 타입( ex => PLCProperty 등) 으로 캐스팅.
            var cast = Expression.Convert(obj, p.DeclaringType);

            /// 캐스팅된 값에서 실제 프로퍼티(pickedProperty) 읽기.
            var property = Expression.Property(cast, p);

            /// 반환 타입 object 로 통일해서 변환
            var box = Expression.Convert(property, typeof(object));

            /// Expression.Lambda 사용해서, 람다식으로 Func<object, object>(boxing, obj) 함수를 즉시 컴파일하고 delegate 형태로 얻음.
            return Expression.Lambda<Func<object, object>>(box, obj).Compile();
        }

        /// <summary>
        /// 지속 로거 스레드에서 실행하는 함수
        /// </summary>
        /// <exception cref="LoggerException"></exception>
        private void ThreadStartLogging()
        {
            /// 로그 데이터 배열
            var dataBuffer = new string[_columns.Length];
            /// Property 들 이름 먼저 로그 파일에 Title 처럼 입력
            Log.Info(string.Join(",", _columns.Select(c => c.header)));

            /// logging = true 인 동안 무한루프
            while(logging)
            {
                try
                {
                    /// 스레드 중복접근 방지
                    lock(Lock)
                    {
                        /// StopLog 함수 실행하면, stopLogging = true 처리되면서 루프 종료
                        if(stopLogging)
                        {
                            /// 로깅 종료 입력
                            Log.Info("Logging Stop");
                            break;
                        }
                        /// PauseLog 함수 실행하면, pauseLogging = true 처리되면서 일시정지
                        if(pauseLogging)
                        {
                            /// 로깅 일시 정지 입력
                            Log.Info("Logging Paused");
                            /// 여기서 일시정지. ResumeLog 실행하면 해제.
                            pauseHandler.Wait();
                        }

                        for(int i = 0; i < _columns.Length; i++)
                        {
                            /// getter 로 받은 인스턴스 property, 인덱스, header(사용 안함이라 _ 처리)
                            var (getter, sourceIndex, _) = _columns[i];
                            /// getter 로 참조 중인 인스턴스들 중 sourceIndex 에 해당하는 property 값 추출.
                            var propertyValue = getter(_sources[sourceIndex]);
                            /// 데이터 버퍼에 propertyValue 값 string 변환해서 저장.
                            dataBuffer[i] = propertyValue.ToString() ?? "null";
                        }
                        /// 로그에 값 작성.
                        Log.Info(string.Join(",", dataBuffer));
                    }
                }
                catch(LoggerException ex)
                {
                    throw;
                }
                catch(Exception ex) when (ex is not LoggerException)
                {
                    throw new LoggerException(LoggerExceptionType.UnknownException, null, ex);
                }

                /// 반복 주기만큼 스레드 Sleep
                Thread.Sleep(loggerInterval);
            }
        }
        public enum Case
        {
            Info,
            Warn,
            Error,
            Fatal,
            Popup,
            Button,
            Automation, /// REVERT : 지울것.
            Exception
        }
    }

    public enum LoggerExceptionType
    {
        UnknownException,
        InputSourcesEmpty,
        FailedToCreateLoggingDataLists,
    }
    public class LoggerException : Exception
    {
        /// <summary></summary>
        public LoggerExceptionType Type { get; }

        /// <summary>
        /// 예외 처리 시, 별도 메세지 없이 타입만 전달하는 생성자
        /// </summary>
        /// <param name="type">예외 타입</param>
        public LoggerException(LoggerExceptionType type) : base(type.ToString())
        {
            /// 예외 타입 할당
            Type = type;
        }

        /// <summary>
        /// 예외 처리 시, 별도 메세지가 있는 경우 생성자
        /// </summary>
        /// <param name="type">예외 타입</param>
        /// <param name="message">예외 관련 별도 메세지</param>
        public LoggerException(LoggerExceptionType type, string message) : base(message)
        {
            /// 예외 타입 할당
            Type = type;
        }

        /// <summary>
        /// 예외 처리 시, JPTLaserExceptionType 으로 구분할 수 없는 예외 생성자.
        /// </summary>
        /// <param name="type">예외 타입</param>
        /// <param name="message">예외 관련 별도 메세지</param>
        /// <param name="inner">예외</param>
        public LoggerException(LoggerExceptionType type, string message, Exception inner) : base(message, inner)
        {
            /// 예외 타입 할당
            Type = type;
        }
    }
}
