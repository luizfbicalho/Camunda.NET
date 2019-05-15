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
namespace org.camunda.bpm.engine.impl.cfg
{

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class TransactionLogger : ProcessEngineLogger
	{

	  public virtual ProcessEngineException exceptionWhileInteractingWithTransaction(string operation, Exception e)
	  {
		throw new ProcessEngineException(exceptionMessage("001", "{} while {}", e.GetType().Name, operation), e);
	  }

	  public virtual void debugTransactionOperation(string @string)
	  {
		logDebug("002", @string);
	  }

	  public virtual void exceptionWhileFiringEvent(TransactionState state, Exception exception)
	  {
		logError("003", "Exception while firing event {}: {}", state, exception.Message, exception);
	  }

	  public virtual void debugFiringEventRolledBack()
	  {
		logDebug("004", "Firing event rolled back");
	  }

	}

}