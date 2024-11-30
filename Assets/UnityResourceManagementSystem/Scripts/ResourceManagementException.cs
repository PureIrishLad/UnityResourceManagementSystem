using System;

namespace UnityResourceManagementSystem
{
    /// <summary>
    /// Exception used by the <see cref="UnityResourceManagementSystem"/> package
    /// </summary>
    public class ResourceManagementException : Exception
    {
        public ResourceManagementException() : base() { }
        public ResourceManagementException(string message) : base(message) { }
        public ResourceManagementException(string message, Exception innerException) : base(message, innerException) { }
    }
}
