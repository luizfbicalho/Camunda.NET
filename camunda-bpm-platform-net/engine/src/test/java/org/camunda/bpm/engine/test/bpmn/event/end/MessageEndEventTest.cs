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
namespace org.camunda.bpm.engine.test.bpmn.@event.end
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertTrue;

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Kristin Polenz
	/// @author Nico Rehwaldt
	/// </summary>
	public class MessageEndEventTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMessageEndEvent() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testMessageEndEvent()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process");
		assertNotNull(processInstance);
		assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testMessageEndEventServiceTaskBehavior() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testMessageEndEventServiceTaskBehavior()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();

		// class
		variables["wasExecuted"] = true;
		variables["expressionWasExecuted"] = false;
		variables["delegateExpressionWasExecuted"] = false;
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("process", variables);
		assertNotNull(processInstance);

		assertProcessEnded(processInstance.Id);
		assertTrue(DummyServiceTask.wasExecuted);

		// expression
		variables = new Dictionary<string, object>();
		variables["wasExecuted"] = false;
		variables["expressionWasExecuted"] = true;
		variables["delegateExpressionWasExecuted"] = false;
		variables["endEventBean"] = new EndEventBean();
		processInstance = runtimeService.startProcessInstanceByKey("process", variables);
		assertNotNull(processInstance);

		assertProcessEnded(processInstance.Id);
		assertTrue(DummyServiceTask.expressionWasExecuted);

		// delegate expression
		variables = new Dictionary<string, object>();
		variables["wasExecuted"] = false;
		variables["expressionWasExecuted"] = false;
		variables["delegateExpressionWasExecuted"] = true;
		variables["endEventBean"] = new EndEventBean();
		processInstance = runtimeService.startProcessInstanceByKey("process", variables);
		assertNotNull(processInstance);

		assertProcessEnded(processInstance.Id);
		assertTrue(DummyServiceTask.delegateExpressionWasExecuted);
	  }

	}

}