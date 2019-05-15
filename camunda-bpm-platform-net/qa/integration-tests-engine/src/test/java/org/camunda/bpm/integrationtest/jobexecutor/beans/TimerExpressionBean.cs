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
namespace org.camunda.bpm.integrationtest.jobexecutor.beans
{


	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using ProcessVariableTyped = org.camunda.bpm.engine.cdi.annotation.ProcessVariableTyped;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// @author Tobias Metzke
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named public class TimerExpressionBean implements java.io.Serializable
	[Serializable]
	public class TimerExpressionBean
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject @ProcessVariableTyped(value="timerExpression") private org.camunda.bpm.engine.variable.value.TypedValue timerExpression;
	  [ProcessVariableTyped(value:"timerExpression")]
	  private TypedValue timerExpression;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject RuntimeService runtimeService;
	  internal RuntimeService runtimeService;

	  public virtual string TimerDuration
	  {
		  get
		  {
			if (timerExpression == null)
			{
			  VariableInstance variable = runtimeService.createVariableInstanceQuery().variableName("timerExpression").singleResult();
			  if (variable != null)
			  {
				timerExpression = variable.TypedValue;
			  }
			}
			if (timerExpression == null)
			{
			  throw new NullValueException("no variable 'timerExpression' found");
			}
			if (timerExpression is StringValue)
			{
			  return ((StringValue) timerExpression).Value;
			}
			return timerExpression.Value.ToString();
		  }
	  }
	}

}