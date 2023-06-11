using System;
using HereticalSolutions.Persistence;

using HereticalSolutions.Time.Factories;

namespace HereticalSolutions.Time.Visitors
{
    public class RuntimeTimerVisitor
        : ILoadVisitorGeneric<IRuntimeTimer, RuntimeTimerDTO>,
          ILoadVisitor,
          ISaveVisitorGeneric<IRuntimeTimer, RuntimeTimerDTO>,
          ISaveVisitor
    {
        #region ILoadVisitorGeneric
        
        public bool Load(
            RuntimeTimerDTO DTO,
            out IRuntimeTimer value)
        {
            value = TimeFactory.BuildRuntimeTimer(
                DTO.ID,
                DTO.DefaultDuration);
            
            ((ITimerWithState)value).SetState(DTO.State);

            ((IRuntimeTimerContext)value).CurrentTimeElapsed = DTO.CurrentTimeElapsed;

            ((IRuntimeTimerContext)value).CurrentDuration = DTO.CurrentDuration;
            
            value.Accumulate = DTO.Accumulate;

            value.Repeat = DTO.Repeat;

            return true;
        }

        public bool Load(RuntimeTimerDTO DTO, IRuntimeTimer valueToPopulate)
        {
            ((ITimerWithState)valueToPopulate).SetState(DTO.State);

            ((IRuntimeTimerContext)valueToPopulate).CurrentTimeElapsed = DTO.CurrentTimeElapsed;

            ((IRuntimeTimerContext)valueToPopulate).CurrentDuration = DTO.CurrentDuration;
            
            valueToPopulate.Accumulate = DTO.Accumulate;

            valueToPopulate.Repeat = DTO.Repeat;

            return true;
        }

        #endregion

        #region ILoadVisitor

        public bool Load<TValue>(object DTO, out TValue value)
        {
            if (!(DTO.GetType().Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{DTO.GetType().ToString()}\"");
            
            bool result = Load((RuntimeTimerDTO)DTO, out IRuntimeTimer returnValue);

            value = result
                ? (TValue)returnValue
                : default(TValue);

            return result;
        }

        public bool Load<TValue, TDTO>(TDTO DTO, out TValue value)
        {
            if (!(typeof(TDTO).Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");
            
            //DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)DTO;
            
            bool result = Load((RuntimeTimerDTO)dtoObject, out IRuntimeTimer returnValue);

            value = result
                ? (TValue)returnValue
                : default(TValue);

            return result;
        }

        public bool Load<TValue>(object DTO, TValue valueToPopulate)
        {
            if (!(DTO.GetType().Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{DTO.GetType().ToString()}\"");
            
            return Load((RuntimeTimerDTO)DTO, (IRuntimeTimer)valueToPopulate);
        }

        public bool Load<TValue, TDTO>(TDTO DTO, TValue valueToPopulate)
        {
            if (!(typeof(TDTO).Equals(typeof(RuntimeTimerDTO))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(RuntimeTimerDTO).ToString()}\" RECEIVED: \"{typeof(TDTO).ToString()}\"");
            
            //DIRTY HACKS DO NOT REPEAT
            var dtoObject = (object)DTO;
            
            return Load((RuntimeTimerDTO)dtoObject, (IRuntimeTimer)valueToPopulate);
        }

        #endregion

        #region ISaveVisitorGeneric

        public bool Save(
            IRuntimeTimer value,
            out RuntimeTimerDTO DTO)
        {
            DTO = new RuntimeTimerDTO
            {
                ID = value.ID,
                State = value.State,
                CurrentTimeElapsed = ((IRuntimeTimerContext)value).CurrentTimeElapsed,
                Accumulate = value.Accumulate,
                Repeat = value.Repeat,
                CurrentDuration = value.CurrentDuration,
                DefaultDuration = value.DefaultDuration
            };

            return true;
        }
        
        #endregion

        #region ISaveVisitor

        public bool Save<TValue>(TValue value, out object DTO)
        {
            if (!(typeof(IRuntimeTimer).IsAssignableFrom(typeof(TValue))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(IRuntimeTimer).ToString()}\" RECEIVED: \"{typeof(TValue).ToString()}\"");
            
            bool result = Save((IRuntimeTimer)value, out RuntimeTimerDTO returnDTO);

            DTO = result
                ? returnDTO
                : default(object);

            return result;
        }

        public bool Save<TValue, TDTO>(TValue value, out TDTO DTO)
        {
            if (!(typeof(IRuntimeTimer).IsAssignableFrom(typeof(TValue))))
                throw new Exception($"[RuntimeTimerVisitor] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(IRuntimeTimer).ToString()}\" RECEIVED: \"{typeof(TValue).ToString()}\"");
            
            bool result = Save((IRuntimeTimer)value, out RuntimeTimerDTO returnDTO);

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