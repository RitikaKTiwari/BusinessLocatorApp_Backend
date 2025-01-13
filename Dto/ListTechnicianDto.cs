using BusinessLocatorApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessLocatorApp.Dto
{
    public class ListTechnicianDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Experience { get; set; }
        public int BusinessServiceId { get; set; }
        public string servicename { get; set; }
    }
}
