﻿namespace Cvijecara_Sanja_Tica_IT80_2019.Entities
{
    public class PakovanjeConfirmation
    {
        public int PakovanjeId { get; set; }

        public string Vrsta { get; set; } = null!;

        public decimal Cijena { get; set; }

        public string Valuta { get; set; } = null!;

    }
}
