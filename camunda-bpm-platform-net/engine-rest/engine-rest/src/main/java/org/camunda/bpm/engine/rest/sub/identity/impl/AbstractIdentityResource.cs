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
namespace org.camunda.bpm.engine.rest.sub.identity.impl
{
	using ObjectMapper = com.fasterxml.jackson.databind.ObjectMapper;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using AbstractAuthorizedRestResource = org.camunda.bpm.engine.rest.impl.AbstractAuthorizedRestResource;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class AbstractIdentityResource : AbstractAuthorizedRestResource
	{

	  protected internal readonly IdentityService identityService;

	  public AbstractIdentityResource(string processEngineName, Resource resource, string resourceId, ObjectMapper objectMapper) : base(processEngineName, resource, resourceId, objectMapper)
	  {
		this.identityService = processEngine.IdentityService;
	  }

	  protected internal virtual void ensureNotReadOnly()
	  {
		if (identityService.ReadOnly)
		{
		  throw new InvalidRequestException(Status.FORBIDDEN, "Identity service implementation is read-only.");
		}
	  }

	}

}