using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ReferenciaPreguntas : BaseEntity
    {
        public int TipoReferenciaId { get; set; }
        public string Pregunta { get; set; }
    }
}