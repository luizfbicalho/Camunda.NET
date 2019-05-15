using System.IO;

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
namespace org.camunda.bpm.integrationtest.functional.migration
{

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using TimerBean = org.camunda.bpm.integrationtest.functional.migration.beans.TimerBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using Asset = org.jboss.shrinkwrap.api.asset.Asset;
	using ByteArrayAsset = org.jboss.shrinkwrap.api.asset.ByteArrayAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class MigrationContextSwitchBeansTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class MigrationContextSwitchBeansTest : AbstractFoxPlatformIntegrationTest
	{

	  public static readonly BpmnModelInstance ONE_TASK_PROCESS = Bpmn.createExecutableProcess("oneTaskProcess").startEvent().userTask("userTask").endEvent().done();

	  public static readonly BpmnModelInstance BOUNDARY_EVENT_PROCESS = Bpmn.createExecutableProcess("boundaryProcess").startEvent().userTask("userTask").boundaryEvent().timerWithDuration("${timerBean.duration}").endEvent().moveToNode("userTask").endEvent().done();

	  [Deployment(name : "sourceDeployment")]
	  public static WebArchive createSourceDeplyoment()
	  {
		return initWebArchiveDeployment("source.war").addAsResource(modelAsAsset(ONE_TASK_PROCESS), "oneTaskProcess.bpmn20.xml");
	  }

	  [Deployment(name : "targetDeployment")]
	  public static WebArchive createTargetDeplyoment()
	  {
		return initWebArchiveDeployment("target.war").addClass(typeof(TimerBean)).addAsResource(modelAsAsset(BOUNDARY_EVENT_PROCESS), "boundaryProcess.bpmn20.xml");
	  }

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "client.war").addClass(typeof(AbstractFoxPlatformIntegrationTest));

		TestContainer.addContainerSpecificResources(webArchive);

		return webArchive;

	  }

	  protected internal static Asset modelAsAsset(BpmnModelInstance modelInstance)
	  {
		MemoryStream byteStream = new MemoryStream();
		Bpmn.writeModelToStream(byteStream, modelInstance);

		sbyte[] bytes = byteStream.toByteArray();
		return new ByteArrayAsset(bytes);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testCreateBoundaryTimer()
	  public virtual void testCreateBoundaryTimer()
	  {
		// given
		ProcessDefinition sourceDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("oneTaskProcess").singleResult();

		ProcessDefinition targetDefinition = repositoryService.createProcessDefinitionQuery().processDefinitionKey("boundaryProcess").singleResult();

		string pi = runtimeService.startProcessInstanceById(sourceDefinition.Id).Id;

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(pi)).execute();

		// then
		Job timerJob = managementService.createJobQuery().singleResult();
		Assert.assertNotNull(timerJob);
		Assert.assertNotNull(timerJob.Duedate);
	  }


	}

}