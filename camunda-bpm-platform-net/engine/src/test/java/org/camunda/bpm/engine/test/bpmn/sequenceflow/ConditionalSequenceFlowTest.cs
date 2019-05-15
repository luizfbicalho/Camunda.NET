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
namespace org.camunda.bpm.engine.test.bpmn.sequenceflow
{

	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;


	/// <summary>
	/// @author Joram Barrez
	/// @author Falko Menge (camunda)
	/// </summary>
	public class ConditionalSequenceFlowTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testUelExpression()
	  public virtual void testUelExpression()
	  {
		IDictionary<string, object> variables = CollectionUtil.singletonMap("input", "right");
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("condSeqFlowUelExpr", variables);

		Task task = taskService.createTaskQuery().processInstanceId(pi.Id).singleResult();

		assertNotNull(task);
		assertEquals("task right", task.Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testValueAndMethodExpression()
	  public virtual void testValueAndMethodExpression()
	  {
		// An order of price 150 is a standard order (goes through an UEL value expression)
		ConditionalSequenceFlowTestOrder order = new ConditionalSequenceFlowTestOrder(150);
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("uelExpressions", CollectionUtil.singletonMap("order", order));
		Task task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertEquals("Standard service", task.Name);

		// While an order of 300, gives us a premium service (goes through an UEL method expression)
		order = new ConditionalSequenceFlowTestOrder(300);
		processInstance = runtimeService.startProcessInstanceByKey("uelExpressions", CollectionUtil.singletonMap("order", order));
		task = taskService.createTaskQuery().processInstanceId(processInstance.Id).singleResult();
		assertEquals("Premium service", task.Name);

	  }

	  /// <summary>
	  /// Test that Conditional Sequence Flows throw an exception, if no condition
	  /// evaluates to true.
	  /// 
	  /// BPMN 2.0.1 p. 427 (PDF 457):
	  /// "Multiple outgoing Sequence Flows with conditions behaves as an inclusive split."
	  /// 
	  /// BPMN 2.0.1 p. 436 (PDF 466):
	  /// "The inclusive gateway throws an exception in case all conditions evaluate to false and a default flow has not been specified."
	  /// </summary>
	  /// <seealso cref= https://app.camunda.com/jira/browse/CAM-1773 </seealso>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testNoExpressionTrueThrowsException()
	  public virtual void testNoExpressionTrueThrowsException()
	  {
		IDictionary<string, object> variables = CollectionUtil.singletonMap("input", "non-existing-value");
		try
		{
		  runtimeService.startProcessInstanceByKey("condSeqFlowUelExpr", variables);
		  fail("Expected ProcessEngineException");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("No conditional sequence flow leaving the Flow Node 'theStart' could be selected for continuing the process", e.Message);
		}
	  }

	}

}