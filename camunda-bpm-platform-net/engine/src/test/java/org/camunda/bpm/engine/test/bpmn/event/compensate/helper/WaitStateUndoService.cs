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
namespace org.camunda.bpm.engine.test.bpmn.@event.compensate.helper
{
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using AbstractBpmnActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.AbstractBpmnActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using SignallableActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.SignallableActivityBehavior;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class WaitStateUndoService : AbstractBpmnActivityBehavior, SignallableActivityBehavior
	{

	  private Expression counterName;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
	  public virtual void execute(ActivityExecution execution)
	  {
		string variableName = (string) counterName.getValue(execution);
		object variable = execution.getVariable(variableName);
		if (variable == null)
		{
		  execution.setVariable(variableName, (int?) 1);
		}
		else
		{
		  execution.setVariable(variableName, ((int?)variable) + 1);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalEvent, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalEvent, object signalData)
	  {
		leave(execution);
	  }

	}

}