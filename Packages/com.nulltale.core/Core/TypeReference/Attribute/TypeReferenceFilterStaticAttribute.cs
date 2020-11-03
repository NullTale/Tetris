using System;

namespace Core
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TypeReferenceFilterStaticAttribute : TypeReferenceFilterAttribute
    {
        private bool m_StaticCondition;

        //////////////////////////////////////////////////////////////////////////
        public TypeReferenceFilterStaticAttribute(bool staticCondition)
        {
            m_StaticCondition = staticCondition;
        }

        public override bool Verify(Type type)
        {
            return (type.IsAbstract && type.IsSealed) == m_StaticCondition;
        }
    }
}