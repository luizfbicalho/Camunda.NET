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
namespace org.camunda.bpm.engine.test.bpmn.servicetask
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;


	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// </summary>
	public class ServiceTaskVariablesTest : PluggableProcessEngineTestCase
	{

	  internal static bool isNullInDelegate2;
	  internal static bool isNullInDelegate3;

	  [Serializable]
	  public class Variable
	  {
		internal const long serialVersionUID = 1L;
		public string value;
	  }

	  public class Delegate1 : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  Variable v = new Variable();
		  execution.setVariable("variable", v);
		  v.value = "delegate1";
		}

	  }

	  public class Delegate2 : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  Variable v = (Variable) execution.getVariable("variable");
		  lock (typeof(ServiceTaskVariablesTest))
		  {
			// we expect this to be 'true'
			isNullInDelegate2 = (!string.ReferenceEquals(v.value, null) && v.value.Equals("delegate1"));
		  }
		  v.value = "delegate2";
		}

	  }

	  public class Delegate3 : JavaDelegate
	  {

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
		public virtual void execute(DelegateExecution execution)
		{
		  Variable v = (Variable) execution.getVariable("variable");
		  lock (typeof(ServiceTaskVariablesTest))
		  {
			// we expect this to be 'true' as well
			isNullInDelegate3 = (!string.ReferenceEquals(v.value, null) && v.value.Equals("delegate2"));
		  }
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSerializedVariablesBothAsync()
	  public virtual void testSerializedVariablesBothAsync()
	  {

		// in this test, there is an async cont. both before the second and the
		// third service task in the sequence

		runtimeService.startProcessInstanceByKey("process");
		waitForJobExecutorToProcessAllJobs(10000);

		lock (typeof(ServiceTaskVariablesTest))
		{
		  assertTrue(isNullInDelegate2);
		  assertTrue(isNullInDelegate3);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSerializedVariablesThirdAsync()
	  public virtual void testSerializedVariablesThirdAsync()
	  {

		// in this test, only the third service task is async

		runtimeService.startProcessInstanceByKey("process");
		waitForJobExecutorToProcessAllJobs(10000);

		lock (typeof(ServiceTaskVariablesTest))
		{
		  assertTrue(isNullInDelegate2);
		  assertTrue(isNullInDelegate3);
		}

	  }

	}


}