using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExemploGraphQL
{
    public class StarsWarsSchema : Schema
    {
        public StarsWarsSchema(Func<Type, GraphType> resolveType) : base(resolveType)
        {
            Query = (StarWarsQuery)resolveType(typeof(StarWarsQuery));
        }
    }
}
