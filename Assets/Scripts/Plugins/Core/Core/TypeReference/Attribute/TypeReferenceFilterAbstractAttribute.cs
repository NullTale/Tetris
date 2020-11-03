using System;

namespace Core
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TypeReferenceFilterAbstractAttribute : TypeReferenceFilterAttribute
    {
        private bool m_AbstractCondition;

        //////////////////////////////////////////////////////////////////////////
        public TypeReferenceFilterAbstractAttribute(bool abstractCondition)
        {
            m_AbstractCondition = abstractCondition;
        }

        public override bool Verify(Type type)
        {
            if (m_AbstractCondition)
            {	// filter static
                if (type.IsSealed)		// cut off type.IsAbstract &&
                    return false;
                // filter class
                if (type.IsClass == false)
                    return false;
            }

            return type.IsAbstract == m_AbstractCondition;
        }
    }
}