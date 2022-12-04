﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Persistance.Entities.Result
{
    [Table("PowerFlowResult")]
    public class PowerFlowResultEntity : CalculationResultBaseEntity
    {
        /// <summary>
        /// Значение напряжения
        /// </summary>
        public double PowerFlowLimit { get; set; }
    }
}
