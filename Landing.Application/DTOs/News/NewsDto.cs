﻿namespace Landing.Application.DTOs.News
{

    public class NewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime PublishedAt { get; set; }


    }

}
