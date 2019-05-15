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

	using AbstractProcessApplication = org.camunda.bpm.application.AbstractProcessApplication;
	using ProcessApplicationInterface = org.camunda.bpm.application.ProcessApplicationInterface;
	using BpmPlatformPlugin = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugin;
	using DefaultVariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.DefaultVariableSerializers;
	using TypedValueSerializer = org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class SpinBpmPlatformPlugin : BpmPlatformPlugin
	{

	  private static readonly SpinPluginLogger LOG = SpinPluginLogger.LOGGER;

	  public virtual void postProcessApplicationDeploy(ProcessApplicationInterface processApplication)
	  {
		ProcessApplicationInterface rawPa = processApplication.RawObject;
		if (rawPa is AbstractProcessApplication)
		{
		  initializeVariableSerializers((AbstractProcessApplication) rawPa);
		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  LOG.logNoDataFormatsInitiailized("process application data formats", "process application is not a sub class of " + typeof(AbstractProcessApplication).FullName);
		}
	  }

	  protected internal virtual void initializeVariableSerializers(AbstractProcessApplication abstractProcessApplication)
	  {
		VariableSerializers paVariableSerializers = abstractProcessApplication.VariableSerializers;

		if (paVariableSerializers == null)
		{
		  paVariableSerializers = new DefaultVariableSerializers();
		  abstractProcessApplication.VariableSerializers = paVariableSerializers;
		}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?> serializer : lookupSpinSerializers(abstractProcessApplication.getProcessApplicationClassloader()))
		foreach (TypedValueSerializer<object> serializer in lookupSpinSerializers(abstractProcessApplication.ProcessApplicationClassloader))
		{
		  paVariableSerializers.addSerializer(serializer);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.List<org.camunda.bpm.engine.impl.variable.serializer.TypedValueSerializer<?>> lookupSpinSerializers(ClassLoader classLoader)
	  protected internal virtual IList<TypedValueSerializer<object>> lookupSpinSerializers(ClassLoader classLoader)
	  {

		DataFormats paDataFormats = new DataFormats();
		paDataFormats.registerDataFormats(classLoader);

		// does not create PA-local serializers for native Spin values;
		// this is still an open feature CAM-5246
		return SpinVariableSerializers.createObjectValueSerializers(paDataFormats);
	  }

	  public virtual void postProcessApplicationUndeploy(ProcessApplicationInterface processApplication)
	  {

	  }

	}

}