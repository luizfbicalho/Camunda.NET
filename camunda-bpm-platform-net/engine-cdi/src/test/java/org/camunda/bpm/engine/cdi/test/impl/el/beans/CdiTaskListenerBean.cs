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
namespace org.camunda.bpm.engine.cdi.test.impl.el.beans
{
	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Named public class CdiTaskListenerBean implements org.camunda.bpm.engine.delegate.TaskListener
	public class CdiTaskListenerBean : TaskListener
	{

	  public const string VARIABLE_NAME = "variable";
	  public const string INITIAL_VALUE = "a";
	  public const string UPDATED_VALUE = "b";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject BusinessProcess businessProcess;
	  internal BusinessProcess businessProcess;

	  public virtual void notify(DelegateTask delegateTask)
	  {
		string variable = businessProcess.getVariable(VARIABLE_NAME);
		assertEquals(INITIAL_VALUE, variable);
		businessProcess.setVariable(VARIABLE_NAME, UPDATED_VALUE);
	  }
	}

}