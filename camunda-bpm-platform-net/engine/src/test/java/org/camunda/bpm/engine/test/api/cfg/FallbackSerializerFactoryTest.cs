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
namespace org.camunda.bpm.engine.test.api.cfg
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using JavaObjectSerializer = org.camunda.bpm.engine.impl.variable.serializer.JavaObjectSerializer;
	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using VariableSerializerFactory = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializerFactory;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class FallbackSerializerFactoryTest
	{

	  protected internal ProcessEngine processEngine;
	  protected internal string deployment;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {

		if (processEngine != null)
		{
		  if (!string.ReferenceEquals(deployment, null))
		  {
			processEngine.RepositoryService.deleteDeployment(deployment, true);
		  }

		  processEngine.close();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFallbackSerializer()
	  public virtual void testFallbackSerializer()
	  {
		// given
		// that the process engine is configured with a fallback serializer factory
		 ProcessEngineConfigurationImpl engineConfiguration = (new StandaloneInMemProcessEngineConfiguration()).setJdbcUrl("jdbc:h2:mem:camunda-forceclose").setProcessEngineName("engine-forceclose");

		 engineConfiguration.FallbackSerializerFactory = new ExampleSerializerFactory();

		 processEngine = engineConfiguration.buildProcessEngine();
		 deployOneTaskProcess(processEngine);

		 // when setting a variable that no regular serializer can handle
		 ObjectValue objectValue = Variables.objectValue("foo").serializationDataFormat(ExampleSerializer.FORMAT).create();

		 ProcessInstance pi = processEngine.RuntimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValueTyped("var", objectValue));

		 ObjectValue fetchedValue = processEngine.RuntimeService.getVariableTyped(pi.Id, "var", true);

		 // then the fallback serializer is used
		 Assert.assertNotNull(fetchedValue);
		 Assert.assertEquals(ExampleSerializer.FORMAT, fetchedValue.SerializationDataFormat);
		 Assert.assertEquals("foo", fetchedValue.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFallbackSerializerDoesNotOverrideRegularSerializer()
	  public virtual void testFallbackSerializerDoesNotOverrideRegularSerializer()
	  {
		// given
		// that the process engine is configured with a serializer for a certain format
		// and a fallback serializer factory for the same format
		 ProcessEngineConfigurationImpl engineConfiguration = (new StandaloneInMemProcessEngineConfiguration()).setJdbcUrl("jdbc:h2:mem:camunda-forceclose").setProcessEngineName("engine-forceclose");

		 engineConfiguration.CustomPreVariableSerializers = Arrays.asList<TypedValueSerializer>(new ExampleConstantSerializer());
		 engineConfiguration.FallbackSerializerFactory = new ExampleSerializerFactory();

		 processEngine = engineConfiguration.buildProcessEngine();
		 deployOneTaskProcess(processEngine);

		 // when setting a variable that no regular serializer can handle
		 ObjectValue objectValue = Variables.objectValue("foo").serializationDataFormat(ExampleSerializer.FORMAT).create();

		 ProcessInstance pi = processEngine.RuntimeService.startProcessInstanceByKey("oneTaskProcess", Variables.createVariables().putValueTyped("var", objectValue));

		 ObjectValue fetchedValue = processEngine.RuntimeService.getVariableTyped(pi.Id, "var", true);

		 // then the fallback serializer is used
		 Assert.assertNotNull(fetchedValue);
		 Assert.assertEquals(ExampleSerializer.FORMAT, fetchedValue.SerializationDataFormat);
		 Assert.assertEquals(ExampleConstantSerializer.DESERIALIZED_VALUE, fetchedValue.Value);
	  }

	  public class ExampleSerializerFactory : VariableSerializerFactory
	  {

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> getSerializer(String serializerName)
		public virtual TypedValueSerializer<object> getSerializer(string serializerName)
		{
		  return new ExampleSerializer();
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> getSerializer(org.camunda.bpm.engine.variable.value.TypedValue value)
		public virtual TypedValueSerializer<object> getSerializer(TypedValue value)
		{
		  return new ExampleSerializer();
		}

	  }

	  public class ExampleSerializer : JavaObjectSerializer
	  {

		public const string FORMAT = "example";

		public ExampleSerializer() : base()
		{
		  this.serializationDataFormat = FORMAT;
		}

		public override string Name
		{
			get
			{
			  return FORMAT;
			}
		}

	  }

	  public class ExampleConstantSerializer : JavaObjectSerializer
	  {

		public const string DESERIALIZED_VALUE = "bar";

		public ExampleConstantSerializer() : base()
		{
		  this.serializationDataFormat = ExampleSerializer.FORMAT;
		}

		public override string Name
		{
			get
			{
			  return ExampleSerializer.FORMAT;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected Object deserializeFromByteArray(byte[] bytes, String objectTypeName) throws Exception
		protected internal override object deserializeFromByteArray(sbyte[] bytes, string objectTypeName)
		{
		  // deserialize everything to a constant string
		  return DESERIALIZED_VALUE;
		}

	  }

	  protected internal virtual void deployOneTaskProcess(ProcessEngine engine)
	  {
		deployment = engine.RepositoryService.createDeployment().addClasspathResource("org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml").deploy().Id;
	  }
	}

}