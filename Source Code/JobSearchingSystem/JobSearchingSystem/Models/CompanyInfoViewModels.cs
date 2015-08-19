using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JobSearchingSystem.Models
{
    public class CoInUpdateViewModel
    {
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        public string cityId { get; set; }
        public string recuiterId { get; set; }
        [StringLength(150, ErrorMessage = "Nội dung nhập vào không vượt quá 150 kí tự.")]
        [DataType(DataType.ImageUrl)]
        public string logoURL { get; set; }
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        [StringLength(100, ErrorMessage = "Nội dung nhập vào không vượt quá 100 kí tự.")]
        public string company { get; set; }
        [StringLength(100, ErrorMessage = "Nội dung nhập vào không vượt quá 100 kí tự.")]
        public string address { get; set; }
        [StringLength(30, ErrorMessage = "Nội dung nhập vào không vượt quá 30 kí tự.")]
        public string district { get; set; }
        [Required(ErrorMessage = "Thông tin này bắt buộc")]
        public string city { get; set; }
        [StringLength(20, ErrorMessage = "Nội dung nhập vào không vượt quá 20 kí tự.")]
        [DataType(DataType.PhoneNumber)]
        public string phoneNumber { get; set; }
        public string description { get; set; }
        public IEnumerable<City> cities { get; set; }

    }
}