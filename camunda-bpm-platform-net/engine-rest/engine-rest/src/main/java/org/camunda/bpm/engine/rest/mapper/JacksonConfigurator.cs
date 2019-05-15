﻿using System;

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
namespace org.camunda.bpm.engine.rest.mapper
{


	using Hal = org.camunda.bpm.engine.rest.hal.Hal;

	using DeserializationFeature = com.fasterxml.jackson.databind.DeserializationFeature;
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using SerializationFeature = com.fasterxml.jackson.databind.SerializationFeature;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Provider @Produces({MediaType.APPLICATION_JSON, Hal.APPLICATION_HAL_JSON}) public class JacksonConfigurator implements javax.ws.rs.ext.ContextResolver<com.fasterxml.jackson.databind.ObjectMapper>
	public class JacksonConfigurator : ContextResolver<ObjectMapper>
	{

	  public const string DEFAULT_DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss.SSSZ";
	  public static string dateFormatString = DEFAULT_DATE_FORMAT;

	  public static ObjectMapper configureObjectMapper(ObjectMapper mapper)
	  {
		SimpleDateFormat dateFormat = new SimpleDateFormat(dateFormatString);
		mapper.DateFormat = dateFormat;
		mapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, false);
		mapper.configure(SerializationFeature.WRITE_DATES_AS_TIMESTAMPS, false);

		return mapper;
	  }

	  public override ObjectMapper getContext(Type clazz)
	  {
		return configureObjectMapper(new ObjectMapper());
	  }

	  public static string DateFormatString
	  {
		  set
		  {
			JacksonConfigurator.dateFormatString = value;
		  }
	  }

	}

}