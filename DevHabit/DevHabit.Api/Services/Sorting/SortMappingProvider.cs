﻿using System.Linq.Dynamic.Core;

namespace DevHabit.Api.Services.Sorting;

public sealed class  SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinitions) 
{
    public SortMapping[] GetMappings<TSource, TDestination>()
    {
        SortMappingDefinition<TSource, TDestination> sortMappingDefinition =  sortMappingDefinitions
            .OfType<SortMappingDefinition<TSource, TDestination>>()
            .FirstOrDefault();

        if (sortMappingDefinition is null)
        {
            throw new InvalidOperationException($"No sort mapping definition found for {typeof(TSource).Name} to {typeof(TDestination).Name}.");
        }

        return sortMappingDefinition.Mappings;
    }

    public bool ValidateMappings<TSource, TDestination>(string? sort)
    {
        if (string.IsNullOrEmpty(sort))
        {
            return true;
        }

        var sortFields = sort
            .Split(',')
            .Select(f => f.Trim().Split(" ")[0])
            .Where(f => !string.IsNullOrEmpty(f))
            .ToList();

        SortMapping[] mappings = GetMappings<TSource, TDestination>();

        return sortFields.All(f => mappings.Any(m => m.SortField.Equals(f, StringComparison.OrdinalIgnoreCase)));
    }
}


