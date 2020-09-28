using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Candidato : BaseEntity
    {
        public ICollection<DocumentacionCandidato> DocumentacionCandidato { get; set; }
    }
}
