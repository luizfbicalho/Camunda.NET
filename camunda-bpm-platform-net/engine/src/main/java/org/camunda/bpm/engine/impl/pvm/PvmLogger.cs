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
namespace org.camunda.bpm.engine.impl.pvm
{
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using PvmExecutionImpl = org.camunda.bpm.engine.impl.pvm.runtime.PvmExecutionImpl;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PvmLogger : ProcessEngineLogger
	{

	  public virtual void notTakingTranistion(PvmTransition outgoingTransition)
	  {
		logDebug("001", "Not taking transition '{}', outgoing execution has ended.", outgoingTransition);
	  }

	  public virtual void debugExecutesActivity(PvmExecutionImpl execution, ActivityImpl activity, string name)
	  {
		logDebug("002", "{} executed activity {}: {}", execution, activity, name);
	  }

	  public virtual void debugLeavesActivityInstance(PvmExecutionImpl execution, string activityInstanceId)
	  {
		logDebug("003", "Execution {} leaves activity instance {}", execution, activityInstanceId);
	  }

	  public virtual void debugDestroyScope(PvmExecutionImpl execution, PvmExecutionImpl propagatingExecution)
	  {
		logDebug("004", "Execution {} leaves parent scope {}", execution, propagatingExecution);
	  }

	  public virtual void destroying(PvmExecutionImpl pvmExecutionImpl)
	  {
		logDebug("005", "Detroying scope {}", pvmExecutionImpl);
	  }

	  public virtual void removingEventScope(PvmExecutionImpl childExecution)
	  {
		logDebug("006", "Removeing event scope {}", childExecution);
	  }

	  public virtual void interruptingExecution(string reason, bool skipCustomListeners)
	  {
		logDebug("007", "Interrupting execution execution {}, {}", reason, skipCustomListeners);
	  }

	  public virtual void debugEnterActivityInstance(PvmExecutionImpl pvmExecutionImpl, string parentActivityInstanceId)
	  {
		logDebug("008", "Enter activity instance {} parent: {}", pvmExecutionImpl, parentActivityInstanceId);
	  }

	  public virtual void exceptionWhileCompletingSupProcess(PvmExecutionImpl execution, Exception e)
	  {
		logError("009", "Exception while completing subprocess of execution {}", execution, e);
	  }

	  public virtual void createScope(PvmExecutionImpl execution, PvmExecutionImpl propagatingExecution)
	  {
		logDebug("010", "Create scope: parent exection {} continues as  {}", execution, propagatingExecution);
	  }

	  public virtual ProcessEngineException scopeNotFoundException(string activityId, string executionId)
	  {
		return new ProcessEngineException(exceptionMessage("011", "Scope with specified activity Id {} and execution {} not found", activityId,executionId));
	  }

	}

}