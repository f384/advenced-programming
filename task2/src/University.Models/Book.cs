using System;
using System.Collections.Generic;
using University.Interfaces;

namespace University.Models
{
    public class Book : IEntity
    {
        public string BookId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime? PublicationDate { get; set; } = null;
        public string ISBN { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string GetKey()
        {
            return BookId;
        }
    }
}
