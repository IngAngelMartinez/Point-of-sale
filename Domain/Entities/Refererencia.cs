using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Refererencia : BaseEntity
    {
        public int CandidatoId { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; } 
        public string ApellidoMaterno { get; set; }
        public string NombreEmpresa { get; set; }
        public string PuestoACargo { get; set; }
        public string Telefono { get; set; }
    }
}