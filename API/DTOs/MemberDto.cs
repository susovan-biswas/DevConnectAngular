using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class MemberDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }    
        public string PhotoUrl { get; set; }    
        public int Age { get; set; }
        public DateTime Created { get; set; } 
        public DateTime LastActive { get; set; } 
        public string Introduction { get; set; }
        public string FullName { get; set; }
        public string Skills { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<PhotoDto> Photos { get; set; } 
        public List<BlogDto> Blogs { get; set; } 

    }
}