using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class EstadoCandidato : BaseEntity
    {
        public int CandidatoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaTermino { get; set; }
        public int ProcesoId { get; set; }
    }
}
