using AutoMapper;
using COLID.AppDataService.Common.DataModel;

namespace COLID.AppDataService.Tests.Unit.Builder
{
    public abstract class EntityBuilderBase<TBuilder, TEntityType, TDtoType, TIdType> where TEntityType : Entity<TIdType>, new()
    {
        protected TEntityType _entity;

        protected IMapper _mapper;

        protected abstract TBuilder SpecificBuilder { get; }

        public EntityBuilderBase()
        {
            _entity = new TEntityType();

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TEntityType, TDtoType>();
                cfg.CreateMap<TDtoType, TEntityType>();
            });
            _mapper = new Mapper(mappingConfig);
        }

        public virtual TEntityType Build()
        {
            return _entity;
        }

        public virtual TDtoType BuildDto()
        {
            var entity = Build();
            return _mapper.Map<TDtoType>(entity);
        }

        public virtual TBuilder WithId(TIdType id)
        {
            _entity.Id = id;
            return SpecificBuilder;
        }
    }
}
