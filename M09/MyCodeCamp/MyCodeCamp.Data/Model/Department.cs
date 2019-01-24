using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyCodeCamp.Data.Model
{
    [Table("Department", Schema = "dbo")]
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Department Id")]
        public int DepartmentId { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        [Display(Name = "Department Code")]
        public string DepartmentCode { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [Display(Name = "Department Description")]
        public string DepartmentName { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Description { get; set; }
    }
}
