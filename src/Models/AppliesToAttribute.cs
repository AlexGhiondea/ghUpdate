using System;

namespace ghUpdate.Models
{
    public class AppliesToAttribute : Attribute
    {
        public AttributeTypeEnum AppliesTo { get; private set; }
        public AppliesToAttribute(AttributeTypeEnum appliesTo)
        {
            AppliesTo = appliesTo;
        }
    }
}
