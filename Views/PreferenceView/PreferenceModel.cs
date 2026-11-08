using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using Application = System.Windows.Application;

namespace BaseUI
{
    public class PreferenceModel :BaseModel<ISharedModel>
    {
        public PreferenceModel() { }

        /// <summary>
        /// UI View 전체 언어 설정 변경 함수
        /// </summary>
        /// <param name="type">설정할 언어</param>
        public void SetLanguage(string type)
        {
            try
            {
                string cType = string.Empty;
                if(type == gProperty.TEXT.LANG.English)
                {
                    cType = gProperty.TEXT.LANG_CODE.English;
                }
                if(type == gProperty.TEXT.LANG.Korean)
                {
                    cType = gProperty.TEXT.LANG_CODE.Korean;
                }
                CultureInfo cInfo = new CultureInfo(cType);
                Thread.CurrentThread.CurrentCulture = cInfo;
                Thread.CurrentThread.CurrentUICulture = cInfo;

                var sourceUri = new Uri($"pack://application:,,,/Resource/Dictionary/String.{cType}.xaml", UriKind.RelativeOrAbsolute);
                var rd = new ResourceDictionary()
                {
                    Source = sourceUri
                };
                /// App.xaml 내부 MergedDictionaries에 1번 줄에 언어 Dictionary가 위치하므로, [0]를 교체.
                Application.Current.Resources.MergedDictionaries[0] = rd;
                /// 유저 설정 언어값 UserData에 할당
                Sh.UserData.UserLanguage = cType;
                /// 변경값으로 UserData 교체
                var clone = Sh.UserData.Clone();
                Sh.UserData = clone;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
        }

        /// <summary>
        /// Build File 경로 변경 함수
        /// </summary>
        public void SetBuildFileDirectory()
        {
            try
            {
                /// 폴더 선택 가능한 OpenFileDialog 선언
                var cofd = new CommonOpenFileDialog()
                {
                    IsFolderPicker = true
                };
                /// 선택 결과 정상일 때
                if(cofd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    /// 선택한 경로로 UserData 업데이트
                    Sh.UserData.BuildFileDir = cofd.FileName;
                    var clone = Sh.UserData.Clone();
                    Sh.UserData = clone;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        public void SetBuildLogStoragePeriod(string period)
        {
            try
            {
                var date = Convert.ToInt32(period);
                if(date > 0)
                {
                    Sh.UserData.BuildLogStoragePeriod = date;
                }
                else
                {
                    /// TODO : date 값 0이하인 경우 에러 띄울것. -> 로그 보관 기간 0일 이하면 안 됌
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        public void SetSystemLogStoragePeriod(string period)
        {
            try
            {
                var date = Convert.ToInt32(period);
                if(date > 0)
                {
                    Sh.UserData.SystemLogStoragePeriod = date;
                }
                else
                {
                    /// TODO : date 값 0이하인 경우 에러 띄울것. -> 로그 보관 기간 0일 이하면 안 됌
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
