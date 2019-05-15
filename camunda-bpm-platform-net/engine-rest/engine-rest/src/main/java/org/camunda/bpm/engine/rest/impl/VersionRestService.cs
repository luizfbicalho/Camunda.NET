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

	using VersionDto = org.camunda.bpm.engine.rest.dto.VersionDto;

	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces(MediaType.APPLICATION_JSON) public class VersionRestService extends AbstractRestProcessEngineAware
	public class VersionRestService : AbstractRestProcessEngineAware
	{

	  public const string PATH = "/version";

	  public VersionRestService(string engineName, ObjectMapper objectMapper) : base(engineName, objectMapper)
	  {
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public org.camunda.bpm.engine.rest.dto.VersionDto getVersion()
	  public virtual VersionDto Version
	  {
		  get
		  {
			return new VersionDto(typeof(VersionRestService).Assembly.ImplementationVersion);
		  }
	  }

	}

}