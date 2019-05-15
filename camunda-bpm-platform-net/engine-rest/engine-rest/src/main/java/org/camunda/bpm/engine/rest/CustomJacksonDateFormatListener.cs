﻿/*
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
namespace org.camunda.bpm.engine.rest
{

	using JacksonConfigurator = org.camunda.bpm.engine.rest.mapper.JacksonConfigurator;

	public class CustomJacksonDateFormatListener : ServletContextListener
	{

	  public const string CONTEXT_PARAM_NAME = "org.camunda.bpm.engine.rest.jackson.dateFormat";

	  public virtual void contextInitialized(ServletContextEvent sce)
	  {
		string dateFormat = sce.ServletContext.getInitParameter(CONTEXT_PARAM_NAME);
		if (!string.ReferenceEquals(dateFormat, null))
		{
		  JacksonConfigurator.DateFormatString = dateFormat;
		}
	  }

	  public virtual void contextDestroyed(ServletContextEvent sce)
	  {
		// reset to default format
		JacksonConfigurator.DateFormatString = JacksonConfigurator.DEFAULT_DATE_FORMAT;
	  }

	}

}