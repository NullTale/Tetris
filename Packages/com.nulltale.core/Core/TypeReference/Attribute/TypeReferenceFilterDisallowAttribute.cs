using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TypeReferenceFilterDisallowAttribute : TypeReferenceFilterAttribute
    {
        public List<Type>		m_DisallowList;

        //////////////////////////////////////////////////////////////////////////
        public TypeReferenceFilterDisallowAttribute(params Type[] typeList)
        {
            m_DisallowList = typeList.ToList();
        }

        public override bool Verify(Type type)
        {
            if (m_DisallowList.Contains(type))
                return false;

            return true;
        }
    }
}