using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FutbolApi.Models
{
    public class League
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int? SportId { get; set; }
        public int? CountryId { get; set; }
        public string? Name { get; set; }
        public bool? Active { get; set; }
        public string? ShortCode { get; set; }
        public string? ImagePath { get; set; }
        public string? Type { get; set; }
        public string? SubType { get; set; }
        public string? LastPlayedAt { get; set; }
        public int? Category { get; set; }
        public bool? HasJerseys { get; set; }
    }

    public class LeagueApiResponse
    {
        public List<League> Data { get; set; }
    }

    public class LeagueDetailApiResponse
    {
        public League Data { get; set; }
    }
}
