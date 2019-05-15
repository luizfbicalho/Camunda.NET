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
namespace org.camunda.bpm.integrationtest.functional.condition
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using EventType = org.camunda.bpm.engine.impl.@event.EventType;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using ConditionalBean = org.camunda.bpm.integrationtest.functional.condition.bean.ConditionalBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class ConditionalStartEventTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class ConditionalStartEventTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createProcessArchiveDeployment()
		public static WebArchive createProcessArchiveDeployment()
		{
		return initWebArchiveDeployment().addClass(typeof(ConditionalBean)).addAsResource("org/camunda/bpm/integrationtest/functional/condition/ConditionalStartEventTest.bpmn20.xml");
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStartInstanceWithBeanCondition()
	  public virtual void testStartInstanceWithBeanCondition()
	  {
		IList<EventSubscription> eventSubscriptions = runtimeService.createEventSubscriptionQuery().list();

		assertEquals(1, eventSubscriptions.Count);
		assertEquals(EventType.CONDITONAL.name(), eventSubscriptions[0].EventType);

		IList<ProcessInstance> instances = runtimeService.createConditionEvaluation().setVariable("foo", 1).evaluateStartConditions();

		assertEquals(1, instances.Count);

		assertNotNull(runtimeService.createProcessInstanceQuery().processDefinitionKey("conditionalEventProcess").singleResult());

		VariableInstance vars = runtimeService.createVariableInstanceQuery().singleResult();
		assertEquals(vars.ProcessInstanceId, instances[0].Id);
		assertEquals(1, vars.Value);
	  }
	}

}