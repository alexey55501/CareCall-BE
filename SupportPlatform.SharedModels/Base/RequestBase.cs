using System;
using System.Collections.Generic;
using System.Text;

namespace SupportPlatform.SharedModels.Base
{
    public enum CategoryType { None, Volunteer, Petsitting, Hire, }
    public class RequestBase
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public CategoryType Category { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonTelegram { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string EventDate { get; set; }
        public string EventTime { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatorId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
