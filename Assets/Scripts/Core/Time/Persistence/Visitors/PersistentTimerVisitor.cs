using System;

using HereticalSolutions.Persistence;

using HereticalSolutions.Time.Factories;

namespace HereticalSolutions.Time.Visitors
{
    public class PersistentTimerVisitor
        : ILoadVisitorGeneric<IPersistentTimer, PersistentTimerDTO>,
          ILoadVisitor,
          ISaveVisitorGeneric<IPersistentTimer, PersistentTimerDTO>,
          ISaveVisitor
    {
        #region ILoadVisitorGeneric
        
        public bool Load(
            PersistentTimerDTO DTO,
            out IPersistentTimer value)
        {
            value = TimeFactory.BuildPersistentTimer(
                DTO.ID,
                DTO.DefaultDurationSpan);
            
            ((ITimerWithState)value).SetState(DTO.State);

            ((IPersistentTimerContext)value).StartTime = DTO.StartTime;
            
            ((IPersistentTimerContext)value).EstimatedFinishTime = DTO.EstimatedFinishTime;
            
            ((IPersistentTimerContext)value).SavedProgress = DTO.SavedProgress;

            ((IPersistentTimerContext)value).CurrentDurationSpan = DTO.CurrentDurationSpan;
            
            value.Accumulate = DTO.Accumulate;

            value.Repeat = DTO.Repeat;

            return true;
        }

        public bool Load(
            PersistentTimerDTO DTO,
            IPersistentTimer valueToPopulate)
        {
            ((ITimerWithState)valueToPopulate).SetState(DTO.State);

            ((IPersistentTimerContext)valueToPopulate).StartTime = DTO.StartTime;
            
            ((IPersistentTimerContext)valueToPopulate).EstimatedFinishTime = DTO.EstimatedFinishTime;
            
            ((IPersistentTimerContext)valueToPopulate).SavedProgress = DTO.SavedProgress;

            ((IPersistentTimerContext)valueToPopulate).CurrentDurationSpan = DTO.CurrentDurationSpan;
            
            valueToPopulate.Accumulate = DTO.Accumulate;

            valueToPopulate.Repeat = DTO.Repeat;

            return true;
        }

        #endregion

        #region ILoadVisitor

        public bool Load<TValue>(object DTO, out TValue value)
        {
            if (!(DTO.GetType().Equals(typeof(PersistentTimerDTO))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(PersistentTimerDTO).ToString()}\" RECEIVED: \"{DTO.GetType().ToString()}\"");
            
            bool result = Load((PersistentTimerDTO)DTO, out IPersistentTimer returnValue);

            value = result
                ? (TValue)returnValue
                : default(TValue);

            return result;
        }

        public bool Load<TValue, TDTO>(TDTO DTO, out TValue value)
        {
            if (!(typeof(TDTO).Equals(typeof(PersistentTimerDTO))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(PersistentTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");
            
            //DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)DTO;
            
            bool result = Load((PersistentTimerDTO)dtoObject, out IPersistentTimer returnValue);

            value = result
                ? (TValue)returnValue
                : default(TValue);

            return result;
        }

        public bool Load<TValue>(object DTO, TValue valueToPopulate)
        {
            if (!(DTO.GetType().Equals(typeof(PersistentTimerDTO))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(PersistentTimerDTO).ToString()}\" RECEIVED: \"{DTO.GetType().ToString()}\"");
            
            return Load((PersistentTimerDTO)DTO, (IPersistentTimer)valueToPopulate);
        }

        public bool Load<TValue, TDTO>(TDTO DTO, TValue valueToPopulate)
        {
            if (!(typeof(TDTO).Equals(typeof(PersistentTimerDTO))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(PersistentTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");
            
            //DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)DTO;
            
            return Load((PersistentTimerDTO)dtoObject, (IPersistentTimer)valueToPopulate);
        }

        #endregion

        #region ISaveVisitorGeneric

        public bool Save(
            IPersistentTimer value,
            out PersistentTimerDTO DTO)
        {
            DTO = new PersistentTimerDTO
            {
                ID = value.ID,
                State = value.State,
                StartTime = ((IPersistentTimerContext)value).StartTime,
                EstimatedFinishTime = ((IPersistentTimerContext)value).EstimatedFinishTime,
                SavedProgress = ((IPersistentTimerContext)value).SavedProgress,
                Accumulate = value.Accumulate,
                Repeat = value.Repeat,
                CurrentDurationSpan = value.CurrentDurationSpan,
                DefaultDurationSpan = value.DefaultDurationSpan
            };

            return true;
        }
        
        #endregion

        #region ISaveVisitor

        public bool Save<TValue>(TValue value, out object DTO)
        {
            if (!(typeof(IPersistentTimer).IsAssignableFrom(typeof(TValue))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(IPersistentTimer).ToString()}\" RECEIVED: \"{typeof(TValue).ToString()}\"");
            
            bool result = Save((IPersistentTimer)value, out PersistentTimerDTO returnDTO);

            DTO = result
                ? returnDTO
                : default(object);

            return result;
        }

        public bool Save<TValue, TDTO>(TValue value, out TDTO DTO)
        {
            if (!(typeof(IPersistentTimer).IsAssignableFrom(typeof(TValue))))
                throw new Exception($"[PersistentTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(IPersistentTimer).ToString()}\" RECEIVED: \"{typeof(TValue).ToString()}\"");
            
            bool result = Save((IPersistentTimer)value, out PersistentTimerDTO returnDTO);

            if (result)
            {
                //DIRTY HACKS DO NOT REPEAT
                var dtoObject = (object)returnDTO;

                DTO = (TDTO)dtoObject;
            }
            else
            {
                DTO = default(TDTO);
            }

            return result;
        }

        #endregion
    }
}