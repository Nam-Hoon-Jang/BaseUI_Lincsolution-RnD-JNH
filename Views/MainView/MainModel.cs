using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Application = System.Windows.Application;

namespace BaseUI
{
    public class MainModel : BaseModel<ISharedModel>
    {
        public MainModel()
        {
            Initialise();
        }

        private void Initialise()
        {
            
        }

        /// <summary>
        /// UserData.json 파일 읽어오는 함수.
        /// </summary>
        /// <returns>성공 시 True 반환.</returns>
        public bool GetUserData()
        {
            try
            {
                Sh.UserData = JsonSerializer.Deserialize<UserData>(File.ReadAllText(gProperty.UserDataPath));
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// UI 색상 테마 Light/Dark 모드 전환 함수
        /// </summary>
        /// <param name="isDark">True면 Dark모드로 전환</param>
        public void SetViewThema(bool isDark)
        {
            try
            {
                string cType = string.Empty;
                if(isDark)
                {
                    cType = gProperty.TEXT.THEMA.Dark;
                }
                else
                {
                    cType = gProperty.TEXT.THEMA.Light;
                }
                var sourceUri = new Uri($"pack://application:,,,/Resource/Dictionary/ColorDic.{cType}.xaml", UriKind.RelativeOrAbsolute);
                var rd = new ResourceDictionary()
                {
                    Source = sourceUri
                };
                /// App.xaml 내부 MergedDictionaries에 3번 줄에 색상 Dictionary가 위치하므로, [2]를 교체.
                Application.Current.Resources.MergedDictionaries[2] = rd;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
        }
    }
}
