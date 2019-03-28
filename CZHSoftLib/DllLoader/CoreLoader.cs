using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CZHSoft.DllLoader
{
    public class CoreLoader : MarshalByRefObject
    {

        private Assembly assembly;
        private Type classType;
        private Type interfaceType;
        private Object instanceObject;

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="dllName">本地的dll名称</param>
        ///// <param name="className">本地的class名</param>
        //public CoreLoader(string dllName, string className)
        //{
        //    assembly = Assembly.Load(dllName);
        //    classType = assembly.GetType(className);
        //    instanceObject = Activator.CreateInstance(classType);
        //}
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dllName">本地的dll名称</param>
        /// <param name="className">本地的class名</param>
        /// <param name="interfaceName">本地的interface名</param>
        public void InitAssemblyLocal(string dllName, string className,string interfaceName)
        {
            assembly = Assembly.Load(dllName);
            classType = assembly.GetType(className);
            //interfaceType = assembly.GetType(interfaceName);
            interfaceType = classType.GetInterface(interfaceName);

            instanceObject = Activator.CreateInstance(classType);

        }

        /// <summary>
        /// class 方法执行
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="parameters">参数</param>
        /// <returns>视返回类型</returns>
        public object MethodInvoke(string methodName, object[] parameters)
        {
            if (classType != null)
            {
                MethodInfo method = classType.GetMethod(methodName);
                return method.Invoke(instanceObject, parameters);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 返回class类型
        /// </summary>
        /// <returns></returns>
        public Type GetClassType()
        {
            return classType;
        }

        public Type GetInterfaceType()
        {
            return interfaceType;
        }

    }
}
