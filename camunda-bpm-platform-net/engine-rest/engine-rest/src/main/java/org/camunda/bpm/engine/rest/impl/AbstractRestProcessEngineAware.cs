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
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using AbstractProcessEngineAware = org.camunda.bpm.engine.rest.spi.impl.AbstractProcessEngineAware;

	public abstract class AbstractRestProcessEngineAware : AbstractProcessEngineAware
	{

	  protected internal ObjectMapper objectMapper;

	  protected internal string relativeRootResourcePath = "/";

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public AbstractRestProcessEngineAware(String engineName, final com.fasterxml.jackson.databind.ObjectMapper objectMapper)
	  public AbstractRestProcessEngineAware(string engineName, ObjectMapper objectMapper) : base(engineName)
	  {
		this.objectMapper = objectMapper;
	  }

	  protected internal virtual ProcessEngine ProcessEngine
	  {
		  get
		  {
			if (processEngine == null)
			{
			  throw new InvalidRequestException(Status.BAD_REQUEST, "No process engine available");
			}
			return processEngine;
		  }
	  }

	  /// <summary>
	  /// Override the root resource path, if this resource is a sub-resource.
	  /// The relative root resource path is used for generation of links to resources in results.
	  /// </summary>
	  /// <param name="relativeRootResourcePath"> </param>
	  public virtual string RelativeRootResourceUri
	  {
		  set
		  {
			this.relativeRootResourcePath = value;
		  }
	  }

	  protected internal virtual ObjectMapper ObjectMapper
	  {
		  get
		  {
			return objectMapper;
		  }
	  }
	}

}