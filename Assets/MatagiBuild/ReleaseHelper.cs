using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace MatagiBuild
{
    public static class ReleaseHelper
    {
        [MenuItem("Matagi/Release")]
        public static async void Release()
        {
            var request = Client.Pack("Assets/Matagi", "../output");
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
    }
}