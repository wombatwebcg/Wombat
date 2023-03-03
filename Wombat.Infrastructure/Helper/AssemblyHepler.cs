using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wombat.Infrastructure
{
    public static class AssemblyHepler
    {
        /// <summary>
        /// 解决方案所有程序集
        /// </summary>
        //public static readonly Assembly[] AllAssemblies = new Assembly[]
        //{
        //    //Assembly.Load("KYDevicesWeb.Infrastructure"),
        //    //Assembly.Load("Wombat.Infrastructure"),
        //    //Assembly.Load("KYDevicesWebManager.Entity"),
        //    //Assembly.Load("KYDevicesWebManager.IBusiness"),
        //    //Assembly.Load("KYDevicesWebManager.Business"),
        //    //Assembly.Load("KYDevicesWebManager.WebApi"),
        //};

        /// <summary>
        /// 解决方案所有自定义类
        /// </summary>
        public static readonly Type[] AllTypes = GetAssemblyLis().SelectMany(x => x.GetTypes()).ToArray();

        public static Assembly[] GetAssemblyLis(Func<Assembly, bool> where = null)
        {
            //#region 查找手动引用的程序集
            //IEnumerable<Assembly> allAssemblies = new List<Assembly>();
            //var entryAssembly = Assembly.GetEntryAssembly();
            //if (entryAssembly == null) return allAssemblies;
            //var referencedAssemblies = entryAssembly.GetReferencedAssemblies().Select(Assembly.Load);
            //allAssemblies = new List<Assembly> { entryAssembly }.Union(referencedAssemblies);
            //#endregion

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();


            HashSet<string> loadedAssemblies = new HashSet<string>();

            foreach (var item in allAssemblies)
            {
                if (item.FullName != null )
                {
                    loadedAssemblies.Add(item.FullName);
                }
            }

            Queue<Assembly> assembliesToCheck = new Queue<Assembly>();
            if (Assembly.GetEntryAssembly() != null)
                assembliesToCheck.Enqueue(Assembly.GetEntryAssembly());

            while (assembliesToCheck.Any())
            {
                var assemblyToCheck = assembliesToCheck.Dequeue();
                if (assembliesToCheck != null)
                    foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
                    {
                        if (!loadedAssemblies.Contains(reference.FullName))
                        {
                            var assembly = Assembly.Load(reference);

                            assembliesToCheck.Enqueue(assembly);

                            loadedAssemblies.Add(reference.FullName);

                            allAssemblies.Add(assembly);
                        }
                    }
            }
            return @where == null ? allAssemblies.ToArray() : allAssemblies.Where(@where).ToArray();

        }



        /// <summary>
        /// 超级管理员UserIId
        /// </summary>
        public const string ADMINID = "Admin";
    }
}
