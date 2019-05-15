using System;

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
	using AtomicOperationInvocation = org.camunda.bpm.engine.impl.interceptor.AtomicOperationInvocation;
	using AsyncContinuationConfiguration = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler.AsyncContinuationConfiguration;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using PvmActivity = org.camunda.bpm.engine.impl.pvm.PvmActivity;
	using AtomicOperation = org.camunda.bpm.engine.impl.pvm.runtime.AtomicOperation;

	/// <summary>
	/// <para>Declaration of a Message Job (Asynchronous continuation job)</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public class MessageJobDeclaration : JobDeclaration<AtomicOperationInvocation, MessageEntity>
	{

	  public const string ASYNC_BEFORE = "async-before";
	  public const string ASYNC_AFTER = "async-after";

	  private const long serialVersionUID = 1L;

	  protected internal string[] operationIdentifier;

	  public MessageJobDeclaration(string[] operationsIdentifier) : base(AsyncContinuationJobHandler.TYPE)
	  {
		this.operationIdentifier = operationsIdentifier;
	  }

	  protected internal override MessageEntity newJobInstance(AtomicOperationInvocation context)
	  {
		MessageEntity message = new MessageEntity();
		message.Execution = context.Execution;

		return message;
	  }

	  public virtual bool isApplicableForOperation(AtomicOperation operation)
	  {
		foreach (string identifier in operationIdentifier)
		{
		  if (operation.CanonicalName.Equals(identifier))
		  {
			return true;
		  }
		}
		return false;
	  }

	  protected internal virtual ExecutionEntity resolveExecution(AtomicOperationInvocation context)
	  {
		return context.Execution;
	  }

	  protected internal override JobHandlerConfiguration resolveJobHandlerConfiguration(AtomicOperationInvocation context)
	  {
		AsyncContinuationConfiguration configuration = new AsyncContinuationConfiguration();

		configuration.AtomicOperation = context.Operation.CanonicalName;

		ExecutionEntity execution = context.Execution;
		PvmActivity activity = execution.getActivity();
		if (activity != null && activity.AsyncAfter)
		{
		  if (execution.Transition != null)
		  {
			// store id of selected transition in case this is async after.
			// id is not serialized with the execution -> we need to remember it as
			// job handler configuration.
			configuration.TransitionId = execution.Transition.Id;
		  }
		}

		return configuration;
	  }

	}

}