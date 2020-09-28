using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class TipoDocumentacion : BaseEntity
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}