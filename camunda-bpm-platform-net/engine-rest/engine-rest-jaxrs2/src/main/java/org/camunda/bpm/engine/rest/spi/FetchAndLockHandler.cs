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
namespace org.camunda.bpm.engine.rest.spi
{
	using FetchExternalTasksExtendedDto = org.camunda.bpm.engine.rest.dto.externaltask.FetchExternalTasksExtendedDto;


	/// <summary>
	/// SPI supposed to replace the default implementation of the long-polling fetch and lock handler
	/// 
	/// @author Tassilo Weidner
	/// </summary>
	public interface FetchAndLockHandler
	{

	  /// <summary>
	  /// Receives a notification that the engine rest web application initialization has been started
	  /// </summary>
	  void start();

	  /// <summary>
	  /// Receives a notification that the engine rest web application is about to be shut down
	  /// </summary>
	  void shutdown();

	  /// <summary>
	  /// Invoked if a fetch and lock request has been sent by the client
	  /// </summary>
	  /// <param name="dto"> which is supposed to hold the payload </param>
	  /// <param name="asyncResponse"> provides means for asynchronous server side response processing </param>
	  /// <param name="processEngine"> provides the process engine context of the respective request </param>
	  void addPendingRequest(FetchExternalTasksExtendedDto dto, AsyncResponse asyncResponse, ProcessEngine processEngine);

	  /// <summary>
	  /// Invoked on initialization of the servlet context
	  /// </summary>
	  /// <param name="servletContextEvent"> provides the servlet context </param>
	  void contextInitialized(ServletContextEvent servletContextEvent);

	}

}