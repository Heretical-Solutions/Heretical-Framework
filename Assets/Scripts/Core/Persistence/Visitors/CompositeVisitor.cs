using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence.Visitors
{
    public class CompositeVisitor 
        : ISaveVisitor, 
          ILoadVisitor
    {
        private readonly IReadOnlyObjectRepository loadVisitorsRepository;

        private readonly IReadOnlyObjectRepository saveVisitorsRepository;

        public CompositeVisitor(
            IReadOnlyObjectRepository loadVisitorsRepository,
            IReadOnlyObjectRepository saveVisitorsRepository)
        {
            this.loadVisitorsRepository = loadVisitorsRepository;

            this.saveVisitorsRepository = saveVisitorsRepository;
        }

        #region ISaveVisitor
        
        public bool Save<TValue>(TValue value, out object DTO)
        {
            DTO = default(object);
            
            if (!saveVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\"");
            
            var concreteVisitor = (ISaveVisitor)concreteVisitorObject;

            return concreteVisitor.Save(value, out DTO);
        }

        public bool Save<TValue, TDTO>(TValue value, out TDTO DTO)
        {
            DTO = default(TDTO);
            
            if (!saveVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\" AND DTO TYPE \"{typeof(TDTO).ToString()}\"");
            
            var concreteVisitor = (ISaveVisitorGeneric<TValue, TDTO>)concreteVisitorObject;

            return concreteVisitor.Save(value, out DTO);
        }
        
        #endregion

        #region ILoadVisitor

        public bool Load<TValue>(object DTO, out TValue value)
        {
            value = default(TValue);
            
            if (!loadVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\"");
            
            var concreteVisitor = (ILoadVisitor)concreteVisitorObject;

            return concreteVisitor.Load(DTO, out value);
        }

        public bool Load<TValue, TDTO>(TDTO DTO, out TValue value)
        {
            value = default(TValue);
            
            if (!loadVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\" AND DTO TYPE \"{typeof(TDTO).ToString()}\"");
            
            var concreteVisitor = (ILoadVisitorGeneric<TValue, TDTO>)concreteVisitorObject;

            return concreteVisitor.Load(DTO, out value);
        }

        public bool Load<TValue>(object DTO, TValue valueToPopulate)
        {
            if (!loadVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\"");
            
            var concreteVisitor = (ILoadVisitor)concreteVisitorObject;

            return concreteVisitor.Load(DTO, valueToPopulate);
        }

        public bool Load<TValue, TDTO>(TDTO DTO, TValue valueToPopulate)
        {
            if (!loadVisitorsRepository.TryGet(typeof(TValue), out object concreteVisitorObject))
                throw new Exception($"[CompositeVisitor] COULD NOT FIND CONCRETE VISITOR FOR VALUE TYPE \"{typeof(TValue).ToString()}\" AND DTO TYPE \"{typeof(TDTO).ToString()}\"");
            
            var concreteVisitor = (ILoadVisitorGeneric<TValue, TDTO>)concreteVisitorObject;

            return concreteVisitor.Load(DTO, valueToPopulate);
        }

        #endregion
    }
}