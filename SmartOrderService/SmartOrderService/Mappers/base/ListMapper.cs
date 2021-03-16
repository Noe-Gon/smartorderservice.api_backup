using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartOrderService.Mappers{
    public class ListMapper<M, E> : IListMapper<M, E>
    {
        private IMapper<M,E> mapper;

        public ListMapper(IMapper<M,E> mapper) {
            this.mapper = mapper;
        }

        public List<E> toEntitiesList(List<M> models)
        {
            List<E> entities = new List<E>();


            if(models!=null)
            foreach (M m in models) {
                E e = mapper.toEntity(m);
                entities.Add(e);
            }

            return entities;
        }

        public List<M> toModelList(List<E> entities)
        {
            List<M> models = new List<M>();

            if (models != null)
                foreach (E e in entities) {
                M m = mapper.toModel(e);
                models.Add(m);
            }

            return models;
        }
    }
}