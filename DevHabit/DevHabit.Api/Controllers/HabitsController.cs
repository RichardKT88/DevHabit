﻿using System.Linq.Dynamic.Core;
using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;
using DevHabit.Api.Services.Sorting;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("habits")]
public sealed class HabitsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits(
        [FromQuery] HabitsQueryParameters query,
        SortMappingProvider sortMappingProvider)
    {
        if (!sortMappingProvider.ValidateMappings<Habit, HabitDto>(query.Sort))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided sort parameter isn't valid: '{query.Sort}'");
        }
        query.Search ??= query.Search?.Trim().ToLower();

        SortMapping[] sortMappings = sortMappingProvider.GetMappings<Habit, HabitDto>();

        List<HabitDto> habits = await dbContext
            .Habits
            .Where(h => query.Search == null ||
                        h.Name.Contains(query.Search, StringComparison.CurrentCultureIgnoreCase) ||
                        h.Description != null && h.Description.Contains(query.Search, StringComparison.CurrentCultureIgnoreCase))
            .Where(h => query.Type == null || h.Type == query.Type)
            .Where(h => query.Status == null || h.Status == query.Status)
            .ApplySort(query.Sort, sortMappings)
            .Select(HabitQueries.ProjectToDto())
            .ToListAsync();

        var habitsCollectionDto = new HabitsCollectionDto
        {
            Data = habits
        };
        
        return Ok(habitsCollectionDto);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<HabitWithTagsDto>> GetHabitId(string id)
    {
        HabitWithTagsDto? habit = await dbContext
            .Habits
            .Where(h => h.Id == id)
            .Select(HabitQueries.ProjectToDtoWithTags())
            .FirstOrDefaultAsync();

        if (habit == null)
        {
            return NotFound();
        }
        
        return Ok(habit);
    }
    [HttpPost]
    public async Task<ActionResult<HabitDto>> CreateHabit(
        CreateHabitDto createHabitDto,
        IValidator<CreateHabitDto> validator)
    {
        await validator.ValidateAndThrowAsync(createHabitDto);      

        Habit habit = createHabitDto.ToEntity();
        
        dbContext.Habits.Add(habit);
                
        await dbContext.SaveChangesAsync();
        
        HabitDto habitDto = habit.ToDto();
        
        return CreatedAtAction(nameof(GetHabits), new { id = habitDto.Id}, habitDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateHabit(string id, UpdateHabitDto updateHabitDto)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);
        
        if (habit == null)
        {
            return NotFound();
        }
        
        habit.UpdateFromDto(updateHabitDto);
        
        await dbContext.SaveChangesAsync();        
      
        return NoContent();
    } 

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchHabit(string id, JsonPatchDocument<HabitDto> patchDocument)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);
        
        if (habit == null)
        {
            return NotFound();
        }
        
        HabitDto habitDto = habit.ToDto();
        
        patchDocument.ApplyTo(habitDto, ModelState);
        
        if (!TryValidateModel(habitDto))
        {
            return ValidationProblem(ModelState);
        }
        
        habit.Name = habitDto.Name;
        habit.Description = habitDto.Description;
        habit.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteHabit(string id)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);
        
        if (habit == null)
        {
            return NotFound();
        }
        
        dbContext.Habits.Remove(habit);
        
        await dbContext.SaveChangesAsync();
        
        return NoContent();
    }

}
