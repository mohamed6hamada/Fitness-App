using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fitness_App.DAL.Models;

namespace Fitness_App.DAL.Interfaces
{
    public interface ICoachRepository
    {
        Task<IEnumerable<Coach>> GetAllCoachesAsync();
        Task<Coach> GetCoachByIdAsync(int id);
        Task<Coach> AddCoachAsync(Coach coach);
        Task<bool> UpdateCoachAsync(Coach coach);
        Task<bool> DeleteCoachAsync(int id);
        Task<IEnumerable<WorkoutPlan>> GetWorkoutPlansByCoachIdAsync(int coachId);
    }
}
