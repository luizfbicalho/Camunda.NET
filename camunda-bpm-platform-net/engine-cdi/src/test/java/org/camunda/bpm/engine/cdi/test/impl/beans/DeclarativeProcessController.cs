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
namespace org.camunda.bpm.engine.cdi.test.impl.beans
{

	using CompleteTask = org.camunda.bpm.engine.cdi.annotation.CompleteTask;
	using ProcessVariable = org.camunda.bpm.engine.cdi.annotation.ProcessVariable;
	using ProcessVariableLocalTyped = org.camunda.bpm.engine.cdi.annotation.ProcessVariableLocalTyped;
	using ProcessVariableTyped = org.camunda.bpm.engine.cdi.annotation.ProcessVariableTyped;
	using StartProcess = org.camunda.bpm.engine.cdi.annotation.StartProcess;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class DeclarativeProcessController
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ProcessVariable String name;
	  internal string name; // this is going to be set as a process variable

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ProcessVariableTyped String untypedName;
	  internal string untypedName;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ProcessVariableTyped StringValue typedName;
	  internal StringValue typedName;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject @ProcessVariableTyped TypedValue injectedValue;
	  internal TypedValue injectedValue;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject @ProcessVariableLocalTyped TypedValue injectedLocalValue;
	  internal TypedValue injectedLocalValue;

	  [StartProcess("keyOfTheProcess")]
	  public virtual void startProcessByKey()
	  {
		name = "camunda";
		untypedName = "untypedName";
		typedName = Variables.stringValue("typedName");
	  }

	  [CompleteTask(endConversation : false)]
	  public virtual void completeTask()
	  {
	  }

	  [CompleteTask(endConversation : true)]
	  public virtual void completeTaskEndConversation()
	  {
	  }

	  public virtual TypedValue InjectedValue
	  {
		  get
		  {
			return injectedValue;
		  }
	  }

	  public virtual TypedValue InjectedLocalValue
	  {
		  get
		  {
			return injectedLocalValue;
		  }
	  }

	}

}