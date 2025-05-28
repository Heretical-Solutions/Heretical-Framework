using System;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Builders;

using HereticalSolutions.Logging.Factories;

namespace HereticalSolutions.Logging.Builders
{
	public class LoggerBuilder
		: ABuilder<LoggerBuilderContext>
	{
		private readonly RepositoryFactory repositoryFactory;

		private readonly LoggerFactory loggerFactory;

		public LoggerBuilder(
			RepositoryFactory repositoryFactory,
			LoggerFactory loggerFactory)
			: base()
		{
			this.repositoryFactory = repositoryFactory;

			this.loggerFactory = loggerFactory;
		}

		public LoggerBuilder New()
		{
			context = new LoggerBuilderContext
			{
				ExplicitLogSourceRules = repositoryFactory
					.BuildDictionaryRepository<Type, bool>(),
	
				RootLogger = null,
	
				CurrentLogger = null,
	
				AllowedByDefault = true
			};

			return this;
		}


		public LoggerBuilder ToggleAllowedByDefault(
			bool allowed)
		{
			context.AllowedByDefault = allowed;

			return this;
		}

		public LoggerBuilder ToggleLogSource<TLogSource>(
			bool allowed)
		{
			context.ExplicitLogSourceRules.AddOrUpdate(
				typeof(TLogSource),
				allowed);

			return this;
		}

		public LoggerBuilder ToggleLogSource(
			Type logSourceType,
			bool allowed)
		{
			context.ExplicitLogSourceRules.AddOrUpdate(
				logSourceType,
				allowed);

			return this;
		}


		public LoggerBuilder AddSink(
			ILoggerSink loggerSink)
		{
			DecorateInner(
				loggerSink);

			return this;
		}

		public LoggerBuilder Decorate(
			ILoggerWrapper loggerWrapper)
		{
			DecorateInner(
				loggerWrapper);

			return this;
		}

		public LoggerBuilder Branch()
		{
			var compositeLogger = loggerFactory.BuildCompositeLoggerWrapper();

			DecorateInner(
				compositeLogger);

			return this;
		}

		private void DecorateInner(
			ILogger logger)
		{
			if (context.CurrentLogger is ICompositeLoggerWrapper compositeLogger)
			{
				compositeLogger
					.InnerLoggers
					.Add(
						logger);
			}
			else if (context.CurrentLogger is ILoggerWrapper currentLoggerWrapper)
			{
				currentLoggerWrapper.InnerLogger = logger;
			}

			context.CurrentLogger = logger;

			if (context.RootLogger == null)
			{
				context.RootLogger = logger;
			}
		}


		public LoggerResolver Build()
		{
			var result = loggerFactory.BuildLoggerResolver(
				context.RootLogger,
				context.ExplicitLogSourceRules,
				context.AllowedByDefault);

			Cleanup();

			return result;
		}
	}
}