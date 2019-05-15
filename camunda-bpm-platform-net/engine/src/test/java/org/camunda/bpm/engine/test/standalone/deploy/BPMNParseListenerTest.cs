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
namespace org.camunda.bpm.engine.test.standalone.deploy
{
	using ProcessInstanceWithVariablesImpl = org.camunda.bpm.engine.impl.persistence.entity.ProcessInstanceWithVariablesImpl;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;
	using ResourceProcessEngineTestCase = org.camunda.bpm.engine.impl.test.ResourceProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;

	/// <summary>
	/// @author Frederik Heremans
	/// </summary>
	public class BPMNParseListenerTest : ResourceProcessEngineTestCase
	{

	  public BPMNParseListenerTest() : base("org/camunda/bpm/engine/test/standalone/deploy/bpmn.parse.listener.camunda.cfg.xml")
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAlterProcessDefinitionKeyWhenDeploying() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testAlterProcessDefinitionKeyWhenDeploying()
	  {
		// Check if process-definition has different key
		assertEquals(0, repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").count());
		assertEquals(1, repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess-modified").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testAlterActivityBehaviors() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testAlterActivityBehaviors()
	  {

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskWithIntermediateThrowEvent-modified");
		ProcessDefinitionImpl processDefinition = ((ProcessInstanceWithVariablesImpl) processInstance).ExecutionEntity.ProcessDefinition;

		ActivityImpl cancelThrowEvent = processDefinition.findActivity("CancelthrowEvent");
		assertTrue(cancelThrowEvent.ActivityBehavior is TestBPMNParseListener.TestCompensationEventActivityBehavior);

		ActivityImpl startEvent = processDefinition.findActivity("theStart");
		assertTrue(startEvent.ActivityBehavior is TestBPMNParseListener.TestNoneStartEventActivityBehavior);

		ActivityImpl endEvent = processDefinition.findActivity("theEnd");
		assertTrue(endEvent.ActivityBehavior is TestBPMNParseListener.TestNoneEndEventActivityBehavior);
	  }
	}

}