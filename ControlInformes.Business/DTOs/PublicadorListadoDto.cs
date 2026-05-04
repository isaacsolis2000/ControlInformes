using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlInformes.Business.DTOs
{
    public class PublicadorListadoDto
    {
        public Guid IdPublicador { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public Guid? IdGrupo { get; set; }
        public string NombreGrupo { get; set; } = string.Empty;
    }
}
