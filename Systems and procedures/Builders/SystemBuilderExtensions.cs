using System.Collections.Generic;

namespace HereticalSolutions.Systems.Builders
{
	public static class SystemBuilderExtensions
	{
		public static string GetStageStartNodeID(string stageID)
		{
			return $"{stageID} {SystemConsts.STAGE_START_PREFIX}";
		}

		public static string GetStageFinishNodeID(string stageID)
		{
			return $"{stageID} {SystemConsts.STAGE_FINISH_POSTFIX}";
		}

		public static void AddStageNodesBetweenStartAndFinish<
				TContext, 
				TSystem, 
				TProcedure>(
			this ISystemBuilder<TContext, TSystem, TProcedure> systemBuilder,
			string stageID)
			where TContext : ASystemBuilderContext<TSystem, TProcedure>
		{
			var context = systemBuilder.Context;

			systemBuilder.TryAddAfterNode(
				systemBuilder.StartNode,
				context.SystemFactory.BuildStageNode<TProcedure>(
					GetStageStartNodeID(stageID)),
				false);

			systemBuilder.TryAddBeforeNode(
				systemBuilder.FinishNode,
				context.SystemFactory.BuildStageNode<TProcedure>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesAfterStage<TContext, TSystem, TProcedure>(
			this ISystemBuilder<TContext, TSystem, TProcedure> systemBuilder,
			string stageID,
			string predecessorStageID,
			bool parallel)
			where TContext : ASystemBuilderContext<TSystem, TProcedure>
		{
			var context = systemBuilder.Context;

			var stageStartNode = context.SystemFactory.BuildStageNode<TProcedure>(
				GetStageStartNodeID(stageID));

			systemBuilder.TryAddAfterStage(
				GetStageFinishNodeID(predecessorStageID),
				stageStartNode,
				parallel);

			systemBuilder.TryAddAfterNode(
				stageStartNode,
				context.SystemFactory.BuildStageNode<TProcedure>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesAfterStages<TContext, TSystem, TProcedure>(
			this ISystemBuilder<TContext, TSystem, TProcedure> systemBuilder,
			string stageID,
			IEnumerable<string> predecessorStageIDs,
			bool parallel)
			where TContext : ASystemBuilderContext<TSystem, TProcedure>
		{
			var context = systemBuilder.Context;

			List<string> predecessorStageFinishNodes = new List<string>();

			foreach (var predecessorStageID in predecessorStageIDs)
			{
				predecessorStageFinishNodes.Add(
					GetStageFinishNodeID(predecessorStageID));
			}

			var stageStartNode = context.SystemFactory.BuildStageNode<TProcedure>(
				GetStageStartNodeID(stageID));

			systemBuilder.TryAddAfterStages(
				predecessorStageFinishNodes,
				stageStartNode,
				parallel);

			systemBuilder.TryAddAfterNode(
				stageStartNode,
				context.SystemFactory.BuildStageNode<TProcedure>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesAfterStageStart<TContext, TSystem, TProcedure>(
			this ISystemBuilder<TContext, TSystem, TProcedure> systemBuilder,
			string stageID,
			string predecessorStageID,
			bool parallel)
			where TContext : ASystemBuilderContext<TSystem, TProcedure>
		{
			var context = systemBuilder.Context;

			var stageStartNode = context.SystemFactory.BuildStageNode<TProcedure>(
				GetStageStartNodeID(stageID));

			systemBuilder.TryAddAfterStage(
				GetStageStartNodeID(predecessorStageID),
				stageStartNode,
				parallel);

			systemBuilder.TryAddAfterNode(
				stageStartNode,
				context.SystemFactory.BuildStageNode<TProcedure>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesBeforeStageFinish<TContext, TSystem, TProcedure>(
			this ISystemBuilder<TContext, TSystem, TProcedure> systemBuilder,
			string stageID,
			string predecessorStageID,
			bool parallel)
			where TContext : ASystemBuilderContext<TSystem, TProcedure>
		{
			var context = systemBuilder.Context;

			var stageFinishNode = context.SystemFactory.BuildStageNode<TProcedure>(
				GetStageFinishNodeID(stageID));

			systemBuilder.TryAddBeforeStage(
				GetStageFinishNodeID(predecessorStageID),
				stageFinishNode,
				parallel);

			systemBuilder.TryAddBeforeNode(
				stageFinishNode,
				context.SystemFactory.BuildStageNode<TProcedure>(
					GetStageStartNodeID(stageID)),
				false);
		}
	}
}