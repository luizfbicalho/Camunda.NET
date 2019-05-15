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
	using ProcessEngine = org.camunda.bpm.engine.ProcessEngine;
	using Base64 = org.camunda.bpm.engine.impl.digest._apacheCommonsCodec.Base64;
	using DefaultJobPriorityProvider = org.camunda.bpm.engine.impl.jobexecutor.DefaultJobPriorityProvider;
	using IoUtil = org.camunda.bpm.engine.impl.util.IoUtil;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using PriorityBean = org.camunda.bpm.integrationtest.jobexecutor.beans.PriorityBean;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
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
//ORIGINAL LINE: @RunWith(Arquillian.class) public class JobPrioritizationFailureJavaSerializationTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class JobPrioritizationFailureJavaSerializationTest : AbstractFoxPlatformIntegrationTest
	{

	  protected internal ProcessInstance processInstance;

	  private ProcessEngine engine1;

	  public const string VARIABLE_CLASS_NAME = "org.camunda.bpm.integrationtest.jobexecutor.beans.PriorityBean";
	  public const string PRIORITY_BEAN_INSTANCE_FILE = "priorityBean.instance";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setEngines()
	  public virtual void setEngines()
	  {
		ProcessEngineService engineService = BpmPlatform.ProcessEngineService;
		engine1 = engineService.getProcessEngine("engine1");

		// unregister process application so that context switch cannot be performed
		unregisterProcessApplication();
	  }

	  protected internal virtual void unregisterProcessApplication()
	  {
		org.camunda.bpm.engine.repository.Deployment deployment = engine1.RepositoryService.createDeploymentQuery().singleResult();

		engine1.ManagementService.unregisterProcessApplication(deployment.Id, false);
	  }

	  [Deployment(order : 1)]
	  public static WebArchive createDeployment()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.shrinkwrap.api.spec.WebArchive webArchive = initWebArchiveDeployment("paJavaSerialization1.war", "org/camunda/bpm/integrationtest/processes-javaSerializationEnabled-pa1.xml").addClass(org.camunda.bpm.integrationtest.jobexecutor.beans.PriorityBean.class).addAsResource("org/camunda/bpm/integrationtest/jobexecutor/JobPrioritizationTest.priorityProcess.bpmn20.xml");
		WebArchive webArchive = initWebArchiveDeployment("paJavaSerialization1.war", "org/camunda/bpm/integrationtest/processes-javaSerializationEnabled-pa1.xml").addClass(typeof(PriorityBean)).addAsResource("org/camunda/bpm/integrationtest/jobexecutor/JobPrioritizationTest.priorityProcess.bpmn20.xml");

		TestContainer.addContainerSpecificProcessEngineConfigurationClass(webArchive);
		return webArchive;
	  }

	  [Deployment(name : "dummy-client", order : 2)]
	  public static WebArchive createDummyClientDeployment()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.shrinkwrap.api.spec.WebArchive webArchive = initWebArchiveDeployment("paJavaSerialization2.war", "org/camunda/bpm/integrationtest/processes-javaSerializationEnabled-pa2.xml").addAsResource(new org.jboss.shrinkwrap.api.asset.ByteArrayAsset(serializeJavaObjectValue(new org.camunda.bpm.integrationtest.jobexecutor.beans.PriorityBean())), PRIORITY_BEAN_INSTANCE_FILE);
		WebArchive webArchive = initWebArchiveDeployment("paJavaSerialization2.war", "org/camunda/bpm/integrationtest/processes-javaSerializationEnabled-pa2.xml").addAsResource(new ByteArrayAsset(serializeJavaObjectValue(new PriorityBean())), PRIORITY_BEAN_INSTANCE_FILE);
		return webArchive;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		if (processInstance != null)
		{
		  engine1.RuntimeService.deleteProcessInstance(processInstance.Id, "");
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("dummy-client") public void testGracefulDegradationOnMissingBean()
	  public virtual void testGracefulDegradationOnMissingBean()
	  {
		// when
		processInstance = engine1.RuntimeService.startProcessInstanceByKey("priorityProcess");

		// then the job was created successfully and has the default priority on bean evaluation failure
		Job job = engine1.ManagementService.createJobQuery().processInstanceId(processInstance.ProcessInstanceId).singleResult();
		Assert.assertEquals(DefaultJobPriorityProvider.DEFAULT_PRIORITY_ON_RESOLUTION_FAILURE, job.Priority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("dummy-client") public void testGracefulDegradationOnMissingClassJava()
	  public virtual void testGracefulDegradationOnMissingClassJava()
	  {
		// given
		sbyte[] serializedPriorityBean = readByteArrayFromClasspath(PRIORITY_BEAN_INSTANCE_FILE);
		string encodedPriorityBean = StringUtil.fromBytes(Base64.encodeBase64(serializedPriorityBean), processEngine);

		IDictionary<string, object> variables = Variables.createVariables().putValue("priorityBean", Variables.serializedObjectValue(encodedPriorityBean).serializationDataFormat(Variables.SerializationDataFormats.JAVA).objectTypeName(VARIABLE_CLASS_NAME).create());

		// when
		processInstance = engine1.RuntimeService.startProcessInstanceByKey("priorityProcess", variables);

		// then the job was created successfully and has the default priority although
		// the bean could not be resolved due to a missing class
		Job job = engine1.ManagementService.createJobQuery().processInstanceId(processInstance.ProcessInstanceId).singleResult();
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

	  protected internal static sbyte[] readByteArrayFromClasspath(string path)
	  {
		try
		{
		  Stream inStream = typeof(JobPrioritizationFailureJavaSerializationTest).ClassLoader.getResourceAsStream(path);
		  sbyte[] serializedValue = IoUtil.readInputStream(inStream, "");
		  inStream.Close();
		  return serializedValue;
		}
		catch (Exception e)
		{
		  throw new Exception(e);
		}
	  }

	}

}