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
namespace org.camunda.bpm.engine.history
{
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface HistoricCaseActivityStatistics
	{

	  /// <summary>
	  /// The case activity id.
	  /// </summary>
	  string Id {get;}

	  /// <summary>
	  /// The number of available case activity instances.
	  /// </summary>
	  long Available {get;}

	  /// <summary>
	  /// The number of enabled case activity instances.
	  /// </summary>
	  long Enabled {get;}

	  /// <summary>
	  /// The number of disabled case activity instances.
	  /// </summary>
	  long Disabled {get;}

	  /// <summary>
	  /// The number of active case activity instances.
	  /// </summary>
	  long Active {get;}

	  /// <summary>
	  /// The number of completed case activity instances.
	  /// </summary>
	  long Completed {get;}

	  /// <summary>
	  /// The number of terminated case activity instances.
	  /// </summary>
	  long Terminated {get;}

	}

}