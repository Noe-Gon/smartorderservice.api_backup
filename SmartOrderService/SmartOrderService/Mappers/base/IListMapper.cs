using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Mappers
{
    public interface IListMapper<M, E> 
    {
        List<M> toModelList(List<E> entities);
        List<E> toEntitiesList(List<M> models);
    }
}