
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Reactive.Linq;
using Ascon.Pilot.SDK;

namespace PilotObjectInfo.Core.Services
{
    internal class SearchService
    {
        public async Task<IEnumerable<Guid>> SearchObjectsByTypesAsync(params int[] types)
        {
            var builder = GI.SearchService.GetObjectQueryBuilder();
            builder.Must(ObjectFields.TypeId.BeAnyOf(types));
            builder.MaxResults(int.MaxValue);
            var SearchResults = await GI.SearchService.Search(builder).FirstAsync(x => x.Kind == SearchResultKind.Remote);
            return SearchResults.Result;
        }

        public async Task<IEnumerable<Guid>> SearchObjectsByTypesAsync(int maxResults, params int[] types)
        {
            var builder = GI.SearchService.GetObjectQueryBuilder();
            builder.Must(ObjectFields.TypeId.BeAnyOf(types));
            builder.MaxResults(maxResults);
            var SearchResults = await GI.SearchService.Search(builder).FirstAsync(x => x.Kind == SearchResultKind.Remote);
            return SearchResults.Result;
        }
    }
}
