using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class ReferenciaRespuestas : BaseEntity
    {
        public int ReferenciaLaboralId { get; set; }
        public int TipoReferenciaId { get; set; }
        public string ReferenciaPreguntasId { get; set; }
        public string Respuesta { get; set; }
    }
}