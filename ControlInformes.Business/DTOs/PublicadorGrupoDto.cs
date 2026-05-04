using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlInformes.Business.DTOs
{
    public class PublicadorGrupoDto
    {
        public Guid IdPublicador { get; set; }
        public string NombrePublicador { get; set; } = string.Empty;
        public int Tipo { get; set; }
        public string TipoDescripcion { get; set; } = string.Empty;
        public Guid? IdGrupo { get; set; }
        public string NombreGrupo { get; set; } = string.Empty;
        public bool EsCapitan { get; set; }
    }
}
