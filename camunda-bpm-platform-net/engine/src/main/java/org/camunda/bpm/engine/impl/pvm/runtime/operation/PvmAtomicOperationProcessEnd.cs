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
namespace org.camunda.bpm.engine.impl.pvm.runtime.operation
{
	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using TransferVariablesActivityBehavior = org.camunda.bpm.engine.impl.cmmn.behavior.TransferVariablesActivityBehavior;
	using CmmnActivityExecution = org.camunda.bpm.engine.impl.cmmn.execution.CmmnActivityExecution;
	using CmmnActivity = org.camunda.bpm.engine.impl.cmmn.model.CmmnActivity;
	using SubProcessActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.SubProcessActivityBehavior;
	using ScopeImpl = org.camunda.bpm.engine.impl.pvm.process.ScopeImpl;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class PvmAtomicOperationProcessEnd : PvmAtomicOperationActivityInstanceEnd
	{

	  private static readonly PvmLogger LOG = PvmLogger.PVM_LOGGER;

	  protected internal override ScopeImpl getScope(PvmExecutionImpl execution)
	  {
		return execution.ProcessDefinition;
	  }

	  protected internal override string EventName
	  {
		  get
		  {
			return org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END;
		  }
	  }

	  protected internal override void eventNotificationsCompleted(PvmExecutionImpl execution)
	  {

		execution.leaveActivityInstance();

		PvmExecutionImpl superExecution = execution.SuperExecution;
		CmmnActivityExecution superCaseExecution = execution.SuperCaseExecution;

		SubProcessActivityBehavior subProcessActivityBehavior = null;
		TransferVariablesActivityBehavior transferVariablesBehavior = null;

		// copy variables before destroying the ended sub process instance
		if (superExecution != null)
		{
		  PvmActivity activity = superExecution.getActivity();
		  subProcessActivityBehavior = (SubProcessActivityBehavior) activity.ActivityBehavior;
		  try
		  {
			subProcessActivityBehavior.passOutputVariables(superExecution, execution);
		  }
		  catch (Exception e)
		  {
			LOG.exceptionWhileCompletingSupProcess(execution, e);
			throw e;
		  }
		  catch (Exception e)
		  {
			LOG.exceptionWhileCompletingSupProcess(execution, e);
			throw new ProcessEngineException("Error while completing sub process of execution " + execution, e);
		  }
		}
		else if (superCaseExecution != null)
		{
		  CmmnActivity activity = superCaseExecution.Activity;
		  transferVariablesBehavior = (TransferVariablesActivityBehavior) activity.ActivityBehavior;
		  try
		  {
			transferVariablesBehavior.transferVariables(execution, superCaseExecution);
		  }
		  catch (Exception e)
		  {
			LOG.exceptionWhileCompletingSupProcess(execution, e);
			throw e;
		  }
		  catch (Exception e)
		  {
			LOG.exceptionWhileCompletingSupProcess(execution, e);
			throw new ProcessEngineException("Error while completing sub process of execution " + execution, e);
		  }
		}

		execution.destroy();
		execution.remove();

		// and trigger execution afterwards
		if (superExecution != null)
		{
		  superExecution.SubProcessInstance = null;
		  try
		  {
			subProcessActivityBehavior.completed(superExecution);
		  }
		  catch (Exception e)
		  {
			LOG.exceptionWhileCompletingSupProcess(execution, e);
			throw e;
		  }
		  catch (Exception e)
		  {
			LOG.exceptionWhileCompletingSupProcess(execution, e);
			throw new ProcessEngineException("Error while completing sub process of execution " + execution, e);
		  }
		}
		else if (superCaseExecution != null)
		{
		  superCaseExecution.complete();
		}
	  }

	  public override string CanonicalName
	  {
		  get
		  {
			return "process-end";
		  }
	  }
	}

}