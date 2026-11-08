using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseUI
{
    public class BaseEnum
    {
        public enum LanguageType
        {
            English,
            Korean
        }

        public enum UiStatus
        {
            IDLE,
            BUSY,
            READY,
            BUILD,
            PAUSE,
            DONE,
            STOP,
            WARNING,
            ERROR
        }

        public enum ComponentStatus
        {
            Enable,
            Disable,
            Left,
            Right
        }

        public enum BuildStatus
        {
            IDLE,
            READY,
            BUILD,
            PAUSE,
            DONE,
            STOP,
            ERROR
        }

        public enum AlarmType
        {
            Info,
            Warning,
            Error
        }

        public enum PopupType
        {
            Info,
            Warning,
            Error,
            InfoSingle,
            WarningSingle,
            ErrorSingle
        }
    }
}
