using System.Text;

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
namespace org.camunda.bpm.engine.test.bpmn.servicetask.util
{
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;

	/// <summary>
	/// Example JavaDelegate that uses an injected
	/// <seealso cref="Expression"/>s in fields 'text1' and 'text2'. While executing, 'var1' is set with the reversed result of the
	/// method invocation and 'var2' will be the reversed result of the value expression.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class ReverseStringsFieldInjected : JavaDelegate
	{

	  private Expression text1;
	  private Expression text2;

	  public virtual void execute(DelegateExecution execution)
	  {
		string value1 = (string) text1.getValue(execution);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
		execution.setVariable("var1", (new StringBuilder(value1)).reverse().ToString());

		string value2 = (string) text2.getValue(execution);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET StringBuilder equivalent to the Java 'reverse' method:
		execution.setVariable("var2", (new StringBuilder(value2)).reverse().ToString());
	  }

	}

}