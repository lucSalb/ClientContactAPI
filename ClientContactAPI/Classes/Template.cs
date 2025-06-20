﻿using System.ComponentModel.DataAnnotations;

namespace ClientContactAPI.Classes
{
    public class Template
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }

    }
}
