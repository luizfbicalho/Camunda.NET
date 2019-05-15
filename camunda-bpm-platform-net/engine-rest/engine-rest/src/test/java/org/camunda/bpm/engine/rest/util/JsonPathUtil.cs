using System;

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
namespace org.camunda.bpm.engine.rest.util
{
	using JacksonConfigurator = org.camunda.bpm.engine.rest.mapper.JacksonConfigurator;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using DefaultJackson2ObjectMapperFactory = io.restassured.mapper.factory.DefaultJackson2ObjectMapperFactory;
	using JsonPath = io.restassured.path.json.JsonPath;

	public sealed class JsonPathUtil
	{

	  public static JsonPath from(string json)
	  {
		return JsonPath.from(json).@using(new DefaultJackson2ObjectMapperFactoryAnonymousInnerClass());
	  }

	  private class DefaultJackson2ObjectMapperFactoryAnonymousInnerClass : DefaultJackson2ObjectMapperFactory
	  {
		  public ObjectMapper create(Type cls, string charset)
		  {
			return JacksonConfigurator.configureObjectMapper(base.create(cls, charset));
		  }
	  }

	}

}