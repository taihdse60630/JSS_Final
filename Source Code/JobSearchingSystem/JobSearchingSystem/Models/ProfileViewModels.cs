using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JobSearchingSystem.Models
{
    public class ProListItem
    {
        public int ProfileID { get; set; }
        public int No { get; set; }
        public string ProfileName { get; set; }
        public bool IsActive { get; set; }
        public int ViewedCount { get; set; }
        public System.DateTime UpdatedTime { get; set; }
        public int PerccentStatus { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class ProListViewModel
    {
        public IEnumerable<ProListItem> proList { get; set; }
    }

    public class ProCommonInfoItem
    {
        public Profile profile { get; set; }
        
        public int expectedCity { get; set; }
        public int categoryID { get; set; }

        public ProCommonInfoItem() 
        {
            profile = new Profile();
            profile.YearOfExperience = 0;
            profile.ExpectedSalary = 0;
            profile.LanguageID = -1;
            profile.Level_ID = -1;
            
            expectedCity = -1;
            categoryID = -1;
        }
    }

    public class ProContactForm
    {
        [Required(ErrorMessage = "Họ và tên không được trống.")]
        [StringLength(50, ErrorMessage = "Họ và tên không được vượt quá 50 kí tự.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được trống.")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 kí tự.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Hãy nhập đúng định dạng số điện thoại.")]
        public string PhoneNumber { get; set; }
    }

    public class ProCommonInfoForm
    {
        [Required(ErrorMessage = "Vị trí mong muốn không được trống.")]
        [StringLength(50, ErrorMessage = "Vị trí mong muốn không được vượt quá 50 kí tự.")]
        public string ExpectedPosition { get; set; }
    }

    public class ProEmploymentHistoryForm
    {
        [Required(ErrorMessage = "Chức danh không được trống.")]
        [StringLength(50, ErrorMessage = "Chức danh không được vượt quá 50 kí tự.")]
        public string Position { get; set; }
        [Required(ErrorMessage = "Công ty không được trống.")]
        [StringLength(50, ErrorMessage = "Công ty không được vượt quá 50 kí tự.")]
        public string Company { get; set; }
    }

    public class ProEducationHistoryForm
    {
        [Required(ErrorMessage = "Chuyên ngành không được trống.")]
        [StringLength(50, ErrorMessage = "Chuyên ngành không được vượt quá 50 kí tự.")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Trường không được trống.")]
        [StringLength(50, ErrorMessage = "Trường không được vượt quá 50 kí tự.")]
        public string School { get; set; }
    }

    public class ProReferencePersonForm
    {
        [Required(ErrorMessage = "Họ tên không được trống.")]
        [StringLength(50, ErrorMessage = "Họ tên không được vượt quá 50 kí tự.")]
        public string ReferencePersonName { get; set; }
        [Required(ErrorMessage = "Chức danh không được trống.")]
        [StringLength(50, ErrorMessage = "Chức danh không được vượt quá 50 kí tự.")]
        public string ReferencePersonPosition { get; set; }
        [Required(ErrorMessage = "Công ty không được trống.")]
        [StringLength(50, ErrorMessage = "Công ty không được vượt quá 50 kí tự.")]
        public string ReferencePersonCompany { get; set; }
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 kí tự.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Hãy nhập đúng định dạng số điện thoại.")]
        public string ReferencePersonPhoneNumber { get; set; }
    }

    public class ProUpdateViewModel
    {
        public IEnumerable<City> cities { get; set; }
        public IEnumerable<SchoolLevel> schoolLevels { get; set; }
        public IEnumerable<Language> languages { get; set; }
        public IEnumerable<Level> levels { get; set; }
        public IEnumerable<JobLevel> jobLevels { get; set; }
        public IEnumerable<Category> categories { get; set; }

        public Contact contact { get; set; }
        public ProContactForm contactForm { get; set; }
        public ProCommonInfoItem commonInfoItem { get; set; }
        public ProCommonInfoForm commonInfoForm { get; set; }
        public EmploymentHistory employmentHistory { get; set; }
        public ProEmploymentHistoryForm employmentHistoryForm { get; set; }
        public EducationHistory educationHistory { get; set; }
        public ProEducationHistoryForm educationHistoryForm { get; set; }
        public ReferencePerson referencePerson { get; set; }
        public ProReferencePersonForm referencePersonForm { get; set; }
    }
}