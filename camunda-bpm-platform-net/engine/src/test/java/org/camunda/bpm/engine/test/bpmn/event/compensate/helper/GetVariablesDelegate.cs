using System.Collections.Generic;

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

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;


	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class GetVariablesDelegate : JavaDelegate
	{

	  private Expression variable;
	  public static IList<string> values = new List<string>();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {

		string variableName = (string) variable.getValue(execution);
		string value = (string) execution.getVariable(variableName);

		values.Add(value);
	  }

	}

}