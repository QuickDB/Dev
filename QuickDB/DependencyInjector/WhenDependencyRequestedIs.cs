namespace QuickDB.DependencyInjector
{
    public static class WhenDependencyRequestedIs<TMapFrom>
    {
        public static DependencyMaping Provide< TMapTo>( ) where TMapTo: TMapFrom
        {
            return new DependencyMaping()
            {
                MapFrom = typeof(TMapFrom),
                MapTo = typeof(TMapTo)
            };
        }
    }
}