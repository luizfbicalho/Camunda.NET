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
namespace org.camunda.bpm.engine.impl.interceptor
{
	using CoreExecution = org.camunda.bpm.engine.impl.core.instance.CoreExecution;
	using CoreAtomicOperation = org.camunda.bpm.engine.impl.core.operation.CoreAtomicOperation;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ContextLogger : ProcessEngineLogger
	{

	  public virtual void debugExecutingAtomicOperation<T1>(CoreAtomicOperation<T1> executionOperation, CoreExecution execution)
	  {
		logDebug("001", "Executing atomic operation {} on {}", executionOperation, execution);
	  }

	  public virtual void debugException(Exception throwable)
	  {
		logDebug("002", "Exception while closing command context: {}",throwable.Message, throwable);
	  }

	  public virtual void infoException(Exception throwable)
	  {
		logInfo("003", "Exception while closing command context: {}",throwable.Message, throwable);
	  }

	  public virtual void errorException(Exception throwable)
	  {
		logError("004", "Exception while closing command context: {}",throwable.Message, throwable);
	  }

	  public virtual void exceptionWhileInvokingOnCommandFailed(Exception t)
	  {
		logError("005", "Exception while invoking onCommandFailed()", t);
	  }

	  public virtual void bpmnStackTrace(string @string)
	  {
		logError("006", @string);
	  }

	}

}