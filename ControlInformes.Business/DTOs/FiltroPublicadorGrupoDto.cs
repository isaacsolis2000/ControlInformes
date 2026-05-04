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
        public int? Tipo { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanoPagina { get; set; } = 20;
    }
}
