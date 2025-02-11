﻿using System;
using System.ComponentModel.DataAnnotations;

namespace DemoProject.Model
{
    public class webpages_Roles
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string RoleCode { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
    public class RolesGridModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RoleCode { get; set; }
        public bool IsActive { get; set; }
    }
}