using System;

namespace HereticalSolutions.Persistence
{
    public interface IVisitable
    {
        Type DTOType { get; }

        bool Accept<TDTO>(ISaveVisitor visitor, out TDTO DTO);
        
        bool Accept(ISaveVisitor visitor, out object DTO);
        
        bool Accept<TDTO>(ILoadVisitor visitor, TDTO DTO);
        
        bool Accept(ILoadVisitor visitor, object DTO);
    }
}