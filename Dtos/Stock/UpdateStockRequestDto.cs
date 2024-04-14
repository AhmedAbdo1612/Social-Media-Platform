using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Dtos.Stock
{
    public class UpdateStockRequestDto
    {

        [Required]
        [MaxLength(15,ErrorMessage ="Symbol Cannot be over 15 characters")]
        public string Symbol {get;set;} = string.Empty;
        
        [Required]
        [MaxLength(15,ErrorMessage ="Company Name Cannot be over 15 characters")]
        public string CompanyName {get; set;} = string.Empty;

        [Required]
        [Range(1,100000000)]
        public decimal Purchase {get;set;}

        [Required]
        [Range(0.001,100)]
        public decimal LastDiv {get;set;}

        [Required]
        [MaxLength(15,ErrorMessage ="Industry cannot be over 15 characters")]
        public string Industry {get;set;} = string.Empty;
        
        [Required]
        [Range(1,100000000)]
        public long MarketCap {get;set;}

    }
}