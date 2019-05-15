using System.Collections.Generic;

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

	/// <summary>
	/// A simple provider SPI used to locate a process engine object.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface ProcessEngineProvider
	{

	  /// <summary>
	  /// Provides the default engine. Has to return null if no default engine exists.
	  /// </summary>
	  ProcessEngine DefaultProcessEngine {get;}

	  /// <summary>
	  /// Provides the engine with the given name. Has to return null if no such engine exists.
	  /// </summary>
	  ProcessEngine getProcessEngine(string name);

	  /// <summary>
	  /// Returns the name of all known process engines. Returns an empty set if no engines are accessible.
	  /// </summary>
	  ISet<string> ProcessEngineNames {get;}

	}

}