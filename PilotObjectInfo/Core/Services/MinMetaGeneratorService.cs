using System;
using System.Collections.Generic;
using System.Linq;
using Ascon.Pilot.SDK;
using PilotObjectInfo.Dto;

namespace PilotObjectInfo.Core.Services
{
    internal class MinMetaGeneratorService
    {
        internal IEnumerable<MinMetaDto> CreateMany(IEnumerable<object> sources)
        {
            if (sources == null)
                return Enumerable.Empty<MinMetaDto>();

            return sources.AsParallel().AsOrdered().Select(Create);
        }
        
        internal MinMetaDto Create(object source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source switch
            {
                IUserState userState => Create(userState),
                IType type => Create(type),
                IAttribute attribute => Create(attribute),
                _ => throw new ArgumentException($"Неподдерживаемый тип объекта для метаданных: {source.GetType().FullName}", nameof(source))
            };
        }
        MinMetaDto Create(IAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            return new MinMetaDto(attribute.Name, attribute.Title, MetaClass.PAttribute);
        }
        
        MinMetaDto Create(IType type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return new MinMetaDto(type.Name, type.Title, MetaClass.PType);
        }
        
        MinMetaDto Create(IUserState userState)
        {
            if (userState == null)
                throw new ArgumentNullException(nameof(userState));

            return new MinMetaDto(userState.Name, userState.Title, MetaClass.PState);
        }
    }
}
