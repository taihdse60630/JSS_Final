using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JobSearchingSystem.Models
{
    public class ComIndexViewModel
    {
        public IEnumerable<City> cities { get; set; }
        public IEnumerable<Category> categories { get; set; }
        public IEnumerable<Language> languages { get; set; }
        public IEnumerable<JobLevel> jobLevels { get; set; }
        public IEnumerable<SchoolLevel> schoolLevels { get; set; }
        public IEnumerable<Level> levels { get; set; }
    }

    public class ComCityListViewModel
    {
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        [StringLength(50, ErrorMessage = "Nội dung nhập vào không vượt quá 50 kí tự.")]
        public string name { get; set; }
        public IEnumerable<City> cities { get; set; }
    }

    public class ComCategoryListViewModel
    {
        public string message { get; set; }
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        [StringLength(30, ErrorMessage = "Nội dung nhập vào không vượt quá 30 kí tự.")]
        public string name { get; set; }
        [StringLength(100, ErrorMessage = "Nội dung nhập vào không vượt quá 100 kí tự.")]
        public string description { set; get; }
        public IEnumerable<Category> categories { get; set; }
    }

    public class ComLanguageViewModel
    {
        public string message { get; set; }
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        [StringLength(50, ErrorMessage = "Nội dung nhập vào không vượt quá 50 kí tự.")]
        public string name { get; set; }
        public IEnumerable<Language> languages { get; set; }
    }

    public class ComJobLevelViewModel
    {
        public string message { get; set; }
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        [StringLength(50, ErrorMessage = "Nội dung nhập vào không vượt quá 50 kí tự.")]
        public string name { get; set; }
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        public int levelNum { get; set; }
        public IEnumerable<JobLevel> jobLevels { get; set; }
    }

    public class ComSchoolLevelViewModel
    {
        public string message { get; set; }
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        [StringLength(30, ErrorMessage = "Nội dung nhập vào không vượt quá 30 kí tự.")]
        public string name { get; set; }
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        public int levelNum { get; set; }
        public IEnumerable<SchoolLevel> schoolLevels { get; set; }
    }

    public class ComLevelViewModel
    {
        public string message { get; set; }
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        [StringLength(30, ErrorMessage = "Nội dung nhập vào không vượt quá 30 kí tự.")]
        public string name { get; set; }
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Nội dung nhập vào phải là số nguyên")]
        public int levelNum { get; set; }
        public IEnumerable<Level> levels { get; set; }
    }
}