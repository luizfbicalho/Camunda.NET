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
namespace org.camunda.spin.plugin.impl
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using DefaultVariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.DefaultVariableSerializers;
	using JsonValueType = org.camunda.spin.plugin.variable.type.JsonValueType;
	using XmlValueType = org.camunda.spin.plugin.variable.type.XmlValueType;
	using Mockito = org.mockito.Mockito;

	/// <summary>
	/// @author Ronny Bräunlich
	/// 
	/// </summary>
	public class SpinProcessEnginePluginTest : PluggableProcessEngineTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testPluginDoesNotRegisterXmlSerializerIfNotPresentInClasspath() throws java.io.IOException
	  public virtual void testPluginDoesNotRegisterXmlSerializerIfNotPresentInClasspath()
	  {
		ClassLoader mockClassloader = Mockito.mock(typeof(ClassLoader));
		Mockito.when(mockClassloader.getResources(Mockito.anyString())).thenReturn(Collections.enumeration(System.Linq.Enumerable.Empty<URL>()));
		DataFormats.loadDataFormats(mockClassloader);
		ProcessEngineConfigurationImpl mockConfig = Mockito.mock(typeof(ProcessEngineConfigurationImpl));
		DefaultVariableSerializers serializers = new DefaultVariableSerializers();
		Mockito.when(mockConfig.VariableSerializers).thenReturn(serializers);
		(new SpinProcessEnginePlugin()).registerSerializers(mockConfig);

		assertTrue(serializers.getSerializerByName(org.camunda.spin.plugin.variable.type.XmlValueType_Fields.TYPE_NAME) == null);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testPluginDoesNotRegisterJsonSerializerIfNotPresentInClasspath() throws java.io.IOException
	  public virtual void testPluginDoesNotRegisterJsonSerializerIfNotPresentInClasspath()
	  {
		ClassLoader mockClassloader = Mockito.mock(typeof(ClassLoader));
		Mockito.when(mockClassloader.getResources(Mockito.anyString())).thenReturn(Collections.enumeration(System.Linq.Enumerable.Empty<URL>()));
		DataFormats.loadDataFormats(mockClassloader);
		ProcessEngineConfigurationImpl mockConfig = Mockito.mock(typeof(ProcessEngineConfigurationImpl));
		DefaultVariableSerializers serializers = new DefaultVariableSerializers();
		Mockito.when(mockConfig.VariableSerializers).thenReturn(serializers);
		(new SpinProcessEnginePlugin()).registerSerializers(mockConfig);

		assertTrue(serializers.getSerializerByName(org.camunda.spin.plugin.variable.type.JsonValueType_Fields.TYPE_NAME) == null);
	  }

	  public virtual void testPluginRegistersXmlSerializerIfPresentInClasspath()
	  {
		DataFormats.loadDataFormats(null);
		ProcessEngineConfigurationImpl mockConfig = Mockito.mock(typeof(ProcessEngineConfigurationImpl));
		Mockito.when(mockConfig.VariableSerializers).thenReturn(processEngineConfiguration.VariableSerializers);
		(new SpinProcessEnginePlugin()).registerSerializers(mockConfig);

		assertTrue(processEngineConfiguration.VariableSerializers.getSerializerByName(org.camunda.spin.plugin.variable.type.XmlValueType_Fields.TYPE_NAME) is XmlValueSerializer);
	  }

	  public virtual void testPluginRegistersJsonSerializerIfPresentInClasspath()
	  {
		DataFormats.loadDataFormats(null);
		ProcessEngineConfigurationImpl mockConfig = Mockito.mock(typeof(ProcessEngineConfigurationImpl));
		Mockito.when(mockConfig.VariableSerializers).thenReturn(processEngineConfiguration.VariableSerializers);
		(new SpinProcessEnginePlugin()).registerSerializers(mockConfig);

		assertTrue(processEngineConfiguration.VariableSerializers.getSerializerByName(org.camunda.spin.plugin.variable.type.JsonValueType_Fields.TYPE_NAME) is JsonValueSerializer);
	  }
	}

}