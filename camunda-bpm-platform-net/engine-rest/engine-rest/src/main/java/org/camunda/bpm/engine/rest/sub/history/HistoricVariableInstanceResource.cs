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
namespace org.camunda.bpm.engine.rest.sub.history
{

	using HistoricVariableInstanceDto = org.camunda.bpm.engine.rest.dto.history.HistoricVariableInstanceDto;

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// @author Ronny Bräunlich
	/// </summary>
	public interface HistoricVariableInstanceResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) public org.camunda.bpm.engine.rest.dto.history.HistoricVariableInstanceDto getResource(@QueryParam(org.camunda.bpm.engine.rest.sub.VariableResource_Fields.DESERIALIZE_VALUE_QUERY_PARAM) @DefaultValue("true") boolean deserializeValue);
	  HistoricVariableInstanceDto getResource(bool deserializeValue);


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Path("/data") public javax.ws.rs.core.Response getResourceBinary();
	  Response ResourceBinary {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @DELETE public javax.ws.rs.core.Response deleteVariableInstance();
	  Response deleteVariableInstance();
	}

}