using System.Diagnostics;

namespace BaseUI
{
    public class TaskManager : IDisposable
    {
        public Task RunTask { get; private set; }
        public CancellationTokenSource CTS { get; private set; }
        public Action OnCompleted { get; set; } = () => { };
        private bool _isStarted = false;
        private bool _isDisposed = false;
        public TaskManager()
        {
            RunTask = Task.CompletedTask;
            CTS = new CancellationTokenSource();
        }
        /// <summary>
        /// Task 매니저 시작. action에 사용할 코드들 집어넣고, timeout 시간 정해주기. Timeout 지나면 Task 자동 종료
        /// </summary>
        /// <param name="action">Task 에서 실행할 내용</param>
        /// <param name="timeoutMilliSeconds">Millisecond 단위의 Timeout 시간</param>
        public void Start(Action<CancellationToken> action, int timeoutMilliSeconds = 10 * 1000)
        {
            if(_isStarted)
            {
                Debug.WriteLine($"TaskManager Already Started");
                return;
            }
            if(timeoutMilliSeconds > 0)
            {
                CTS.CancelAfter(timeoutMilliSeconds);
            }
            var token = CTS.Token;
            RunTask = Task.Run(() =>
            {
                try
                {
                    action(token);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"TaskManager Run Error : {ex}\n{ex.InnerException}");
                }
            }, token).ContinueWith(t =>
            {
                if(t.IsCompletedSuccessfully && !token.IsCancellationRequested)
                {
                    OnCompleted();
                }
            });
            _isStarted = true;
        }
        public void Reset()
        {
            Dispose();
            _isStarted = false;
            _isDisposed = false;
            CTS = new CancellationTokenSource();
            RunTask = Task.CompletedTask;
        }
        public void Stop()
        {
            if(_isStarted && !CTS.IsCancellationRequested)
            {
                CTS.Cancel();
            }
        }
        public void Dispose()
        {
            if(_isDisposed || !_isStarted) return;

            try
            {
                Stop();
                if(!RunTask.IsCompleted)
                {
                    Task.WaitAny(RunTask, Task.Delay(1000));
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"TaskManager Dispose Error : {ex}");
            }
            CTS.Dispose();
            _isStarted = false;
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
