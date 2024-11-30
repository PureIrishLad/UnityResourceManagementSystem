using System;

namespace UnityResourceManagementSystem
{
    /// <summary>
    /// Attribute to define resource creation details
    /// </summary>
    public class UnityResourceAttribute : Attribute
    {
        private readonly string m_fileName;
        private readonly string m_directory;
        private readonly bool m_isRelativeToResources;

        /// <summary>
        /// The file name of this resource
        /// </summary>
        public string FileName => m_fileName;
        /// <summary>
        /// The directory relative to the resources folder that this resource is located at
        /// </summary>
        public string ResourcesDirectory
        {
            get
            {
                if (m_isRelativeToResources)
                {
                    return m_directory;
                }

                if (m_directory.EndsWith("/Resources") || m_directory.EndsWith("/Resources/"))
                {
                    return string.Empty;
                }

                return m_directory[m_directory.IndexOf("/Resources/")..];
            }
        }
        /// <summary>
        /// The directory relative to the project folder that this resource is located at
        /// </summary>
        public string ProjectDirectory
        {
            get
            {
                if (m_isRelativeToResources)
                {
                    return $"Assets/Resources/{m_directory}";
                }

                return m_directory;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_fileName">The name to give this resource on creation</param>
        /// <param name="_directory">
        /// Where this resource gets generated, the path must either be relative to the resources folder or contain a resources folder.
        ///     <para>
        ///         Valid Example Paths:<br />
        ///         - Characters/Bob<br />
        ///         - Assets/Game/Resources/Items<br />
        ///     </para>
        /// </param>
        /// <exception cref="ResourceManagementException"></exception>
        public UnityResourceAttribute(string _fileName, string _directory = "")
        {
            if (string.IsNullOrEmpty(_fileName)) throw new ResourceManagementException("Resource file name is null or empty");

            if (!IsValidDirectory(_directory)) throw new ResourceManagementException("Resource directory is invalid");

            m_isRelativeToResources = IsRelativeToResources(_directory);

            m_fileName = _fileName;
            m_directory = _directory;
        }

        private static bool IsRelativeToResources(string _directory)
        {
            return !_directory.StartsWith("Assets/");
        }

        private static bool IsValidDirectory(string _directory)
        {
            if (IsRelativeToResources(_directory))
            {
                return true;
            }

            return _directory.Contains("/Resources/") || _directory.EndsWith("/Resources");
        }
    }
}
