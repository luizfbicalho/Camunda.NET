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
namespace org.camunda.bpm.engine.test.api.mock
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Mocks = org.camunda.bpm.engine.test.mock.Mocks;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertNull;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class MocksTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule rule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
	  public ProcessEngineRule rule = new ProvidedProcessEngineRule();

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
		taskService = rule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMethodsOfMocksAPI()
	  public virtual void testMethodsOfMocksAPI()
	  {
		//given
		Dictionary<string, object> map = new Dictionary<string, object>();

		for (int i = 0; i < 5; i++)
		{
		  map["key" + i] = new object();
		}

		//when
		foreach (string key in map.Keys)
		{
		  Mocks.register(key, map[key]);
		}

		//then
		foreach (string key in map.Keys)
		{
		  assertEquals(map[key], Mocks.get(key));
		}

		assertEquals(map, Mocks.Mocks);

		Mocks.reset();

		foreach (string key in map.Keys)
		{
		  assertNull(Mocks.get(key));
		}

		assertEquals(0, Mocks.Mocks.Count);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testMockAvailabilityInScriptTask()
	  public virtual void testMockAvailabilityInScriptTask()
	  {
		testMockAvailability();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment public void testMockAvailabilityInExpressionLanguage()
	  public virtual void testMockAvailabilityInExpressionLanguage()
	  {
		testMockAvailability();
	  }

	  //helper ////////////////////////////////////////////////////////////
	  private void testMockAvailability()
	  {
		//given
		const string testStr = "testValue";

		Mocks.register("myMock", new ObjectAnonymousInnerClass(this, testStr));

		//when
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("mocksTest");
		Mocks.reset();

		//then
		assertEquals(testStr, runtimeService.getVariable(pi.Id, "testVar"));
	  }

	  private class ObjectAnonymousInnerClass : object
	  {
		  private readonly MocksTest outerInstance;

		  private string testStr;

		  public ObjectAnonymousInnerClass(MocksTest outerInstance, string testStr)
		  {
			  this.outerInstance = outerInstance;
			  this.testStr = testStr;
		  }


		  public string Test
		  {
			  get
			  {
				return testStr;
			  }
		  }

		  public void testMethod(DelegateExecution execution, string str)
		  {
			execution.setVariable("testVar", str);
		  }

	  }

	}

}