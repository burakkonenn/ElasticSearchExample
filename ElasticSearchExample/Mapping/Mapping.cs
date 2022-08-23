using ElasticSearchExample.Entities;
using Nest;

namespace ElasticSearchExample.Mapping
{
    public static class Mapping
    {
        public static CreateIndexDescriptor ProductMapping(this CreateIndexDescriptor descriptor)
        {
            return descriptor.Map<Product>(a => a.Properties(i => i

                .Keyword(a => a.Name(a => a.Id))
                .Text(a => a.Name(a => a.Name))
                .Text(a => a.Name(a => a.ImageUrl))
                .Number(a => a.Name(a => a.Price))));
        }
    }
}
