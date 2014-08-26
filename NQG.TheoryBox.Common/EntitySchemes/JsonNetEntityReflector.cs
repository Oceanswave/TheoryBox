namespace NQG.TheoryBox.EntitySchemes
{
    using MyCouch.EntitySchemes;
    using MyCouch.EntitySchemes.Reflections;

    public class JsonNetEntityReflector : IEntityReflector
    {
        public IEntityMember IdMember { get; protected set; }
        public IEntityMember RevMember { get; protected set; }

        public JsonNetEntityReflector(IDynamicPropertyFactory dynamicPropertyFactory)
        {
            IdMember = new JsonNetEntityIdMember(dynamicPropertyFactory);
            RevMember = new EntityRevMember(dynamicPropertyFactory);
        }
    }
}
