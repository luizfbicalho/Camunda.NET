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
namespace org.camunda.bpm.integrationtest.functional.classloading.variables
{
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricVariableInstanceEntity;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using GetDeserializableVariableDelegate = org.camunda.bpm.integrationtest.functional.classloading.variables.beans.GetDeserializableVariableDelegate;
	using SerializableVariable = org.camunda.bpm.integrationtest.functional.classloading.variables.beans.SerializableVariable;
	using SetVariableDelegate = org.camunda.bpm.integrationtest.functional.classloading.variables.beans.SetVariableDelegate;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// <para>Ensures that process variables can be deserialized on a
	/// process instance restart, when running on an appication server</para>
	/// 
	/// @author Nikola Koevski
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class DeserializableVariableTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class DeserializableVariableTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createProcessArchiveDeplyoment()
		public static WebArchive createProcessArchiveDeplyoment()
		{
		return initWebArchiveDeployment().addClass(typeof(GetDeserializableVariableDelegate)).addClass(typeof(SetVariableDelegate)).addClass(typeof(SerializableVariable)).addAsResource("org/camunda/bpm/integrationtest/functional/classloading/DeserializableVariableTest.testVariableDeserializationOnProcessApplicationRestart.bpmn20.xml");
		}

	  [Deployment(name:"clientDeployment")]
	  public static WebArchive clientDeployment()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "client.war").addClass(typeof(AbstractFoxPlatformIntegrationTest));

		TestContainer.addContainerSpecificResources(webArchive);

		return webArchive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("clientDeployment") public void testVariableDeserializationOnProcessApplicationRestart()
	  public virtual void testVariableDeserializationOnProcessApplicationRestart()
	  {
		// given
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testDeserializeVariable");

		// when
		Assert.assertNull(runtimeService.createProcessInstanceQuery().processInstanceId(pi.Id).singleResult());
		runtimeService.restartProcessInstances(pi.ProcessDefinitionId).startAfterActivity("servicetask1").processInstanceIds(pi.Id).execute();

		// then

		// Since the variable retrieval is done outside the Process Application Context as well,
		// custom object deserialization is disabled and a null check is performed
		IList<HistoricVariableInstance> variableInstances = historyService.createHistoricVariableInstanceQuery().disableCustomObjectDeserialization().list();
		foreach (HistoricVariableInstance variable in variableInstances)
		{
		  if (!string.ReferenceEquals(variable.ProcessInstanceId, pi.Id) && variable is HistoricVariableInstanceEntity)
		  {
			Assert.assertNotNull(((HistoricVariableInstanceEntity) variable).getByteArrayValue());
		  }
		}
	  }
	}

}