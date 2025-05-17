using Fitness_App.BL.Interfaces;
using Fitness_App.BL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitness_App.BL.Servecies
{
    public class ExerciseServices
    {
        private readonly IAdminRepository<ExerciseViewModel> adminRepository;

        public ExerciseServices(IAdminRepository<ExerciseViewModel> adminRepository)
        {
            this.adminRepository = adminRepository;
        }
        public void AddExercise(ExerciseViewModel exercise)
        {
            adminRepository.AddExercise(new DAL.Models.Exercises
            {
                Name = exercise.Name,
                Description = exercise.Description,
                GIF = exercise.GIF,
                Type = exercise.Type,
                //AdminId = exercise.AdminId
            });
        }
        public void UpdateExercise(ExerciseViewModel exercise)
        {
            adminRepository.UpdateExercise(new DAL.Models.Exercises
            {
                ExerciseId = adminRepository.GetExerciseData().FirstOrDefault(e => e.Name == exercise.Name).ExerciseId,
                Name = exercise.Name,
                Description = exercise.Description,
                GIF = exercise.GIF,
                Type = exercise.Type,
                //AdminId = exercise.AdminId
            });
        }
        public void DeleteExercise(int id)
        {

            try
            {
                adminRepository.DeleteExercise(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public List<ExerciseViewModel> GetAllExercises()
        {
            var exercises = adminRepository.GetExerciseData();
            return exercises.Select(e => new ExerciseViewModel
            {
                ExerciseId = e.ExerciseId,
                Name = e.Name,
                Description = e.Description,
                GIF = e.GIF,
                Type = e.Type,
                //AdminId = e.AdminId
            }).ToList();
        }
    }
}
