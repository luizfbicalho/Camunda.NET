﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.engine.externaltask
{

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public interface ExternalTaskQueryBuilder
	{

	  /// <summary>
	  /// Specifies that tasks of a topic should be fetched and locked for
	  /// a certain amount of time
	  /// </summary>
	  /// <param name="topicName"> the name of the topic </param>
	  /// <param name="lockDuration"> the duration in milliseconds for which tasks should be locked;
	  ///   begins at the time of fetching
	  /// @return </param>
	  ExternalTaskQueryTopicBuilder topic(string topicName, long lockDuration);

	  /// <summary>
	  /// Performs the fetching. Locks candidate tasks of the given topics
	  /// for the specified duration.
	  /// </summary>
	  /// <returns> fetched external tasks that match the topic and that can be
	  ///   successfully locked </returns>
	  IList<LockedExternalTask> execute();
	}

}