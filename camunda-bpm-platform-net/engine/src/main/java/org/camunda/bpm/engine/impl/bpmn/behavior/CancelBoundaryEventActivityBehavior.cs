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
namespace org.camunda.bpm.engine.impl.bpmn.behavior
{
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using LegacyBehavior = org.camunda.bpm.engine.impl.pvm.runtime.LegacyBehavior;

	/// <summary>
	/// See CancelEndEventActivityBehavior: the cancel end event interrupts the scope and performs compensation.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class CancelBoundaryEventActivityBehavior : BoundaryEventActivityBehavior
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {

		if (LegacyBehavior.signalCancelBoundaryEvent(signalName))
		{
		  // join compensating executions
		  if (!execution.hasChildren())
		  {
			leave(execution);
		  }
		  else
		  {
			((ExecutionEntity)execution).forceUpdate();
		  }
		}
		else
		{
		  base.signal(execution, signalName, signalData);
		}
	  }

	}

}