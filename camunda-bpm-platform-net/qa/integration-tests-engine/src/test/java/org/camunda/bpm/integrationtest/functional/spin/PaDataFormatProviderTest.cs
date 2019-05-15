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
	using ProcessApplicationContext = org.camunda.bpm.application.ProcessApplicationContext;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using Foo = org.camunda.bpm.integrationtest.functional.spin.dataformat.Foo;
	using FooDataFormat = org.camunda.bpm.integrationtest.functional.spin.dataformat.FooDataFormat;
	using FooDataFormatProvider = org.camunda.bpm.integrationtest.functional.spin.dataformat.FooDataFormatProvider;
	using FooSpin = org.camunda.bpm.integrationtest.functional.spin.dataformat.FooSpin;
	using AbstractFoxPlatformIntegrationTest = org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest;
	using DataFormatProvider = org.camunda.spin.spi.DataFormatProvider;
	using Deployment = org.jboss.arquillian.container.test.api.Deployment;
	using Arquillian = org.jboss.arquillian.junit.Arquillian;
	using ShrinkWrap = org.jboss.shrinkwrap.api.ShrinkWrap;
	using WebArchive = org.jboss.shrinkwrap.api.spec.WebArchive;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.serializedObjectValue;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(Arquillian.class) public class PaDataFormatProviderTest extends org.camunda.bpm.integrationtest.util.AbstractFoxPlatformIntegrationTest
	public class PaDataFormatProviderTest : AbstractFoxPlatformIntegrationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public static org.jboss.shrinkwrap.api.spec.WebArchive createDeployment()
		public static WebArchive createDeployment()
		{
		WebArchive webArchive = ShrinkWrap.create(typeof(WebArchive), "PaDataFormatTest.war").addAsResource("META-INF/processes.xml").addClass(typeof(AbstractFoxPlatformIntegrationTest)).addAsResource("org/camunda/bpm/integrationtest/oneTaskProcess.bpmn").addClass(typeof(Foo)).addClass(typeof(FooDataFormat)).addClass(typeof(FooDataFormatProvider)).addClass(typeof(FooSpin)).addAsServiceProvider(typeof(DataFormatProvider), typeof(FooDataFormatProvider)).addClass(typeof(ReferenceStoringProcessApplication));

		return webArchive;
		}

	  /// <summary>
	  /// Tests that
	  /// 1) a serialized value can be set OUT OF process application context
	  ///   even if the data format is not available (using the fallback serializer)
	  /// 2) and that this value can be deserialized IN process application context
	  ///   by using the PA-local serializer
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void customFormatCanBeUsedForVariableSerialization()
	  public virtual void customFormatCanBeUsedForVariableSerialization()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.runtime.ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", org.camunda.bpm.engine.variable.Variables.createVariables().putValue("serializedObject", serializedObjectValue("foo").serializationDataFormat(org.camunda.bpm.integrationtest.functional.spin.dataformat.FooDataFormat.NAME).objectTypeName(org.camunda.bpm.integrationtest.functional.spin.dataformat.Foo.class.getName())));
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessInstance pi = runtimeService.startProcessInstanceByKey("testProcess", Variables.createVariables().putValue("serializedObject", serializedObjectValue("foo").serializationDataFormat(FooDataFormat.NAME).objectTypeName(typeof(Foo).FullName)));

		ObjectValue objectValue = null;
		try
		{
		  ProcessApplicationContext.CurrentProcessApplication = ReferenceStoringProcessApplication.INSTANCE;
		  objectValue = runtimeService.getVariableTyped(pi.Id, "serializedObject", true);
		}
		finally
		{
		  ProcessApplicationContext.clear();
		}

		object value = objectValue.Value;
		Assert.assertNotNull(value);
		Assert.assertTrue(value is Foo);
	  }

	}

}