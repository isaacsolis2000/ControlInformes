using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlInformes.Business.DTOs
{
    public class AsignarPublicadoresDto
    {
        public Guid IdGrupo { get; set; }
        public List<Guid> IdPublicadores { get; set; } = new();
    }
}
