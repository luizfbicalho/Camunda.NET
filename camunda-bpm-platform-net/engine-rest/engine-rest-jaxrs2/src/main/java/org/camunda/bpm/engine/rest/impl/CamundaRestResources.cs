using System;
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
namespace org.camunda.bpm.engine.rest.impl
{
	using JacksonJsonProvider = com.fasterxml.jackson.jaxrs.json.JacksonJsonProvider;
	using JsonMappingExceptionHandler = org.camunda.bpm.engine.rest.exception.JsonMappingExceptionHandler;
	using JsonParseExceptionHandler = org.camunda.bpm.engine.rest.exception.JsonParseExceptionHandler;
	using ProcessEngineExceptionHandler = org.camunda.bpm.engine.rest.exception.ProcessEngineExceptionHandler;
	using RestExceptionHandler = org.camunda.bpm.engine.rest.exception.RestExceptionHandler;
	using JacksonHalJsonProvider = org.camunda.bpm.engine.rest.hal.JacksonHalJsonProvider;
	using JacksonConfigurator = org.camunda.bpm.engine.rest.mapper.JacksonConfigurator;
	using MultipartPayloadProvider = org.camunda.bpm.engine.rest.mapper.MultipartPayloadProvider;


	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class CamundaRestResources
	{

	  private static readonly ISet<Type> RESOURCE_CLASSES = new HashSet<Type>();

	  private static readonly ISet<Type> CONFIGURATION_CLASSES = new HashSet<Type>();

	  static CamundaRestResources()
	  {
		RESOURCE_CLASSES.Add(typeof(JaxRsTwoNamedProcessEngineRestServiceImpl));
		RESOURCE_CLASSES.Add(typeof(JaxRsTwoDefaultProcessEngineRestServiceImpl));

		CONFIGURATION_CLASSES.Add(typeof(JacksonConfigurator));
		CONFIGURATION_CLASSES.Add(typeof(JacksonJsonProvider));
		CONFIGURATION_CLASSES.Add(typeof(JsonMappingExceptionHandler));
		CONFIGURATION_CLASSES.Add(typeof(JsonParseExceptionHandler));
		CONFIGURATION_CLASSES.Add(typeof(ProcessEngineExceptionHandler));
		CONFIGURATION_CLASSES.Add(typeof(RestExceptionHandler));
		CONFIGURATION_CLASSES.Add(typeof(MultipartPayloadProvider));
		CONFIGURATION_CLASSES.Add(typeof(JacksonHalJsonProvider));
	  }

	  /// <summary>
	  /// Returns a set containing all resource classes provided by camunda BPM. </summary>
	  /// <returns> a set of resource classes. </returns>
	  public static ISet<Type> ResourceClasses
	  {
		  get
		  {
			return RESOURCE_CLASSES;
		  }
	  }

	  /// <summary>
	  /// Returns a set containing all provider / mapper / config classes used in the
	  /// default setup of the camunda REST api. </summary>
	  /// <returns> a set of provider / mapper / config classes. </returns>
	  public static ISet<Type> ConfigurationClasses
	  {
		  get
		  {
			return CONFIGURATION_CLASSES;
		  }
	  }

	}

}