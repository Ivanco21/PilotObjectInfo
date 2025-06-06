
using Ascon.Pilot.SDK;
using PilotObjectInfo.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PilotObjectInfo.Core.Services
{
    internal class QPilot
    {
        internal async Task<IEnumerable<Guid>> SearchObjectsByTypesAsync(params int[] types)
        {
            var builder = GI.SearchService.GetObjectQueryBuilder();
            builder.Must(ObjectFields.TypeId.BeAnyOf(types));
            builder.MaxResults(int.MaxValue);
            var SearchResults = await GI.SearchService.Search(builder).FirstAsync(x => x.Kind == SearchResultKind.Remote);
            return SearchResults.Result;
        }

        internal async Task<IEnumerable<Guid>> SearchObjectsByTypesAsync(int maxResults, params int[] types)
        {
            var builder = GI.SearchService.GetObjectQueryBuilder();
            builder.Must(ObjectFields.TypeId.BeAnyOf(types));
            builder.MaxResults(maxResults);
            var SearchResults = await GI.SearchService.Search(builder).FirstAsync(x => x.Kind == SearchResultKind.Remote);
            return SearchResults.Result;
        }

        internal HashSet<AttributeModel> GetAllPilotAttributes()
        {
            ConcurrentDictionary<AttributeModel, byte> data = new();
            IEnumerable<IType> types = GI.Repository.GetTypes();
            Parallel.ForEach(types, type =>
            {
                foreach (IAttribute attr in type.Attributes)
                {
                    AttributeModel am = new()
                    {
                        Name = attr.Name,
                        Title = attr.Title
                    };
                    if (!data.Any(d =>d.Key.Name == am.Name))
                    {
                        data.TryAdd(am, 0);
                    }
                }
            });

            return new HashSet<AttributeModel>(data.Keys);
        }

        internal HashSet<TypeModel> GetAllPilotTypes()
        {
            IEnumerable<IType> types = GI.Repository.GetTypes();
            ConcurrentDictionary<TypeModel, byte> data = new();
            Parallel.ForEach(types, type =>
            {
                TypeModel tm = new()
                {
                    Name = type.Name,
                    Title = type.Title
                };
                data.TryAdd(tm, 0);
            });
            return new HashSet<TypeModel>(data.Keys);
        }
    }
}
