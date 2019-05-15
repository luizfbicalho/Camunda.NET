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
namespace org.camunda.bpm.integrationtest.functional.spin
{
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using CustomDataFormatConfigurator = org.camunda.bpm.integrationtest.functional.spin.dataformat.CustomDataFormatConfigurator;
	using XmlSerializableJsonDeserializer = org.camunda.bpm.integrationtest.functional.spin.dataformat.XmlSerializableJsonDeserializer;
	using XmlSerializableJsonSerializer = org.camunda.bpm.integrationtest.functional.spin.dataformat.XmlSerializableJsonSerializer;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DeploymentHelper = org.camunda.bpm.integrationtest.util.DeploymentHelper;
	using TestContainer = org.camunda.bpm.integrationtest.util.TestContainer;
	using DataFormats = org.camunda.spin.DataFormats;
	using DataFormatConfigurator = org.camunda.spin.spi.DataFormatConfigurator;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using OperateOnDeployment = org.jboss.arquillian.container.test.api.OperateOnDeployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using EmptyAsset = org.jboss.shrinkwrap.api.asset.EmptyAsset;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.application.ProcessApplicationContext.withProcessApplicationContext;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class PaContextSwitchCustomSerializerTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class PaContextSwitchCustomSerializerTest : AbstractFoxPlatformIntegrationTest
	{
		[Deployment(name : "pa3")]
		public static WebArchive createDeployment1()
		{
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "pa3.war").addAsResource("META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(ProcessApplication3)).addClass(typeof(XmlSerializable)).addClass(typeof(XmlSerializableJsonDeserializer)).addClass(typeof(XmlSerializableJsonSerializer)).addAsResource("org/camunda/bpm/integrationtest/functional/spin/paContextSwitchCustomSerializer.bpmn20.xml").addClass(typeof(CustomDataFormatConfigurator)).addAsServiceProvider(typeof(DataFormatConfigurator), typeof(CustomDataFormatConfigurator));

		TestContainer.addSpinJacksonJsonDataFormat(webArchive);

		return webArchive;
		}

	  [Deployment(name : "pa4")]
	  public static WebArchive createDeployment2()
	  {
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "pa4.war").addAsWebInfResource(EmptyAsset.INSTANCE, "beans.xml").addAsLibraries(DeploymentHelper.EngineCdi).addAsResource("META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addClass(typeof(ProcessApplication4));

		return webArchive;
	  }

	  /// <summary>
	  /// Tests following scenario:
	  /// 1. Process application 1 declares custom de-/serializer for object variable. Process is started with object variable within process application 1.
	  /// 2. Process is modified within process application 2, so that variable deserialization is required -> correct deserializer is used.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @OperateOnDeployment("pa3") public void test() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void test()
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance processInstance = withProcessApplicationContext(new java.util.concurrent.Callable<org.camunda.bpm.engine.runtime.ProcessInstance>()
		ProcessInstance processInstance = withProcessApplicationContext(new CallableAnonymousInnerClass(this)
	   , "pa3");

		withProcessApplicationContext(new CallableAnonymousInnerClass2(this, processInstance)
	   , "pa4");

		assertEquals(1, historyService.createHistoricActivityInstanceQuery().activityId("exclusiveGateway").finished().count());

	  }

	  private class CallableAnonymousInnerClass : Callable<ProcessInstance>
	  {
		  private readonly PaContextSwitchCustomSerializerTest outerInstance;

		  public CallableAnonymousInnerClass(PaContextSwitchCustomSerializerTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public org.camunda.bpm.engine.runtime.ProcessInstance call() throws Exception
		  public override ProcessInstance call()
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final XmlSerializable var = new XmlSerializable();
			XmlSerializable var = new XmlSerializable();
			var.Property = "jonny";
			return outerInstance.runtimeService.startProcessInstanceByKey("processWithTimer", Variables.createVariables().putValueTyped("testObject", Variables.objectValue(var).serializationDataFormat(DataFormats.JSON_DATAFORMAT_NAME).create()));
		  }

	  }

	  private class CallableAnonymousInnerClass2 : Callable<Void>
	  {
		  private readonly PaContextSwitchCustomSerializerTest outerInstance;

		  private ProcessInstance processInstance;

		  public CallableAnonymousInnerClass2(PaContextSwitchCustomSerializerTest outerInstance, ProcessInstance processInstance)
		  {
			  this.outerInstance = outerInstance;
			  this.processInstance = processInstance;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Void call() throws Exception
		  public override Void call()
		  {
			outerInstance.runtimeService.createProcessInstanceModification(processInstance.ProcessInstanceId).startTransition("flow2").execute();
			return null;
		  }

	  }

	  public virtual string getInstanceIdForActivity(ActivityInstance activityInstance, string activityId)
	  {
		ActivityInstance instance = getChildInstanceForActivity(activityInstance, activityId);
		if (instance != null)
		{
		  return instance.Id;
		}
		return null;
	  }

	  public virtual ActivityInstance getChildInstanceForActivity(ActivityInstance activityInstance, string activityId)
	  {
		if (activityId.Equals(activityInstance.ActivityId))
		{
		  return activityInstance;
		}

		foreach (ActivityInstance childInstance in activityInstance.ChildActivityInstances)
		{
		  ActivityInstance instance = getChildInstanceForActivity(childInstance, activityId);
		  if (instance != null)
		  {
			return instance;
		  }
		}

		return null;
	  }

	}

}