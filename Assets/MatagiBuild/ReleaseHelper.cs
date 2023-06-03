using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace MatagiBuild
{
    public static class ReleaseHelper
    {
        private const string Root = "Assets/Matagi";
        private const string License = "LICENSE";
        private const string Readme = "README.md";

        [MenuItem("Matagi/Release")]
        public static async void Release()
        {
            CopyLicenseAndReadMe();
            var request = Client.Pack(Root, "../output");
            while (!request.IsCompleted)
            {
                await Task.Delay(1000);
            }

            if (request.Status == StatusCode.Success)
            {
                Debug.Log(request.Result);
            }
            else
            {
                Debug.LogError($"message:{request.Error.message}, errorCode:{request.Error.errorCode}");
            }
        }

        private static void CopyLicenseAndReadMe()
        {
            FileUtil.DeleteFileOrDirectory($"{Root}/{License}.md");
            FileUtil.DeleteFileOrDirectory($"{Root}/{Readme}");
            FileUtil.CopyFileOrDirectory(License, $"{Root}/{License}.md");
            FileUtil.CopyFileOrDirectory(Readme, $"{Root}/{Readme}");
        }
    }
}