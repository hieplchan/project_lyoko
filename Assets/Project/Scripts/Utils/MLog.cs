using Unity.VisualScripting;

namespace StartledSeal.Common
{
    public static class MLog
    {
        public static void Debug(string prefix, string message)
        {
            UnityEngine.Debug.Log($"{prefix}: {message}");
        }
        
        public static void Debug(string message)
        {
            UnityEngine.Debug.Log(message);
        }
        
        public static void Error(string prefix, string message)
        {
            UnityEngine.Debug.LogError($"{prefix}: {message}");
        }
        
        public static void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}