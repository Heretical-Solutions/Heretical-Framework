using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Systems.Factories
{
	public class SystemFactory
	{
		private readonly RepositoryFactory repositoryFactory;

		public SystemFactory(
			RepositoryFactory repositoryFactory)
		{
			this.repositoryFactory = repositoryFactory;
		}

		public ProcedureNode<TProcedure>
			BuildProcedureNode<TProcedure>(
				TProcedure system,
				sbyte priority = 0)
		{
			return new ProcedureNode<TProcedure>(
				system,
				priority);
		}

		public StageNode<TProcedure> BuildStageNode<TProcedure>(
			string stage,
			TProcedure system = default(TProcedure),
			sbyte priority = 0)
		{
			return new StageNode<TProcedure>(
				stage,
				system,
				priority);
		}

		public void PrepareSystemBuilderDependencies<TSystem, TProcedure>(
			out HashSet<IProcedureNode<TProcedure>> allProcedureNodes,
			out IRepository<string, IStageNode<TProcedure>> stages,
			out IRepository<Type, IList<IProcedureNode<TProcedure>>> procedures,

			out IStageNode<TProcedure> startNode,
			out IStageNode<TProcedure> finishNode)
		{
			startNode = BuildStageNode<TProcedure>(
				SystemConsts.START_NODE_ID,
				default(TProcedure));

			((StageNode<TProcedure>)startNode).IsDetached = false;

			((StageNode<TProcedure>)startNode).ExpectedThread = 1;

			finishNode = BuildStageNode<TProcedure>(
				SystemConsts.FINISH_NODE_ID,
				default(TProcedure));

			((StageNode<TProcedure>)finishNode).IsDetached = false;

			((StageNode<TProcedure>)finishNode).ExpectedThread = 1;
			

			allProcedureNodes =
				new HashSet<IProcedureNode<TProcedure>>();

			stages =
				repositoryFactory.BuildDictionaryRepository<string, IStageNode<TProcedure>>();

			procedures =
				repositoryFactory.BuildDictionaryRepository<Type, IList<IProcedureNode<TProcedure>>>();


			((StageNode<TProcedure>)startNode).SequentialNext = finishNode;

			((StageNode<TProcedure>)finishNode).SequentialPrevious = startNode;


			allProcedureNodes.Add(startNode
				as IProcedureNode<TProcedure>);

			allProcedureNodes.Add(finishNode
				as IProcedureNode<TProcedure>);


			stages.Add(
				startNode.Stage,
				startNode);

			stages.Add(
				finishNode.Stage,
				finishNode);
		}
	}
}