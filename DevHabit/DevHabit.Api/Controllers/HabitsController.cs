using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("habits")]
public sealed class HabitsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits()
    {
        List<HabitDto> habits = await dbContext
            .Habits
            .Select(h => new HabitDto
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Type = h.Type,
                Frequency = new FrequencyDto
                {
                    Type = h.Frequency.Type,
                    TimesPerPeriod = h.Frequency.TimesPerPeriod
                },
                Target = new TargetDto
                {
                    Value = h.Target.Value,
                    Unit = h.Target.Unit
                },
                Status = h.Status,
                IsArchived = h.IsArchived,
                EndDate = h.EndDate,
                Milestone = h.Milestone != null ? new MilestoneDto
                {
                    Target = h.Milestone.Target,
                    Current = h.Milestone.Current
                } : null,
                CreatedAtUtc = h.CreatedAtUtc,
                UpdatedAtUtc = h.UpdatedAtUtc,
                LastCompletedAtUtc = h.LastCompletedAtUtc
            }).
            ToListAsync();
        var habitsCollectionDto = new HabitsCollectionDto
        {
            Data = habits
        };
        return Ok(habitsCollectionDto);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<HabitDto>> GetHabitId(string id)
    {
        HabitDto? habit = await dbContext
            .Habits
            .Where(h => h.Id == id)
            .Select(h => new HabitDto
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Type = h.Type,
                Frequency = new FrequencyDto
                {
                    Type = h.Frequency.Type,
                    TimesPerPeriod = h.Frequency.TimesPerPeriod
                },
                Target = new TargetDto
                {
                    Value = h.Target.Value,
                    Unit = h.Target.Unit
                },
                Status = h.Status,
                IsArchived = h.IsArchived,
                EndDate = h.EndDate,
                Milestone = h.Milestone != null ? new MilestoneDto
                {
                    Target = h.Milestone.Target,
                    Current = h.Milestone.Current
                } : null,
                CreatedAtUtc = h.CreatedAtUtc,
                UpdatedAtUtc = h.UpdatedAtUtc,
                LastCompletedAtUtc = h.LastCompletedAtUtc
            })
            .FirstOrDefaultAsync();

        if (habit == null)
        {
            return NotFound();
        }
        return Ok(habit);
    }
    /*[HttpPost]
    public IActionResult CreateHabit([FromBody] Habit habit)
    {
        if (habit == null)
        {
            return BadRequest("Habit cannot be null.");
        }
        dbContext.Habits.Add(habit);
        dbContext.SaveChanges();
        return CreatedAtAction(nameof(GetHabits), new { id = habit.Id }, habit);
    }*/
}
