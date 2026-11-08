

namespace BaseUI
{
    public class gEnum
    {
        public enum MainViewCommand
        {
            ChangeThema,
            OpenAlarmList,
            AllAlarmClear,
            EmergencyStop,
            OpenDashboard,
            OpenBuild,
            OpenMonitoring,
            OpenControl,
            OpenLogList,
            OpenBuildParameter,
            OpenPreference,
        }

        public enum DashboardViewCommand
        {
            TestAnimation,
            OpenPreview,
            OpenBuildStart,
        }

        public enum MonitoringViewCommand
        {
            
        }

        public enum PreferenceViewCommand
        {
            LanguageSet,
            BuildFileDirectorySet,
            BuildLogStoragePeriodSet,
            SystemLogStoragePeriodSet,
        }

        public enum PopupCommand
        {
            Ok,
            Yes,
            No,
            ClearAllAlarm,
        }
    }
}
