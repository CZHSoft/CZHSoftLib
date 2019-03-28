using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace CZHSoft.DllLoader
{
    public class LocalDllLoader
    {
        private AppDomain appDomain;

        private CoreLoader remoteLoader;

        public LocalDllLoader()
        {
            AppDomainSetup setup = new AppDomainSetup();

            setup.ApplicationName = "Test";

            setup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;

            setup.PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "private");

            setup.CachePath = setup.ApplicationBase;

            setup.ShadowCopyFiles = "true";

            setup.ShadowCopyDirectories = setup.ApplicationBase;

            appDomain = AppDomain.CreateDomain("TestDomain", null, setup);

            string name = Assembly.GetExecutingAssembly().GetName().FullName;

            remoteLoader = (CoreLoader)appDomain.CreateInstanceAndUnwrap(

                name,

                typeof(CoreLoader).FullName);

        }

        public void Unload()
        {

            AppDomain.Unload(appDomain);
            appDomain = null;

        }

        #region 操作

        /// <summary>
        /// DLL加载，必须为第一步
        /// </summary>
        /// <param name="dllName"></param>
        /// <param name="className"></param>
        public void LoadAssemblyByLocal(string dllName,string className,string interfaceName)
        {
            remoteLoader.InitAssemblyLocal(dllName, className, interfaceName);
        }

        /// <summary>
        /// 方法执行，必须于DLL加载后
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteAssembly(string methodName, object[] parameters)
        {
            return remoteLoader.MethodInvoke(methodName, parameters);
        }

        /// <summary>
        /// 获取class类型，必须于DLL加载后
        /// </summary>
        /// <returns></returns>
        public Type GetAssemblyClassType()
        {
            return remoteLoader.GetClassType();
        }

        public Type GetAssemblyInterfaceType()
        {
            return remoteLoader.GetInterfaceType();
        }

        #endregion

    } 
}
