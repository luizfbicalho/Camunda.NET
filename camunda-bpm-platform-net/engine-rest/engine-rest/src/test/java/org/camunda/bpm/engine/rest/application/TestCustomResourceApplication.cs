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
namespace org.camunda.bpm.engine.rest.application
{
	using JsonMappingExceptionMapper = com.fasterxml.jackson.jaxrs.@base.JsonMappingExceptionMapper;
	using JsonParseExceptionMapper = com.fasterxml.jackson.jaxrs.@base.JsonParseExceptionMapper;
	using JacksonJsonProvider = com.fasterxml.jackson.jaxrs.json.JacksonJsonProvider;
	using ExceptionHandler = org.camunda.bpm.engine.rest.exception.ExceptionHandler;
	using ProcessEngineExceptionHandler = org.camunda.bpm.engine.rest.exception.ProcessEngineExceptionHandler;
	using RestExceptionHandler = org.camunda.bpm.engine.rest.exception.RestExceptionHandler;
	using JacksonConfigurator = org.camunda.bpm.engine.rest.mapper.JacksonConfigurator;


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ApplicationPath("/") public class TestCustomResourceApplication extends javax.ws.rs.core.Application
	public class TestCustomResourceApplication : Application
	{

	  private static readonly ISet<Type> RESOURCES = new HashSet<Type>();
	  private static readonly ISet<Type> PROVIDERS = new HashSet<Type>();

	  static TestCustomResourceApplication()
	  {
		// RESOURCES
		RESOURCES.Add(typeof(UnannotatedResource));

		// PROVIDERS
		PROVIDERS.Add(typeof(ConflictingProvider));

		PROVIDERS.Add(typeof(JacksonConfigurator));

		PROVIDERS.Add(typeof(JacksonJsonProvider));
		PROVIDERS.Add(typeof(JsonMappingExceptionMapper));
		PROVIDERS.Add(typeof(JsonParseExceptionMapper));

		PROVIDERS.Add(typeof(ProcessEngineExceptionHandler));
		PROVIDERS.Add(typeof(RestExceptionHandler));
		PROVIDERS.Add(typeof(ExceptionHandler));
	  }

	  public override ISet<Type> Classes
	  {
		  get
		  {
			ISet<Type> classes = new HashSet<Type>();
			classes.addAll(RESOURCES);
			classes.addAll(PROVIDERS);
    
			return classes;
		  }
	  }

	  public static ISet<Type> ResourceClasses
	  {
		  get
		  {
			return RESOURCES;
		  }
	  }

	  public static ISet<Type> ProviderClasses
	  {
		  get
		  {
			return PROVIDERS;
		  }
	  }
	}

}