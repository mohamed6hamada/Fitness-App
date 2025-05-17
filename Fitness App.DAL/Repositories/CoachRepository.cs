using Fitness_App.DAL.DbContext;
using Fitness_App.DAL.Interfaces;
using Fitness_App.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_App.DAL.Repositories
{
    public class CoachRepository : ICoachRepository
    {
        private readonly FitnessAppDbContext _context;

        public CoachRepository(FitnessAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Coach>> GetAllCoachesAsync()
        {
            return await _context.Coaches
                .Include(c => c.WorkoutPlans)
                .ToListAsync();
        }

        public async Task<Coach> GetCoachByIdAsync(int id)
        {
            return await _context.Coaches
                .Include(c => c.WorkoutPlans)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Coach> AddCoachAsync(Coach coach)
        {
            await _context.Coaches.AddAsync(coach);
            await _context.SaveChangesAsync();
            return coach;
        }

        public async Task<bool> UpdateCoachAsync(Coach coach)
        {
            var existingCoach = await _context.Coaches.FindAsync(coach.Id);
            if (existingCoach == null)
                return false;

            _context.Entry(existingCoach).CurrentValues.SetValues(coach);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCoachAsync(int id)
        {
            var coach = await _context.Coaches.FindAsync(id);
            if (coach == null)
                return false;

            _context.Coaches.Remove(coach);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WorkoutPlan>> GetWorkoutPlansByCoachIdAsync(int coachId)
        {
            return await _context.WorkoutPlans
                .Include(w => w.AssignedUsers)
                .Where(w => w.CoachId == coachId)
                .ToListAsync();
        }
    }
}
