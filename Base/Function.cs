using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;

namespace BaseUI
{
    public class Function
    {

        /// <summary>
        /// 폴더 경로 읽어서 존재하는지 확인하는 함수
        /// </summary>
        /// <param name="path">경로값</param>
        /// <returns>존재여부</returns>
        public static bool DirExists(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            return di.Exists;
        }

        /// <summary>
        /// 파일 경로 읽어서 존재하는지 확인하는 함수
        /// </summary>
        /// <param name="path">경로값</param>
        /// <returns>존재여부</returns>
        public static bool FileExits(string path)
        {
            FileInfo fi = new FileInfo(path);
            return fi.Exists;
        }

        /// <summary>
        /// 특정 경로에서, 특정 확장명식의 파일들을 가져오는 함수
        /// </summary>
        /// <param name="dir">탐색 경로</param>
        /// <param name="formatName">확장명</param>
        /// <param name="files">찾은 파일들 전체 경로 배열</param>
        /// <returns></returns>
        public static bool GetFormattedFiles(string dir, string formatName, out string[] files)
        {
            files = null;

            /// 입력된 경로 및 확장명이 잘못되었을 때
            if(string.IsNullOrWhiteSpace(dir) || string.IsNullOrWhiteSpace(formatName))
                return false;

            /// 경로가 존재하지 않을 때
            if(!Directory.Exists(dir))
                return false;

            /// 입력한 포맷이 .jpg 또는 jpg 등 구분하기 위해.
            string format = formatName.Trim();
            if(format.StartsWith("."))
            {
                format = "*" + format;
            }
            else
            {
                format = "*." + format;
            }

            var options = new EnumerationOptions
            {
                MatchCasing = MatchCasing.CaseInsensitive,
                IgnoreInaccessible = true
            };

            files = Directory.EnumerateFiles(dir, format, options).ToArray<string>();
            return true;
        }

