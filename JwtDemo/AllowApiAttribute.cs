using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtDemo
{
    /// <summary>
    /// 允许跳过的方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AllowApiAttribute : Attribute
    {
        public AllowApiAttribute(string name="default")
        {
            Name = name;
        }
        public string Name { get; set; }
    }
}
