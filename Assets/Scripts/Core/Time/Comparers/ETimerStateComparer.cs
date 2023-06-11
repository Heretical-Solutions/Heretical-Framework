using System.Collections.Generic;

namespace HereticalSolutions.Time
{
    public struct ETimerStateComparer : IEqualityComparer<ETimerState>
    {
        public bool Equals(ETimerState stateA, ETimerState stateB)
        {
            return stateA == stateB;
        }

        public int GetHashCode(ETimerState state)
        {
            // you need to do some thinking here,
            return (int)state;
        }
    }
}