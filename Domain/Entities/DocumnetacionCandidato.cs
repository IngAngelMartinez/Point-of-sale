using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class DocumentacionCandidato : BaseEntity
    {
        public int CandidatoId { get; set; }
        public int TipoDocumentacionId { get; set; }
        public string RutaDocumento { get; set; }
    }
}
