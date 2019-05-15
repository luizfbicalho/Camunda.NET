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
namespace org.camunda.bpm.engine.rest.exception
{
	using JsonMappingException = com.fasterxml.jackson.databind.JsonMappingException;
	using ExceptionDto = org.camunda.bpm.engine.rest.dto.ExceptionDto;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Provider public class JsonMappingExceptionHandler implements javax.ws.rs.ext.ExceptionMapper<com.fasterxml.jackson.databind.JsonMappingException>
	public class JsonMappingExceptionHandler : ExceptionMapper<JsonMappingException>
	{
		public override Response toResponse(JsonMappingException exception)
		{
		ExceptionDto dto = ExceptionDto.fromException(exception);
		return Response.status(Response.Status.BAD_REQUEST).entity(dto).type(MediaType.APPLICATION_JSON_TYPE).build();
		}
	}

}