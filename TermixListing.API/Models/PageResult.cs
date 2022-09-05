﻿namespace TermixListing.API.Models
{
    public class PageResult<T>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int TotalPage { get; set; }
        public int RecordNumber { get; set; }
        public List<T> Items { get; set; }
    }
}