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
    }
}