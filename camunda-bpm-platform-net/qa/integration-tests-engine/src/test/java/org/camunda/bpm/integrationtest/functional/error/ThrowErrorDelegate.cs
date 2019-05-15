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
namespace org.camunda.bpm.integrationtest.functional.error
{

	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using AbstractBpmnActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.AbstractBpmnActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named public class ThrowErrorDelegate extends org.camunda.bpm.engine.impl.bpmn.behavior.AbstractBpmnActivityBehavior
	public class ThrowErrorDelegate : AbstractBpmnActivityBehavior
	{
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
		public virtual void execute(ActivityExecution execution)
		{
		handle(execution, "executed");
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void signal(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String signalName, Object signalData) throws Exception
	  public virtual void signal(ActivityExecution execution, string signalName, object signalData)
	  {
		handle(execution, "signaled");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void handle(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution, String action) throws Exception
	  protected internal virtual void handle(ActivityExecution execution, string action)
	  {
		execution.setVariable(action, true);
		string type = (string) execution.getVariable("type");
		if ("error".Equals(type, StringComparison.OrdinalIgnoreCase))
		{
		  throw new BpmnError("MyError");
		}
		else if ("exception".Equals(type, StringComparison.OrdinalIgnoreCase))
		{
		  throw new MyBusinessException("MyException");
		}
		else if ("leave".Equals(type, StringComparison.OrdinalIgnoreCase))
		{
		  execution.setVariable("type", null);
		  leave(execution);
		}
	  }

	}

}