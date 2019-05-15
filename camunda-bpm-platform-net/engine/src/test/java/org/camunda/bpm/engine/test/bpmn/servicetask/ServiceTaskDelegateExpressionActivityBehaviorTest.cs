﻿using System.Collections.Generic;

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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public class ServiceTaskDelegateExpressionActivityBehaviorTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment @Test public void testExceptionThrownBySecondScopeServiceTaskIsNotHandled()
	  public virtual void testExceptionThrownBySecondScopeServiceTaskIsNotHandled()
	  {
		IDictionary<object, object> beans = processEngineConfiguration.Beans;
		beans["dummyServiceTask"] = new DummyServiceTask();
		processEngineConfiguration.Beans = beans;

		try
		{
		  runtimeService.startProcessInstanceByKey("process", Collections.singletonMap<string, object> ("count", 0));
		  fail();
		} // since the NVE extends the ProcessEngineException we have to handle it
		  // separately
		catch (NullValueException)
		{
		  fail("Shouldn't have received NullValueException");
		}
		catch (ProcessEngineException e)
		{
		  assertThat(e.Message, containsString("Invalid format"));
		}
	  }

	}

}