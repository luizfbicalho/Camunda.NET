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
namespace org.camunda.bpm.engine.test.bpmn.external
{
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExternalTaskParseTest : PluggableProcessEngineTestCase
	{

	  public virtual void testParseExternalTaskWithoutTopic()
	  {
		DeploymentBuilder deploymentBuilder = repositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/bpmn/external/ExternalTaskParseTest.testParseExternalTaskWithoutTopic.bpmn20.xml");

		try
		{
		  deploymentBuilder.deploy();
		  fail("exception expected");
		}
		catch (ProcessEngineException e)
		{
		  assertTextPresent("External tasks must specify a 'topic' attribute in the camunda namespace", e.Message);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseExternalTaskWithExpressionTopic()
	  public virtual void testParseExternalTaskWithExpressionTopic()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["topicName"] = "testTopicExpression";

		runtimeService.startProcessInstanceByKey("oneExternalTaskWithExpressionTopicProcess", variables);
		ExternalTask task = externalTaskService.createExternalTaskQuery().singleResult();
		assertEquals("testTopicExpression", task.TopicName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testParseExternalTaskWithStringTopic()
	  public virtual void testParseExternalTaskWithStringTopic()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();

		runtimeService.startProcessInstanceByKey("oneExternalTaskWithStringTopicProcess", variables);
		ExternalTask task = externalTaskService.createExternalTaskQuery().singleResult();
		assertEquals("testTopicString", task.TopicName);
	  }
	}

}