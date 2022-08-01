using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lidya.Toolkit
{
    public static class CastingHelper
    {
        public static TResult Clone<T, TResult>(this T myobj, Func<T, TResult, TResult> action = null)
        where T : class
        where TResult : class
        {
            if (myobj == null)
                return null;
          
            Type objectType = myobj.GetType();
            Type target = typeof(TResult);
            var x = Activator.CreateInstance(target, false);
            var z = from source in objectType.GetMembers().ToList()
                    where source.MemberType == MemberTypes.Property
                    select source;
            var d = from source in target.GetMembers().ToList()
                    where source.MemberType == MemberTypes.Property
                    select source;
            List<MemberInfo> members = d.Where(memberInfo => d.Select(c => c.Name)
               .ToList().Contains(memberInfo.Name)).ToList();
            PropertyInfo propertyInfo;
            object value;
            foreach (var memberInfo in members)
            {
                propertyInfo = typeof(TResult).GetProperty(memberInfo.Name);
                var isProp = myobj.GetType().GetProperty(memberInfo.Name);
                if (isProp != null && isProp.PropertyType == propertyInfo.PropertyType)
                {
                    value = myobj.GetType().GetProperty(memberInfo.Name).GetValue(myobj, null);
                    propertyInfo.SetValue(x, value, null);
                }
            }
            if (action != null)
            {
                return action(myobj, (TResult)x);
            }
            return (TResult)x;
        }

        public static void Mapper<T, TResult>(this T myobj, TResult targetModel, Func<T, TResult, TResult> action = null)
           where T : class
           where TResult : class
        {
            if (myobj != null)
            {
                Type objectType = myobj.GetType();
                Type target = targetModel.GetType();
                var z = from source in objectType.GetMembers().ToList()
                        where source.MemberType == MemberTypes.Property
                        select source;
                var d = from source in target.GetMembers().ToList()
                        where source.MemberType == MemberTypes.Property
                        select source;
                List<MemberInfo> members = d.Where(memberInfo => d.Select(c => c.Name)
                   .ToList().Contains(memberInfo.Name)).ToList();
                PropertyInfo propertyInfo;
                object value;
                foreach (var memberInfo in members)
                {
                    propertyInfo = typeof(TResult).GetProperty(memberInfo.Name);
                    var isProp = myobj.GetType().GetProperty(memberInfo.Name);
                    if (isProp != null && isProp.PropertyType == propertyInfo.PropertyType)
                    {
                        value = myobj.GetType().GetProperty(memberInfo.Name).GetValue(myobj, null);
                        propertyInfo.SetValue(targetModel, value, null);
                    }
                }
                if (action != null)
                {
                    action(myobj, targetModel);
                }
            }
        }
    }
}
