using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class Fix_HabitTags : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_habit_tags_tags_habit_id",
            schema: "dev_habit",
            table: "habit_tags");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddForeignKey(
            name: "fk_habit_tags_tags_habit_id",
            schema: "dev_habit",
            table: "habit_tags",
            column: "habit_id",
            principalSchema: "dev_habit",
            principalTable: "tags",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
