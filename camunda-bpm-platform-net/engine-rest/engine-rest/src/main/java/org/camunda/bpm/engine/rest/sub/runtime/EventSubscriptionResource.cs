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
namespace org.camunda.bpm.engine.rest.sub.runtime
{
	using EventSubscriptionDto = org.camunda.bpm.engine.rest.dto.runtime.EventSubscriptionDto;
	using ExecutionTriggerDto = org.camunda.bpm.engine.rest.dto.runtime.ExecutionTriggerDto;


	public interface EventSubscriptionResource
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @GET @Produces(javax.ws.rs.core.MediaType.APPLICATION_JSON) org.camunda.bpm.engine.rest.dto.runtime.EventSubscriptionDto getEventSubscription();
	  EventSubscriptionDto EventSubscription {get;}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @POST @Path("/trigger") @Consumes(javax.ws.rs.core.MediaType.APPLICATION_JSON) void triggerEvent(org.camunda.bpm.engine.rest.dto.runtime.ExecutionTriggerDto triggerDto);
	  void triggerEvent(ExecutionTriggerDto triggerDto);
	}

}