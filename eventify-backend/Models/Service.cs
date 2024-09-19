﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eventify_backend.Models
{
    public class Service : ServiceAndResource
    {
        [ForeignKey("ServiceCategory")]
        public int ServiceCategoryId { get; set; }

        public ServiceCategory? ServiceCategory { get; set; }

    }
}
