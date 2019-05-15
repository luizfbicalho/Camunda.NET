using System;
using System.Collections.Generic;
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
namespace org.camunda.bpm.integrationtest.jobexecutor
{
	using DefaultJobPriorityProvider = org.camunda.bpm.engine.impl.jobexecutor.DefaultJobPriorityProvider;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using PriorityBean = org.camunda.bpm.integrationtest.jobexecutor.beans.PriorityBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ByteArrayAsset = org.jboss.shrinkwrap.api.asset.ByteArrayAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class JobPrioritizationFailureTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class JobPrioritizationFailureTest : AbstractFoxPlatformIntegrationTest
	{

	  protected internal ProcessInstance processInstance;

	  public const string VARIABLE_CLASS_NAME = "org.camunda.bpm.integrationtest.jobexecutor.beans.PriorityBean";
	  public const string PRIORITY_BEAN_INSTANCE_FILE = "priorityBean.instance";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setEngines()
	  public virtual void setEngines()
	  {

		// unregister process application so that context switch cannot be performed
		unregisterProcessApplication();
	  }

	  protected internal virtual void unregisterProcessApplication()
	  {
		org.camunda.bpm.engine.repository.Deployment deployment = processEngine.RepositoryService.createDeploymentQuery().singleResult();

		managementService.unregisterProcessApplication(deployment.Id, false);
	  }

	  [Deployment(order : 1)]
	  public static WebArchive createDeployment()
	  {
		return initWebArchiveDeployment().addClass(typeof(PriorityBean)).addAsResource("org/camunda/bpm/integrationtest/jobexecutor/JobPrioritizationTest.priorityProcess.bpmn20.xml");
	  }

	  [Deployment(name : "dummy-client", order : 2)]
	  public static WebArchive createDummyClientDeployment()
	  {
		return initWebArchiveDeployment("pa2.war").addAsResource(new ByteArrayAsset(serializeJavaObjectValue(new PriorityBean())), PRIORITY_BEAN_INSTANCE_FILE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		if (processInstance != null)
		{
		  runtimeService.deleteProcessInstance(processInstance.Id, "");
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("dummy-client") public void testGracefulDegradationOnMissingBean()
	  public virtual void testGracefulDegradationOnMissingBean()
	  {
		// when
		processInstance = runtimeService.startProcessInstanceByKey("priorityProcess");

		// then the job was created successfully and has the default priority on bean evaluation failure
		Job job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(DefaultJobPriorityProvider.DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE, job.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("dummy-client") public void testGracefulDegradationOnMissingClassSpinJson()
	  public virtual void testGracefulDegradationOnMissingClassSpinJson()
	  {
		// given
		IDictionary<string, object> variables = Variables.createVariables().putValue("priorityBean", Variables.serializedObjectValue("{}").serializationDataFormat(Variables.SerializationDataFormats.JSON).objectTypeName(VARIABLE_CLASS_NAME).create());

		// when
		processInstance = runtimeService.startProcessInstanceByKey("priorityProcess", variables);

		// then the job was created successfully and has the default priority although
		// the bean could not be resolved due to a missing class
		Job job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(DefaultJobPriorityProvider.DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE, job.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("dummy-client") public void testGracefulDegradationOnMissingClassSpinXml()
	  public virtual void testGracefulDegradationOnMissingClassSpinXml()
	  {
		// given
		IDictionary<string, object> variables = Variables.createVariables().putValue("priorityBean", Variables.serializedObjectValue("<?xml version=\"1.0\" encoding=\"utf-8\"?><prioritybean></prioritybean>").serializationDataFormat(Variables.SerializationDataFormats.XML).objectTypeName(VARIABLE_CLASS_NAME).create());

		// when
		processInstance = runtimeService.startProcessInstanceByKey("priorityProcess", variables);

		// then the job was created successfully and has the default priority although
		// the bean could not be resolved due to a missing class
		Job job = managementService.createJobQuery().singleResult();
		Assert.assertEquals(DefaultJobPriorityProvider.DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE, job.Priority);
	  }

	  protected internal static sbyte[] serializeJavaObjectValue(Serializable @object)
	  {

		try
		{
		  MemoryStream baos = new MemoryStream();
		  (new ObjectOutputStream(baos)).writeObject(@object);
		  return baos.toByteArray();
		}
		catch (Exception e)
		{
		  throw new Exception(e);
		}
	  }

	}

}