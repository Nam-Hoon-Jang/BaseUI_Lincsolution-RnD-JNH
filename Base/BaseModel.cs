using CommunityToolkit.Mvvm.ComponentModel;

namespace BaseUI
{
    public interface IModelWithSharedModel<in TShared>
    {
        void AttachShared(TShared shared);
    }

    public abstract class BaseModel<TShared> : ObservableObject, IDisposable, IModelWithSharedModel<TShared>
    {
        /// <summary>공통 사용 모델 상속용(클래스 내부).</summary>
        private TShared? _shared;

        /// <summary>공통 사용 모델 상속용(클래스 외부).</summary>
        protected TShared Sh => _shared ?? throw new InvalidOperationException("Shared not attached");

        /// <summary>
        /// SharedModel 없어도 되는 Model 들을 위한 기본 생성자.
        /// </summary>
        protected BaseModel(){ }

        /// <summary>
        /// SharedModel 필요한 Model 들을 위한 생성자
        /// </summary>
        /// <param name="shared"></param>
        protected BaseModel(TShared shared)
        {
            AttachShared(shared);
        }

        /// <summary>
        /// BaseModel 생성자에서 SharedModel 사용하는 경우, 생성자에서 AttachShared 메서드로 SharedModel을 전달하여 초기화.
        /// </summary>
        /// <param name="shared">공통 사용 모델</param>
        /// <exception cref="ArgumentNullException">shared가 null 인 경우 예외 처리</exception>
        /// <exception cref="InvalidOperationException">shared와 내부에서 사용하는 _shared 변수간 불일치 시 예외 처리</exception>
        public virtual void AttachShared(TShared shared)
        {
            if(shared is null) throw new ArgumentNullException(nameof(shared));
            if(_shared is null)
            {
                _shared = shared;
                return;
            }
            if(!ReferenceEquals(_shared, shared))
            {
                throw new InvalidOperationException("Differenct Shared instance is being attached");
            }
            
        }

        /// <summary>Dispose 여부 저장하는 내부 변수</summary>
        private bool disposed;

        /// <summary>
        /// BaseModel Dispose 시 호출되는 메서드.
        /// </summary>
        public void Dispose()
        {
            if(disposed)
                return;
            DisposeResources();

            disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// BaseModel Dispose 시 별도로 리소스 정리가 필요한 경우 해당 메소드 오버라이해서 내부에 구현
        /// </summary>
        protected virtual void DisposeResources()
        {
            /// 상속 클래스에서 구현 => 명시적 Dispose 필요한 내용들 Dispose 하는 것들만 넣어두기
        }
        ~BaseModel()
        {
            Dispose();
        }
    }
}
