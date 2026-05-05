using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlInformes.Business.DTOs
{
    public class FiltroPublicadorGrupoDto
    {
        public Guid? IdGrupo { get; set; }
        public Guid? IdPublicador { get; set; }
        public string? NombreCompleto { get; set; }  // ← nuevo
        public int? Tipo { get; set; }
        public bool? Inactivo { get; set; }           // ← nuevo
        public int Pagina { get; set; } = 1;
        public int TamanoPagina { get; set; } = 20;
    }
}
