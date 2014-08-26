namespace NQG.TheoryBox.EntitySchemes
{
    using MyCouch.EntitySchemes;
    using MyCouch.EntitySchemes.Reflections;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Reflection;

    public class JsonNetEntityIdMember : EntityIdMember
    {
        public JsonNetEntityIdMember(IDynamicPropertyFactory dynamicPropertyFactory)
            : base(dynamicPropertyFactory)
        {
        }

        //TODO: Yuk! override the IEntityMember methods first...

        protected override IStringGetter GetGetterFor(Type type)
        {
            var property = FindIdProperty(type);
            if (property == null)
                return base.GetGetterFor(type);

            return DynamicPropertyFactory.PropertyFor(property).Getter;
        }

        protected override IStringSetter GetSetterFor(Type type)
        {
            var property = FindIdProperty(type);

            if (property == null)
                return base.GetSetterFor(type);

            return DynamicPropertyFactory.PropertyFor(property).Setter;
        }

        private PropertyInfo FindIdProperty(Type type)
        {
            return GetPropertiesFor(type).FirstOrDefault(property =>
            {
                var jsonPropertyAttribute = property.GetCustomAttribute<JsonPropertyAttribute>();

                if (jsonPropertyAttribute == null)
                    return false;

                if (jsonPropertyAttribute.PropertyName.Equals("_id", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (jsonPropertyAttribute.PropertyName.Equals(string.Concat(type.Name, "id"),
                    StringComparison.OrdinalIgnoreCase))
                    return true;

                if (jsonPropertyAttribute.PropertyName.Equals("documentid", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (jsonPropertyAttribute.PropertyName.Equals("entityid", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (jsonPropertyAttribute.PropertyName.Equals("id", StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            });
        }
    }
}
