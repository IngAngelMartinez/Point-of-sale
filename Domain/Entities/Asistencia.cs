using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Asistencia : BaseEntity
    {
        public Guid UsuarioId { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public TimeSpan Hora { get; set; } = DateTime.Now.TimeOfDay;
        public int TipoAsistencia { get; set; }
    }
}
