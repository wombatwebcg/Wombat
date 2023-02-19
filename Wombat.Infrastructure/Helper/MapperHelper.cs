using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wombat.Infrastructure
{ 
    public  class MapperHelper
    {
        /// <summary>
        /// 通过反射，将 T1 映射为 T2
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="t1"></param>
        /// <returns></returns>
        public static T2 T1MapToT2<T1, T2>(T1 t1)
            where T1 : class
            where T2 : class //, new()
        {
            T2 t2 = Activator.CreateInstance<T2>();  //T2 t2 = new T2(); //后面这种写法，要在 where 中添加 new()
            if (t1 == null)
            {
                return t2;
            }

            var p1 = t1.GetType().GetProperties();
            var p2 = typeof(T2).GetProperties();
            for (int i = 0; i < p1.Length; i++)
            {
                //条件：1、属性名相同；2、t2属性可写；3、属性可读性一致；4、数据类型相近（相同，或者接近。接近如：int 和 int?）
                var p = p2.Where(t => t.Name == p1[i].Name && t.CanWrite && t.CanRead == p1[i].CanRead).FirstOrDefault();
                if (p == null)
                    continue;
                var v = p1[i].GetValue(t1);
                if (v == null)
                    continue;
                try { p.SetValue(t2, v); } //难判定数据类型，暂时这样处理
                catch
                {
                    try { p.SetValue(t2, Convert.ChangeType(v, p.PropertyType)); } //int? -> object -> int? 会抛错
                    catch { }
                }

            }

            return t2;
        }

        //这种写法和上面的写法没啥差别
        public static T2 T1MapToT2_2<T1, T2>(T1 t1, T2 Target = null, string exceptAttribute = "" )
            where T1 : class
            where T2 : class //, new()
        {
            if (Target==null)
            {
               Target = Activator.CreateInstance<T2>();  //T2 t2 = new T2(); //后面这种写法，要在 where 中添加 new()
            }
            var p1 = t1.GetType().GetProperties();
            var p2 = typeof(T2);
            for (int i = 0; i < p1.Length; i++)
            {
                if (exceptAttribute == p1[i].Name) continue;

                //条件：1、属性名相同；2、t2属性可写；3、属性可读性一致；4、数据类型相近（相同，或者接近。接近如：int 和 int?）
                var p = p2.GetProperty(p1[i].Name);
                if (p == null || !p.CanWrite || p.CanRead != p1[i].CanRead)
                    continue;
                var v = p1[i].GetValue(t1);
                if (v == null)
                    continue;
                try { p.SetValue(Target, Convert.ChangeType(v, p.PropertyType)); }
                catch { }
            }

            return Target;
        }


        public static D T1MapToT2_3<D, S>(S s)
        {
            D d = Activator.CreateInstance<D>();
            try
            {
                var sType = s.GetType();
                var dType = typeof(D);
                foreach (PropertyInfo sP in sType.GetProperties())
                {
                    foreach (PropertyInfo dP in dType.GetProperties())
                    {
                        if (dP.Name == sP.Name)
                        {
                            dP.SetValue(d, sP.GetValue(s)); break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return d;
        }

    }
}
