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
namespace org.camunda.bpm.engine.test.api.runtime
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	public class ProcessInstanceQueryByLeafInstancesTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule(true);
	  public ProcessEngineRule engineRule = new ProcessEngineRule(true);

	  protected internal RuntimeService runtimeService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		runtimeService = engineRule.RuntimeService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/superProcessWithNestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/nestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml" }) public void testQueryByLeafInstancesThreeLayers()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/superProcessWithNestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/nestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml" })]
	  public virtual void testQueryByLeafInstancesThreeLayers()
	  {
		/*
		 * nested structure: 
		 * superProcessWithNestedSubProcess 
		 * +-- nestedSubProcess
		 *     +-- subProcess
		 */
		ProcessInstance threeLayerProcess = runtimeService.startProcessInstanceByKey("nestedSubProcessQueryTest");
		ProcessInstanceQuery simpleSubProcessQuery = runtimeService.createProcessInstanceQuery().processDefinitionKey("simpleSubProcess");

		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(3L));
		assertThat(runtimeService.createProcessInstanceQuery().processDefinitionKey("nestedSubProcessQueryTest").count(), @is(1L));
		assertThat(runtimeService.createProcessInstanceQuery().processDefinitionKey("nestedSimpleSubProcess").count(), @is(1L));
		assertThat(simpleSubProcessQuery.count(), @is(1L));

		ProcessInstance instance = runtimeService.createProcessInstanceQuery().leafProcessInstances().singleResult();
		assertThat(instance.RootProcessInstanceId, @is(threeLayerProcess.Id));
		assertThat(instance.Id, @is(simpleSubProcessQuery.singleResult().Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/nestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml" }) public void testQueryByLeafInstancesTwoLayers()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/nestedSubProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml" })]
	  public virtual void testQueryByLeafInstancesTwoLayers()
	  {
		/*
		 * nested structure: 
		 * nestedSubProcess 
		 * +-- subProcess
		 */
		ProcessInstance twoLayerProcess = runtimeService.startProcessInstanceByKey("nestedSimpleSubProcess");
		ProcessInstanceQuery simpleSubProcessQuery = runtimeService.createProcessInstanceQuery().processDefinitionKey("simpleSubProcess");

		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(2L));
		assertThat(runtimeService.createProcessInstanceQuery().processDefinitionKey("nestedSimpleSubProcess").count(), @is(1L));
		assertThat(simpleSubProcessQuery.count(), @is(1L));

		ProcessInstance instance = runtimeService.createProcessInstanceQuery().leafProcessInstances().singleResult();
		assertThat(instance.RootProcessInstanceId, @is(twoLayerProcess.Id));
		assertThat(instance.Id, @is(simpleSubProcessQuery.singleResult().Id));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = { "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml" }) public void testQueryByLeafInstancesOneLayer()
	  [Deployment(resources : { "org/camunda/bpm/engine/test/api/runtime/subProcess.bpmn20.xml" })]
	  public virtual void testQueryByLeafInstancesOneLayer()
	  {
		ProcessInstance process = runtimeService.startProcessInstanceByKey("simpleSubProcess");
		ProcessInstanceQuery simpleSubProcessQuery = runtimeService.createProcessInstanceQuery().processDefinitionKey("simpleSubProcess");

		assertThat(runtimeService.createProcessInstanceQuery().count(), @is(1L));
		assertThat(simpleSubProcessQuery.count(), @is(1L));

		ProcessInstance instance = runtimeService.createProcessInstanceQuery().leafProcessInstances().singleResult();
		assertThat(instance.RootProcessInstanceId, @is(process.Id));
		assertThat(instance.Id, @is(simpleSubProcessQuery.singleResult().Id));
	  }
	}

}