using Fitness_App.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Fitness_App.BL.ViewModels
{
    public class ExerciseViewModel
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] GIF { get; set; } // check this how to upload the photos
        public ExerciseType Type { get; set; }
        public int? AdminId { get; set; }
        public IFormFile GIFFile { get; set; }

    }

}
