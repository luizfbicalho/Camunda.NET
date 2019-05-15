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
	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public interface HistoricActivityStatistics
	{

	  /// <summary>
	  /// The activity id.
	  /// </summary>
	  string Id {get;}

	  /// <summary>
	  /// The number of all running instances of the activity.
	  /// </summary>
	  long Instances {get;}

	  /// <summary>
	  /// The number of all finished instances of the activity.
	  /// </summary>
	  long Finished {get;}

	  /// <summary>
	  /// The number of all canceled instances of the activity.
	  /// </summary>
	  long Canceled {get;}

	  /// <summary>
	  /// The number of all instances, which complete a scope (ie. in bpmn manner: an activity
	  /// which consumed a token and did not produced a new one), of the activity.
	  /// </summary>
	  long CompleteScope {get;}

	}

}