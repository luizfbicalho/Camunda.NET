﻿/*
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
namespace org.camunda.bpm.engine.test.bpmn.@event.error
{
	using BpmnError = org.camunda.bpm.engine.@delegate.BpmnError;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;


	/// <summary>
	/// @author Falko Menge
	/// </summary>
	public class ThrowBpmnErrorDelegate : JavaDelegate
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {
		int? executionsBeforeError = (int?) execution.getVariable("executionsBeforeError");
		int? executions = (int?) execution.getVariable("executions");
		bool? exceptionType = (bool?) execution.getVariable("exceptionType");
		if (executions == null)
		{
		  executions = 0;
		}
		executions++;
		if (executionsBeforeError == null || executionsBeforeError < executions)
		{
		  if (exceptionType != null && exceptionType)
		  {
			throw new MyBusinessException("This is a business exception, which can be caught by a BPMN Error Event.");
		  }
		  else
		  {
			throw new BpmnError("23", "This is a business fault, which can be caught by a BPMN Error Event.");
		  }
		}
		else
		{
		  execution.setVariable("executions", executions);
		}
	  }

	}

}