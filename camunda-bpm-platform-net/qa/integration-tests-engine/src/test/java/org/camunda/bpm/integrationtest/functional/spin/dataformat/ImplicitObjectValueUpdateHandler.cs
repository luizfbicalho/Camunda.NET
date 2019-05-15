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
namespace org.camunda.bpm.integrationtest.functional.spin.dataformat
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ImplicitObjectValueUpdateHandler : JavaDelegate, TaskListener
	{

	  public const string VARIABLE_NAME = "var";
	  public const long ONE_DAY_IN_MILLIS = 1000 * 60 * 60 * 24;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {
		JsonSerializable variable = (JsonSerializable) execution.getVariable(VARIABLE_NAME);

		addADay(variable); // implicit update, i.e. no setVariable call

	  }

	  public virtual void notify(DelegateTask delegateTask)
	  {
		JsonSerializable variable = (JsonSerializable) delegateTask.getVariable(VARIABLE_NAME);

		addADay(variable); // implicit update, i.e. no setVariable call

	  }

	  public static void addADay(JsonSerializable jsonSerializable)
	  {
		DateTime newDate = new DateTime(jsonSerializable.DateProperty.Ticks + ONE_DAY_IN_MILLIS);
		jsonSerializable.DateProperty = newDate;
	  }


	}

}