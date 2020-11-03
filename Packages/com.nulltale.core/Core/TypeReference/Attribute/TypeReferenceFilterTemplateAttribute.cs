using System;

namespace Core
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TypeReferenceFilterTemplateAttribute : TypeReferenceFilterAttribute
    {
        private bool m_TemplateCondition;

        //////////////////////////////////////////////////////////////////////////
        public TypeReferenceFilterTemplateAttribute(bool templateCondition)
        {
            m_TemplateCondition = templateCondition;
        }

        public override bool Verify(Type type)
        {
            return type.IsGenericType == m_TemplateCondition;
        }
    }
}