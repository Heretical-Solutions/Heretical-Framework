using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Systems.Factories;

namespace HereticalSolutions.Systems.Builders
{
	public class ASystemBuilderContext<TSystem, TProcedure>
	{
		public SystemFactory SystemFactory;

		public HashSet<IProcedureNode<TProcedure>> AllProcedureNodes;

		public IRepository<string, IStageNode<TProcedure>> StageRepository;

		public IRepository<Type, IList<IProcedureNode<TProcedure>>> 
			ProcedureListByTypeRepository;

		public byte FreeThreadIndex;

		public bool Dirty;

		public bool Validated;

		public IStageNode<TProcedure> StartNode;

		public IStageNode<TProcedure> FinishNode;
	}
}