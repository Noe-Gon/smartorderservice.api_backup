using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartOrderService.Mappers
{
    public interface IMapper<M,E>
    {
         M toModel(E entity);
        E toEntity(M model);
    }
}
