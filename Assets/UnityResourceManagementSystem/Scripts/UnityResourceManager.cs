using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityResourceManagementSystem
{
    /// <summary>
    /// Manages <see cref="ScriptableObject"/> resources in Unity's Resources folder/s
    /// </summary>
    public static class UnityResourceManager
    {
        /// <summary>
        /// Struct containing data for generating a resource.
        /// </summary>
        private struct ResourceLocationData
        {
            private string m_fileName;
            private string m_resourcesDirectory;
            private string m_projectDirectory;

            public string FileName
            {
                get => m_fileName;
                private set => m_fileName = value;
            }
            public string ResourcesDirectory
            {
                get => m_resourcesDirectory;
                private set => m_resourcesDirectory = value;
            }
            public string ProjectDirectory
            {
                get => m_projectDirectory;
                private set => m_projectDirectory = value;
            }
            
            public string ResourcesPath => $"{ResourcesDirectory}/{FileName}";
            public string ProjectPath => $"{ProjectDirectory}/{FileName}.asset";

            public ResourceLocationData(string _fileName, string _resourcesDirectory, string _projectDirectory)
            {
                m_fileName = _fileName;
                m_resourcesDirectory = _resourcesDirectory;
                m_projectDirectory = _projectDirectory;
            }

            public void FillWithAttributeInfo(UnityResourceAttribute _resourceAttribute)
            {
                if (!string.IsNullOrEmpty(_resourceAttribute.FileName))
                {
                    FileName = _resourceAttribute.FileName;
                }

                ResourcesDirectory = _resourceAttribute.ResourcesDirectory;

                if (!string.IsNullOrEmpty(_resourceAttribute.ProjectDirectory))
                {
                    ProjectDirectory = _resourceAttribute.ProjectDirectory;
                }
            }
        }

        /// <summary>
        /// Dictionary that links a type to an instance, allowing for quick access to previously loaded resources.
        /// </summary>
        private static readonly Dictionary<Type, ScriptableObject> m_instances = new Dictionary<Type, ScriptableObject>();

        /// <summary>
        /// Gets a <see cref="ScriptableObject"/> resource of type <typeparamref name="T"/> from the resources folder. 
        /// If no instance exists, one is created.
        /// </summary>
        public static T GetResource<T>() where T : ScriptableObject
        {
            Type type = typeof(T);
            // Check Dictionary
            if (m_instances.ContainsKey(type) && m_instances[type] != null)
            {
                return m_instances[type] as T ?? throw new ResourceManagementException($"Cached resource type mismatch");
            }

            ResourceLocationData resourceLocationData = GetResourceLocationData(type);

            // Check Resources
            T instance = Resources.Load<T>(resourceLocationData.ResourcesPath);

            // Generate Resource
            if (instance == null)
            {
                instance = CreateResource<T>(resourceLocationData);
            }

            if (m_instances.ContainsKey(type))
            {
                m_instances[type] = instance;
            }
            else
            {
                m_instances.Add(type, instance);
            }

            return instance;
        }

        /// <summary>
        /// Creates a resource of type <typeparamref name="T"/> with the given name in the given directory.
        /// </summary>
        private static T CreateResource<T>(ResourceLocationData _resourceLocationData) where T : ScriptableObject
        {
            T instance = ScriptableObject.CreateInstance<T>();

#if UNITY_EDITOR
            if (!Directory.Exists(_resourceLocationData.ProjectDirectory))
            {
                Directory.CreateDirectory(_resourceLocationData.ProjectDirectory);
            }

            AssetDatabase.CreateAsset(instance, _resourceLocationData.ProjectPath);
#endif

            return instance;
        }

        /// <summary>
        /// Retrieves the file name and path of the resource from its <see cref="Type"/>'s <see cref="UnityResourceAttribute"/>.
        /// If no <see cref="UnityResourceAttribute"/> is declared, it returns a generic name and path.
        /// </summary>
        private static ResourceLocationData GetResourceLocationData(Type _type)
        {
            object[] attributes = _type.GetCustomAttributes(typeof(UnityResourceAttribute), false);
            ResourceLocationData output = new ResourceLocationData(_type.ToString(), string.Empty, "Assets/Resources");

            if (attributes.Length > 0)
            {
                output.FillWithAttributeInfo(attributes[0] as UnityResourceAttribute);
            }

            return output;
        }
    }
}
