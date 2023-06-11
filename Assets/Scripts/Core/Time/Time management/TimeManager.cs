using HereticalSolutions.Repositories;

namespace HereticalSolutions.Time
{
    public class TimeManager
    {
        private readonly IRepository<string, ISynchronizable> contextsRepository;

        public TimeManager(IRepository<string, ISynchronizable> contextsRepository)
        {
            this.contextsRepository = contextsRepository;
        }

        public ISynchronizable GetSynchronizable(string id)
        {
            //There may not be the synchronizable by the given id therefore TryGet
            if (!contextsRepository.TryGet(id, out var synchronizable))
                return null;

            return synchronizable;
        }
        
        public ISynchronizationProvider GetProvider(string id)
        {
            //There may not be the synchronizable by the given id therefore TryGet
            if (!contextsRepository.TryGet(id, out var synchronizable))
                return null;
            
            return (ISynchronizationProvider)synchronizable;
        }
        
        public void AddSynchronizable(string id, ISynchronizable context)
        {
            contextsRepository.Add(id, context);
        }

        public void RemoveSynchronizable(string id)
        {
            contextsRepository.Remove(id);
        }
    }
}