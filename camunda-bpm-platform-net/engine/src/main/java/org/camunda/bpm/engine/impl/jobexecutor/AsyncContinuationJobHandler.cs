using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.impl.jobexecutor
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AsyncContinuationConfiguration = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler.AsyncContinuationConfiguration;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using TransitionImpl = org.camunda.bpm.engine.impl.pvm.process.TransitionImpl;
	using LegacyBehavior = org.camunda.bpm.engine.impl.pvm.runtime.LegacyBehavior;
	using PvmAtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// @author Thorben Lindhauer
	/// </summary>
	public class AsyncContinuationJobHandler : JobHandler<AsyncContinuationConfiguration>
	{

	  public const string TYPE = "async-continuation";

	  private IDictionary<string, PvmAtomicOperation> supportedOperations;

	  public AsyncContinuationJobHandler()
	  {
		supportedOperations = new Dictionary<string, PvmAtomicOperation>();
		// async before activity
		supportedOperations[org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_CREATE_SCOPE.CanonicalName] = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_CREATE_SCOPE;
		supportedOperations[org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CREATE_SCOPE.CanonicalName] = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_START_CREATE_SCOPE;
		// async before start event
		supportedOperations[org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.PROCESS_START.CanonicalName] = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.PROCESS_START;

		// async after activity depending if an outgoing sequence flow exists
		supportedOperations[org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_TAKE.CanonicalName] = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_NOTIFY_LISTENER_TAKE;
		supportedOperations[org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_END.CanonicalName] = org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.ACTIVITY_END;
	  }

	  public virtual string Type
	  {
		  get
		  {
			return TYPE;
		  }
	  }

	  public virtual void execute(AsyncContinuationConfiguration configuration, ExecutionEntity execution, CommandContext commandContext, string tenantId)
	  {

		LegacyBehavior.repairMultiInstanceAsyncJob(execution);

		PvmAtomicOperation atomicOperation = findMatchingAtomicOperation(configuration.AtomicOperation);
		ensureNotNull("Cannot process job with configuration " + configuration, "atomicOperation", atomicOperation);

		// reset transition id.
		string transitionId = configuration.TransitionId;
		if (!string.ReferenceEquals(transitionId, null))
		{
		  PvmActivity activity = execution.getActivity();
		  TransitionImpl transition = (TransitionImpl) activity.findOutgoingTransition(transitionId);
		  execution.Transition = transition;
		}

		Context.CommandInvocationContext.performOperation(atomicOperation, execution);
	  }

	  public virtual PvmAtomicOperation findMatchingAtomicOperation(string operationName)
	  {
		if (string.ReferenceEquals(operationName, null))
		{
		  // default operation for backwards compatibility
		  return org.camunda.bpm.engine.impl.pvm.runtime.operation.PvmAtomicOperation_Fields.TRANSITION_CREATE_SCOPE;
		}
		else
		{
		  return supportedOperations[operationName];
		}
	  }

	  protected internal virtual bool isSupported(PvmAtomicOperation atomicOperation)
	  {
		return supportedOperations.ContainsKey(atomicOperation.CanonicalName);
	  }

	  public virtual AsyncContinuationConfiguration newConfiguration(string canonicalString)
	  {
		string[] configParts = tokenizeJobConfiguration(canonicalString);

		AsyncContinuationConfiguration configuration = new AsyncContinuationConfiguration();

		configuration.AtomicOperation = configParts[0];
		configuration.TransitionId = configParts[1];

		return configuration;
	  }

	  /// <returns> an array of length two with the following contents:
	  /// <ul><li>First element: pvm atomic operation name
	  /// <li>Second element: transition id (may be null) </returns>
	  protected internal virtual string[] tokenizeJobConfiguration(string jobConfiguration)
	  {

		string[] configuration = new string[2];

		if (!string.ReferenceEquals(jobConfiguration, null))
		{
		  string[] configParts = jobConfiguration.Split("\\$", true);
		  if (configuration.Length > 2)
		  {
			throw new ProcessEngineException("Illegal async continuation job handler configuration: '" + jobConfiguration + "': exprecting one part or two parts seperated by '$'.");
		  }
		  configuration[0] = configParts[0];
		  if (configParts.Length == 2)
		  {
			configuration[1] = configParts[1];
		  }
		}

		return configuration;
	  }

	  public class AsyncContinuationConfiguration : JobHandlerConfiguration
	  {

		protected internal string atomicOperation;
		protected internal string transitionId;

		public virtual string AtomicOperation
		{
			get
			{
			  return atomicOperation;
			}
			set
			{
			  this.atomicOperation = value;
			}
		}


		public virtual string TransitionId
		{
			get
			{
			  return transitionId;
			}
			set
			{
			  this.transitionId = value;
			}
		}


		public virtual string toCanonicalString()
		{
		  string configuration = atomicOperation;

		  if (!string.ReferenceEquals(transitionId, null))
		  {
			// store id of selected transition in case this is async after.
			// id is not serialized with the execution -> we need to remember it as
			// job handler configuration.
			configuration += "$" + transitionId;
		  }

		  return configuration;
		}

	  }

	  public virtual void onDelete(AsyncContinuationConfiguration configuration, JobEntity jobEntity)
	  {
		// do nothing
	  }
	}

}