        /// <summary>
        /// 폴더를 읽고, 내부 파일 Dictionary로 저장.
        /// GetPath로 읽으려는 경로가 존재하는지부터 체크. 없으면 return false
        /// </summary>
        /// <param name="path">읽으려는 경로</param>
        /// <param name="dic">key=파일명, value=파일 전체 경로</param>
        /// <returns>Dictionary 저장까지 성공하면 true, 실패하면 false</returns>
        public static bool ReadFolderAndGetFiles(string path, ref Dictionary<string, string>? dic)
        {
            if(DirExists(path))
            {
                if(dic == null)
                {
                    dic = new Dictionary<string, string>();
                    string[] files = Directory.GetFiles(path);
                    foreach(var fullPath in files)
                    {
                        dic[Path.GetFileName(fullPath)] = fullPath;
                    }
                    return true;
                }
                else
                {
                    dic.Clear();
                    string[] files = Directory.GetFiles(path);
                    foreach(var fullPath in files)
                    {
                        dic[Path.GetFileName(fullPath)] = fullPath;
                    }
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// 매개변수로 입력된 텍스트 배열 확인해서 null 또는 empty 있으면 false 반환
        /// </summary>
        /// <param name="texts"></param>
        /// <returns></returns>
        public static bool IsTextNullOrEmpty(string[] texts)
        {
            return texts.All(s => !string.IsNullOrEmpty(s));
        }

        /// <summary>
        /// 리소스에 저장된 SolidColorBrush 가져오는 함수
        /// </summary>
        /// <param name="resourceKey">리소스 키 명칭</param>
        /// <returns>key와 일치하는 SolidColorBrush</returns>
        /// <exception cref="ResourceReferenceKeyNotFoundException"></exception>
        public static SolidColorBrush GetResourceBrush(string resourceKey)
        {
            return Application.Current.Resources[resourceKey] as SolidColorBrush
                   ?? throw new ResourceReferenceKeyNotFoundException($"Resource '{resourceKey}' not found.", resourceKey);
        }

        /// <summary>
        /// 리소스에 저장된 String 가져오는 함수
        /// </summary>
        /// <param name="resourceKey">리소스 키 명칭</param>
        /// <returns>key와 일치하는 String</returns>
        /// <exception cref="ResourceReferenceKeyNotFoundException"></exception>
        public static string GetResourceString(string resourceKey)
        {
            return Application.Current.Resources[resourceKey] as string
                   ?? throw new ResourceReferenceKeyNotFoundException($"Resource '{resourceKey}' not found.", resourceKey);
        }

        /// <summary>
        /// 리소스에 저장된 Object 가져오는 함수
        /// </summary>
        /// <param name="resourceKey">리소스 키 명칭</param>
        /// <returns>key와 일치하는 Object</returns>
        /// <exception cref="ResourceReferenceKeyNotFoundException"></exception>
        public static object GetResourceObject(string resourceKey)
        {
            return Application.Current.Resources[resourceKey] as object
                   ?? throw new ResourceReferenceKeyNotFoundException($"Resource '{resourceKey}' not found.", resourceKey);
        }

        /// <summary>
        /// 리소스에 저장된 xaml ImageSource 가져오는 함수
        /// </summary>
        /// <param name="resourceKey">리소스 키 명칭</param>
        /// <returns>key와 일치하는 ImageSource</returns>
        /// <exception cref="ResourceReferenceKeyNotFoundException"></exception>
        public static ImageSource GetResourceImage(string resourceKey)
        {
            return Application.Current.Resources[resourceKey] as ImageSource
                ?? throw new ResourceReferenceKeyNotFoundException($"Resource '{resourceKey}' not found.", resourceKey);
        }

        public static Geometry GetResourceGeometry(string resourceKey)
        {
            return Application.Current.Resources[resourceKey] as Geometry 
                ?? throw new ResourceReferenceKeyNotFoundException($"Resource '{resourceKey}' not found.", resourceKey);
        }

        /// <summary>
        /// 리소스에 저장된 xaml Style 가져오는 함수
        /// </summary>
        /// <param name="resourceKey">리소스 키 명칭</param>
        /// <returns>key와 일치하는 Style</returns>
        /// <exception cref="ResourceReferenceKeyNotFoundException"></exception>
        public static Style GetResourceStyle(string resourceKey)
        {
            return Application.Current.Resources[resourceKey] as Style
                ?? throw new ResourceReferenceKeyNotFoundException($"Resource '{resourceKey}' not found.", resourceKey);
        }

        /// <summary>
        /// 파일 크기 가져오는 함수
        /// </summary>
        /// <param name="size">파일 크기값 입력</param>
        /// <returns>B~TB 까지 텍스트로 출력</returns>
        public static string GetFileSize(long size)
        {
            string[] units = {"B", "KB", "MB", "GB", "TB"};
            int index = 0;
            /// size 값이 1024 이상(즉, 아직 올라갈 단위가 있다면) + 인덱스 마지막 번호가 되기 전까지.
            while(size >= 1024 && index < units.Length - 1)
            {
                size = size / 1024;
                index++;
            }
            return $"{size:0.##}{units[index]}";
        }

        /// <summary>
        /// Bool 값에 따라 Visibility로 변환해주는 함수
        /// </summary>
        /// <param name="value">true/false</param>
        /// <param name="invert">invert 할지 여부</param>
        /// <returns>Visibility.Visible/Collapsed</returns>
        public static Visibility BoolToVisibility(bool value, bool invert = false)
        {
            if(invert)
                return value ? Visibility.Collapsed : Visibility.Visible;

            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// UI의 Enabled 상태에 따른 이미지 전환용 함수
        /// </summary>
        /// <param name="values"></param>
        /// <returns>상태에 맞는 ImageSource 반환</returns>
        public static ImageSource GetEnabledImageSource(object[] values)
        {
            if(values.Length < 3)
                return null;

            if(values[0] is bool isEnabled &&
                values[1] is string enabledImage &&
                values[2] is string disabledImage)
            {
                var uri = isEnabled ? enabledImage : disabledImage;
                if(!string.IsNullOrWhiteSpace(uri))
                {
                    return new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
                }
            }

            return null;
        }
    }
}
