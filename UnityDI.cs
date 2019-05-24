using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SharpUtils
{
    /// <summary>
    /// Simple manual injection container for Unity
    /// </summary>
    public static class UnityDI
    {
        private enum Mode
        {
            /// <summary>
            /// Singleton instance
            /// </summary>
            Singleton,
            /// <summary>
            /// New instance every scene
            /// </summary>
            SceneScoped,
            /// <summary>
            /// New instance every frame
            /// </summary>
            FrameScoped,
            /// <summary>
            /// New instance every call
            /// </summary>
            Transient
        }

        private class TypeConfig
        {
            public Mode Mode;
            public object Instance;
            public int ReferenceId;
        }

        private static readonly Dictionary<Type, TypeConfig> classMap = new Dictionary<Type, TypeConfig>();

        /// <summary>
        /// Returns instance of selected class
        /// </summary>
        public static T Get<T>() where T : class
        {
            return Get(typeof(T)) as T;
        }

        /// <summary>
        /// Returns instance of selected class
        /// </summary>
        public static object Get(Type pType)
        {
            if (classMap.TryGetValue(pType, out TypeConfig config))
            {
                switch (config.Mode)
                {
                    case Mode.Singleton:
                        return config.Instance ?? (config.Instance = CreateInstance(pType));

                    case Mode.FrameScoped:
                        if (config.ReferenceId != Time.frameCount)
                        {
                            config.Instance = CreateInstance(pType);
                            config.ReferenceId = Time.frameCount;
                        }
                        return config.Instance;

                    case Mode.SceneScoped:
                        int currentSceneId = SceneManager.GetActiveScene().GetHashCode();
                        if (config.ReferenceId != currentSceneId)
                        {
                            config.Instance = CreateInstance(pType);
                            config.ReferenceId = currentSceneId;
                        }
                        return config.Instance;

                    case Mode.Transient:
                        return CreateInstance(pType);

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                throw new Exception($"Object of type {pType} not registered");
            }
        }

        /// <summary>
        ///   <para>Registers singleton class</para>
        ///   <para>Caller get the same instance every call</para>
        /// </summary>
        public static void AddSingleton<T>(bool pLazy = true) where T : class
        {
            Type type = typeof(T);
            classMap.Add(type, new TypeConfig
            {
                Instance = pLazy ? null : CreateInstance(type),
                Mode = Mode.Singleton
            });
        }

        /// <summary>
        ///   <para>Registers scene scoped class</para>
        ///   <para>Caller get new instance every scene</para>
        /// </summary>
        public static void AddSceneScoped<T>() where T : class
        {
            classMap.Add(typeof(T), new TypeConfig
            {
                Mode = Mode.SceneScoped,
                ReferenceId = -1
            });
        }

        /// <summary>
        ///   <para>Registers frame scoped class</para>
        ///   <para>Caller get new instance every frame</para>
        /// </summary>
        public static void AddFrameScoped<T>() where T : class
        {
            classMap.Add(typeof(T), new TypeConfig
            {
                Mode = Mode.FrameScoped,
                ReferenceId = -1
            });
        }

        /// <summary>
        ///   <para>Registers transient class.</para>
        ///   <para>Caller get new instance every call</para>
        /// </summary>
        public static void AddTransient<T>() where T : class
        {
            classMap.Add(typeof(T), new TypeConfig
            {
                Mode = Mode.Transient
            });
        }

        /// <summary>
        /// Removes class from container
        /// </summary>
        public static void Remove<T>() where T : class
        {
            classMap.Remove(typeof(T));
        }

        /// <summary>Creates an instance of the specified type using that type's default constructor.</summary>
        /// <param name="pType">The type of object to create. </param>
        /// <returns>A reference to the newly created object.</returns>
        private static object CreateInstance(Type pType)
        {
            var parameters = pType.GetConstructors()[0].GetParameters();
            int paramCount = parameters.Length;

            if (paramCount == 0)
            {
                return Activator.CreateInstance(pType);
            }

            var args = new object[paramCount];
            for (int i = 0; i < paramCount; i++)
            {
                args[i] = Get(parameters[i].ParameterType);
            }
            return Activator.CreateInstance(pType, args);
        }
    }
}
