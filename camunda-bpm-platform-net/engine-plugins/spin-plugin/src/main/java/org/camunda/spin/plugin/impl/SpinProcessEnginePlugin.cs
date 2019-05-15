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
namespace org.camunda.spin.plugin.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.plugin.variable.type.SpinValueType_Fields.JSON;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.spin.plugin.variable.type.SpinValueType_Fields.XML;

	using AbstractProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.AbstractProcessEnginePlugin;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClassLoaderUtil = org.camunda.bpm.engine.impl.util.ClassLoaderUtil;
	using JavaObjectSerializer = org.camunda.bpm.engine.impl.variable.serializer.JavaObjectSerializer;
	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;
	using ValueTypeResolver = org.camunda.bpm.engine.variable.type.ValueTypeResolver;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SpinProcessEnginePlugin : AbstractProcessEnginePlugin
	{

	  public override void preInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		// use classloader which loaded the plugin
		ClassLoader classloader = ClassLoaderUtil.getClassloader(typeof(SpinProcessEnginePlugin));
		DataFormats.loadDataFormats(classloader);
	  }

	  public override void postInit(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		registerFunctionMapper(processEngineConfiguration);
		registerScriptResolver(processEngineConfiguration);
		registerSerializers(processEngineConfiguration);
		registerValueTypes(processEngineConfiguration);
		registerFallbackSerializer(processEngineConfiguration);
	  }

	  protected internal virtual void registerFallbackSerializer(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		processEngineConfiguration.FallbackSerializerFactory = new SpinFallbackSerializerFactory();
	  }

	  protected internal virtual void registerSerializers(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?>> spinDataFormatSerializers = lookupSpinSerializers();
		IList<TypedValueSerializer<object>> spinDataFormatSerializers = lookupSpinSerializers();

		VariableSerializers variableSerializers = processEngineConfiguration.VariableSerializers;

		int javaObjectSerializerIdx = variableSerializers.getSerializerIndexByName(JavaObjectSerializer.NAME);

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> spinSerializer : spinDataFormatSerializers)
		foreach (TypedValueSerializer<object> spinSerializer in spinDataFormatSerializers)
		{
		  // add before java object serializer
		  variableSerializers.addSerializer(spinSerializer, javaObjectSerializerIdx);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?>> lookupSpinSerializers()
	  protected internal virtual IList<TypedValueSerializer<object>> lookupSpinSerializers()
	  {
		DataFormats globalFormats = DataFormats.Instance;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?>> serializers = SpinVariableSerializers.createObjectValueSerializers(globalFormats);
		IList<TypedValueSerializer<object>> serializers = SpinVariableSerializers.createObjectValueSerializers(globalFormats);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: serializers.addAll(SpinVariableSerializers.createSpinValueSerializers(globalFormats));
		((IList<TypedValueSerializer<object>>)serializers).AddRange(SpinVariableSerializers.createSpinValueSerializers(globalFormats));

		return serializers;
	  }

	  protected internal virtual void registerScriptResolver(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		processEngineConfiguration.EnvScriptResolvers.Add(new SpinScriptEnvResolver());
	  }

	  protected internal virtual void registerFunctionMapper(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		processEngineConfiguration.ExpressionManager.addFunctionMapper(new SpinFunctionMapper());
	  }

	  protected internal virtual void registerValueTypes(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		ValueTypeResolver resolver = processEngineConfiguration.ValueTypeResolver;
		resolver.addType(JSON);
		resolver.addType(XML);
	  }

	}

}