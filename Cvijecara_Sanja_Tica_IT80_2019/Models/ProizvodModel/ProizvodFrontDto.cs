﻿using System.ComponentModel.DataAnnotations;

namespace Cvijecara_Sanja_Tica_IT80_2019.Models.ProizvodModel
{
    public class ProizvodFrontDto
    {
        [Key]
        public int ProizvodId { get; set; }
        public string Naziv { get; set; } = null!;

        public decimal? Cijena { get; set; }

        public string Valuta { get; set; } = null!;

        public string? Velicina { get; set; }

        public decimal Zalihe { get; set; }
        public string Pakovanje { get; set; }

        public string Kategorija { get; set; }

        public string Vrsta { get; set; }
        public string? Slika { get; set; }
    }
}
