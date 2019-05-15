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
namespace org.camunda.bpm.container.impl.metadata.spi
{

	using ProcessEngineConfiguration = org.camunda.bpm.engine.ProcessEngineConfiguration;
	using ProcessEnginePlugin = org.camunda.bpm.engine.impl.cfg.ProcessEnginePlugin;
	using StandaloneProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneProcessEngineConfiguration;

	/// <summary>
	/// <para>Java API representation of a ProcessEngine definition inside an XML
	/// deployment descriptor.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ProcessEngineXml
	{

	  /// <returns> the name of the process engine. Must not be null. </returns>
	  string Name {get;}

	  /// <returns> true if the process engine is the default process engine. </returns>
	  bool Default {get;}

	  /// <returns> the name of the Java Class that is to be used in order to create
	  ///         the process engine instance. Must be a subclass of
	  ///         <seealso cref="ProcessEngineConfiguration"/>. If no value is specified,
	  ///         <seealso cref="StandaloneProcessEngineConfiguration"/> is used. </returns>
	  string ConfigurationClass {get;}

	  /// <returns> the JNDI Name of the datasource to be used.  </returns>
	  string Datasource {get;}

	  /// <returns> a set of additional properties. The properties are directly set on
	  ///         the <seealso cref="ProcessEngineConfiguration"/> class (see
	  ///         <seealso cref="#getConfigurationClass()"/>). This means that each property
	  ///         name used here must be a bean property name on the process engine
	  ///         configuration class and the bean property must be of type
	  ///         <seealso cref="String"/>, <seealso cref="Integer"/> or <seealso cref="Boolean"/>. </returns>
	  IDictionary<string, string> Properties {get;}

	  /// <returns> the name of the job acquisition to be used. </returns>
	  string JobAcquisitionName {get;}

	  /// <returns> a list of <seealso cref="ProcessEnginePlugin"/> definitions. </returns>
	  IList<ProcessEnginePluginXml> Plugins {get;}

	}

}