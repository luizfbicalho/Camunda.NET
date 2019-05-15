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
namespace org.camunda.bpm.engine.impl.cmmn.operation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.@delegate.CaseExecutionListener_Fields.COMPLETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.cmmn.execution.CaseExecutionState_Fields.COMPLETED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.ActivityBehaviorUtil.getActivityBehavior;

	using CmmnActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnActivityBehavior;
	using CmmnCompositeActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.CmmnCompositeActivityBehavior;
	using TransferVariablesActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.TransferVariablesActivityBehavior;
	using CmmnExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnExecution;
	using SubProcessActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.SubProcessActivityBehavior;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class AbstractAtomicOperationCaseExecutionComplete : AbstractCmmnEventAtomicOperation
	{

	  protected internal static readonly CmmnOperationLogger LOG = ProcessEngineLogger.CMMN_OPERATION_LOGGER;

	  protected internal override string EventName
	  {
		  get
		  {
			return COMPLETE;
		  }
	  }

	  protected internal virtual CmmnExecution eventNotificationsStarted(CmmnExecution execution)
	  {
		CmmnActivityBehavior behavior = getActivityBehavior(execution);
		triggerBehavior(behavior, execution);

		execution.CurrentState = COMPLETED;

		return execution;
	  }

	  protected internal override void postTransitionNotification(CmmnExecution execution)
	  {
		if (!execution.CaseInstanceExecution)
		{
		  execution.remove();

		}
		else
		{
		  CmmnExecution superCaseExecution = execution.SuperCaseExecution;
		  PvmExecutionImpl superExecution = execution.SuperExecution;

		  if (superCaseExecution != null)
		  {
			TransferVariablesActivityBehavior behavior = (TransferVariablesActivityBehavior) getActivityBehavior(superCaseExecution);
			behavior.transferVariables(execution, superCaseExecution);
			superCaseExecution.complete();

		  }
		  else if (superExecution != null)
		  {
			SubProcessActivityBehavior behavior = (SubProcessActivityBehavior) getActivityBehavior(superExecution);

			try
			{
			  behavior.passOutputVariables(superExecution, execution);
			}
			catch (Exception e)
			{
			  LOG.completingSubCaseError(execution, e);
			  throw e;
			}
			catch (Exception e)
			{
			  LOG.completingSubCaseError(execution, e);
			  throw LOG.completingSubCaseErrorException(execution, e);
			}

			// set sub case instance to null
			superExecution.SubCaseInstance = null;

			try
			{
			  behavior.completed(superExecution);
			}
			catch (Exception e)
			{
			  LOG.completingSubCaseError(execution, e);
			  throw e;
			}
			catch (Exception e)
			{
			  LOG.completingSubCaseError(execution, e);
			  throw LOG.completingSubCaseErrorException(execution, e);
			}
		  }

		  execution.SuperCaseExecution = null;
		  execution.SuperExecution = null;
		}

		CmmnExecution parent = execution.Parent;
		if (parent != null)
		{
		  CmmnActivityBehavior behavior = getActivityBehavior(parent);
		  if (behavior is CmmnCompositeActivityBehavior)
		  {
			CmmnCompositeActivityBehavior compositeBehavior = (CmmnCompositeActivityBehavior) behavior;
			compositeBehavior.handleChildCompletion(parent, execution);
		  }
		}

	  }

	  protected internal abstract void triggerBehavior(CmmnActivityBehavior behavior, CmmnExecution execution);

	}

}